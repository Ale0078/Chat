using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

using Chat.Models;
using Chat.Interfaces;
using Chat.Client.Services.Interfaces;

namespace Chat.Client.Services
{
    public class ChatService
    {
        private readonly IChutConnection _connection;

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
        public event Action<string> SendConnectionIdToCallerServerHandler;
        public event Action<string, Guid> SetReadStatusToMessageToUserServerHandler;

        public ChatService(IChutConnection connection)
        {
            _connection = connection;

            _connection.Connection.On<IEnumerable<UserConnection>>(
                methodName: nameof(IChat.SendConnectionsIdToCaller),
                handler: connections => SendConnectionsIdToCallerEvent?.Invoke(connections));

            _connection.Connection.On<string, string>(
                methodName: nameof(IChat.Connect),
                handler: (userName, connectionId) => ConnectUser?.Invoke(userName, connectionId));

            _connection.Connection.On<string, DateTime>(
                methodName: nameof(IChat.Logout), 
                handler: (userName, disconnectTime) => Logout?.Invoke(userName, disconnectTime));

            _connection.Connection.On<ChatMessageModel>(
                methodName: nameof(IChat.ReciveMessage), 
                handler: message => ReciveMessage?.Invoke(message));

            _connection.Connection.On<string, bool>(
                methodName: nameof(IChat.ChangeBlockStatusUserToAllUsersExceptBlocked),
                handler: (userId, isBlocked) => SetBlockStateUserToAllUsersExeptBlocked?.Invoke(userId, isBlocked));

            _connection.Connection.On<string, bool>(
                methodName: nameof(IChat.ChangeMuteStateUserToAllUsersExceptMuted),
                handler: (userId, isMuted) => SetMuteStateUserToAllUsersExeptMuted?.Invoke(userId, isMuted));

            _connection.Connection.On<UserState>(
                methodName: nameof(IChat.ChangeBlockStatusUserToUser),
                handler: state => SetBlockedStateUserToBlockedUser?.Invoke(state));

            _connection.Connection.On<bool>(
                methodName: nameof(IChat.ChangeMuteStateUserToUser),
                handler: isMuted => SetMuteStateToUser?.Invoke(isMuted));

            _connection.Connection.On<BlockModel>(
                methodName: nameof(IChat.SendBlackListStateToUser),
                handler: block => SendBlackListStateToUserServerHandler?.Invoke(block));

            _connection.Connection.On<bool, string>(
                methodName: nameof(IChat.SendTypingStatusToUser),
                handler: (isTyping, typingUserId) => SendTypingStatusToUserServerHandler?.Invoke(isTyping, typingUserId));

            _connection.Connection.On<string, byte[]>(
                methodName: nameof(IChat.ChangeUserPhotoToAllExceptChanged),
                handler: (userName, photo) => SetNewPhotoToUserServerHandler?.Invoke(userName, photo));

            _connection.Connection.On<string, Guid, string>(
                methodName: nameof(IChat.ChangeMessageToUserAsync),
                handler: (userId, messageId, message) => ChangeMessageToUserServerHandler?.Invoke(userId, messageId, message));

            _connection.Connection.On<string>(
                methodName: nameof(IChat.SendConnectionIdToCaller),
                handler: connectionId => SendConnectionIdToCallerServerHandler?.Invoke(connectionId));

            _connection.Connection.On<string, Guid>(
                methodName: nameof(IChat.ReadMessageToUserAsync),
                handler: (userId, messageId) => SetReadStatusToMessageToUserServerHandler?.Invoke(userId, messageId));
        }

        public void SetToken(string token) 
        {
            _connection.Token = token;
        }

        public async Task Connect() 
        {
            if (_connection.Connection.ConnectionId != null)
            {
                return;
            }

            await _connection.Connection.StartAsync();
        }

        public async Task<ChatMessageModel> ReciveMessageUserAsync(string connectionId, ChatMessageModel chatMessage) 
        {
            return await _connection.Connection.InvokeAsync<ChatMessageModel>("ReciveMessage", connectionId, chatMessage);
        }

        public async Task<bool> SetBlockStateToUserAsync(string userId, string connectionId, bool isBlocked) 
        {
            return await _connection.Connection.InvokeAsync<bool>("SetBlockState", userId, connectionId, isBlocked);
        }

        public async Task<bool> SetMuteStateToUserAsync(string userId, string connectionId, bool isMuted) 
        {
            return await _connection.Connection.InvokeAsync<bool>("SetMuteState", userId, connectionId, isMuted);
        }

        public async Task<BlockModel> SendBlackListStateAsync(string userId, string connectionId, string blockedUserId, bool doesBlock) 
        {
            return await _connection.Connection.InvokeAsync<BlockModel>("SetUserBlackListState", userId, connectionId, blockedUserId, doesBlock);
        }

        public async Task SendUserTypingStatusToUserAsync(bool isTyping, string connectionId, string typingUserId) 
        {
            await _connection.Connection.InvokeAsync("SendTypingStatusToUserAsync", isTyping, connectionId, typingUserId);
        }

        public async Task SendUserPhotoToAllUsersExceptChanged(string userName, byte[] photo) 
        {
            await _connection.Connection.InvokeAsync("SetNewPhotoToUserAsync", userName, photo);
        }

        public async Task SetMessageToChatMessageAsync(string connectionId, string userId, Guid messageId, string message) 
        {
            await _connection.Connection.InvokeAsync("ChangeMessageAsync", connectionId, userId, messageId, message);
        }

        public async Task ReadMessageToUserAsync(string userIdToSendReadStatus, string connectionId, Guid messageId) 
        {
            await _connection.Connection.InvokeAsync("ReadMessageAsync", userIdToSendReadStatus, connectionId, messageId);
        }
    }
}
