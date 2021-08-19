using System.Threading.Tasks;

using Chat.Models;

namespace Chat.Interfaces
{
    public interface IChatRegistration
    {
        Task RegisterUser(User newUser);
        Task LoginUser(User newUser);
    }
}
