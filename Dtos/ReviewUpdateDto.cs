using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimesApi.Dtos
{
    public class ReviewUpdateDto
    {
        public string Anime {  get; set; }
        public int Puntuacion { get; set; }    
        public string Comentario { get; set; }

    }
}
