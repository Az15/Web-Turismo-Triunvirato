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

        [Required(ErrorMessage = "El Nombre es obligatorio.")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "El Apellido es obligatorio.")]
        [StringLength(100)]
        public string Surname { get; set; }

        public string Country { get; set; }

        //public string Rol { get; set; } 


    }
}