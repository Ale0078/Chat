using System.Collections.Generic;

namespace Chat.Models
{
    public class LoginResult
    {
        public string Token { get; set; }
        public FullUserModel CurrentUser { get; set; }
        public IEnumerable<UserModel> Users { get; set; }
    }
}
