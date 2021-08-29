using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Chat.Entities
{
    public class User : IdentityUser
    {
        public List<Chatter> Chatters { get; set; }
    }
}
