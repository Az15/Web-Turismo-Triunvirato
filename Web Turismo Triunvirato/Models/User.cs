using System.ComponentModel.DataAnnotations;

namespace Web_Turismo_Triunvirato.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }

        public string Pais { get; set; }




        //public string Rol { get; set; } 


    }
}