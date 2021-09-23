using System.Threading.Tasks;

using Chat.Models;

namespace Chat.Client.Services.Interfaces
{
    public interface IRegistrationChatService
    {
        Task ConnectAsync();
        Task<bool> RegisterUser(RegisterUserModel model);
        Task<LoginResult> LoginUser(string name, string password);
    }
}
