using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chat.Entities
{
    public class ChatMessage
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Message { get; set; }

        public Guid ChatId { get; set; }

        public string ReceiverId { get; set; }

        public string SenderId { get; set; }

        [Required]
        public DateTime Time { get; set; }

        [ForeignKey(nameof(ReceiverId))]
        public virtual User Receiver { get; set; }

        [ForeignKey(nameof(SenderId))]
        public virtual User Sender { get; set; }
    }
}