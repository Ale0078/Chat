using System;
using System.Collections.Generic;

namespace Chat.Models
{
    public class GroupModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public byte[] Photo { get; set; }
        public List<GroupUser> Users { get; set; }
        public List<GroupMessageModel> GroupMessages { get; set; }
    }
}
