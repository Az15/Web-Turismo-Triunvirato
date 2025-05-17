namespace Web_Turismo_Triunvirato.Models
{
    public class Destination
    {
        public string NombreDestio { get; set; }
        public string ImagenDestino { get; set; }
        public decimal Precio { get; set; }
        public string From { get; set; }
        public string DetalleDestino { get; set; } // URL para la página de detalles
        public bool IsHotWeek { get; set; } // Opcional: para la etiqueta "Hot Week"
    }
}