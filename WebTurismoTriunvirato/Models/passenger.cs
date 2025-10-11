using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Web_Turismo_Triunvirato.Models // Ajusta el namespace a tu proyecto
{
    public enum GenderType // Enumeración para el tipo de género
    {
        Male,
        Female,
        Other,
        PreferNotToSay
    }

    public class Passenger
    {
        [Key]
        public int Id { get; set; } // Identificador único del pasajero

        [Required(ErrorMessage = "El Nombre es obligatorio.")]
        [StringLength(100)]
        [DisplayName("Nombre")]
        public string Name { get; set; }

        [Required(ErrorMessage = "El Apellido es obligatorio.")]
        [StringLength(100)]
        [DisplayName("Apellido")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "El DNI es obligatorio.")]
        [StringLength(20)]
        public string DNI { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        [DataType(DataType.Date)]
        [DisplayName("Fecha de nacimiento")]
        public DateTime DateOfBirth { get; set; }

        [EmailAddress(ErrorMessage = "Formato de email inválido.")]
        [StringLength(255)]
        public string Email { get; set; } // Puede ser opcional para un pasajero adicional

        [DisplayName("Genero")]
        public GenderType Gender { get; set; } // Usa la enumeración definida

        [DisplayName("AsistCard")]
        public bool AssistanceToTraveler { get; set; } // Indica si requiere asistencia (Sí/No)
    }
}
