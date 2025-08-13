using System;
using System.ComponentModel.DataAnnotations;

namespace Web_Turismo_Triunvirato.Models
{
    public class PackagePromotion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Tipo de servicio")]
        public string ServiceType { get; set; } = "3"; // '3' para paquetes

        [Required]
        [StringLength(20)]
        [Display(Name = "Tipo de Paquete")]
        // Para diferenciar entre los paquetes: "VueloYHotel" o "BusYHotel"
        public string PackageType { get; set; }

        [Required(ErrorMessage = "Descripción es requerida.")]
        [StringLength(200)]
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

        [Display(Name = "URL de Imagen")]
        [StringLength(255)]
        public string? ImageUrl { get; set; } // Se hizo nullable para aceptar nulls si no se provee una URL

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

        // Propiedades específicas de Vuelo y Hotel, se hacen nullable.
        [StringLength(100)]
        [Display(Name = "Compañía Aérea")]
        public string? AirlineName { get; set; }

        [StringLength(100)]
        [Display(Name = "Nombre del Hotel")]
        public string? HotelName { get; set; }

        [Display(Name = "Cantidad de Estrellas")]
        public int Stars { get; set; }

        // Propiedades específicas de Bus y Hotel, se hacen nullable.
        [StringLength(100)]
        [Display(Name = "Compañía de Bus")]
        public string? BusCompanyName { get; set; }

        [StringLength(50)]
        [Display(Name = "Categoría de Bus")]
        public string? Category { get; set; }
    }
}