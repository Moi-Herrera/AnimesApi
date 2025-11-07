using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimesApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        
        //relacion 1 a muchos reviews de usuarios
        public ICollection<Review> Reviews { get; set; }
    }
}
