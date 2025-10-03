using System.ComponentModel.DataAnnotations.Schema;
using Web_Turismo_Triunvirato.DataAccess;

namespace Web_Turismo_Triunvirato.Models
{
    [Table("view_destinationcarouselitem", Schema = "dbo")]
    public class View_Index_DestinationCarouselItem //CARRUSEL DE DESTINOS DEL INDEX
    {
        public int? Id { get; set; }
        public int Whatsapp_Id { get; set; }
        public string? ImageUrl { get; set; }
        public string? AltText { get; set; }
        public string? Title { get; set; }
        public string? LinkUrl { get; set; }
        public bool IsActive { get; set; }


    }


}
