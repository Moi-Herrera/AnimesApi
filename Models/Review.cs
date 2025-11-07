using System;
using System.Collections.Generic;
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
        public required User? User { get; set; }  //navegacion

        public int Puntuacion { get; set; } // 1 al 10
        public string Comentario { get; set; } = string.Empty;
        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}
