using System.Collections.Generic;
using System.Threading.Tasks;

using Chat.Models;

namespace Chat.Interfaces
{
    public interface IChat//ToDo: change naming
    {
        //Task Login(UserModel newUser);
        //Task GetConnectionIdToOthers(string userId, string connectionId);
        Task SendConnectionsIdToCaller(IEnumerable<UserConnection> connectionsId);
        Task Connect(string userName, string connectionId);
        Task Logout(string oldUser);
        Task ReciveMessage(ChatMessageModel message);
    }
}
