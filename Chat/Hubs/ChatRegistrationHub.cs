using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

using Chat.Interfaces;
using Chat.Models;
using Chat.Entities;
using Chat.Entities.Contexts;

namespace Chat.Server.Hubs
{
    public class ChatRegistrationHub : Hub
    {
        private readonly UserManager<User> _userManager;

        public ChatRegistrationHub(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task Register(RegisterUserModel model)
        {
            await _userManager.CreateAsync(
                user: new User { UserName = model.UserName },
                password: model.Password);
        }

        public async Task<string> Login(string name, string password)
        {
            return await Task.FromResult(name);
        }
    }
}
