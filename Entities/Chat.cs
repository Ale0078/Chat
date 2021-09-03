using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chat.Entities
{
    public class Chat
    {
        [Key]
        public Guid Id { get; set; }

        public string FirstUserId { get; set; }

        public string SecondUserId { get; set; }

        [ForeignKey(nameof(FirstUserId))]
        public virtual User FirstUser { get; set; }

        [ForeignKey(nameof(SecondUserId))]
        public virtual User SecondUser { get; set; }

        public virtual List<ChatMessage> ChatMessages { get; set; }

        public Chat()
        {
            ChatMessages = new List<ChatMessage>();
        }
    }
}
