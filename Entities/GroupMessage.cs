using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chat.Entities
{
    public class GroupMessage
    {
        [Key]
        public Guid Id { get; set; }

        public string TextMessage { get; set; }

        public byte[] FileMessage { get; set; }

        [Required]
        public bool IsEdit { get; set; }

        public string SenderId { get; set; }

        public Guid GroupId { get; set; }

        [Required]
        public DateTime SendingTime { get; set; }

        [ForeignKey(nameof(SenderId))]
        public virtual User SenderUser { get; set; }

        [ForeignKey(nameof(GroupId))]
        public virtual Group Group { get; set; }

        public virtual List<GroupMessageReadStatus> GroupMessageReadStatuses { get; set; }
    }
}
