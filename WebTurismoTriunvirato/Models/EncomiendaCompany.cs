using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Web_Turismo_Triunvirato.Models
{
    public class EncomiendaCompany
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la empresa es requerido.")]
        [StringLength(100)]
        [DisplayName("Nombre de la compañia")]
        public string CompanyName { get; set; }

        // Se permite que la URL de la imagen sea nula ya que la imagen es subida como un archivo
        [StringLength(255)]
        [DisplayName("Logo de empresa")]
        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "La URL de la empresa es requerida.")]
        [Url(ErrorMessage = "La URL no es válida.")]
        [StringLength(255)]
        [DisplayName("Url de busqueda")]
        public string CompanyUrl { get; set; }
    }
}
