using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Web_Turismo_Triunvirato.Models;
    using System;
//using System.Windows.Forms;
namespace Web_Turismo_Triunvirato.Controllers
{

    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View(); // Muestra la vista de login (si accedes directamente a /Login)
        }

        [HttpPost]
        public IActionResult Authenticate(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                // Podrías devolver un error a la ventana emergente (JSON o ViewBag)
                ViewBag.ErrorMessage = "El correo electrónico y la contraseña son requeridos.";
                return View("Index"); // O podrías devolver un PartialView con el error
            }

            // Simulación de autenticación (¡reemplazar con lógica de base de datos!)
            if (email == "alan@alan.com" && password == "123456")
            {
                // Autenticación exitosa
                // Aquí normalmente establecerías cookies de sesión o Claims
                return RedirectToAction("Index", "Home"); // Redirigir a la página principal
            }
            else
            {
                // Autenticación fallida
                ViewBag.ErrorMessage = "Credenciales incorrectas.";
                return View("Index"); // O podrías devolver un PartialView con el error
            }
        }
    }
}
