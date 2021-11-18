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

        Task<ChatMessageModel> AddChatMessageAsync(ChatMessageModel message);

        Task<IEnumerable<UserModel>> GetUsersAsync(string userName);

        Task<FullUserModel> GetUserAsync(string userName);

        Task<bool> SetBlockOrMuteStateAsync(string id, bool isBlockOrMuted, bool doBlock);

        Task<BlockModel> SetUserBlackListStatusAsync(string userId, string blockedUserId, bool doesBlock);

        Task<bool> UpdateDisconnectTimeAsync(string userName, DateTime disconnectTime);

        Task<bool> SetNewPhotoAsync(string userName, byte[] photo);

        Task<bool> ChangeUserMessageAsync(Guid messageId, string message);

        Task ReadMessageAsync(Guid messageId);
    }
}
