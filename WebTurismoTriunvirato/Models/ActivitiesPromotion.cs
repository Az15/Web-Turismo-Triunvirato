using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
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
        [Required]
        [Display(Name = "Tipo de servicio")]
        //public string ServiceType { get; set; } = "1";  
        public int entidad { get; set; }

        [DisplayName("Mensaje de Whatsapp")]
        public int Whatsapp_Id { get; set; } // El ID que agregaste

        //[DisplayName("Activo")]
        //public bool Is_Active { get; set; } // Campo para indicar si la promoción está activa

        // Título de la actividad. Este campo es obligatorio.
        [DisplayName("Titulo")]
        [Required(ErrorMessage = "El título es obligatorio.")]
        public string Title { get; set; }

        // Descripción de la actividad. Este campo es obligatorio.
        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [DisplayName("Descripcion")]
        public string Description { get; set; }

        // Ubicación de la actividad. Este campo es obligatorio.
        [Required(ErrorMessage = "La ubicación es obligatoria.")]
        [DisplayName("Ubicación de actividad")]
        public string Location { get; set; }

        // URL de la imagen. La hacemos opcional (nullable) con el '?'
        // para que la validación no falle cuando se crea una nueva actividad
        // sin una URL inicial.
        [DisplayName("Eliga una imagen")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Está Activo")]
        public bool IsActive { get; set; } = true;


        [BindNever]
        [NotMapped]
        [ValidateNever]
        public string RenderedWhatsappMessage { get; set; }



        // --- PROPIEDAD PARA GALERÍA ---
        // NotMapped: No existe en la tabla de paquetes.
        // ValidateNever: No causa error si el formulario no la envía.
        [ValidateNever]
        [NotMapped]
        public List<Imagen> ImagenesAdicionales { get; set; } = new List<Imagen>();
    }
}
