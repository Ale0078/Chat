using Chat.Models;
using System.Threading.Tasks;

namespace Chat.Interfaces
{
    public interface IChat
    {
        Task Login(UserModel newUser);
        Task Logout(UserModel oldUser);
        Task ReciveMessage(ChatMessageModel message);
    }
}
