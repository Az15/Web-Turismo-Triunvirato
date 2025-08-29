using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Web_Turismo_Triunvirato.Models;
using Web_Turismo_Triunvirato.ViewModels; // Asegúrate de tenerlo si usas otros ViewModels, si no, puedes quitarlo
using Web_Turismo_Triunvirato.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System; // Para DateTime

using Web_Turismo_Triunvirato.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Web_Turismo_Triunvirato.Controllers
{
    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IPromotionService _promotionService; // Campo privado para almacenar el servicio

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext, IPromotionService promotionService)
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

            var collection_Index = new View_Index_Collection
            {
                DestinationCarrousel = Carousel,
                PopularDestinations = Destinys
            };

            return View(collection_Index);
        }

        [HttpGet]
        public async Task<IActionResult> Flights() // <--- ESTA ACCIÓN ES LA QUE DEBE ESTAR ACTUALIZADA
        {
            // Obtener solo las promociones de tipo Vuelos que están activas y dentro del rango de fechas
            // La fecha actual es Jueves, 24 de julio de 2025.

            //var activeFlightPromotions = await _promotionService.GetPromotionsByTypeAsync(ServiceType.Vuelos);
            var activeFlightPromotions = await _dbContext.GetViewFlightpromotionsItemsAsync();

            ViewData["Title"] = "Vuelos";
            // Pasar la lista de promociones directamente a la vista
            return View(activeFlightPromotions); // <--- Aquí se pasa IEnumerable<Promotion>
        }

        [HttpGet]
        public async Task<IActionResult> RutaAtlantica() // <--- ESTA ACCIÓN ES LA QUE DEBE ESTAR ACTUALIZADA
        {
            // Obtener solo las promociones de tipo Vuelos que están activas y dentro del rango de fechas
            // La fecha actual es Jueves, 24 de julio de 2025.

            return View(); // <--- Aquí se pasa IEnumerable<Promotion>
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


        // NUEVO: Acción para cargar las empresas de encomiendas y pasarlas a la vista
        [HttpGet]
        public async Task<IActionResult> SendBox()
        {
            // Obtiene la lista de empresas de encomiendas de la base de datos
            var encomiendaCompanies = await _dbContext.EncomiendaCompanies.ToListAsync();

            // Pasa la lista de empresas a la vista SendBox.cshtml
            return View(encomiendaCompanies);
        }


        [HttpGet]
        public async Task<IActionResult> Hotels()
        {
            var ModelHotelsItems = await _dbContext.GetViewHotelspromotionsItemsAsync();

            return View(ModelHotelsItems);
        }

        [HttpGet]
        public async Task<IActionResult> Bus()
        {
            var ModelBusesItems = await _dbContext.GetViewBusspromotionsItemsAsync();

            return View(ModelBusesItems);
        }

        [HttpGet]
        public IActionResult TravelPackages()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Offers()
        {
            // Por ahora, pasamos listas vacías para que se muestre el mensaje de "no se encontraron"
            ViewBag.Packages = new List<string>();
            ViewBag.Trips = new List<string>();
            ViewBag.Lodging = new List<string>();
            ViewBag.Activities = new List<string>();

            // Cuando tengas la base de datos, tu código se verá así:
            // ViewBag.Packages = _repository.GetPromotionalPackages();
            // ViewBag.Trips = _repository.GetPromotionalTrips();
            // ...etc.

            return View();
        }

        [HttpGet]
        public IActionResult Activities()
        {
            return View();
        }

        [HttpGet]
        public IActionResult TravelAssistance()
        {
            return View();
        }

    }
}
