using System;

namespace Chat.Models
{
    public class GroupUser
    {
        public string Id { get; set; }
        public string ConnectionId { get; set; }
        public string Name { get; set; }
        public byte[] Photo { get; set; }
        public DateTime DisconnectTime { get; set; }
        public UserRole Role { get; set; }
    }
}
