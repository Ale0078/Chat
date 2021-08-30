using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using Chat.Models;
using Chat.Entities;
using Chat.Entities.Contexts;
using Chat.Server.Services.Interfaces;

namespace Chat.Server.Services
{
    internal class UserService : IUserService
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

        public async Task<bool> AddUserAsync(RegisterUserModel userModel)
        {
            if (userModel is null)
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

        public async Task<bool> CanLoginUserAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName) is null;
        }

        public async Task<UserModel> GetUserAsync(string userName)
        {
            User dbUser = await _userManager.FindByNameAsync(userName);

            return new UserModel
            {
                Id = dbUser.Id,
                Name = dbUser.UserName
            };
        }

        public async Task<IEnumerable<UserModel>> GetUsersAsync(string exceptUserName = null)
        {
            IQueryable<User> users = _userManager.Users.Where(user => user.UserName != exceptUserName);
            List<UserModel> userModels = new(users.Count());

            foreach (User user in users)
            {
                userModels.Add(new UserModel
                {
                    Id = user.Id,
                    Name = user.UserName
                });
            }

            return userModels;
        }
    }
}
