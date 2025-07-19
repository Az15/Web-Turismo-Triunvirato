using System.ComponentModel.DataAnnotations;

namespace Fenix.Models // Ajusta el namespace a tu proyecto
{
    public class Package
    {
        [Key]
        public int Id { get; set; } // Identificador único del paquete

        // Un paquete define un destino, un alojamiento y actividades específicas.
        // Se asume que estos son objetos completos, no solo IDs.
        [Required(ErrorMessage = "El destino definido es obligatorio.")]
        public Destination DefinedDestination { get; set; }

        [Required(ErrorMessage = "El alojamiento definido es obligatorio.")]
        public Accommodation DefinedAccommodation { get; set; }

        [Required(ErrorMessage = "Las actividades definidas son obligatorias.")]
        public Activities DefinedActivities { get; set; }
    }
}
