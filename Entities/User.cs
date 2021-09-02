using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Chat.Entities
{
    public class User : IdentityUser
    {
        public virtual List<Chatter> Chatters { get; set; }

        public User() : base()
        {
            Chatters = new List<Chatter>();
        }
    }
}
