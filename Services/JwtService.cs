using AnimesApi.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AnimesApi.Services
{
    public class JwtService
    {
        private readonly IConfiguration _config;

        private readonly SymmetricSecurityKey _key;

        public JwtService(IConfiguration config)
        {
            _config = config;

            _key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"])
            );
        }
        
        //genera el token con la informacion del usuario
        public string GenerateToken(User user)
        {
            
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);

            //información dentro del token
            var claims = new[]
            {
                 new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),//id del usuario
                 new Claim(JwtRegisteredClaimNames.Email, user.Email)//email del usuario
            };

            //construccion del token
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"], // Quién emite el token
                audience: _config["Jwt:Audience"],// Quién debería aceptarlo
                claims: claims,// Datos incluidos
                expires: DateTime.UtcNow.AddMinutes(1),// Duración del token
                signingCredentials: creds// Firma del token
                );

            //token a string para retornarlo
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        //crear refresh token aleatorio
        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        //validar si un token aun no ha expirado
        public bool IsRefreshTokenValid(User user, string refreshToken)
        {
            return user.RefreshToken == refreshToken &&
                user.RefreshTokenExpiryTime > DateTime.UtcNow;
        }
    }
}