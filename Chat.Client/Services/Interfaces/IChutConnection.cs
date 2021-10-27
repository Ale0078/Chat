using Microsoft.AspNetCore.SignalR.Client;

namespace Chat.Client.Services.Interfaces
{
    public interface IChutConnection
    {
        public HubConnection Connection { get; set; }
        public string Token { get; set; }
    }
}
