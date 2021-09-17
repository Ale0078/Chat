using System.Collections.Generic;
using System.Threading.Tasks;

using Chat.Models;

namespace Chat.Interfaces
{
    public interface IChat//ToDo: change naming
    {
        Task SendConnectionsIdToCaller(IEnumerable<UserConnection> connectionsId);
        Task Connect(string userName, string connectionId);
        Task Logout(string oldUser);
        Task ReciveMessage(ChatMessageModel message);
        Task ChangeBlockStatusUserToUser(UserState state);
        Task ChangeBlockStatusUserToAllUsersExceptBlocked(string id, bool isBlocked);
        Task ChangeMuteStateUserToUser(bool isMuted);
        Task SendBlackListStateToUser(BlockModel blockModel);
    }
}
