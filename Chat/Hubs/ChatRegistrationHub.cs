using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

using Chat.Interfaces;
using Chat.Models;

namespace Chat.Server.Hubs
{
    public class ChatRegistrationHub : Hub
    {
        //public async Task Register(RegisterUserModel model) 
        //{

        //}

        public async Task<string> Login(string name, string password)
        {
            return await Task.FromResult(name);
        }
    }
}
