using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Estoque.API.Repositories;
using Estoque.API.Models;

namespace Estoque.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            // 🔒 1️⃣ Validação geral de campos obrigatórios
            if (string.IsNullOrWhiteSpace(model.Username) ||
                string.IsNullOrWhiteSpace(model.Password))
            {
                return BadRequest("❌ Usuário e senha são obrigatórios e não podem estar vazios.");
            }

            // 🔒 2️⃣ Impede os valores padrão "string"
            if (model.Username == "string" || model.Password == "string")
            {
                return BadRequest("❌ Usuário e senha não podem conter o valor 'string'.");
            }

            // 🔒 3️⃣ Impede espaços em branco dentro do login ou senha
            if (model.Username.Contains(" ") || model.Password.Contains(" "))
            {
                return BadRequest("❌ Usuário e senha não podem conter espaços em branco.");
            }

            // 🔍 (Opcional) Caso ainda queira validar com o banco:
            var user = await _authRepo.GetUserAsync(model.Username, model.Password);
            if (user == null)
            {
                return Unauthorized("❌ Usuário ou senha inválidos.");
            }

            // 🔑 Gera token JWT
            var jwtSection = _config.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSection["Key"]!);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, "User")
                }),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSection["ExpiryMinutes"] ?? "60")),
                Issuer = jwtSection["Issuer"],
                Audience = jwtSection["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new { token = tokenHandler.WriteToken(token) });
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] LoginModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
                return BadRequest("❌ Usuário e senha são obrigatórios.");

            var existingUser = await _authRepo.GetUserByUsernameAsync(model.Username);
            if (existingUser != null)
                return BadRequest("❌ Usuário já existe.");

            var user = new User
            {
                Username = model.Username,
                Password = model.Password
            };

            await _authRepo.CreateUserAsync(user);

            return Ok("✅ Usuário criado com sucesso!");
        }
    }
    
    public class LoginModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}