using System.Threading.Tasks;

using Chat.Client.Services.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;

namespace Chat.Client.Services
{
    public class ChutConnection : IChutConnection
    {
        private const string URL = "http://localhost:5000/chat";

        public ChutConnection()
        {
            Connection = new HubConnectionBuilder()
                .WithUrl(URL, options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(Token);
                })
                .Build();
        }

        public HubConnection Connection { get; set; }
        public string Token { get; set; }
    }
}
