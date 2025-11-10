using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimesApi.Dtos
{
    public  class ReviewDto
    {
        public int Id { get; set; }
        public string Anime { get; set; }
        public int Puntuacion { get; set; } // 1 al 10
        public string Comentario { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }

        //referencia a UserDto
        public UserDto User { get; set; }
    }
}
