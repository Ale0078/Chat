using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

using Chat.Models;
using Chat.Interfaces;

namespace Chat.Client.Services
{
    public class RegistrationChatService
    {
        private const string URL = "http://localhost:5000/chat_register";

        private readonly HubConnection _connection;

        public event Action<FullUserModel> RegisterUserToOthersServerHandler;
        //public event Action<string> LoginUserToOthersServerHandler;
        public event Action<IEnumerable<UserModel>> SendUsersToCallerServerHandler;
        public event Action<FullUserModel> SendUserToCallerServerHandler;
        public event Action<string> SendTokenToCallerServerHandler;
        public event Action<UserState> SendUserStateToCallerServerHandler;

        public RegistrationChatService()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl(URL)
                .Build();

            _connection.On<FullUserModel>(
                methodName: nameof(IChatRegistration.RegisterUserToOthers), 
                handler: userModel => /*RegisterUserToOthersServerHandler?.Invoke(userModel)*/A(userModel));

            //_connection.On<string>(
            //    methodName: nameof(IChatRegistration.LoginUserToOthers), 
            //    handler: userName => LoginUserToOthersServerHandler?.Invoke(userName));

            _connection.On<IEnumerable<UserModel>>(
                methodName: nameof(IChatRegistration.SendListOfUsersToCaller),
                handler: users => SendUsersToCallerServerHandler?.Invoke(users));

            _connection.On<FullUserModel>(
                methodName: nameof(IChatRegistration.SendCurrentUserToCaller),
                handler: user => SendUserToCallerServerHandler?.Invoke(user));

            _connection.On<string>(
                methodName: nameof(IChatRegistration.SendTokenToClaller),
                handler: token => SendTokenToCallerServerHandler?.Invoke(token));

            _connection.On<UserState>(
                methodName: nameof(IChatRegistration.SendUserStateToCaller),
                handler: userState => SendUserStateToCallerServerHandler?.Invoke(userState));
        }

        private void A(FullUserModel b) => RegisterUserToOthersServerHandler?.Invoke(b);

        public async Task Connect() 
        {
            await _connection.StartAsync();
        }

        public async Task Disconect() 
        {
            await _connection.DisposeAsync();
        }

        public async Task<bool> RegisterUser(RegisterUserModel model) 
        {
            return await _connection.InvokeAsync<bool>("Register", model);
        }

        public async Task<LoginResult> LoginUser(string name, string password) 
        {
            return await _connection.InvokeAsync<LoginResult>("Login", name, password);
        }
    }
}
