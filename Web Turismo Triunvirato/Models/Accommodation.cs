//using System;
//using System.ComponentModel.DataAnnotations;

//namespace Fenix.Models // Ajusta el namespace a tu proyecto
//{
//    public class Accommodation
//    {
//        [Key]
//        public int Id { get; set; } // Identificador �nico del alojamiento

//        [Required(ErrorMessage = "El costo es obligatorio.")]
//        [Range(0.01, double.MaxValue, ErrorMessage = "El costo debe ser mayor a cero.")]
//        [DataType(DataType.Currency)]
//        public decimal Cost { get; set; }

//        [Required(ErrorMessage = "La fecha de llegada es obligatoria.")]
//        [DataType(DataType.Date)]
//        public DateTime ArrivalDate { get; set; }

//        [Required(ErrorMessage = "La fecha de salida es obligatoria.")]
//        [DataType(DataType.Date)]
//        public DateTime DepartureDate { get; set; }

//        [Required(ErrorMessage = "El n�mero de habitaciones es obligatorio.")]
//        [Range(1, int.MaxValue, ErrorMessage = "Debe haber al menos una habitaci�n.")]
//        public int NumberOfRooms { get; set; }

//        [Required(ErrorMessage = "El n�mero de personas es obligatorio.")]
//        [Range(1, int.MaxValue, ErrorMessage = "Debe haber al menos una persona.")]
//        public int NumberOfPeople { get; set; }

//        [Required(ErrorMessage = "El tipo de alojamiento es obligatorio.")]
//        [StringLength(100)]
//        public string Type { get; set; } // Ej. "Hotel", "Apartamento", "Hostal"

//        [Required(ErrorMessage = "La calidad es obligatoria.")]
//        [Range(1, 5, ErrorMessage = "La calidad debe ser entre 1 y 5.")]
//        public int Quality { get; set; } // Calidad (1-5 estrellas, etc.)

//        [StringLength(1000)]
//        public string Description { get; set; }
//    }
//}
