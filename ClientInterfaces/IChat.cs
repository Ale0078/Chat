using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Chat.Models;
using Chat.Interfaces.BaseInterfaces;

namespace Chat.Interfaces
{
    public interface IChat : IGroupChat//ToDo: change naming
    {
        Task SendConnectionsIdToCaller(IEnumerable<UserConnection> connectionsId);

        Task SendConnectionIdToCaller(string connectionId);

        Task Connect(string userName, string connectionId);

        Task Logout(string oldUser, DateTime disconnectTime);

        Task ReciveMessage(ChatMessageModel message);

        Task ChangeBlockStatusUserToUser(UserState state);

        Task ChangeBlockStatusUserToAllUsersExceptBlocked(string id, bool isBlocked);

        Task ChangeMuteStateUserToUser(bool isMuted);

        Task ChangeMuteStateUserToAllUsersExceptMuted(string userId, bool isMuted);

        Task SendBlackListStateToUser(BlockModel blockModel);

        Task SendTypingStatusToUser(bool isTyping, string typingUserId);

        Task ChangeUserPhotoToAllExceptChanged(string userName, byte[] photo);

        Task ChangeMessageToUserAsync(string userId, Guid messageId, string message);

        Task ReadMessageToUserAsync(string userId, Guid messageId);
    }
}
