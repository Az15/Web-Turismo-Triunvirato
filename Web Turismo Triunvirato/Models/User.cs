using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Para atributos como [Required] si usas validaci�n o EF Core

namespace Web_Turismo_Triunvirato.Models // Ajusta el namespace a tu proyecto
{
    public class User
    {
        [Key] // Indica que esta es la clave primaria si usas Entity Framework Core
        public int Id { get; set; } // Identificador �nico del usuario

        [Required(ErrorMessage = "El Name es obligatorio.")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "El Surname es obligatorio.")]
        [StringLength(100)]
        public string Surname { get; set; }

        [Required(ErrorMessage = "El DNI es obligatorio.")]
        [StringLength(20)] // Ajusta el tama�o seg�n el formato de DNI que uses (ej. "XX.XXX.XXX")
        public string DNI { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        [DataType(DataType.Date)] // Sugiere que es solo una fecha, sin hora
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de email inv�lido.")]
        [StringLength(255)]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contrase�a es obligatoria.")]
        [StringLength(255)] // Almacena el hash de la contrase�a, no la contrase�a en texto plano
        [DataType(DataType.Password)] // Sugiere que es un campo de contrase�a
        public string Password { get; set; } // Nota: En una aplicaci�n real, almacenar�as un hash de la contrase�a, no el texto plano.

        [StringLength(500)]
        public string Address { get; set; }
    }
}
