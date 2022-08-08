using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService : iTokenService
    {
        private readonly SymmetricSecurityKey _key;
        public TokenService(IConfiguration config)
        { 
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));

        }

        public string createToken(AppUser user)
        {
            var Claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.UserName)
            };
            var Creds = new SigningCredentials(_key , SecurityAlgorithms.HmacSha512Signature);

            var Tokendescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(Claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = Creds
                
            };
            var tokenHandler = new JwtSecurityTokenHandler();
          var   token =   tokenHandler.CreateToken(Tokendescriptor);
          return tokenHandler.WriteToken(token);
        }
    }

}