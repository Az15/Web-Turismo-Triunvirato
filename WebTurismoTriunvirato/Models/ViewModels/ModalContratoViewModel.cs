using System.ComponentModel.DataAnnotations;

namespace Web_Turismo_Triunvirato.Models.ViewModels
{
    // Usaremos el nombre que solicitaste
    public class ModalContratoViewModel
    {
          public int IdItem { get; set; }
          public string Title { get; set; }

        // 3. URL de la imagen (la que va en el modal)
        public string ImageUrl { get; set; }

        // 4. Descripción larga o detallada (cuerpo del modal)
        // Puedes usar el atributo [AllowHtml] si manejas HTML en la descripción.
        public string LongDescription { get; set; }
        public string LongDescription2 { get; set; }

        // 5. El precio o valor (para mostrar con formato de moneda)
        public decimal Price { get; set; }

        // 6. Texto adicional (ej: TripType | Desde donde)
        public string DetailText { get; set; }

        // 7. Mensaje de WhatsApp ya renderizado y codificado (crucial para el enlace wa.me)
        public string RenderedWhatsappMessage { get; set; }

        // 8. Opcional: Número de WhatsApp (siempre el mismo, pero es bueno pasarlo)
        public string WhatsappNumber { get; set; } = "+541122296405";
        [Required]
        public string PackageDetailsSerialized { get; set; } // O un string plano con formato

        // Datos del que reserva (el contacto principal)
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Message { get; set; }
        public string DocumentoResponsable { get; set; }
        // LA CLAVE: La lista de pasajeros
        public List<PasajeroViewModel> Pasajeros { get; set; } = new List<PasajeroViewModel>();
    }
}