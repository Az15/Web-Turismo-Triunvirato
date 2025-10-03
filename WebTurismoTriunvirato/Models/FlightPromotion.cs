using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding; // Necesitas agregar esta referencia
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_Turismo_Triunvirato.Models
{
    public class FlightPromotion
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Mensaje de Whatsapp")]
        [Required(ErrorMessage ="No se especifico un tipo de mensaje")]
        public int Whatsapp_Id { get; set; }

        [Required(ErrorMessage = "Se requiere que se especifique el tipo de servicio.")]
        [StringLength(100)]
        [Display(Name = "Tipo de servicio")]
        public string ServiceType { get; set; } = "0";

        [Required(ErrorMessage = "Se requiere el nombre del destino del viaje.")]
        [StringLength(100)]
        [Display(Name = "Nombre del destino")]
        public string DestinationName { get; set; }

        [Required(ErrorMessage = "Se requiere el origen del viaje.")]
        [StringLength(100)]
        [Display(Name = "Nombre del origen")]
        public string OriginName { get; set; }


        [Required(ErrorMessage ="Necesitamos una imagen de referencia")]
        [Display(Name = "Imagen de vuelo")]
        public string ImageUrl { get; set; } //No tenemos URL

        [Display(Name = "Esta en oferta?")]
        public bool IsHotWeek { get; set; } = false;

        [Required(ErrorMessage = "El precio original es requerido.")]
        [Range(0.01, 10000000.00,ErrorMessage = "El precio esta fuera de los numeros establecidos")]
        [DataType(DataType.Currency)]
        [Display(Name = "Precio Original")]
        public decimal OriginalPrice { get; set; }

        [Required(ErrorMessage = "El precio en oferta es necesario.")]
        [Range(0.01, 10000000.00)]
        [DataType(DataType.Currency)]
        [Display(Name = "Precio en oferta")]
        public decimal OfferPrice { get; set; }

        [Display(Name = "Porcentaje de Descuento")]
        public decimal DiscountPercentage { get; set; }

        [Required(ErrorMessage = "Fecha de Inicio es Requerida.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha de salida")]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Fecha de Finalación es requerida.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha de finalización")]
        public DateTime EndDate { get; set; } = DateTime.Today.AddMonths(1);

        [Display(Name = "Estado")]
        public bool IsActive { get; set; } = true;

        [StringLength(200)]
        [Display(Name = "Descripción")]
        public string Description { get; set; }
        
        [Display(Name = "Calificación")]
        public int? Stars { get; set; } = 0;


        [NotMapped]
        [BindNever]
        [ValidateNever]
        public string? RenderedWhatsappMessage { get; set; } 


    }
}
