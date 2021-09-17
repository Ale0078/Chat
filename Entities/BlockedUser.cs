using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chat.Entities
{
    public class BlockedUser
    {
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public Guid BlockerId { get; set; }

        [ForeignKey(nameof(BlockerId))]
        public virtual Block Blocker { get; set; }
    }
}
