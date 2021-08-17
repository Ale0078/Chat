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

        private readonly HubConnection _connection;

        public event Action<User> Login;
        public event Action<User> Logout;
        public event Action<ChatMessage> ReciveMessage;

        public ChatService()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl(URL, options => 
                {
                    options.AccessTokenProvider = () => Task.FromResult("Alex");
                })
                .Build();

            _connection.On<User>(nameof(IChat.Login), user => Login?.Invoke(user));
            _connection.On<User>(nameof(IChat.Logout), user => Logout?.Invoke(user));
            _connection.On<ChatMessage>(nameof(IChat.ReciveMessage), message => ReciveMessage?.Invoke(message));
        }

        public async Task Connect() 
        {
            await _connection.StartAsync();
        }

        public async Task<IEnumerable<User>> UserLogin(string userName) 
        {
            return await _connection.InvokeAsync<IEnumerable<User>>("Login", userName);
        }

        public async Task UserLogout(string userName) 
        {
            await _connection.InvokeAsync("Logout", userName);
        }

        public async Task<ChatMessage> UserReciveMessage(string fromUserName, string toUserId, string message) 
        {
            return await _connection.InvokeAsync<ChatMessage>("ReciveMessage", fromUserName, toUserId, message);
        }
    }
}
