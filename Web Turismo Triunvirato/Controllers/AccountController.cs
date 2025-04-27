using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Web_Turismo_Triunvirato.Models;
    using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
//using System.Windows.Forms;
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
            // Puedes agregar más claims si lo deseas
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    // Puedes configurar la duración de la cookie, si es persistente, etc.
                };

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

    }
}
