using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chat.Entities
{
    public class GroupMessageReadStatus
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public bool IsRead { get; set; }

        public string UserId { get; set; }

        public Guid GroupMessageId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User Reader { get; set; }
    }
}
