// Models/BusPromotion.cs

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_Turismo_Triunvirato.Models
{
    public class BusPromotion
    {
        [Key]
        public int Id { get; set; }

        public int entidad { get; set; }

        public int Whatsapp_Id { get; set; } // El ID que agregaste

        [Required]
        [Display(Name = "Tipo de servicio")]
        public string ServiceType { get; set; } = "2"; // Valor por defecto para buses

        [Required(ErrorMessage = "Descripción es requerida.")]
        [StringLength(4000)]
        [Display(Name = "Descripción")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Destino es requerido.")]
        [StringLength(100)]
        [Display(Name = "Destino")]
        public string DestinationName { get; set; }

        [Required(ErrorMessage = "Origen es requerido.")]
        [StringLength(100)]
        [Display(Name = "Origen")]
        public string OriginName { get; set; }

        [Display(Name = "Imagen Referencia")]
        [StringLength(255)]
        public string ImageUrl { get; set; }

        [Display(Name = "Esta en oferta?")]
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
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha de Inicio")]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Fecha de fin es requerida.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha de Fin")]
        public DateTime EndDate { get; set; } = DateTime.Today.AddMonths(1);

        [Display(Name = "Está Activo")]
        public bool IsActive { get; set; } = true;

        [Required(ErrorMessage = "Compañía de bus es requerida.")]
        [StringLength(100)]
        [Display(Name = "Compañía de Bus")]
        public string BusCompanyName { get; set; }

        [Required(ErrorMessage = "Categoría es requerida.")]
        [StringLength(50)]
        [Display(Name = "Categoría")]
        public string Category { get; set; }

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