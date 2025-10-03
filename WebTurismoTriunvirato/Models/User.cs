using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace Web_Turismo_Triunvirato.Models

{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public int Whatsapp_Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [DisplayName("Contraseña")]
        [Required]
        public string Password { get; set; }

        [Required(ErrorMessage = "El Name es obligatorio.")]
        [StringLength(100)]
        [DisplayName("Nombre")]
        public string Name { get; set; }

        [DisplayName("Apellido")]
        [Required(ErrorMessage = "El Surname es obligatorio.")]
        [StringLength(100)]
        public string Surname { get; set; }

        [DisplayName("Pais")]
        public string Country { get; set; }

        public string Rol { get; set; } 

    }
}