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
                user: new User { UserName = userModel.UserName },
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
            User userToCheckPassword = (await _userManager.FindByNameAsync(userName));

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
                Name = dbUser.UserName
            };

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
                    Messages = messageModels,
                    ChatId = chat.ChatId
                });
            }

            return userModels;
        }
    }
}