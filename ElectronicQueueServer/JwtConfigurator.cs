using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicQueueServer
{
    public class JwtConfigurator
    {
        private const string validIssuer = "ElectronicQueueServer";
        private const string validAudience = "ElectronicQueueClient";
        private const string secretWord = "secretkeybytes123";
        public TokenValidationParameters GetTokenValidationParameters()
        {
            return new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = validIssuer,
                ValidAudience = validAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretWord))
            };
        }

        public JwtSecurityToken GetJwtSecurityToken(string userRole)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretWord));
            var signingCreditionals = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            return new JwtSecurityToken(
                   issuer: validIssuer,
                   audience: validAudience,
                   claims: new List<Claim>() { new Claim(ClaimTypes.Role, userRole) },
                   expires: DateTime.Now.AddDays(10),
                   signingCredentials: signingCreditionals
                   );
        }
    }
}
