using AnimesApi.Data;
using AnimesApi.Dtos;
using AnimesApi.Models;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AnimesApi.Controllers
{
    [ApiController]
    [Route("api/reviews")]// ruta del controlador 
    [Authorize]
    public class ReviewController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReviewController(AppDbContext context)
        {
            _context = context;
        }

        //obtener todas las reviews
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviews()
        {
                return await _context.Reviews
               .Include(r => r.User)
               .Select(r => new ReviewDto
               {
                   Id = r.Id,
                   Anime = r.Anime,
                   Puntuacion = r.Puntuacion,
                   Comentario = r.Comentario,
                   Fecha = r.Fecha,
                   User = new UserDto
                   {
                       Id = r.User.Id,
                       Username = r.User.Username
                   }
               })
               .ToListAsync();

        }

        //obtener todas las reviews
        [HttpGet("my-reviews")]
        public async Task<ActionResult<PagedResult<ReviewDto>>> GetMyReviews(int page= 1, int pageSize=10)
        {

            //verificar el usuario 
            var UserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (UserIdClaim == null)
                return Unauthorized("usuario no autorizado");
            
            var userId = int.Parse(UserIdClaim.Value);

            var totalCount = await _context.Reviews
           .Where(r => r.UserId == userId)
           .CountAsync();

            var reviews = await _context.Reviews
            .Where(r => r.UserId == userId)
            .OrderByDescending(r=> r.Fecha)
            .Skip((page - 1 ) * pageSize)
            .Take(pageSize)
            .Select(r => new ReviewDto
               {
                   Id = r.Id,
                   Anime = r.Anime,
                   Puntuacion = r.Puntuacion,
                   Comentario = r.Comentario,
                   Fecha = r.Fecha,
                   User = new UserDto
                   {
                       Id = r.User.Id,
                       Username = r.User.Username
                   }
               })
            .ToListAsync();

            return Ok(new PagedResult<ReviewDto>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                Items = reviews
            });
        }

        //Obtener review por id
        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewDto>> GetReview(int id)
        {
            var review = await _context.Reviews
                .Include(r => r.User)
               .Select(r => new ReviewDto
               {
                   Id = r.Id,
                   Anime = r.Anime,
                   Puntuacion = r.Puntuacion,
                   Comentario = r.Comentario,
                   Fecha = r.Fecha,
                   User = new UserDto
                   {
                       Id = r.User.Id,
                       Username = r.User.Username
                   }
               })
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null) return NotFound(); // si no existe
            return review;
        }

        //Crear review
        [HttpPost]
        public async Task<ActionResult<ReviewDto>> PostReview(Review review)
        {
            //verificar el usuario 
            var UserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (UserIdClaim == null) 
                return Unauthorized("usuario no autorizado token");

            //obtener el userid del token 
            var userId = int.Parse(UserIdClaim.Value);

            review.UserId = userId;//asigna usuario de la review
            review.Fecha = DateTime.Now;//fecha automatica 

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            //json de la review creada
            var dto = await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.Id == review.Id)
                .Select(r => new ReviewDto
                {
                    Id = r.Id,
                    Anime = r.Anime,
                    Puntuacion = r.Puntuacion,
                    Comentario = r.Comentario,
                    Fecha = r.Fecha,
                    User = new UserDto
                    {
                        Id = r.User.Id,
                        Username = r.User.Username
                    }
                })
                .FirstOrDefaultAsync();

            return CreatedAtAction(nameof(GetReview), new { id = review.Id }, dto);
        }

        //Update review
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReview(int id, ReviewUpdateDto reviewDto)
        {
            //verificar el usuario
            var UserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (UserIdClaim == null)
                return Unauthorized("token invalido");

            var userId = int.Parse(UserIdClaim.Value);//obtener id del usuario

            //la review pertenece al usuario y exista la review
            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == id);
            
            if (review == null) 
                return Unauthorized("Review no encontrada");

            //verificar que la review pertenece al usuario
            if (review.UserId != userId)
                return StatusCode(403,"Review no puede ser editada");

            //actualizar campos editables
            review.Anime = reviewDto.Anime;
            review.Puntuacion = reviewDto.Puntuacion;
            review.Comentario = reviewDto.Comentario;

            await _context.SaveChangesAsync();

            //cargar usuario
            await _context.Entry(review).Reference(r => r.User).LoadAsync();

            //mapear datos
            var dto = new ReviewDto
            {
                Id = review.Id,
                Anime = review.Anime,
                User = new UserDto
                {
                    Id = review.User.Id,
                    Username = review.User.Username
                },
                Puntuacion = review.Puntuacion,
                Comentario = review.Comentario,
                Fecha = review.Fecha
            };

            return Ok(dto);

        }

        //eliminar una review solo el usuario 
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview( int id)
        {
            ////verificar el usuario
            var UserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (UserIdClaim == null)
                return Unauthorized("usuario no autorizado token");

            var userId = int.Parse(UserIdClaim.Value);

            //buscar la review
            var review = await _context.Reviews.FindAsync(id);

            if (review == null)
                return NotFound("La review no existe");

            //validar que la review pertenece al usuario
            if (review.UserId != userId)
                return StatusCode(403, "Revire no puede ser eliminada");
          

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return Ok("Review eliminada") ;
        }
    }
}
