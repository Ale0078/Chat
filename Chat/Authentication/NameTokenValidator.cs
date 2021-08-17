using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;

namespace Chat.Server.Authentication
{
    public class NameTokenValidator : ISecurityTokenValidator
    {
        public bool CanValidateToken => true;
        public int MaximumTokenSizeInBytes { get; set; }

        public bool CanReadToken(string securityToken)
        {
            return true;
        }

        public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            validatedToken = null;

            return new ClaimsPrincipal(new List<ClaimsIdentity>
            {
                new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Name, securityToken)
                })
            });
        }
    }
}
