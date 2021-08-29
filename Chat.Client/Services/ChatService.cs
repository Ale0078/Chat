using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR.Client;

using Chat.Models;
using Chat.Interfaces;

namespace Chat.Client.Services
{
    public class ChatService
    {
        private const string URL = "http://localhost:5000/chat";

        private readonly string _token;
        private readonly HubConnection _connection;

        public event Action<UserModel> Login;
        public event Action<UserModel> Logout;
        public event Action<ChatMessage> ReciveMessage;

        public ChatService(string token = null)
        {
            _token = token;
            _connection = new HubConnectionBuilder()
                .WithUrl(URL, options => 
                {
                    options.AccessTokenProvider = () => Task.FromResult(_token);
                })
                .Build();
            
            _connection.On<UserModel>(nameof(IChat.Login), user => Login?.Invoke(user));
            _connection.On<UserModel>(nameof(IChat.Logout), user => Logout?.Invoke(user));
            _connection.On<ChatMessage>(nameof(IChat.ReciveMessage), message => ReciveMessage?.Invoke(message));
        }

        public async Task Connect() 
        {
            await _connection.StartAsync();
        }

        public async Task<IEnumerable<UserModel>> LoginUser(string userName) 
        {
            return await _connection.InvokeAsync<IEnumerable<UserModel>>("Login", userName);
        }

        public async Task LogoutUser(string userName) 
        {
            await _connection.InvokeAsync("Logout", userName);
        }

        public async Task<ChatMessage> ReciveMessageUser(string fromUserName, string toUserId, string message) 
        {
            return await _connection.InvokeAsync<ChatMessage>("ReciveMessage", fromUserName, toUserId, message);
        }
    }
}
