using System;
using System.Linq;
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
        private readonly IGroupService _groupService;

        static ChatHub()
        {
            _connections = new List<UserConnection>();
        }

        public ChatHub(IUserService userService, IGroupService groupService)
        {
            _userService = userService;
            _groupService = groupService;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            await Clients.Others.Connect(
                userName: Context.User.Identity.Name,
                connectionId: Context.ConnectionId);

            await AddOrRemoveUserFromGroup(async groupName => await Groups.AddToGroupAsync(Context.ConnectionId, groupName));

            await Clients.Caller.SendConnectionsIdToCaller(_connections);
            await Clients.Caller.SendConnectionIdToCaller(Context.ConnectionId);

            _connections.Add(new UserConnection
            {
                UserName = Context.User.Identity.Name,
                ConnectionId = Context.ConnectionId
            });
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (await _userService.UpdateDisconnectTimeAsync(Context.User.Identity.Name, DateTime.Now))
            {
                await Clients.Others.Logout(Context.User.Identity.Name, DateTime.Now);
            }

            _connections.Remove(_connections.Find(connection =>
            {
                return connection.UserName == Context.User.Identity.Name;
            }));

            await AddOrRemoveUserFromGroup(async groupName => await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName));

            await base.OnDisconnectedAsync(exception);
        }

        public async Task<ChatMessageModel> ReciveMessage(string connectionId, ChatMessageModel chatMessage) 
        {
            ChatMessageModel message = await _userService.AddChatMessageAsync(chatMessage);

            if (IsValidConnectionId(connectionId))
            {
                await Clients.Client(connectionId).ReciveMessage(message);
            }

            return message;
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

        public async Task SendTypingStatusToUserAsync(bool isTyping, string connectionId, string typingUserId)
        {
            if (!IsValidConnectionId(connectionId))
            {
                return;
            }

            await Clients.Client(connectionId).SendTypingStatusToUser(isTyping, typingUserId);
        }

        public async Task SetNewPhotoToUserAsync(string userName, byte[] photo) 
        {
            await Clients.Others.ChangeUserPhotoToAllExceptChanged(userName, photo);

            await _userService.SetNewPhotoAsync(userName, photo);
        }

        public async Task ChangeMessageAsync(string connectionId, string userId, Guid messageId, string message) 
        {
            if (IsValidConnectionId(connectionId))
            {
                await Clients.Client(connectionId).ChangeMessageToUserAsync(userId, messageId, message);
            }

            await _userService.ChangeUserMessageAsync(messageId, message);
        }

        public async Task ReadMessageAsync(string userIdToSendReadStatus, string connectionId, Guid messageId) 
        {
            if (IsValidConnectionId(connectionId))
            {
                await Clients.Client(connectionId).ReadMessageToUserAsync(userIdToSendReadStatus, messageId);
            }

            await _userService.ReadMessageAsync(messageId);
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
                
                await Clients.AllExcept(connectionId).ChangeMuteStateUserToAllUsersExceptMuted(userId, isMuted);
            }
            else 
            {
                await Clients.All.ChangeMuteStateUserToAllUsersExceptMuted(userId, isMuted);
            }

            return await _userService.SetBlockOrMuteStateAsync(userId, isMuted, false);
        }

        public async Task AddToGroup(GroupUser user, string groupName)//ToDo: send group to new user
        {
            if (await _groupService.AddGroupUserToGroupAsync(user, groupName))
            {
                await Clients.OthersInGroup(groupName).SendNewGroupMemberToGroupMembersAsync(user, groupName);
            }

            if (IsValidConnectionId(user.ConnectionId))
            {
                await Groups.AddToGroupAsync(user.ConnectionId, groupName);
            }
        }

        public async Task RemoveFromGroup(GroupUser user, string groupName)//ToDo: remove group from removed user
        {
            if (await _groupService.RemoveGroupUserFromGroupAsync(user, groupName))
            {
                await Clients.OthersInGroup(groupName).RemoveGroupMembertToGroupMembersAsync(user, groupName);
            }

            if (IsValidConnectionId(user.ConnectionId))
            {
                await Groups.RemoveFromGroupAsync(user.ConnectionId, groupName);
            }
        }

        public async Task<GroupModel> CreateGroup(string groupName, byte[] groupPhoto, List<GroupUser> users)
        {
            GroupModel group = await _groupService.CreateGroupAsync(
                groupName: groupName,
                groupPhoto: groupPhoto,
                users: users);

            foreach (GroupUser user in users)
            {
                if (string.IsNullOrEmpty(user.ConnectionId))
                {
                    continue;
                }

                await Groups.AddToGroupAsync(user.ConnectionId, groupName);
            }

            await Clients.OthersInGroup(groupName).SendGroupToGroupMembersAsync(group);

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            return group;
        }

        public async Task<GroupMessageModel> SendGroupMessage(string groupName, GroupMessageModel message)
        {
            GroupMessageModel groupMessage = await _groupService.CreateGroupMessageAsync(message);

            await Clients.OthersInGroup(groupName).SendMessageToGroupMembersAsync(groupMessage);

            return groupMessage;
        }

        public async Task ChangeGroupMessage(string groupName, Guid messageId, string message) 
        {
            await Clients.OthersInGroup(groupName).ChangeGroupMessageAsync(groupName, messageId, message);

            await _groupService.ChangeMessageAsync(messageId, message);
        }

        public async Task ReadGroupMessage(string senderConnectionId, string readerId, Guid groupId, Guid messageId) 
        {
            if (IsValidConnectionId(senderConnectionId))
            {
                await Clients.Client(senderConnectionId).SendGroupMessageReadStatusAsync(groupId, messageId);
            }

            await _groupService.ReadGroupMessageAsync(readerId, messageId);
        }

        private bool IsValidConnectionId(string connectionId) =>
            connectionId is not null && !connectionId.IsConnectionIdEmpty();

        private async Task AddOrRemoveUserFromGroup(Func<string, Task> addOrRemoveUserFunc)
        {
            List<string> groupNames = await _groupService.GetListOfGroupNamesByUserNameAsync(Context.User.Identity.Name);

            if (groupNames.Any())
            {
                foreach (string groupName in groupNames)
                {
                    await addOrRemoveUserFunc(groupName);
                }
            }
        }
    }
}