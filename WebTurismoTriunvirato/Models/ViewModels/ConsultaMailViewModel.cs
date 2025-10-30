using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Web_Turismo_Triunvirato.Models.ViewModels
{
    public class ConsultaMailViewModel
    {
        // Campo oculto para llevar la referencia al paquete (se pasa desde la URL)
        [Required]
        public int DestinyId { get; set; }

        // Datos del Usuario
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [DisplayName("Nombre Completo")]
        public string Name { get; set; }

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de email incorrecto.")]
        [DisplayName("Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El número de teléfono es obligatorio.")]
        [Phone(ErrorMessage = "Formato de teléfono incorrecto.")]
        [DisplayName("Teléfono/WhatsApp")]
        public string Phone { get; set; }

        [DisplayName("Mensaje")]
        [Required(ErrorMessage = "Por favor, especifique su consulta.")]
        [MinLength(10, ErrorMessage = "El mensaje debe tener al menos 10 caracteres.")]
        public string Message { get; set; }

        // Propiedad auxiliar para mostrar el Título del Paquete al usuario
        // No necesita ser persistida, solo se usa en la vista.
        public string? DestinyTitle { get; set; }
        public string PackageDetailsSerialized { get; set; }
    }
}
