using System.Collections.Generic; // Asegúrate de incluir esto para usar List<>
using System.ComponentModel.DataAnnotations;

namespace Web_Turismo_Triunvirato.Models.ViewModels
{
    public class ModalContratoViewModel
    {
        public int IdItem { get; set; }
        public string Title { get; set; }

        // URL de la imagen principal (portada)
        public string ImageUrl { get; set; }

        // --- NUEVA PROPIEDAD PARA LA GALERÍA ---
        // Aquí guardaremos todas las rutas de la colección de imágenes que se subieron
        public List<Imagen> GaleriaImagenes { get; set; }

        public string LongDescription { get; set; }
        public string LongDescription2 { get; set; }

        public decimal Price { get; set; }

        public string DetailText { get; set; }

        public string RenderedWhatsappMessage { get; set; }

        public string WhatsappNumber { get; set; } = "+541122296405";

        [Required]
        public string PackageDetailsSerialized { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Message { get; set; }
        public string DocumentoResponsable { get; set; }

        public List<PasajeroViewModel> Pasajeros { get; set; } = new List<PasajeroViewModel>();


    }
}