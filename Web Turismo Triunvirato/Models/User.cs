using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Para atributos como [Required] si usas validación o EF Core

namespace Web_Turismo_Triunvirato.Models // Ajusta el namespace a tu proyecto
{
    public class User
    {
        [Key] // Indica que esta es la clave primaria si usas Entity Framework Core
        public int Id { get; set; } // Identificador único del usuario

        [Required(ErrorMessage = "El Name es obligatorio.")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "El Surname es obligatorio.")]
        [StringLength(100)]
        public string Surname { get; set; }

        [Required(ErrorMessage = "El DNI es obligatorio.")]
        [StringLength(20)] // Ajusta el tamaño según el formato de DNI que uses (ej. "XX.XXX.XXX")
        public string DNI { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        [DataType(DataType.Date)] // Sugiere que es solo una fecha, sin hora
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de email inválido.")]
        [StringLength(255)]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(255)] // Almacena el hash de la contraseña, no la contraseña en texto plano
        [DataType(DataType.Password)] // Sugiere que es un campo de contraseña
        public string Password { get; set; } // Nota: En una aplicación real, almacenarías un hash de la contraseña, no el texto plano.

        [StringLength(500)]
        public string Address { get; set; }
    }
}
