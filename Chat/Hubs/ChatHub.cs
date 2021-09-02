using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

using Chat.Server.Services.Interfaces;
using Chat.Interfaces;
using Chat.Models;

namespace Chat.Server.Hubs
{
    [Authorize]
    public class ChatHub : Hub<IChat>
    {
        private readonly IUserService _userService;

        public ChatHub(IUserService userService)
        {
            _userService = userService;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            
            await Clients.Others.Connect(
                userName: Context.User.Identity.Name,
                connectionId: Context.ConnectionId);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Clients.Others.Logout(Context.User.Identity.Name);

            await base.OnDisconnectedAsync(exception);
        }

        public async Task<ChatMessageModel> ReciveMessage(Guid chatId, string fromUserId, string toUserId, string message, string connectionId) 
        {
            ChatMessageModel chatMessage = new()
            {
                ChatId = chatId,
                FromUserId = fromUserId,
                ToUserId = toUserId,
                Message = message,
                SendingTime = DateTime.Now
            };

            await _userService.AddChatMessageAsync(chatMessage);

            if (connectionId is not null || connectionId == string.Empty)
            {
                await Clients.Client(connectionId).ReciveMessage(chatMessage);
            }

            return chatMessage;
        }
    }
}