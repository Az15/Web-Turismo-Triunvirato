//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;

//namespace Fenix.Models // Ajusta el namespace a tu proyecto
//{
//    public class Trip
//    {
//        [Key]
//        public int Id { get; set; } // Identificador único del viaje

//        // El usuario que compra el pasaje y puede ser un pasajero.
//        // Los adicionales son otros pasajeros.
//        // Es más lógico tener una lista de pasajeros asociados al viaje.
//        [Required(ErrorMessage = "Debe haber al menos un pasajero.")]
//        public List<Passenger> Passengers { get; set; } = new List<Passenger>();

//        [Required(ErrorMessage = "Debe haber al menos un destino.")]
//        public List<Destination> Destinations { get; set; } = new List<Destination>();

//        // El alojamiento puede ser opcional si el viaje no incluye pernoctación.
//        public List<Accommodation> Accommodations { get; set; } = new List<Accommodation>();

//        // Si un viaje puede estar asociado a un paquete predefinido.
//        public Package? Package { get; set; } // Puede ser nulo si el viaje es personalizado
//    }
//}
