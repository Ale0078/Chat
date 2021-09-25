using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Chat.Models;

namespace Chat.Server.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> AddUserAsync(RegisterUserModel userModel);

        Task<bool> CanLoginUserAsync(string userName, string password);

        Task<bool> AddChatMessageAsync(ChatMessageModel message);

        Task<IEnumerable<UserModel>> GetUsersAsync(string userName);

        Task<FullUserModel> GetUserAsync(string userName);

        Task<bool> SetBlockOrMuteStateAsync(string id, bool isBlockOrMuted, bool doBlock);

        Task<BlockModel> SetUserBlackListStatusAsync(string userId, string blockedUserId, bool doesBlock);

        Task<bool> UpdateDisconnectTime(string userName, DateTime disconnectTime);
    }
}
