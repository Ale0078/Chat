using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Chat.Entities
{
    public class User : IdentityUser
    {
        public bool IsBlocked { get; set; }
        public bool IsMuted { get; set; }
        public byte[] Photo { get; set; }
        public virtual List<Chatter> Chatters { get; set; }
        public virtual List<BlockedUser> BlockedUsers { get; set; }

        public User() : base()
        {
            Chatters = new List<Chatter>();
            BlockedUsers = new List<BlockedUser>();
        }
    }
}
