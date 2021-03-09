using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
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
        private readonly IOptions<ApiBehaviorOptions> _apiBehaviorOptions;
        private readonly AppSettings _appSettigns;

        public AccountsController(UserManager<ApplicationUser> userManager,IOptions<AppSettings> appSettigns,IOptions<ApiBehaviorOptions> apiBehaviorOptions)
        {
            _userManager = userManager;
            _apiBehaviorOptions = apiBehaviorOptions;
            _appSettigns = appSettigns.Value;
        }

        // api/Accounts/Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            // gelen veriler geçerli mi?
            if (!ModelState.IsValid)
            {
                return ModelStateErrors();
            }
            //parola doğrulama
            var user = await _userManager.FindByNameAsync(dto.UserName);

            if (user != null && await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                //token oluşturma
                var authClaims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettigns.JwtSecret));
                var token = new JwtSecurityToken(
                    issuer: _appSettigns.JwtIssuer,
                    audience: _appSettigns.JwtIssuer,
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

            ModelState.AddModelError("InvalidCredentials", "Invalid username or password!");
            return ModelStateErrors();
        }

        [HttpPost("Register")]
        public async Task<IActionResult> register(RegisterDto dto)
        {
            // gelen veriler geçerli mi?
            if (!ModelState.IsValid)
            {
                return ModelStateErrors();
            }
            var user = new ApplicationUser()
            {
                UserName = dto.Email,
                Email = dto.Email
            };
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (result.Succeeded)
            {
                return Ok();
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }

            return ModelStateErrors();
        }

        private IActionResult ModelStateErrors()
        {
            return _apiBehaviorOptions.Value.InvalidModelStateResponseFactory(ControllerContext);
        }
    }
}
