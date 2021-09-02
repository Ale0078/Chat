using System;
using System.Collections.Generic;

namespace Chat.Models
{
    public class ChatModel
    {
        public Guid Id { get; set; }
        public string FirstUserId { get; set; }
        public string SecondUserId { get; set; }
        public List<ChatMessageModel> Messages { get; set; }
    }
}
