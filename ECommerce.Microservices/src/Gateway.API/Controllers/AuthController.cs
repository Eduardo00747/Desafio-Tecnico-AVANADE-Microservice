using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Gateway.API.Models;
using Gateway.API.Repositories;

namespace Gateway.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IAuthRepository _authRepo;

        public AuthController(IConfiguration config, IAuthRepository authRepo)
        {
            _config = config;
            _authRepo = authRepo;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Username) ||
                string.IsNullOrWhiteSpace(model.Password))
            {
                return BadRequest("❌ Usuário e senha são obrigatórios e não podem estar vazios.");
            }

            if (model.Username == "string" || model.Password == "string")
            {
                return BadRequest("❌ Usuário e senha não podem conter o valor 'string'.");
            }

            if (model.Username.Contains(" ") || model.Password.Contains(" "))
            {
                return BadRequest("❌ Usuário e senha não podem conter espaços em branco.");
            }

            var user = await _authRepo.GetUserAsync(model.Username, model.Password);
            if (user == null)
            {
                return Unauthorized("❌ Usuário ou senha inválidos.");
            }

            var jwtKey = _config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key não configurada");
            var key = Encoding.ASCII.GetBytes(jwtKey);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, "User")
                }),
                Issuer = "ECommerceAuth",
                Audience = "ECommerceClients",
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new { token = tokenHandler.WriteToken(token) });
        }
        

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] LoginModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Username) || 
                string.IsNullOrWhiteSpace(model.Password))
            {
                return BadRequest("❌ Usuário e senha são obrigatórios.");
            }

            var existingUser = await _authRepo.GetUserByUsernameAsync(model.Username);
            if (existingUser != null)
            {
                return BadRequest("❌ Usuário já existe.");
            }

            var user = new User
            {
                Username = model.Username,
                Password = model.Password
            };

            await _authRepo.CreateUserAsync(user);

            return Ok("✅ Usuário criado com sucesso!");
        }
    }
}