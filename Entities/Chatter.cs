using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chat.Entities
{
    public class Chatter
    {
        [Key]
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public Guid ChatId { get; set; }

        [ForeignKey(nameof(ChatId))]
        public virtual Chat Chat { get; set; }
    }
}
