﻿using System;
using System.Collections.ObjectModel;

namespace Chat.Models
{
    public class UserModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ConnectionId { get; set; }
        public Guid ChatId { get; set; }
        public byte[] Photo { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsLogin { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsMuted { get; set; }
        public DateTime DisconnectTime { get; set; }
        public ObservableCollection<ChatMessageModel> Messages { get; set; }
        
        public UserModel()
        {
            Messages = new ObservableCollection<ChatMessageModel>();
        }
    }
}
