using System.Collections.Generic;
using System.Threading.Tasks;

using Chat.Models;

namespace Chat.Interfaces
{
    public interface IChatRegistration
    {
        Task SendUserStateToCaller(UserState userState);
        Task SendListOfUsersToCaller(IEnumerable<UserModel> users);
        Task SendCurrentUserToCaller(FullUserModel user);
        Task SendTokenToClaller(string token);
        Task RegisterUserToOthers(FullUserModel newUser);
        Task SendBlockersToCaller(IEnumerable<BlockModel> blocks);
    }
}
