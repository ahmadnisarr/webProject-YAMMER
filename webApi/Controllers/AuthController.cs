using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application;
using Infrastructure;
namespace webApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly string _key;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly Iuser _user;

        public AuthController(IConfiguration config,Iuser user)

        {
            _user = user;
            _key = config["Jwt:Key"];

            _issuer = config["Jwt:Issuer"];

            _audience = config["Jwt:Audience"];
        }


        [HttpPost("token")]
        public IActionResult GenerateToken(string Email, string Password)
        {

            if (!string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password)) 
            {
                if (_user.isExist(Email, Password))
                {

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.UTF8.GetBytes(_key);

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new[]
                        {
                                new Claim(ClaimTypes.Name,Email)
                        }),
                        Expires = DateTime.UtcNow.AddHours(1),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256),
                        Issuer = _issuer,
                        Audience = _audience
                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    return Ok(new { Token = tokenHandler.WriteToken(token) });
                }
                return Unauthorized();//401
            }
            return BadRequest("Invalid Data");//400
        }


    }
}
