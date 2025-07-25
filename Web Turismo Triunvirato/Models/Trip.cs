
namespace Web_Turismo_Triunvirato.Models // Ajusta el namespace a tu proyecto
{
    public class Controll_Trip
    {
        [Key]
        public int Id { get; set; } // Identificador único del viaje

        [Required(ErrorMessage = "Debe haber al menos un destino.")]
        public List<View_Index_Destination> Destinations { get; set; } = new List<View_Index_Destination>();

  }
}
