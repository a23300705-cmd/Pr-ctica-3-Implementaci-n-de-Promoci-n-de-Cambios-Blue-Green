using MicroscopioBackend.Models;
using MicroscopioBackend.Services._Interfaces.Auth;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MicroscopioBackend.Services.Auth
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(Usuario usuario)
        {
            var claims = new[]
            {
            new Claim("id_usuario", usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Email)
        };

            var keyString = Environment.GetEnvironmentVariable("JWT_KEY");

            if (string.IsNullOrEmpty(keyString))
            {
                throw new Exception("JWT_KEY no está configurada en variables de entorno");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
