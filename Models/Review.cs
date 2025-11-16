using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimesApi.Models
{
    public class Review
    {
        public int Id { get; set; }
        public required string Anime { get; set; }
        
        //relacion tabla usuario
        public int UserId { get; set; } //Fk
        public User? User { get; set; }  //navegacion

        [Range(1, 10, ErrorMessage ="puntuacion del 1 al 10")]
        public int Puntuacion { get; set; } // 1 al 10
        public string Comentario { get; set; } = string.Empty;
        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}
