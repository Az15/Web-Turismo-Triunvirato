using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; 
using System.Threading.Tasks;

namespace Web_Turismo_Triunvirato.Controllers
{
    
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        // Puedes inyectar servicios aqu� si los necesitas (ej. IPromotionService, IUserService)
        // private readonly IPromotionService _promotionService;
        // public AdminController(IPromotionService promotionService)
        // {
        //     _promotionService = promotionService;
        // }

        // Acci�n por defecto al entrar al panel de administraci�n (la "pantalla principal")
        public IActionResult Index()
        {
            ViewData["Title"] = "Panel de Administraci�n";
            return View(); 
        }

   
        public IActionResult Perfil()
        {
            ViewData["Title"] = "Perfil del Administrador";
            return View();
        }

        public IActionResult Vuelos()
        {
            ViewData["Title"] = "Administraci�n de Vuelos"; 
            return View();
        }

        public IActionResult Hoteles()
        {
            ViewData["Title"] = "Administraci�n de Hoteles";
            return View();
        }

        public IActionResult Buses()
        {
            ViewData["Title"] = "Administraci�n de Buses";
            return View();
        }

        public IActionResult Paquetes()
        {
            ViewData["Title"] = "Administraci�n de Paquetes";
            return View();
        }

        
    }
}