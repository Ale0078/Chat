using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

using Chat.Client.Services.Interfaces;
using Chat.Models;
using Chat.Interfaces.BaseInterfaces;

namespace Chat.Client.Services
{
    public class ChatGroupService
    {
        private readonly IChutConnection _connection;

        public event Action<GroupModel> SendGorupToGroupMembersAsyncServerHandler;
        public event Action<GroupMessageModel> SendMessageToGroupMembersAsyncServerHandler;
        public event Action<GroupUser, string> SendNewGroupMemberToGroupMembersAsyncServerHandler;
        public event Action<GroupUser, string> RemoveGroupMembertToGroupMembersAsyncServerHandler;
        public event Action<string, Guid, string> ChangedGrouMessageToGroupMembersAsyncServerHanler;
        public event Action<Guid, Guid> SendGroupMessageReadStatusToGroupMemberAsyncServerHandler;

        public ChatGroupService(IChutConnection connection)
        {
            _connection = connection;

            _connection.Connection.On<GroupModel>(
                methodName: nameof(IGroupChat.SendGroupToGroupMembersAsync),
                handler: group => SendGorupToGroupMembersAsyncServerHandler?.Invoke(group));

            _connection.Connection.On<GroupMessageModel>(
                methodName: nameof(IGroupChat.SendMessageToGroupMembersAsync),
                handler: message => SendMessageToGroupMembersAsyncServerHandler?.Invoke(message));

            _connection.Connection.On<GroupUser, string>(
                methodName: nameof(IGroupChat.SendNewGroupMemberToGroupMembersAsync),
                handler: (user, groupName) => SendNewGroupMemberToGroupMembersAsyncServerHandler?.Invoke(user, groupName));

            _connection.Connection.On<GroupUser, string>(
                methodName: nameof(IGroupChat.RemoveGroupMembertToGroupMembersAsync),
                handler: (user, groupName) => RemoveGroupMembertToGroupMembersAsyncServerHandler?.Invoke(user, groupName));

            _connection.Connection.On<string, Guid, string>(
                methodName: nameof(IGroupChat.ChangeGroupMessageAsync),
                handler: (groupName, messageId, message) => ChangedGrouMessageToGroupMembersAsyncServerHanler?.Invoke(groupName, messageId, message));

            _connection.Connection.On<Guid, Guid>(
                methodName: nameof(IGroupChat.SendGroupMessageReadStatusAsync),
                handler: (grouId, messageId) => SendGroupMessageReadStatusToGroupMemberAsyncServerHandler?.Invoke(grouId, messageId));
        }

        public async Task AddUserToGroupAsync(GroupUser user, string groupName) 
        {
            await _connection.Connection.InvokeAsync("AddToGroup", user, groupName);
        }

        public async Task RemoveUserFormGroup(GroupUser user, string groupName) 
        {
            await _connection.Connection.InvokeAsync("RemoveFromGroup", user, groupName);
        }

        public async Task<GroupModel> CreateNewGroupAsync(string groupName, byte[] groupPhoto, List<GroupUser> users) 
        {
            return await _connection.Connection.InvokeAsync<GroupModel>("CreateGroup", groupName, groupPhoto, users);
        }

        public async Task<GroupMessageModel> SendGroupMessageAsync(string groupName, GroupMessageModel message) 
        {
            return await _connection.Connection.InvokeAsync<GroupMessageModel>("SendGroupMessage", groupName, message);
        }

        public async Task ChangeMessageAsync(string groupName, Guid messageId, string message) 
        {
            await _connection.Connection.InvokeAsync("ChangeGroupMessage", groupName, messageId, message);
        }

        public async Task ReadGroupMessageAsync(string senderConnectionId, string readerId, Guid groupId, Guid messageId) 
        {
            await _connection.Connection.InvokeAsync("ReadGroupMessage", senderConnectionId, readerId, groupId, messageId);
        }
    }
}
