using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountsController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // api/Accounts/Login
        [HttpPost("Login")]
        public async Task<ActionResult> Login(LoginDto dto)
        {
            //parola doğrulama
            var user = await _userManager.FindByNameAsync(dto.UserName);

            if (user != null && await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                //token oluşturma
                var authClaims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, dto.UserName)
                };

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("u7dfdfsa111xxAAbbCCDDFF3345"));
                var token = new JwtSecurityToken(
                    issuer: "https://localhost",
                    audience: "https://localhost",
                    expires: DateTime.Now.AddDays(14),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey,SecurityAlgorithms.HmacSha256)
                );

                return Ok(new 
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }


            return Unauthorized();
        }
    }
}
