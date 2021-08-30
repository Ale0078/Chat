using System.Collections.Generic;
using System.Threading.Tasks;

using Chat.Models;

namespace Chat.Server.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> AddUserAsync(RegisterUserModel userModel);

        Task<bool> CanLoginUserAsync(string userName);

        Task<bool> AddChatMessageAsync(ChatMessageModel message);

        Task<IEnumerable<UserModel>> GetUsersAsync(string exceptUserName = null);

        Task<UserModel> GetUserAsync(string userName);
    }
}
