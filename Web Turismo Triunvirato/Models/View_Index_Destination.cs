namespace Web_Turismo_Triunvirato.Models
{
    public class View_Index_Destination
    {
        public int Id { get; set; } // �Est� el SP devolviendo una columna 'Id'?
        public string Title { get; set; }
        public string PictureDestiny { get; set; }
        public decimal Price { get; set; }
        public string From { get; set; }
        public string DetailDestinyURL { get; set; }
        public bool IsHotWeek { get; set; }
        public string TripType { get; set; } // Aseg�rate de que esta columna est� en el SP si es un campo nuevo
    }
}