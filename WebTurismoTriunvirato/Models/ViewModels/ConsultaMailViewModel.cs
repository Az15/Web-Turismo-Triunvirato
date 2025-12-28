using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Web_Turismo_Triunvirato.Models.ViewModels;

namespace Web_Turismo_Triunvirato.Models.ViewModels
{
    public class ConsultaMailViewModel
    {
        [Required]
        public int DestinyId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "El email es obligatorio.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El número de teléfono es obligatorio.")]
        public string Phone { get; set; }

        //[Required(ErrorMessage = "Por favor, especifique su consulta.")]
        public string? Message { get; set; }

        public string? Title { get; set; }
        public string? PackageDetailsSerialized { get; set; }

        // --- NUEVO: Lista de pasajeros ---
        public List<PasajeroViewModel> Pasajeros { get; set; } = new List<PasajeroViewModel>();
    }
}
