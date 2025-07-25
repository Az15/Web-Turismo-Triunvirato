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
        public async Task<IActionResult> Authenticate(string email, string password) // <--- CORREGIDO AQUÍ: "string password"
        {
            var user = await _userService.GetByEmailAsync(email);

            // La lógica de verificación de contraseña y asignación de claims es correcta
            if (user != null && user.Password == password) // ¡Recuerda el tema de las contraseñas seguras!
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, email),

                    new Claim("Name", user.Name ?? ""), // Guarda el Name en las claims
                    new Claim("Surname", user.Surname ?? ""), // Guarda el Surname
                   // new Claim("Pais", user.Pais ?? "") // Guarda el país

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

                // MODIFICACIÓN CLAVE PARA LA REDIRECCIÓN
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
            TempData["Codigo"] = "123456"; // Código fijo por ahora
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

            ViewBag.Error = "El código ingresado no es correcto.";
            return View();
        }

        [HttpGet]
        public IActionResult CompletarPerfil()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CompletarPerfil(string Name, string Surname, string Pais, string Contraseña)
        {
            var email = TempData["Email"]?.ToString();
            if (email == null)
                return RedirectToAction("IngresarEmail");

            var nuevoUsuario = new User { Email = email, Name = Name, Surname = Surname, /*Pais = Pais,*/ Password = Contraseña }; // Guarda la contraseña
            await _userService.AddAsync(nuevoUsuario);

            // Iniciar sesión automáticamente después del registro
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, email),
                new Claim("Name", Name ?? ""),
                new Claim("Surname", Surname ?? ""),
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
                    return View(user); // Pasa el objeto User a la vista de edición
                }
            }
            return RedirectToAction("Index", "Home"); // Si no está autenticado o no se encuentra el usuario
        }


        [HttpPost]
        public async Task<IActionResult> EditProfile(User model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userService.GetByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    existingUser.Name = model.Name;
                    existingUser.Surname = model.Surname;
                    //existingUser.Pais = model.Pais;
                    existingUser.Password = model.Password; // Permite cambiar la contraseña
                    await _userService.UpdateAsync(existingUser);

                    // Actualizar las Claims del usuario si es necesario
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, model.Email),
                        new Claim("Name", model.Name ?? ""),
                        new Claim("Surname", model.Surname ?? ""),
                       // new Claim("Pais", model.Pais ?? "")
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties { IsPersistent = true }; // Mantener la sesión

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return RedirectToAction("Index", "Home"); // Redirigir a la página principal
                }
                ViewBag.ErrorMessage = "Error al guardar el perfil.";
                return View(model);
            }
            return View(model);
        }
    }
}