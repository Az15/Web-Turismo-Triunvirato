using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;




namespace Web_Turismo_Triunvirato.Models
{
    [Table("entidades")]
    public class Entidad
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Nombre_Tabla { get; set; }

        // Propiedad de navegación para relación uno a muchos
        public virtual ICollection<Imagen> Imagenes { get; set; }
    }
}