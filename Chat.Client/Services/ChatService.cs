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
        public event Action<string, DateTime> Logout;
        public event Action<ChatMessageModel> ReciveMessage;
        public event Action<IEnumerable<UserConnection>> SendConnectionsIdToCallerEvent;
        public event Action<string, bool> SetBlockStateUserToAllUsersExeptBlocked;
        public event Action<string, bool> SetMuteStateUserToAllUsersExeptMuted;
        public event Action<UserState> SetBlockedStateUserToBlockedUser;
        public event Action<bool> SetMuteStateToUser;
        public event Action<BlockModel> SendBlackListStateToUserServerHandler;
        public event Action<bool, string> SendTypingStatusToUserServerHandler;
        public event Action<string, byte[]> SetNewPhotoToUserServerHandler;
        public event Action<string, Guid, string> ChangeMessageToUserServerHandler;

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

            _connection.On<string, DateTime>(
                methodName: nameof(IChat.Logout), 
                handler: (userName, disconnectTime) => Logout?.Invoke(userName, disconnectTime));

            _connection.On<ChatMessageModel>(
                methodName: nameof(IChat.ReciveMessage), 
                handler: message => ReciveMessage?.Invoke(message));

            _connection.On<string, bool>(
                methodName: nameof(IChat.ChangeBlockStatusUserToAllUsersExceptBlocked),
                handler: (userId, isBlocked) => SetBlockStateUserToAllUsersExeptBlocked?.Invoke(userId, isBlocked));

            _connection.On<string, bool>(
                methodName: nameof(IChat.ChangeMuteStateUserToAllUsersExceptMuted),
                handler: (userId, isMuted) => SetMuteStateUserToAllUsersExeptMuted?.Invoke(userId, isMuted));

            _connection.On<UserState>(
                methodName: nameof(IChat.ChangeBlockStatusUserToUser),
                handler: state => SetBlockedStateUserToBlockedUser?.Invoke(state));

            _connection.On<bool>(
                methodName: nameof(IChat.ChangeMuteStateUserToUser),
                handler: isMuted => SetMuteStateToUser?.Invoke(isMuted));

            _connection.On<BlockModel>(
                methodName: nameof(IChat.SendBlackListStateToUser),
                handler: block => SendBlackListStateToUserServerHandler?.Invoke(block));

            _connection.On<bool, string>(
                methodName: nameof(IChat.SendTypingStatusToUser),
                handler: (isTyping, typingUserId) => SendTypingStatusToUserServerHandler?.Invoke(isTyping, typingUserId));

            _connection.On<string, byte[]>(
                methodName: nameof(IChat.ChangeUserPhotoToAllExceptChanged),
                handler: (userName, photo) => SetNewPhotoToUserServerHandler?.Invoke(userName, photo));

            _connection.On<string, Guid, string>(
                methodName: nameof(IChat.ChangeMessageToUserAsync),
                handler: (userId, messageId, message) => ChangeMessageToUserServerHandler?.Invoke(userId, messageId, message));
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

        public async Task<bool> SetMuteStateToUserAsync(string userId, string connectionId, bool isMuted) 
        {
            return await _connection.InvokeAsync<bool>("SetMuteState", userId, connectionId, isMuted);
        }

        public async Task<BlockModel> SendBlackListStateAsync(string userId, string connectionId, string blockedUserId, bool doesBlock) 
        {
            return await _connection.InvokeAsync<BlockModel>("SetUserBlackListState", userId, connectionId, blockedUserId, doesBlock);
        }

        public async Task SendUserTypingStatusToUserAsync(bool isTyping, string connectionId, string typingUserId) 
        {
            await _connection.InvokeAsync("SendTypingStatusToUserAsync", isTyping, connectionId, typingUserId);
        }

        public async Task SendUserPhotoToAllUsersExceptChanged(string userName, byte[] photo) 
        {
            await _connection.InvokeAsync("SetNewPhotoToUserAsync", userName, photo);
        }

        public async Task SetMessageToChatMessageAsync(string connectionId, string userId, Guid messageId, string message) 
        {
            await _connection.InvokeAsync("ChangeMessageAsync", connectionId, userId, messageId, message);
        }
    }
}
