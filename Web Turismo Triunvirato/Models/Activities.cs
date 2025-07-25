//using System.ComponentModel.DataAnnotations;
//namespace Fenix.Models // Ajusta el namespace a tu proyecto
//{
//    public class Activities
//    {
//        [Key]
//        public int Id { get; set; } // Identificador único de la actividad

//        [Required(ErrorMessage = "El Name de la actividad es obligatorio.")]
//        [StringLength(200)]
//        public string Name { get; set; }


//        [Required(ErrorMessage = "El tipo de actividad es obligatorio.")]
//        [StringLength(100)]
//        public string Type { get; set; } // Ej. "Tour", "Aventura", "Cultural"

//        [Required(ErrorMessage = "El número de personas es obligatorio.")]
//        [Range(1, int.MaxValue, ErrorMessage = "Debe haber al menos una persona.")]
//        public int NumberOfPeople { get; set; }

//        [Required(ErrorMessage = "El costo es obligatorio.")]
//        [Range(0.01, double.MaxValue, ErrorMessage = "El costo debe ser mayor a cero.")]
//        [DataType(DataType.Currency)]
//        public decimal Cost { get; set; }

//        [StringLength(1000)]
//        public string Description { get; set; }
//    }
//}
