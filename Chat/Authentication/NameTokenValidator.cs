using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;

namespace Chat.Server.Authentication
{
    public class NameTokenValidator : ISecurityTokenValidator
    {
        private const string ADMIN_NAME = "Admin";

        public bool CanValidateToken => true;
        public int MaximumTokenSizeInBytes { get; set; }

        public bool CanReadToken(string securityToken)
        {
            return true;
        }

        public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            validatedToken = null;

            ClaimsPrincipal principal = new(new List<ClaimsIdentity>
            {
                new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Name, securityToken)
                })
            });

            if (securityToken == ADMIN_NAME)
            {
                principal.AddIdentity(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Role, securityToken)
                }));
            }

            return principal;
        }
    }
}
