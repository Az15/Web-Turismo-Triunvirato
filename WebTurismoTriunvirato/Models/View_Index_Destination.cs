using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Web_Turismo_Triunvirato.Models
{
    [Table("view_destination", Schema = "dbo")]
    public class View_Index_Destination
    {
        public int Id { get; set; } // ¿Está el SP devolviendo una columna 'Id'?
        [DisplayName("Mensaje de Whatsapp")]
        public int Whatsapp_Id { get; set; }
        [DisplayName("Titulo")]
        public string Title { get; set; }
        [DisplayName("Foto del destino")]
        public string? PictureDestiny { get; set; }
        [DisplayName("Precio")]
        public decimal Price { get; set; }
        [DisplayName("Desde donde saldria")]
        public string From { get; set; }
        
        public string? DetailDestinyURL { get; set; }
        [DisplayName("Oferta")]
        public bool IsHotWeek { get; set; }
        public string TripType { get; set; } // Asegúrate de que esta columna esté en el SP si es un campo nuevo


        [BindNever]
        [NotMapped]
        [ValidateNever]
        public string RenderedWhatsappMessage { get; set; }
    }
}