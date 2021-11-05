using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

using Chat.Models;
using Chat.Entities;
using Chat.Entities.Contexts;
using Chat.Server.Services.Interfaces;

namespace Chat.Server.Services
{
    internal class UserService : IUserService
    {
        private readonly ApplicationContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public UserService(ApplicationContext dbContext, UserManager<User> userManager, IMapper mapper)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<bool> AddChatMessageAsync(ChatMessageModel message)
        {
            await _dbContext.ChatMessages.AddAsync(_mapper.Map<ChatMessage>(message));

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
                user: _mapper.Map<User>(userModel),
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

            FullUserModel userModel = _mapper.Map<FullUserModel>(dbUser);

            foreach (Chatter chatter in dbUser.Chatters)
            {
                List<ChatMessageModel> messageModels = new(chatter.Chat.ChatMessages.Count);

                foreach (ChatMessage message in chatter.Chat.ChatMessages)
                {
                    ChatMessageModel messageModel = _mapper.Map<ChatMessageModel>(message);
                    messageModel.IsFromCurrentUser = dbUser.Id.Equals(message.SenderId);

                    messageModels.Add(messageModel);
                }

                ChatModel chat = _mapper.Map<ChatModel>(chatter);
                chat.Messages = messageModels;

                userModel.Chats.Add(chat);
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

                ObservableCollection<ChatMessageModel> messageModels = new();

                foreach (ChatMessage message in chat.Chat.ChatMessages)
                {
                    ChatMessageModel messageModel = _mapper.Map<ChatMessageModel>(message);
                    messageModel.IsFromCurrentUser = currentUser.Id.Equals(message.SenderId);

                    messageModels.Add(messageModel);
                }

                UserModel userModel = _mapper.Map<UserModel>(user);
                userModel.ChatId = chat.ChatId;
                userModel.Messages = messageModels;
                
                userModels.Add(userModel);
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

            return _mapper.Map<BlockModel>(blockedUser);
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

        public async Task<bool> SetNewPhotoAsync(string userName, byte[] photo) 
        {
            User userToChangePhoto = await _userManager.FindByNameAsync(userName);

            if (userToChangePhoto is null)
            {
                return false;
            }

            userToChangePhoto.Photo = photo;

            return (await _userManager.UpdateAsync(userToChangePhoto)).Succeeded;
        }

        public async Task<bool> ChangeUserMessageAsync(Guid messageId, string message) 
        {
            ChatMessage chatMessage = await _dbContext.ChatMessages.FindAsync(messageId);

            if (chatMessage is null)
            {
                return false;
            }

            chatMessage.Message = message;

            if (!chatMessage.IsEdit)
            {
                chatMessage.IsEdit = true;
            }

            _dbContext.ChatMessages.Update(chatMessage);

            await _dbContext.SaveChangesAsync();

            return true;
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