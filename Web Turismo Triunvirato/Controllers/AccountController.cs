using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Web_Turismo_Triunvirato.Models;
using Web_Turismo_Triunvirato.Services;
using Microsoft.AspNetCore.Localization;

namespace Web_Turismo_Triunvirato.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Authenticate(string email, string password) // <--- CORREGIDO AQU�: "string password"
        {
            var user = await _userService.GetByEmailAsync(email);

            // La l�gica de verificaci�n de contrase�a y asignaci�n de claims es correcta
            if (user != null && user.Password == password) // �Recuerda el tema de las contrase�as seguras!
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, email),
                    new Claim("Nombre", user.Nombre ?? ""),
                    new Claim("Apellido", user.Apellido ?? ""),
                    // new Claim("Pais", user.Pais ?? "")
                };
                // ***** ROL DE ADMIN *****
                if (email == "admin@admin.com")
                {
                    claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                }
                // ********************************************

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties { };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // MODIFICACI�N CLAVE PARA LA REDIRECCI�N
                if (email == "admin@admin.com")
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Credenciales incorrectas.";
                return View("Login");
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

        [HttpGet]
        public IActionResult IngresarEmail()
        {
            return View();
        }

        [HttpPost]
        public IActionResult EnviarCodigo(string Email)
        {
            TempData["Email"] = Email;
            TempData["Codigo"] = "123456"; // C�digo fijo por ahora
            return RedirectToAction("VerificarCodigo");
        }

        [HttpGet]
        public IActionResult VerificarCodigo()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerificarCodigo(string CodigoIngresado)
        {
            var codigoCorrecto = TempData["Codigo"]?.ToString();
            if (CodigoIngresado == codigoCorrecto)
            {
                TempData.Keep("Email");
                return RedirectToAction("CompletarPerfil");
            }

            ViewBag.Error = "El c�digo ingresado no es correcto.";
            return View();
        }

        [HttpGet]
        public IActionResult CompletarPerfil()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CompletarPerfil(string Nombre, string Apellido, string Pais, string Contrase�a)
        {
            var email = TempData["Email"]?.ToString();
            if (email == null)
                return RedirectToAction("IngresarEmail");

            var nuevoUsuario = new User { Email = email, Nombre = Nombre, Apellido = Apellido, /*Pais = Pais,*/ Password = Contrase�a }; // Guarda la contrase�a
            await _userService.AddAsync(nuevoUsuario);

            // Iniciar sesi�n autom�ticamente despu�s del registro
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, email),
                new Claim("Nombre", Nombre ?? ""),
                new Claim("Apellido", Apellido ?? ""),
                new Claim("Pais", Pais ?? "")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties { };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToAction("Index", "Home");
        }

        // -----------------------------
        // Modificar Perfil
        // -----------------------------

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            if (User.Identity.IsAuthenticated)
            {
                var email = User.Identity.Name;
                var user = await _userService.GetByEmailAsync(email);
                if (user != null)
                {
                    return View(user); // Pasa el objeto User a la vista de edici�n
                }
            }
            return RedirectToAction("Index", "Home"); // Si no est� autenticado o no se encuentra el usuario
        }


        [HttpPost]
        public async Task<IActionResult> EditProfile(User model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userService.GetByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    existingUser.Nombre = model.Nombre;
                    existingUser.Apellido = model.Apellido;
                    //existingUser.Pais = model.Pais;
                    existingUser.Password = model.Password; // Permite cambiar la contrase�a
                    await _userService.UpdateAsync(existingUser);

                    // Actualizar las Claims del usuario si es necesario
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, model.Email),
                        new Claim("Nombre", model.Nombre ?? ""),
                        new Claim("Apellido", model.Apellido ?? ""),
                       // new Claim("Pais", model.Pais ?? "")
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties { IsPersistent = true }; // Mantener la sesi�n

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return RedirectToAction("Index", "Home"); // Redirigir a la p�gina principal
                }
                ViewBag.ErrorMessage = "Error al guardar el perfil.";
                return View(model);
            }
            return View(model);
        }
    }
}