using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Web_Turismo_Triunvirato.Models;
using Web_Turismo_Triunvirato.ViewModels; // Asegúrate de tenerlo si usas otros ViewModels, si no, puedes quitarlo
using Web_Turismo_Triunvirato.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System; // Para DateTime

namespace Web_Turismo_Triunvirato.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPromotionService _promotionService; // Campo privado para almacenar el servicio

        // Constructor: Inyecta ILogger y IPromotionService
        public HomeController(ILogger<HomeController> logger, IPromotionService promotionService)
        {
            _logger = logger;
            _promotionService = promotionService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Vuelos() // <--- ESTA ACCIÓN ES LA QUE DEBE ESTAR ACTUALIZADA
        {
            // Obtener solo las promociones de tipo Vuelos que están activas y dentro del rango de fechas
            // La fecha actual es Jueves, 24 de julio de 2025.
            var activeFlightPromotions = await _promotionService.GetPromotionsByTypeAsync(ServiceType.Vuelos);
            activeFlightPromotions = activeFlightPromotions
                                        .Where(p => p.IsActive && p.EndDate >= DateTime.Today)
                                        .OrderByDescending(p => p.IsHotWeek) // Hot Week primero
                                        .ThenBy(p => p.OfferPrice)           // Luego por precio
                                        .ToList();

            ViewData["Title"] = "Vuelos";
            // Pasar la lista de promociones directamente a la vista
            return View(activeFlightPromotions); // <--- Aquí se pasa IEnumerable<Promotion>
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Booking()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Help()
        {
            return View();
        }

        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult SendBox()
        {
            return View();
        }
    }
}