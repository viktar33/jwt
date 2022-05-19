using Common;
using jwt.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace jwt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IOptions<AuthOptions> options;

        public AuthController(IOptions<AuthOptions> options)
        {
            this.options = options;
        }

        private List<Account> Accounts => new List<Account>
        {
            new Account()
            {
                Id = 1,
                Email ="viktar.varabei@mail.ru",
                Password ="admin",
                Roles=new Role[]{ Role.Admin}
            },
              new Account()
            {
                Id = 2,
                Email ="@mail.ru",
                Password ="1",
                Roles=new Role[]{ Role.User}
            }
        };

        [Route("login")]
        [HttpPost]
        public IActionResult Login([FromBody] Login request)
        {
            var user = AuthenticationUser(request.Email, request.Password);

            if (user != null)
            {
                var token = GenerateJWT(user);

                return Ok(new { access_token = token });
            }

            return Unauthorized();
                
        }

        private Account AuthenticationUser(string email,string password)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return Accounts.FirstOrDefault(x => x.Email == email && x.Password == password);
#pragma warning restore CS8603 // Possible null reference return.
        }

        private string GenerateJWT(Account user)
        {
            var authParams = options.Value;

            var secutityKey = authParams.GetSummetricSecurityKey();

            var credentials = new SigningCredentials(secutityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim(JwtRegisteredClaimNames.Sub,user.Id.ToString())
            };

            foreach (var role in user.Roles)
            {
                claims.Add(new Claim("role", role.ToString()));
            }

            var token = new JwtSecurityToken(authParams.Issuer, authParams.Audience, claims, expires: DateTime.Now.AddSeconds(authParams.TokenLifeTime), signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
