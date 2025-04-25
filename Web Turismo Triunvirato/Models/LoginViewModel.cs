
using System.ComponentModel.DataAnnotations;

namespace Web_Turismo_Triunvirato.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El correo electr�nico es requerido.")]
        [EmailAddress(ErrorMessage = "Por favor, introduce un correo electr�nico v�lido.")]
        [Display(Name = "Correo Electr�nico")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contrase�a es requerida.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contrase�a")]
        public string Password { get; set; }
    }
}