using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Web_Turismo_Triunvirato.Models
{
    public class Promotion : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El tipo de servicio es obligatorio.")]
        public ServiceType ServiceType { get; set; }

        // Propiedad específica para Hoteles
        // Ya no tiene el atributo Range aquí, lo validaremos de forma condicional.
        public int Stars { get; set; } = 0; // Valor predeterminado para los que no son hoteles

        // Nuevas propiedades para la visualización en Home/Vuelos.cshtml
        [Required(ErrorMessage = "El Nombre del destino es obligatorio.")]
        [StringLength(100, ErrorMessage = "El Nombre del destino no puede exceder 100 caracteres.")]
        public string DestinationName { get; set; }

        // Propiedad específica para Vuelos
        [StringLength(100, ErrorMessage = "El origen no puede exceder 100 caracteres.")]
        public string OriginName { get; set; }

        [Display(Name = "Imagen")]
        [StringLength(255, ErrorMessage = "La URL de la imagen no puede exceder 255 caracteres.")]
        public string ImageUrl { get; set; }

        [Display(Name = "Esta en oferta")]
        public bool IsHotWeek { get; set; } = false;

        // Propiedades ya existentes para precios y fechas
        [Required(ErrorMessage = "El precio original es obligatorio.")]
        [Range(0.01, 10000000.00, ErrorMessage = "El precio original debe ser mayor a cero y menor a 10,000,000.")]
        [DataType(DataType.Currency)]
        public decimal OriginalPrice { get; set; }

        [Required(ErrorMessage = "El precio de oferta es obligatorio.")]
        [Range(0.01, 10000000.00, ErrorMessage = "El precio de oferta debe ser mayor a cero y menor a 10,000,000.")]
        [DataType(DataType.Currency)]
        public decimal OfferPrice { get; set; }

        [Range(0, 100, ErrorMessage = "El porcentaje de descuento debe estar entre 0 y 100.")]
        public decimal DiscountPercentage { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "La fecha de fin es obligatoria.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; } = DateTime.Today.AddMonths(1);

        [Display(Name = "Activa")]
        public bool IsActive { get; set; } = true;

        [StringLength(200, ErrorMessage = "La descripción no puede exceder 200 caracteres.")]
        public string Description { get; set; }

        // Implementación de IValidatableObject para validación condicional
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Valida la propiedad Stars solo si el tipo de servicio es Hoteles
            if (ServiceType == ServiceType.Hoteles && (Stars < 1 || Stars > 5))
            {
                yield return new ValidationResult(
                    "El número de estrellas debe estar entre 1 y 5.",
                    new[] { nameof(Stars) });
            }

            // Aquí puedes agregar otras validaciones condicionales si las necesitas
            // Por ejemplo, para Vuelos, si OriginName es obligatorio
            // if (ServiceType == ServiceType.Vuelos && string.IsNullOrEmpty(OriginName))
            // {
            //     yield return new ValidationResult(
            //         "El nombre de origen es obligatorio para vuelos.",
            //         new[] { nameof(OriginName) });
            // }
        }
    }
}