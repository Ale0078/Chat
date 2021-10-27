using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Chat.Entities
{
    public class Group
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public byte[] Photo { get; set; }

        public virtual List<GroupMessage> GroupMessages { get; set; }

        public virtual List<User> Users { get; set; }
    }
}
