using System;
using System.ComponentModel.DataAnnotations;

namespace Fenix.Models // Ajusta el namespace a tu proyecto
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

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(100)]
        public string Surname { get; set; }

        [Required(ErrorMessage = "El DNI es obligatorio.")]
        [StringLength(20)]
        public string DNI { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [EmailAddress(ErrorMessage = "Formato de email inválido.")]
        [StringLength(255)]
        public string Email { get; set; } // Puede ser opcional para un pasajero adicional

        public GenderType Gender { get; set; } // Usa la enumeración definida

        public bool AssistanceToTraveler { get; set; } // Indica si requiere asistencia (Sí/No)
    }
}
