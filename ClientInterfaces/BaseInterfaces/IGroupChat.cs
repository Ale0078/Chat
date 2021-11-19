using System;
using System.Threading.Tasks;

using Chat.Models;

namespace Chat.Interfaces.BaseInterfaces
{
    public interface IGroupChat
    {
        Task SendGroupToGroupMembersAsync(GroupModel group);

        Task SendMessageToGroupMembersAsync(GroupMessageModel message);

        Task SendNewGroupMemberToGroupMembersAsync(GroupUser user, string groupName);

        Task RemoveGroupMembertToGroupMembersAsync(GroupUser user, string groupName);

        Task ChangeGroupMessageAsync(string groupName, Guid messageId, string message);

        Task SendGroupMessageReadStatusAsync(Guid groupId, Guid messageId);
    }
}
