using System.ComponentModel.DataAnnotations;

namespace Web_Turismo_Triunvirato.Models.ViewModels
{
    // Usaremos el nombre que solicitaste
    public class ModalContratoViewModel
    {
        // 1. Identificador único para el botón de Mail y el ID del Modal
        public int IdItem { get; set; }

        // 2. Título principal del paquete/item
        public string Title { get; set; }

        // 3. URL de la imagen (la que va en el modal)
        public string ImageUrl { get; set; }

        // 4. Descripción larga o detallada (cuerpo del modal)
        // Puedes usar el atributo [AllowHtml] si manejas HTML en la descripción.
        public string LongDescription { get; set; }

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
    }
}
