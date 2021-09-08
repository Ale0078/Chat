﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

using static System.Console;

using Chat.Server.Services.Interfaces;
using Chat.Models;
using Chat.Interfaces;

namespace Chat.Server.Hubs
{
    public class ChatRegistrationHub : Hub<IChatRegistration>
    {
        private readonly IUserService _userService;

        public ChatRegistrationHub(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<bool> Register(RegisterUserModel model)//ToDo: drag to one hub
        {
            if (!await _userService.AddUserAsync(model))
            {
                return false;
            }

            FullUserModel user = await _userService.GetUserAsync(model.UserName);

            await Clients.Others.RegisterUserToOthers(user);

            WriteLine("RegisterUserToOthers is finished");

            await Clients.Caller.SendUserStateToCaller(UserState.NoLogin);

            return true;
        }

        public async Task<LoginResult> Login(string name, string password)
        {
            if (!await _userService.CanLoginUserAsync(name, password))
            {
                return new LoginResult();
            }

            //await Clients.Others.LoginUserToOthers(name);
            
            await Clients.Caller.SendTokenToClaller(name);
            await Clients.Caller.SendCurrentUserToCaller(await _userService.GetUserAsync(name));
            await Clients.Caller.SendListOfUsersToCaller(await _userService.GetUsersAsync(name));
            await Clients.Caller.SendUserStateToCaller(UserState.Login);

            return new LoginResult
            {
                Token = name,
                CurrentUser = await _userService.GetUserAsync(name),
                Users = await _userService.GetUsersAsync(name)
            };
        }
    }
}   