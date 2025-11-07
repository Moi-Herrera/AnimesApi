using Microsoft.EntityFrameworkCore;
using AnimesApi.Data;
using AnimesApi.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

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
        public async Task<ActionResult<IEnumerable<Review>>> GetReviews()
        { 
            return await _context.Reviews.ToListAsync();
        }
        
        //Obtener review por id
        [HttpGet("{id}")]
        public async Task<ActionResult<Review>> GetReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return NotFound(); // si no existe
            return review;
        }

        //Crear review
        [HttpPost]
        public async Task<ActionResult<Review>> PostReview(Review review)
        {   
            //obtener el userid del token 
            var userId = int.Parse(User.FindFirst("sub")!.Value);

            review.UserId = userId;//asigna usuario de la review

            review.Fecha = DateTime.Now;//fecha automatica 

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetReview), new { id = review.Id }, review);
        }

        //modificar review
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReview(int id, Review review)
        {
            if (id != review.Id) 
                return BadRequest();// si el id es diferente devuelve error
            
            var userId = int.Parse(User.FindFirst("sub")!.Value);//obtener id del usuario

            //revisa que el la review pertenece al usuario y exista la review
            var existing = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
            
            if (existing == null) 
                return Unauthorized("El post no ha sido encontrado");

            //actualizar campos editables
            existing.Anime = review.Anime;
            existing.Puntuacion = review.Puntuacion;
            existing.Comentario = review.Comentario;

            await _context.SaveChangesAsync();

            return NoContent();

        }

        //eliminar una review solo el usuario 
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview( int id)
        {
            var userId = int.Parse(User.FindFirst("sub")!.Value);

            var review = await _context.Reviews.
                FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (review == null) 
                return NotFound("La review no puede ser borrada");

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            
            return NoContent();
        }
    }
}
