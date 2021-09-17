using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

using Chat.Server.Extensions;
using Chat.Server.Services.Interfaces;
using Chat.Interfaces;
using Chat.Models;

namespace Chat.Server.Hubs
{
    [Authorize]
    public class ChatHub : Hub<IChat>
    {
        private const string ADMIN_ROLE = "Admin";

        private static List<UserConnection> _connections;

        private readonly IUserService _userService;

        static ChatHub()
        {
            _connections = new List<UserConnection>();
        }

        public ChatHub(IUserService userService)
        {
            _userService = userService;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            await Clients.Others.Connect(
                userName: Context.User.Identity.Name,
                connectionId: Context.ConnectionId);

            await Clients.Caller.SendConnectionsIdToCaller(_connections);

            _connections.Add(new UserConnection
            {
                UserName = Context.User.Identity.Name,
                ConnectionId = Context.ConnectionId
            });
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Clients.Others.Logout(Context.User.Identity.Name);

            _connections.Remove(_connections.Find(connection =>
            {
                return connection.UserName == Context.User.Identity.Name;
            }));

            await base.OnDisconnectedAsync(exception);
        }

        public async Task<ChatMessageModel> ReciveMessage(Guid chatId, string fromUserId, string toUserId, string message, string connectionId) 
        {
            ChatMessageModel chatMessage = new()
            {
                ChatId = chatId,
                FromUserId = fromUserId,
                ToUserId = toUserId,
                Message = message,
                SendingTime = DateTime.Now
            };
            
            await _userService.AddChatMessageAsync(chatMessage);

            if (IsValidConnectionId(connectionId))
            {
                await Clients.Client(connectionId).ReciveMessage(chatMessage);
            }

            return chatMessage;
        }

        public async Task SetUserBlackListState(string userId, string connectionId, string blockedUserid, bool doesBlock) 
        {
            BlockModel block = await _userService.SetUserBlackListStatusAsync(
                userId: userId,
                blockedUserId: blockedUserid,
                doesBlock: doesBlock);

            if (IsValidConnectionId(connectionId))
            {
                await Clients.Client(connectionId).SendBlackListStateToUser(block);
            }
        }

        [Authorize(Roles = ADMIN_ROLE)]
        public async Task<bool> SetBlockState(string userId, string connectionId, bool isBlocked) 
        {
            if (IsValidConnectionId(connectionId))
            {
                await Clients.Client(connectionId).ChangeBlockStatusUserToUser(isBlocked
                    ? UserState.Blocked
                    : UserState.Login);

                await Clients.AllExcept(connectionId).ChangeBlockStatusUserToAllUsersExceptBlocked(userId, isBlocked);
            }
            else 
            {
                await Clients.All.ChangeBlockStatusUserToAllUsersExceptBlocked(userId, isBlocked);
            }

            return await _userService.SetBlockOrMuteStateAsync(userId, isBlocked, true);
        }

        [Authorize(Roles = ADMIN_ROLE)]
        public async Task<bool> SetMuteState(string userId, string connectionId, bool isMuted) 
        {
            if (IsValidConnectionId(connectionId))
            {
                await Clients.Client(connectionId).ChangeMuteStateUserToUser(isMuted);
            }

            return await _userService.SetBlockOrMuteStateAsync(userId, isMuted, false);
        }

        private bool IsValidConnectionId(string connectionId) =>
            connectionId is not null && !connectionId.IsConnectionIdEmpty();
    }
}