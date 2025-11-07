using AnimesApi.Data;
using AnimesApi.Dtos;
using AnimesApi.Models;
using AnimesApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimesApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase  
    {
        //acceso a la base de datos y generar jwt
        private readonly AppDbContext _context;
        private readonly JwtService _jwt;

        //constructor del controlador
        public AuthController(AppDbContext context, JwtService jwt)
        {
            _context = context;
            _jwt = jwt;
        }

        //Registro de usuario
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] UserRegisterDto dto)
        {
            var exist = await _context.Users.AnyAsync(u => u.Username == dto.Username);

            var user = new User
            {
                Username = dto.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("usuario registrado correctamente");
        }

        //login 
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody]  UserLoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null) 
                return Unauthorized();

            bool isValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.Password);
            if (!isValid) 
                return Unauthorized();

            var token = _jwt.GenerateToken(user);

            return Ok(token);
        }
    }
}
