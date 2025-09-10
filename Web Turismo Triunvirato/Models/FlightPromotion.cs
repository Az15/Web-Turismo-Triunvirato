using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding; // Necesitas agregar esta referencia

namespace Web_Turismo_Triunvirato.Models
{
    public class FlightPromotion
    {
        [Key]
        public int Id { get; set; }

        public int Whatsapp_Id { get; set; }

        [Required(ErrorMessage = "Tipo de servicio.")]
        [StringLength(100)]
        [Display(Name = "Tipo de servicio")]
        public string ServiceType { get; set; } = "0";

        [Required(ErrorMessage = "Destination name is required.")]
        [StringLength(100)]
        [Display(Name = "Destination Name")]
        public string DestinationName { get; set; }

        [Required(ErrorMessage = "Origin name is required.")]
        [StringLength(100)]
        [Display(Name = "Origin Name")]
        public string OriginName { get; set; }

        [Display(Name = "Image URL")]
        [StringLength(255)]
        public string ImageUrl { get; set; }

        [Display(Name = "Is Hot Week")]
        public bool IsHotWeek { get; set; } = false;

        [Required(ErrorMessage = "Original price is required.")]
        [Range(0.01, 10000000.00)]
        [DataType(DataType.Currency)]
        [Display(Name = "Original Price")]
        public decimal OriginalPrice { get; set; }

        [Required(ErrorMessage = "Offer price is required.")]
        [Range(0.01, 10000000.00)]
        [DataType(DataType.Currency)]
        [Display(Name = "Offer Price")]
        public decimal OfferPrice { get; set; }

        [Display(Name = "Discount Percentage")]
        public decimal DiscountPercentage { get; set; }

        [Required(ErrorMessage = "Fecha de Inicio es Requerida.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Fecha de Finalación es requerida.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; } = DateTime.Today.AddMonths(1);

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        [StringLength(200)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        public int? Stars { get; set; } = 0;

        [NotMapped]
        [BindNever] // <-- Agrega este atributo
        public string RenderedWhatsappMessage { get; set; }
    }
}
