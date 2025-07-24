using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; 
using System.Threading.Tasks;

namespace Web_Turismo_Triunvirato.Controllers
{
    
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        // Puedes inyectar servicios aquí si los necesitas (ej. IPromotionService, IUserService)
        // private readonly IPromotionService _promotionService;
        // public AdminController(IPromotionService promotionService)
        // {
        //     _promotionService = promotionService;
        // }

        // Acción por defecto al entrar al panel de administración (la "pantalla principal")
        public IActionResult Index()
        {
            ViewData["Title"] = "Panel de Administración";
            return View(); 
        }

   
        public IActionResult Perfil()
        {
            ViewData["Title"] = "Perfil del Administrador";
            return View();
        }

        public IActionResult Vuelos()
        {
            ViewData["Title"] = "Administración de Vuelos"; 
            return View();
        }

        public IActionResult Hoteles()
        {
            ViewData["Title"] = "Administración de Hoteles";
            return View();
        }

        public IActionResult Buses()
        {
            ViewData["Title"] = "Administración de Buses";
            return View();
        }

        public IActionResult Paquetes()
        {
            ViewData["Title"] = "Administración de Paquetes";
            return View();
        }

        
    }
}