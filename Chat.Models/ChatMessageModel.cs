using System;

namespace Chat.Models
{
    public class ChatMessageModel
    {
        public Guid Id { get; set; }
        public Guid ChatId { get; set; }
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
        public string Message { get; set; }
        public bool IsFromCurrentUser { get; set; }
        public DateTime SendingTime { get; set; }
        public bool IsEdit { get; set; }
        public byte[] ByteFile { get; set; }
    }
}
