using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_Turismo_Triunvirato.Models
{
    // Define el nombre de la tabla en la base de datos si es diferente al nombre de la clase
    [Table("EncomiendaCompanies")]
    public class EncomiendaCompany
    {
        // Propiedad para el ID único de la empresa. Se utiliza como clave primaria.
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Propiedad para el nombre de la empresa. Es un campo obligatorio.
        [Required(ErrorMessage = "El nombre de la empresa es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        [Display(Name = "Nombre de la Empresa")]
        public string CompanyName { get; set; }

        // Propiedad para la URL de seguimiento. Es un campo obligatorio.
        [Required(ErrorMessage = "La URL de seguimiento es obligatoria.")]
        [StringLength(255, ErrorMessage = "La URL de seguimiento no puede exceder los 255 caracteres.")]
        [Display(Name = "URL de Seguimiento")]
        public string CompanyUrl { get; set; }

        // Propiedad para la URL de la imagen del logo. Es un campo obligatorio.
        [Required(ErrorMessage = "La URL de la imagen es obligatoria.")]
        [StringLength(255, ErrorMessage = "La URL de la imagen no puede exceder los 255 caracteres.")]
        [Display(Name = "URL de la Imagen")]
        public string ImageUrl { get; set; }

        // Propiedad para la fecha de creación.
        [Display(Name = "Fecha de Creación")]
        public DateTime CreatedAt { get; set; }
    }
}
