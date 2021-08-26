using System.Threading.Tasks;

using Chat.Models;

namespace Chat.Interfaces
{
    public interface IChatRegistration
    {
        Task RegisterUser(UserModel newUser);
        Task LoginUser(UserModel newUser);
    }
}
