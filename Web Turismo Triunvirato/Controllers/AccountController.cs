using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Web_Turismo_Triunvirato.Models;
using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Web_Turismo_Triunvirato.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Authenticate(string email, string password)
        {
            if (email == "alan@alan.com" && password == "123456")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, email)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties { };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ErrorMessage = "Credenciales incorrectas.";
                return View("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // -----------------------------
        // Flujo de registro paso a paso
        // -----------------------------

        // Paso 1: Mostrar formulario para ingresar el email
        [HttpGet]
        public IActionResult IngresarEmail()
        {
            return View();
        }

        // Paso 1: Procesar email y enviar código (fijo por ahora)
        [HttpPost]
        public IActionResult EnviarCodigo(string Email)
        {
            TempData["Email"] = Email;
            TempData["Codigo"] = "123456"; // Código fijo por ahora
            return RedirectToAction("VerificarCodigo");
        }

        // Paso 2: Mostrar formulario para ingresar el código
        [HttpGet]
        public IActionResult VerificarCodigo()
        {
            return View();
        }

        // Paso 2: Verificar código ingresado
        [HttpPost]
        public IActionResult VerificarCodigo(string CodigoIngresado)
        {
            var codigoCorrecto = TempData["Codigo"]?.ToString();
            if (CodigoIngresado == codigoCorrecto)
            {
                TempData.Keep("Email"); // Conservar el email para el siguiente paso
                return RedirectToAction("CompletarPerfil");
            }

            ViewBag.Error = "El código ingresado no es correcto.";
            return View();
        }

        // Paso 3: Mostrar formulario para completar perfil
        [HttpGet]
        public IActionResult CompletarPerfil()
        {
            return View();
        }

        // Paso 3: Guardar datos del perfil
        [HttpPost]
        public IActionResult CompletarPerfil(string Nombre, string Apellido, string Pais)
        {
            var email = TempData["Email"]?.ToString();
            if (email == null)
                return RedirectToAction("IngresarEmail");

            // Guardar usuario en base de datos
            // Este bloque depende de tu modelo. Acá un ejemplo simulado:
            // var nuevoUsuario = new Usuario { Email = email, Nombre = Nombre, Apellido = Apellido, Pais = Pais };
            // _context.Usuarios.Add(nuevoUsuario);
            // _context.SaveChanges();

            return RedirectToAction("Index", "Home"); // Redirige al login
        }
    }
}
