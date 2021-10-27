using System.Collections.Generic;
using System.Threading.Tasks;

using Chat.Models;

namespace Chat.Server.Services.Interfaces
{
    public interface IGroupService
    {
        Task<List<string>> GetListOfGroupNamesByUserNameAsync(string userName);

        Task<GroupModel> CreateGroupAsync(string groupName, byte[] groupPhoto, List<GroupUser> users);

        Task<GroupMessageModel> CreateGroupMessageAsync(GroupMessageModel message);

        Task<bool> AddGroupUserToGroup(GroupUser user, string groupName);

        Task<bool> RemoveGroupUserFromGroup(GroupUser user, string groupName);
    }
}
