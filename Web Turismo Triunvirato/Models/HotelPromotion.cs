using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_Turismo_Triunvirato.Models
{
    public class HotelPromotion
    {
        [Key]
        public int Id { get; set; }

        public int Whatsapp_Id { get; set; }

        [Required(ErrorMessage = "Tipo de servicio.")]
        [StringLength(100)]
        [Display(Name = "Tipo de servicio")]
        // CORRECCIÓN: Se convierte en propiedad.
        public string ServiceType { get; set; }

        [Required(ErrorMessage = "Destination name is required.")]
        [StringLength(100)]
        [Display(Name = "Destinatino")]
        // CORRECCIÓN: Se hace anulable para evitar el error si el campo es nulo en la BD.
        public string? DestinationName { get; set; }

        [Required(ErrorMessage = "El nombre del hotel es requerido.")]
        [StringLength(100)]
        [Display(Name = "HotelName")]
        // CORRECCIÓN: Se hace anulable.
        public string? HotelName { get; set; }

        [Display(Name = "Image URL")]
        [StringLength(255)]
        // CORRECCIÓN: Se hace anulable.
        public string? ImageUrl { get; set; }

        [Display(Name = "Is Hot Week")]
        public bool IsHotWeek { get; set; } = false;

        [Required(ErrorMessage = "Precio original es requerido.")]
        [Range(0.01, 10000000.00)]
        [DataType(DataType.Currency)]
        [Display(Name = "Precio Original")]
        public decimal OriginalPrice { get; set; }

        [Required(ErrorMessage = "Precio de oferta es requerido.")]
        [Range(0.01, 10000000.00)]
        [DataType(DataType.Currency)]
        [Display(Name = "Precio de Oferta")]
        public decimal OfferPrice { get; set; }

        [Display(Name = "Discount Percentage")]
        public decimal DiscountPercentage { get; set; }

        [Required(ErrorMessage = "Start date is required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "End date is required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; } = DateTime.Today.AddMonths(1);

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        [StringLength(200)]
        [Display(Name = "Description")]
        // CORRECCIÓN: Se hace anulable.
        public string? Description { get; set; }

        // CORRECCIÓN: Se convierte en propiedad.
        public int Stars { get; set; }

        [NotMapped]
        public string? RenderedWhatsappMessage { get; set; }
    }
}
