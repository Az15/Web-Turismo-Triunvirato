using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Web_Turismo_Triunvirato.Models;
using Web_Turismo_Triunvirato.ViewModels;
using Web_Turismo_Triunvirato.Data;

namespace Web_Turismo_Triunvirato.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger; 
        private readonly ApplicationDbContext _dbContext;

        public HomeController(ILogger<HomeController> logger , ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
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
        public IActionResult Vuelos()
        {
            ViewData.Clear();
            var viewModel = new DestinationsViewModel
            {
                PopularDestinations = new List<View_Index_Destination>
                    {
                        new View_Index_Destination { Title = "Bariloche", PictureDestiny = "~/img/Bariloche.jpg", Price = 1056067, From = "Desde Buenos Aires", DetailDestinyURL = "/Destinos/Madrid", IsHotWeek = true },
                        new View_Index_Destination { Title = "Río de Janeiro", PictureDestiny = "~/img/RiodeJaneiro.jpeg", Price = 217460, From = "Desde Buenos Aires", DetailDestinyURL = "/Destinos/RioDeJaneiro" },
                        new View_Index_Destination { Title = "Barcelona", PictureDestiny = "~/img/Barcelona.jpeg", Price = 932300, From = "Desde Buenos Aires", DetailDestinyURL = "/Destinos/Barcelona", IsHotWeek = true },
                        new View_Index_Destination { Title = "Miami", PictureDestiny = "~/img/Miami.jpeg", Price = 675513, From = "Desde Buenos Aires", DetailDestinyURL = "/Destinos/Miami" },
                        new View_Index_Destination { Title = "Roma", PictureDestiny = "~/images/roma.jpg", Price = 1096552, From = "Desde Buenos Aires", DetailDestinyURL = "/Destinos/Roma" },
                        new View_Index_Destination { Title = "San Carlos de Bariloche", PictureDestiny = "~/images/san-carlos-de-bariloche.jpg", Price = 47425, From = "Desde Buenos Aires", DetailDestinyURL = "/Destinos/Bariloche", IsHotWeek = true },
                        new View_Index_Destination { Title = "Cataratas del Iguazú", PictureDestiny = "~/images/cataratas-del-iguazu.jpg", Price = 49738, From = "Desde Buenos Aires", DetailDestinyURL = "/Destinos/Iguazu" },
                        new View_Index_Destination { Title = "Santiago de Chile", PictureDestiny = "~/images/santiago-de-chile.jpg", Price = 192012, From = "Desde Buenos Aires", DetailDestinyURL = "/Destinos/Santiago" },
                        new View_Index_Destination { Title = "Punta Cana", PictureDestiny = "~/images/punta-cana.jpg", Price = 636185, From = "Desde Buenos Aires", DetailDestinyURL = "/Destinos/PuntaCana", IsHotWeek = true },
                        new View_Index_Destination { Title = "Aruba", PictureDestiny = "~/images/aruba.jpg", Price = 493911, From = "Desde Buenos Aires", DetailDestinyURL = "/Destinos/Aruba" },
                        new View_Index_Destination { Title = "Cancún", PictureDestiny = "~/images/cancun.jpg", Price = 643125, From = "Desde Buenos Aires", DetailDestinyURL = "/Destinos/Cancun" },
                        new View_Index_Destination { Title = "Nueva York", PictureDestiny = "~/images/nueva-york.jpg", Price = 743758, From = "Desde Buenos Aires", DetailDestinyURL = "/Destinos/NuevaYork", IsHotWeek = true }
                    }
            };
            return View(viewModel);
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
