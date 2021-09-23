using System;
using System.Threading.Tasks;

using Chat.Models;

namespace Chat.Client.Services.Interfaces
{
    public interface IChatService
    {
        Task ConnectAsync();
        Task<ChatMessageModel> ReciveMessageUserAsync(Guid chatId, string fromUserId, string toUserId, string message, string connectionId);
        Task<bool> SetBlockStateToUserAsync(string userId, string connectionId, bool isBlocked);
        Task<bool> SetMuteStateToUserAsync(string userId, string connectionId, bool isMuted);
        Task<BlockModel> SendBlackListStateAsync(string userId, string connectionId, string blockedUserId, bool doesBlock);
    }
}
