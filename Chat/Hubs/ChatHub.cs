using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

using Chat.Interfaces;
using Chat.Models;

namespace Chat.Server.Hubs
{
    [Authorize]
    public class ChatHub : Hub<IChat>
    {
        private static readonly ConcurrentDictionary<string, UserModel> _users;

        private const string ADMIN = "admin";

        private static int _nextChatId;
        private static int _nextMessageId;

        static ChatHub()
        {
            _users = new ConcurrentDictionary<string, UserModel>();
            
            _nextChatId = 0;
            _nextMessageId = 0;
        }

        public async Task<IEnumerable<UserModel>> Login(string userName) 
        {
            if (_users.ContainsKey(userName))
            {
                return null;
            }

            List<UserModel> users = new(_users.Values);

            UserModel newUser = new()
            {
                Id = Context.ConnectionId,
                Name = userName
            };

            if (userName == ADMIN)
            {
                newUser.IsAdmin = true;
            }

            if (!_users.TryAdd(userName, newUser))
            {
                return null;
            }

            _nextChatId++;

            await Clients.Others.Login(newUser);

            return users;
        }

        public void Logout(string userName) 
        {
            _users.TryRemove(userName, out UserModel removedUser);

            Clients.Others.Logout(removedUser);
        }

        public ChatMessage ReciveMessage(string fromUserName, string toUserId, string message) 
        {
            ChatMessage chatMessage = new()
            {
                Id = _nextMessageId,
                FromUserId = _users[fromUserName].Id,
                ToUserId = toUserId,
                Message = message,
                SendingTime = DateTime.Now
            };

            _nextMessageId++;

            Clients.Client(toUserId).ReciveMessage(chatMessage);

            return chatMessage;
        }
    }
}