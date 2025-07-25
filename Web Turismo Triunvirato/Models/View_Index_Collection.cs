namespace Web_Turismo_Triunvirato.Models
{
    public class View_Index_Collection
    {
        public View_Index_Collection() { } 
        
        public List<View_Index_DestinationCarouselItem> DestinationCarrousel { get; set; } = new List<View_Index_DestinationCarouselItem>();
        public List<View_Index_Destination> PopularDestinations { get; set; } = new List<View_Index_Destination>();
    }
}
