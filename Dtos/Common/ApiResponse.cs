using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimesApi.Dtos.Common
{
    // Clase genérica para todas las respuestas
    public class ApiResponse<T>
    {
        public bool Success { get; set; }// indica si fue exitoso
        public string? Message { get; set; } // mensaje informacion o error)
        public T? Data { get; set; } // contenido

        // Constructor para respuestas exitosas
        public ApiResponse(T data, string? message = null )
        {
            Success = true;
            Message = message;
            Data = data;
        }

        // éxito (sin data)
        public ApiResponse(string? message = null)
        {
            Success = true;
            Message = message;
        }

        // Constructor para errores
        public ApiResponse<T> Fail(string message) 
        {
            return new ApiResponse<T> 
            {
                Success = false,
                Message = message
            };      
        }
    }
}
