using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Web_Turismo_Triunvirato.Models;
using Web_Turismo_Triunvirato.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Web_Turismo_Triunvirato.DataAccess;

namespace Web_Turismo_Triunvirato.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public AccountController( ApplicationDbContext dbContext)
        {
           _dbContext=  dbContext;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Authenticate(string email, string password)
        {
            var user = await _dbContext.LoginUserAsync(email, password);

            if (user != null)
            {
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, email),
            new Claim("Name", user.Name ?? ""),
            new Claim("Surname", user.Surname ?? ""),
            new Claim(ClaimTypes.Role, user.Rol) // Se obtiene el rol directamente de la base de datos
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties { };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // Redirige basándose en el rol del usuario
                if (user.Rol == "Admin")
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

            // Crea el objeto User con todos los datos, incluyendo el país
            var nuevoUsuario = new User
            {
                Email = email,
                Name = Name,
                Surname = Surname,
                Country = Pais, // Asegúrate de que tu modelo usa 'Country', no 'Pais'
                Password = Contraseña
            };

            // Llama al Stored Procedure a través del DbContext
            await _dbContext.CreateUserAsync(nuevoUsuario);

            // El resto de la lógica de autenticación se mantiene igual
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
                // Reemplaza la llamada a _userService con la nueva llamada a _dbContext
                var user = await _dbContext.GetUserByEmailAsync(email);

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
                // Obtener el usuario existente por su email
                var existingUser = await _dbContext.GetUserByEmailAsync(model.Email);

                if (existingUser != null)
                {
                    // Asignar el ID del usuario existente al modelo para la actualización
                    model.Id = existingUser.Id;

                    // Llamar al Stored Procedure para actualizar el usuario
                    await _dbContext.UpdateUserAsync(model);

                    // Actualizar las Claims del usuario si es necesario
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Email),
                new Claim("Name", model.Name ?? ""),
                new Claim("Surname", model.Surname ?? ""),
                new Claim("Pais", model.Country ?? "")
            };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties { IsPersistent = true };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return RedirectToAction("Index", "Home");
                }

                ViewBag.ErrorMessage = "Error al guardar el perfil.";
                return View(model);
            }
            return View(model);
        }
    }
}