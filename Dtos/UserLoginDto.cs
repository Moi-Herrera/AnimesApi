using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimesApi.Dtos
{
    public class UserLoginDto
    {
        [Required(ErrorMessage ="Correo electronico obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido.")]
        public string Email { get; set; }

       
        public string Password { get; set; }
    }
}
