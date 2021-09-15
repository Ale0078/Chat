using System.Collections.Generic;

namespace Chat.Models
{
    public class FullUserModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsBlocked { get; set; }
        public List<ChatModel> Chats { get; set; }

        public FullUserModel()
        {
            Chats = new List<ChatModel>();
        }
    }
}