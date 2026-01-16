
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_Turismo_Triunvirato.Models
{
    [Table("imagenes")]
    public class Imagen
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int Id_Entidad { get; set; }

        [Required]
        public int Id_Objeto { get; set; }

        [Required]
        public string Url { get; set; }

        // Relación con el modelo Entidad
        [ForeignKey("Id_Entidad")]
        public virtual Entidad Entidad { get; set; }
    }
}