using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using Microsoft.AspNetCore.Mvc;
using API.Entities;
using System.Security.Cryptography;
using System.Text;
using API.DTO;
using Microsoft.EntityFrameworkCore;
using API.Interfaces;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
       
        private readonly iTokenService _TokenService;
        
        public AccountController(DataContext  context , iTokenService TokenService)
        {
            _TokenService = TokenService;
            
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register (RegisterDTO registerDTO)
        {
            if( await  UserExists (registerDTO.Username)) return BadRequest("User already exists");
            using var hmac = new HMACSHA512();
            var user = new AppUser
            {
               UserName = registerDTO.Username,
               PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
               PasswordSalt = hmac.Key
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return new UserDTO{
                Username = user.UserName,
                Token  = _TokenService.createToken(user)
            };
 
        }

        [HttpPost("login")]

        public async Task<ActionResult<UserDTO>> Login (LoginDTO logindto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x =>x.UserName == logindto.username);

            if (user == null) return Unauthorized("Invalid User Name ");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(logindto.password));

            for(int i = 0 ; i <  computedHash.Length; i++ )
            {
                if(computedHash[i] != user.PasswordHash[i] ) return Unauthorized("Invalid Password");
            }

             return new UserDTO{
                Username = user.UserName,
                Token  = _TokenService.createToken(user)
            };

        }

        private async Task<Boolean> UserExists(string username )
        {
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }

    }
}