using System;
using System.ComponentModel.DataAnnotations;

namespace Fenix.Models // Ajusta el namespace a tu proyecto
{
    public enum GenderType // Enumeraci�n para el tipo de g�nero
    {
        Male,
        Female,
        Other,
        PreferNotToSay
    }

    public class Passenger
    {
        [Key]
        public int Id { get; set; } // Identificador �nico del pasajero

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

        [EmailAddress(ErrorMessage = "Formato de email inv�lido.")]
        [StringLength(255)]
        public string Email { get; set; } // Puede ser opcional para un pasajero adicional

        public GenderType Gender { get; set; } // Usa la enumeraci�n definida

        public bool AssistanceToTraveler { get; set; } // Indica si requiere asistencia (S�/No)
    }
}
