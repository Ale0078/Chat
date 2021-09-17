using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chat.Entities
{
    public class Block
    {
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public bool DoesBlock { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User Blocker { get; set; }
    }
}
