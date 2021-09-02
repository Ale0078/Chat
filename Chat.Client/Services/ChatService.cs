using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

using Chat.Models;
using Chat.Interfaces;

namespace Chat.Client.Services
{
    public class ChatService
    {
        private const string URL = "http://localhost:5000/chat";

        private readonly HubConnection _connection;

        public event Action<string, string> ConnectUser;
        public event Action<string> Logout;
        public event Action<ChatMessageModel> ReciveMessage;

        public ChatService()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl(URL, options => 
                {
                    options.AccessTokenProvider = () => Task.FromResult(Token);
                })
                .Build();
            
            _connection.On<string, string>(nameof(IChat.Connect), (userName, connectionId) => ConnectUser?.Invoke(userName, connectionId));
            _connection.On<string>(nameof(IChat.Logout), userName => Logout?.Invoke(userName));
            _connection.On<ChatMessageModel>(nameof(IChat.ReciveMessage), message => ReciveMessage?.Invoke(message));
        }

        public string Token { get; set; }

        public async Task Connect() 
        {
            await _connection.StartAsync();
        }

        public async Task<ChatMessageModel> ReciveMessageUser(Guid chatId, string fromUserId, string toUserId, string message, string connectionId) 
        {
            return await _connection.InvokeAsync<ChatMessageModel>("ReciveMessage", chatId, fromUserId, toUserId, message, connectionId);
        }
    }
}
