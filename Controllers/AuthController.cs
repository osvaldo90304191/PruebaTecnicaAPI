using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using PruebaTecnicaAPI.Models; // La clase 'LoginRequest' y 'User' se usarán aquí

namespace PruebaTecnicaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        // Constructor para inyectar la configuración, que usaremos para obtener la clave secreta
        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Modelo de datos del usuario, hardcodeado por praticidad
        private static readonly User hardcodedUser = new User
        {
            Email = "usuario@prueba.com",
            Password = "password123"
        };

        // Endpoint POST para /api/Auth/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            // 1. Valida las credenciales proporcionadas por el usuario.
            if (loginRequest.Email != hardcodedUser.Email || loginRequest.Password != hardcodedUser.Password)
            {
                return Unauthorized(); // Devuelve 401 Unauthorized si las credenciales son incorrectas.
            }

            // 2. Si las credenciales son válidas, generar un token JWT.
            var token = GenerateJwtToken();

            // 3. Devolver el token generado en un objeto anónimo.
            return Ok(new { token });
        }

        // Método privado para generar el token JWT.
        private string GenerateJwtToken()
        {
            // Obtiene la clave secreta desde la configuración (Program.cs).
            var secretKey = _configuration["Jwt:Key"] ?? "EstaEsUnaClaveSuperSecretaParaTuPruebaJWTDeBackendCon.NETCoreYDebeSerLoSuficientementeLargaParaSerSegura";
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Crea los claims del token, que son pares de clave-valor que representan la información del usuario.
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "PruebaTecnicaAPI"), // 'Subject' del token
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // 'ID' del token
                new Claim(ClaimTypes.NameIdentifier, hardcodedUser.Email),
                new Claim(ClaimTypes.Email, hardcodedUser.Email),
                new Claim(ClaimTypes.Role, "Admin") // Un ejemplo de un claim de rol para permisos.
            };

            // Configura los parámetros del token.
            var token = new JwtSecurityToken(
                issuer: "PruebaTecnicaAPI",
                audience: "PruebaTecnicaAPI",
                claims: claims,
                expires: DateTime.Now.AddHours(1), // Vigencia del token: 1 hora desde la emisión.
                signingCredentials: credentials);

            // Serializa el token a una cadena de texto y lo devuelve.
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}