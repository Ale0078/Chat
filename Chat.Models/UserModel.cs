﻿using System.Collections.ObjectModel;

namespace Chat.Models
{
    public class UserModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ObservableCollection<ChatMessageModel> Messages { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsLogin { get; set; }

        public UserModel()
        {
            Messages = new ObservableCollection<ChatMessageModel>();
        }
    }
}
