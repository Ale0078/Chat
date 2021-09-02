using Chat.Models;
using System.Threading.Tasks;

namespace Chat.Interfaces
{
    public interface IChat
    {
        //Task Login(UserModel newUser);
        //Task GetConnectionIdToOthers(string userId, string connectionId);
        Task Connect(string userName, string connectionId);
        Task Logout(string oldUser);
        Task ReciveMessage(ChatMessageModel message);
    }
}
