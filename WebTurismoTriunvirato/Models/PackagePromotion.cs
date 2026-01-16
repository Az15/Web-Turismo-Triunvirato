using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_Turismo_Triunvirato.Models
{
    public class PackagePromotion
    {
        [Key]
        public int Id { get; set; }

        public int entidad { get; set; }

        [Display(Name = "Mensaje de Whatsapp")]
        [Required(ErrorMessage = "No se especifico un tipo de mensaje")]
        public int Whatsapp_Id { get; set; }


        [Required]
        [Display(Name = "Tipo de servicio")]
        public string ServiceType { get; set; } = "3"; // '3' para paquetes

        [Required]
        [Display(Name = "Tipo de Paquete")]
        // Para diferenciar entre los paquetes: "VueloYHotel" o "BusYHotel"
        public string PackageType { get; set; }


        [Required(ErrorMessage = "Descripción es requerida.")]
        [StringLength(4000)]
        [Display(Name = "Descripción")]
        public string Description { get; set; }

        //[Column("CompanyName")]
        [Display(Name = "Empresa")]
        [StringLength(100)]
        public string? CompanyName { get; set; }


        [Required(ErrorMessage = "Destino es requerido.")]
        [StringLength(100)]
        [Display(Name = "Destino")]
        public string DestinationName { get; set; }

   

        [Required(ErrorMessage = "Origen es requerido.")]
        [StringLength(100)]
        [Display(Name = "Origen")]
        public string OriginName { get; set; }

        [Display(Name = "URL de Imagen")]
        [StringLength(255)]
        public string? ImageUrl { get; set; }

        [Display(Name = "Es Hot Week")]
        public bool IsHotWeek { get; set; } = false;

        [Required(ErrorMessage = "Precio original es requerido.")]
        [Range(0.01, 10000000.00, ErrorMessage = "El precio debe ser mayor a 0.")]
        [DataType(DataType.Currency)]
        [Display(Name = "Precio Original")]
        public decimal OriginalPrice { get; set; }

        [Required(ErrorMessage = "Precio de oferta es requerido.")]
        [Range(0.01, 10000000.00, ErrorMessage = "El precio de oferta debe ser mayor a 0.")]
        [DataType(DataType.Currency)]
        [Display(Name = "Precio de Oferta")]
        public decimal OfferPrice { get; set; }

        [Display(Name = "Porcentaje de Descuento")]
        public decimal DiscountPercentage { get; set; }

        [Required(ErrorMessage = "Fecha de inicio es requerida.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Inicio")]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Fecha de fin es requerida.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Fin")]
        public DateTime EndDate { get; set; } = DateTime.Today.AddMonths(1);

 
        [Display(Name = "Nombre Hotel")]
        [StringLength(100)]
        public string? HotelName { get; set; }
        public bool IsActive { get; set; } = true;
        
        [ValidateNever]
        [BindNever]
        [NotMapped]
        public string RenderedWhatsappMessage { get; set; }
    }
}