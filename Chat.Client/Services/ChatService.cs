using System;
using System.Collections.Generic;
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
        public event Action<IEnumerable<UserConnection>> SendConnectionsIdToCallerEvent;
        public event Action<string, bool> SetBlockStateUserToAllUsersExeptBlocked;
        public event Action<UserState> SetBlockedStateUserToBlockedUser;

        public ChatService()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl(URL, options => 
                {
                    options.AccessTokenProvider = () => Task.FromResult(Token);
                })
                .Build();

            _connection.On<IEnumerable<UserConnection>>(
                methodName: nameof(IChat.SendConnectionsIdToCaller),
                handler: connections => SendConnectionsIdToCallerEvent?.Invoke(connections));

            _connection.On<string, string>(
                methodName: nameof(IChat.Connect),
                handler: (userName, connectionId) => ConnectUser?.Invoke(userName, connectionId));

            _connection.On<string>(
                methodName: nameof(IChat.Logout), 
                handler: userName => Logout?.Invoke(userName));

            _connection.On<ChatMessageModel>(
                methodName: nameof(IChat.ReciveMessage), 
                handler: message => ReciveMessage?.Invoke(message));

            _connection.On<string, bool>(
                methodName: nameof(IChat.ChangeBlockStatusUserToAllUsersExceptBlocked),
                handler: (userId, isBlocked) => SetBlockStateUserToAllUsersExeptBlocked?.Invoke(userId, isBlocked));

            _connection.On<UserState>(
                methodName: nameof(IChat.ChangeBlockStatusUserToUser),
                handler: state => SetBlockedStateUserToBlockedUser?.Invoke(state));
        }

        public string Token { get; set; }

        public async Task Connect() 
        {
            if (_connection.ConnectionId != null)
            {
                return;
            }

            await _connection.StartAsync();
        }

        public async Task<ChatMessageModel> ReciveMessageUserAsync(Guid chatId, string fromUserId, string toUserId, string message, string connectionId) 
        {
            return await _connection.InvokeAsync<ChatMessageModel>("ReciveMessage", chatId, fromUserId, toUserId, message, connectionId);
        }

        public async Task<bool> SetBlockStateToUserAsync(string userId, string connectionId, bool isBlocked) 
        {
            return await _connection.InvokeAsync<bool>("SetBlockState", userId, connectionId, isBlocked);
        }
    }
}
