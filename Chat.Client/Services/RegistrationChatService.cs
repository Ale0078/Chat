using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

using Chat.Models;

namespace Chat.Client.Services
{
    public class RegistrationChatService//ToDo: full feahers
    {
        private const string URL = "http://localhost:5000/chat_register";

        private readonly HubConnection _connection;

        public RegistrationChatService()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl(URL)
                .Build();
        }

        public async Task Connect() 
        {
            await _connection.StartAsync();
        }

        public async Task RegisterUser(RegisterUserModel model) 
        {
            await _connection.InvokeAsync("Register", model);
        }
    }
}
