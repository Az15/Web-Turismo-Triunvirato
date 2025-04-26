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
                // Podr�as devolver un error a la ventana emergente (JSON o ViewBag)
                ViewBag.ErrorMessage = "El correo electr�nico y la contrase�a son requeridos.";
                return View("Index"); // O podr�as devolver un PartialView con el error
            }

            // Simulaci�n de autenticaci�n (�reemplazar con l�gica de base de datos!)
            if (email == "alan@alan.com" && password == "123456")
            {
                // Autenticaci�n exitosa
                // Aqu� normalmente establecer�as cookies de sesi�n o Claims
                return RedirectToAction("Index", "Home"); // Redirigir a la p�gina principal
            }
            else
            {
                // Autenticaci�n fallida
                ViewBag.ErrorMessage = "Credenciales incorrectas.";
                return View("Index"); // O podr�as devolver un PartialView con el error
            }
        }
    }
}
