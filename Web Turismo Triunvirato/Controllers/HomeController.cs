using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Web_Turismo_Triunvirato.Models;
using Web_Turismo_Triunvirato.ViewModels; // Asegúrate de tenerlo si usas otros ViewModels, si no, puedes quitarlo
using Web_Turismo_Triunvirato.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System; // Para DateTime

using Web_Turismo_Triunvirato.Data;

namespace Web_Turismo_Triunvirato.Controllers
{
    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger; 
        private readonly ApplicationDbContext _dbContext;
        private readonly IPromotionService _promotionService; // Campo privado para almacenar el servicio

        public HomeController(ILogger<HomeController> logger , ApplicationDbContext dbContext , IPromotionService promotionService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _promotionService = promotionService;
        }

        [HttpGet]
        public async Task<IActionResult> Index() // <-- ¡IMPORTANTE! Hacer el método async Task<IActionResult>
        {

            // Ejecuta la tarea asíncrona y espera su resultado
            var Carousel = await _dbContext.GetCarouselItemsAsync();
            // Verifica si la lista de Carousel es nula o está vacía
            var Destinys = await _dbContext.GetHotDestinyItemsAsync();

            //Comente esta linea porque tengo que hacer unas pruebas de integridad de datos antes de continuar

            //if ( (Carousel == null || !Carousel.Any()) && (Destinys == null || !Destinys.Any()) )
            //{
            //    // Maneja el caso donde no hay elementos en el carrusel
            //    _logger.LogWarning("No carousel items found or Destinys Not Found.");
            //    return View(new View_Index_Collection()); // Retorna una vista vacía o con un modelo vacío
            //}

            var collection_Index = new View_Index_Collection
            {
                DestinationCarrousel = Carousel,
                PopularDestinations = Destinys
            };


            return View(collection_Index);
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