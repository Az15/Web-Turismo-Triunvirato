// Models/FlightPromotion.cs

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_Turismo_Triunvirato.Models
{
    public class HotelPromotion
    {
        [Key]
        public int Id { get; set; }

        [Display(Name ="Mensaje de Whatsapp")]
        public int Whatsapp_Id { get; set; } // El ID que agregaste

        [Required(ErrorMessage = "El tipo de servicio es requerido.")]
        [StringLength(100)]
        [Display(Name = "Tipo de servicio")]
        public string ServiceType = "1";

        [Required(ErrorMessage = "El nombre de destino es requerido.")]
        [StringLength(100)]
        [Display(Name = "Nombre de Hotel")]
        public string DestinationName { get; set; }

        // Las propiedades comentadas no se alteran, ya que no están activas en tu modelo
        //[Required(ErrorMessage = "El nombre de origen es requerido.")]
        //[StringLength(100)]
        //[Display(Name = "Nombre de Origen")]
        //public string OriginName { get; set; }

        [StringLength(255)]
        [Display(Name = "URL de Imagen")]
        public string ImageUrl { get; set; }

        [Display(Name = "Semana de oferta")]
        public bool IsHotWeek { get; set; } = false;

        [Required(ErrorMessage = "El precio original es requerido.")]
        [Range(0.01, 10000000.00, ErrorMessage = "El precio debe estar entre {1} y {2}.")]
        [DataType(DataType.Currency)]
        [Display(Name = "Precio Original")]
        public decimal OriginalPrice { get; set; }

        [Required(ErrorMessage = "El precio de la oferta es requerido.")]
        [Range(0.01, 10000000.00, ErrorMessage = "El precio de la oferta debe estar entre {1} y {2}.")]
        [DataType(DataType.Currency)]
        [Display(Name = "Precio de Oferta")]
        public decimal OfferPrice { get; set; }

        [Display(Name = "Porcentaje de Descuento")]
        public decimal DiscountPercentage { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es requerida.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha de Inicio")]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "La fecha de finalización es requerida.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha de Finalización")]
        public DateTime EndDate { get; set; } = DateTime.Today.AddMonths(1);

        [Display(Name = "Está Activo")]
        public bool IsActive { get; set; } = true;

        [StringLength(200)]
        [Display(Name = "Descripción")]
        [Required(ErrorMessage ="Es necesario dar una descripcion o nombre de hotel")]
        public string Description { get; set; }

        [Display(Name = "Calificación")]
        [Range(1,5, ErrorMessage ="Es necesario dar una calificación entre 1 y 5")]
        [Required(ErrorMessage ="La calificacion tiene que ser mayor a uno")]
        public int Stars = 0;
       
        [ValidateNever]
        [BindNever]
        [NotMapped]
        public string RenderedWhatsappMessage { get; set; }
    }
}
