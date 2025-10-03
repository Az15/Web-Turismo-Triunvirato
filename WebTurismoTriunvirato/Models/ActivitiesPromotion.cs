using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_Turismo_Triunvirato.Models
{
    // He cambiado el nombre de la clase a ActivitiesPromotion para evitar conflictos
    // con otras clases existentes, según tu indicación.
    [Table("ActivitiesPromotion")]
    public class ActivitiesPromotion
    {
        // La clave primaria para la entidad.
        [Key]
        public int Id { get; set; }

        public int Whatsapp_Id { get; set; } // El ID que agregaste

        public bool Is_Active { get; set; } // Campo para indicar si la promoción está activa

        // Título de la actividad. Este campo es obligatorio.
        [Required(ErrorMessage = "El título es obligatorio.")]
        public string Title { get; set; }

        // Descripción de la actividad. Este campo es obligatorio.
        [Required(ErrorMessage = "La descripción es obligatoria.")]
        public string Description { get; set; }

        // Ubicación de la actividad. Este campo es obligatorio.
        [Required(ErrorMessage = "La ubicación es obligatoria.")]
        public string Location { get; set; }

        // URL de la imagen. La hacemos opcional (nullable) con el '?'
        // para que la validación no falle cuando se crea una nueva actividad
        // sin una URL inicial.
        public string? ImageUrl { get; set; }


        [BindNever]
        [NotMapped]
        [ValidateNever]
        public string RenderedWhatsappMessage { get; set; }
    }
}
