using AnimesApi.Data;
using AnimesApi.Dtos;
using AnimesApi.Dtos.Common;
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
        private readonly JwtService _jwtServices;

        //constructor del controlador
        public AuthController(AppDbContext context, JwtService jwtServices)
        {
            _context = context;
            _jwtServices = jwtServices;
        }

        //Registro de usuario
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(UserRegisterDto dto)
        {
            //verifica que el correo y el usuario no esten en uso
            var userExist = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email || u.Username == dto.Username);

            if (userExist != null) {
                if (userExist.Email == dto.Email)
                    return BadRequest(new ApiResponse<string>("Correo electronico en uso"));

                if (userExist.Username == dto.Username)
                    return BadRequest(new ApiResponse<string>("Nombre de usuario en uso, intenta otro nombre"));
            }

            var user = new User
            {
                Email = dto.Email,
                Username = dto.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var dtoUser = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
            };

            return Ok(new ApiResponse<UserDto>(dtoUser,"usuario registrado correctamente"));
        }

        //login 
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<AuthReponseDto>>> Login([FromBody]  UserLoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password)) 
                return BadRequest(new ApiResponse<string>("Usuario y/o contransena incorrectos"));

            //generar token
            var accessToken = _jwtServices.GenerateToken(user);
            var refreshToken = _jwtServices.GenerateRefreshToken();

            //guardar refreshtoken en la base de datos
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            var response = new AuthReponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,   
            };

            return Ok(new ApiResponse<AuthReponseDto>(response, "Sesion iniciada"));
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthReponseDto>> Refresh([FromBody]string refreshToken)
        {
            //validar si el token es viene vacio o no es valido
            var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
            if (user == null) return 
                    Unauthorized(new ApiResponse<string>("token invalido"));

            if(!_jwtServices.IsRefreshTokenValid(user, refreshToken))
                return Unauthorized(new ApiResponse<string>("token expirado"));

            //generar nuevo token 
            var newAccess = _jwtServices.GenerateToken(user);
            var newRefresh = _jwtServices.GenerateRefreshToken();

            user.RefreshToken = newRefresh;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _context.SaveChangesAsync();

            var response = new AuthReponseDto
            {
                AccessToken = newAccess,
                RefreshToken = newRefresh
            };

            return Ok(new ApiResponse<AuthReponseDto>(response));
        }
    }
}
