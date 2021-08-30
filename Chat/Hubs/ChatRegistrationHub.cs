using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

using Chat.Server.Services.Interfaces;
using Chat.Models;
using Chat.Entities;

namespace Chat.Server.Hubs
{
    public class ChatRegistrationHub : Hub
    {
        private readonly IUserService _userService;

        public ChatRegistrationHub(IUserService userService)
        {
            _userService = userService;
        }

        public async Task Register(RegisterUserModel model)
        {
            await _userService.AddUserAsync(model);
        }

        public async Task<string> Login(string name, string password)
        {
            return await Task.FromResult(name);
        }
    }
}
