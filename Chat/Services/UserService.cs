using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;

using Chat.Models;
using Chat.Entities;
using Chat.Entities.Contexts;
using Chat.Server.Services.Interfaces;

namespace Chat.Server.Services//ToDo: use include to all entities
{
    internal class UserService : IUserService//ToDo: auto map to models
    {
        private ApplicationContext _dbContext;
        private UserManager<User> _userManager;

        public UserService(ApplicationContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<bool> AddChatMessageAsync(ChatMessageModel message)
        {
            if (message is null || string.IsNullOrEmpty(message.Message))
            {
                return false;
            }

            await _dbContext.ChatMessages.AddAsync(new ChatMessage
            {
                ChatId = message.ChatId,
                ReceiverId = message.ToUserId,
                SenderId = message.FromUserId,
                Message = message.Message,
                Time = message.SendingTime
            });

            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> AddUserAsync(RegisterUserModel userModel)//ToDo: change chat generation
        {
            if (userModel is null ||
                await _userManager.FindByNameAsync(userModel.UserName) is not null)
            {
                return false;
            }

            List<User> users = _userManager.Users.ToList();

            IdentityResult result = await _userManager.CreateAsync(
                user: new User 
                { 
                    UserName = userModel.UserName, 
                    Photo = userModel.Photo,
                    DisconnectTime = DateTime.Now
                },
                password: userModel.Password);

            if (!result.Succeeded)
            {
                return false;
            }

            User newUser = await _userManager.FindByNameAsync(userModel.UserName);

            foreach (User user in users)
            {
                EntityEntry<Entities.Chat> addedChat = await _dbContext.Chats.AddAsync(new Entities.Chat
                {
                    FirstUserId = newUser.Id,
                    SecondUserId = user.Id
                });

                await _dbContext.Chatters.AddRangeAsync(new Chatter
                {
                    UserId = newUser.Id,
                    ChatId = addedChat.Entity.Id
                }, new Chatter
                {
                    UserId = user.Id,
                    ChatId = addedChat.Entity.Id
                });
            }

            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CanLoginUserAsync(string userName, string password)
        {
            User userToCheckPassword = await _userManager.FindByNameAsync(userName);

            return userToCheckPassword is null
                ? false
                : await _userManager.CheckPasswordAsync(
                    user: userToCheckPassword,
                    password: password);
        }

        public async Task<FullUserModel> GetUserAsync(string userName)
        {
            User dbUser = await _userManager.FindByNameAsync(userName);

            FullUserModel userModel = new()
            {
                Id = dbUser.Id,
                Name = dbUser.UserName,
                Photo = dbUser.Photo,
                IsAdmin = dbUser.UserName == "Admin",
                IsBlocked = dbUser.IsBlocked,
                IsMuted = dbUser.IsMuted,
                DisconnectTime = dbUser.DisconnectTime
            };

            List<BlockModel> blockModels = new(userModel.BlockModels.Count);

            foreach (BlockedUser block in dbUser.BlockedUsers)
            {
                blockModels.Add(new BlockModel
                {
                    UserId = block.Blocker.UserId,
                    DoesBlocked = block.Blocker.DoesBlock
                });
            }

            userModel.BlockModels = blockModels;

            foreach (Chatter chatter in dbUser.Chatters)
            {
                List<ChatMessageModel> messageModels = new(chatter.Chat.ChatMessages.Count);

                foreach (ChatMessage message in chatter.Chat.ChatMessages)
                {
                    messageModels.Add(new ChatMessageModel
                    {
                        Id = message.Id,
                        ChatId = message.ChatId,
                        FromUserId = message.SenderId,
                        ToUserId = message.ReceiverId,
                        Message = message.Message,
                        SendingTime = message.Time,
                        IsFromCurrentUser = dbUser.Id.Equals(message.SenderId)
                    });
                }

                userModel.Chats.Add(new ChatModel
                {
                    Id = chatter.ChatId,
                    FirstUserId = chatter.Chat.FirstUserId,
                    SecondUserId = chatter.Chat.SecondUserId,
                    Messages = messageModels
                });
            }

            return userModel;
        }

        public async Task<IEnumerable<UserModel>> GetUsersAsync(string userName)
        {
            List<User> users = _userManager.Users
                .Where(user => user.UserName != userName)
                .ToList();

            List<UserModel> userModels = new(users.Count());
            User currentUser = await _userManager.FindByNameAsync(userName);

            foreach (User user in users)
            {
                Chatter chat = user.Chatters
                    .Find(chatter =>
                    {
                        return chatter.Chat.FirstUserId == currentUser.Id ||
                            chatter.Chat.SecondUserId == currentUser.Id;
                    });

                List<ChatMessage> messages = chat.Chat.ChatMessages;

                ObservableCollection<ChatMessageModel> messageModels = new();

                foreach (ChatMessage message in messages)
                {
                    messageModels.Add(new ChatMessageModel
                    {
                        Id = message.Id,
                        ChatId = message.ChatId,
                        FromUserId = message.SenderId,
                        ToUserId = message.ReceiverId,
                        Message = message.Message,
                        SendingTime = message.Time,
                        IsFromCurrentUser = currentUser.Id.Equals(message.SenderId)
                    });
                }

                userModels.Add(new UserModel
                {
                    Id = user.Id,
                    Name = user.UserName,
                    Photo = user.Photo,
                    Messages = messageModels,
                    ChatId = chat.ChatId,
                    IsBlocked = user.IsBlocked,
                    IsMuted = user.IsMuted,
                    DisconnectTime = user.DisconnectTime
                });
            }

            return userModels;
        }

        public async Task<bool> SetBlockOrMuteStateAsync(string id, bool isBlockOrMuted, bool doBlock)
        {
            User userToSetState = await _userManager.FindByIdAsync(id);

            if (userToSetState is null)
            {
                return false;
            }

            if (doBlock)
            {
                userToSetState.IsBlocked = isBlockOrMuted;
            }
            else
            {
                userToSetState.IsMuted = isBlockOrMuted;
            }

            IdentityResult result = await _userManager.UpdateAsync(userToSetState);

            return result.Succeeded;
        }

        public async Task<BlockModel> SetUserBlackListStatusAsync(string userId, string blockedUserId, bool doesBlock) 
        {
            BlockedUser blockedUser = _dbContext.BlockedUsers
                .Include(entity => entity.Blocker)
                .AsEnumerable()
                .FirstOrDefault(user =>
                {
                    return user.UserId == blockedUserId
                        && user.Blocker.UserId == userId;
                });

            if (blockedUser is null)
            {
                blockedUser = await AddBlockedUserAsync(userId, blockedUserId, doesBlock);
            }
            else 
            {
                blockedUser.Blocker.DoesBlock = doesBlock;

                _dbContext.Update(blockedUser);

                await _dbContext.SaveChangesAsync();
            }

            return new BlockModel
            {
                UserId = blockedUser.Blocker.UserId,
                DoesBlocked = blockedUser.Blocker.DoesBlock
            };
        }

        public async Task<bool> UpdateDisconnectTimeAsync(string userName, DateTime disconnectTime) 
        {
            User disconnectedUser = await _userManager.FindByNameAsync(userName);

            if (disconnectedUser is null)
            {
                return false;
            }

            disconnectedUser.DisconnectTime = disconnectTime;

            return (await _userManager.UpdateAsync(disconnectedUser)).Succeeded;
        }

        private async Task<BlockedUser> AddBlockedUserAsync(string userId, string blockedUserId, bool doesBlock) 
        {
            EntityEntry<Block> addedBlock = await _dbContext.Blocks.AddAsync(new Block
            {
                UserId = userId,
                DoesBlock = doesBlock
            });

            EntityEntry<BlockedUser> addedBlockedUser = await _dbContext.BlockedUsers.AddAsync(new BlockedUser
            {
                UserId = blockedUserId,
                BlockerId = addedBlock.Entity.Id
            });

            await _dbContext.SaveChangesAsync();

            return addedBlockedUser.Entity;
        }
    }
}