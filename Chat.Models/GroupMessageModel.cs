using System;

namespace Chat.Models
{
    public class GroupMessageModel
    {
        public Guid Id { get; set; }

        public string TextMessage { get; set; }

        public byte[] FileMessage { get; set; }

        public bool IsEdit { get; set; }

        public bool IsRead { get; set; }

        public string SenderId { get; set; }

        public Guid GroupId { get; set; }

        public DateTime SendingTime { get; set; }
    }
}
