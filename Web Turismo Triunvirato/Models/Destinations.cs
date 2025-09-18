
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_Turismo_Triunvirato.Models // Ajusta el namespace a tu proyecto
{
    public class Destinations
    {
        [Key]
        public int Id { get; set; } // Identificador único del destino

        public int Whatsapp_Id { get; set; } // El ID que agregaste

        [Required(ErrorMessage = "El origen es obligatorio.")]
        [StringLength(200)]
        public string Origin { get; set; }

        [Required(ErrorMessage = "La fecha de salida es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime DepartureDate { get; set; }

        [Required(ErrorMessage = "La fecha de llegada es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime ArrivalDate { get; set; }

        [Required(ErrorMessage = "El monto es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a cero.")]
        [DataType(DataType.Currency)] // Sugiere que es un valor monetario
        public decimal Amount { get; set; }

        [StringLength(500)]
        public string Luggage { get; set; } // Descripción del equipaje (ej. "1 maleta, 1 de mano")

        [StringLength(500)]
        public string Addons { get; set; } // Descripción de adicionales (ej. "Asiento premium, Comida especial")

        [StringLength(1000)]
        public string Description { get; set; } // Descripción general del destino o viaje

        [BindNever]
        [NotMapped]
        public string RenderedWhatsappMessage { get; set; }
    }
}

