using ClienteAPI.Identity;
using ClienteAPI.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System; 

namespace ClienteAPI.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        /// <summary>
        /// Registra um novo usuário na aplicação.
        /// </summary>
        /// <param name="model">Dados para o registro do usuário.</param>
        /// <returns>Status de sucesso ou erro do registro.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email, // Usamos o Email como UserName para o Identity
                Email = model.Email,
                Nome = model.Nome
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Opcional: Adicionar o usuário a uma role padrão após o registro
                // Ex: await _userManager.AddToRoleAsync(user, "User");

                return Ok(new { Message = "Usuário registrado com sucesso!" });
            }

            return BadRequest(new { Errors = result.Errors.Select(e => e.Description) });
        }

        /// <summary>
        /// Realiza o login de um usuário e gera um token JWT.
        /// </summary>
        /// <param name="model">Credenciais de login (email e senha).</param>
        /// <returns>Token JWT se o login for bem-sucedido.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized(new { Message = "Credenciais inválidas." });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (result.Succeeded)
            {
                var token = GenerateJwtToken(user);
                return Ok(new { Token = token });
            }

            return Unauthorized(new { Message = "Credenciais inválidas." });
        }

        /// <summary>
        /// Gera um token JWT para o usuário autenticado.
        /// </summary>
        /// <param name="user">O usuário para o qual o token será gerado.</param>
        /// <returns>Uma string contendo o token JWT.</returns>
        private string GenerateJwtToken(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty)
            };

            // Adiciona o nome personalizado do usuário aos claims
            if (!string.IsNullOrEmpty(user.Nome))
            {
                claims.Add(new Claim("NomeCompleto", user.Nome));
            }

            // Opcional: Adicionar roles do usuário aos claims do token
            // var roles = await _userManager.GetRolesAsync(user);
            // foreach (var role in roles)
            // {
            //     claims.Add(new Claim(ClaimTypes.Role, role));
            // }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"] ??
                throw new InvalidOperationException("A chave secreta JWT não está configurada no appsettings.json."))); // Mensagem mais clara

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Define a expiração do token com base na configuração.
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["Jwt:ExpiresInDays"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}