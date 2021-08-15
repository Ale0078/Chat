using Chat.Models;
using System.Threading.Tasks;

namespace Chat.Interfaces
{
    public interface IChat
    {
        Task Login(User newUser);
        Task Logout(User oldUser);
        Task ReciveMessage(ChatMessage message);
    }
}
