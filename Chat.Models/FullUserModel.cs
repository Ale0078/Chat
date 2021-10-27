using System;
using System.Collections.Generic;

namespace Chat.Models
{
    public class FullUserModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public byte[] Photo { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsMuted { get; set; }
        public DateTime DisconnectTime { get; set; }
        public List<ChatModel> Chats { get; set; }
        public List<BlockModel> BlockModels { get; set; }
        public List<GroupModel> Groups { get; set; }

        public FullUserModel()
        {
            Chats = new List<ChatModel>();
            BlockModels = new List<BlockModel>();
            Groups = new List<GroupModel>();
        }
    }
}