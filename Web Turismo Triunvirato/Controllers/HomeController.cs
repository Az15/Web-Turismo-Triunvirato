using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Web_Turismo_Triunvirato.Models;
using Web_Turismo_Triunvirato.ViewModels;

namespace Web_Turismo_Triunvirato.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {

                    return View();

        }


        [HttpGet]
        public IActionResult Vuelos()
        {
            ViewData.Clear();
            var viewModel = new DestinationsViewModel
            {
                PopularDestinations = new List<Destination>
                    {
                        new Destination { NombreDestio = "Bariloche", ImagenDestino = "~/img/Bariloche.jpg", Precio = 1056067, From = "Desde Buenos Aires", DetalleDestino = "/Destinos/Madrid", IsHotWeek = true },
                        new Destination { NombreDestio = "Río de Janeiro", ImagenDestino = "~/img/RiodeJaneiro.jpeg", Precio = 217460, From = "Desde Buenos Aires", DetalleDestino = "/Destinos/RioDeJaneiro" },
                        new Destination { NombreDestio = "Barcelona", ImagenDestino = "~/img/Barcelona.jpeg", Precio = 932300, From = "Desde Buenos Aires", DetalleDestino = "/Destinos/Barcelona", IsHotWeek = true },
                        new Destination { NombreDestio = "Miami", ImagenDestino = "~/img/Miami.jpeg", Precio = 675513, From = "Desde Buenos Aires", DetalleDestino = "/Destinos/Miami" },
                        //new Destination { NombreDestio = "Roma", ImagenDestino = "~/images/roma.jpg", Precio = 1096552, From = "Desde Buenos Aires", DetalleDestino = "/Destinos/Roma" },
                        //new Destination { NombreDestio = "San Carlos de Bariloche", ImagenDestino = "~/images/san-carlos-de-bariloche.jpg", Precio = 47425, From = "Desde Buenos Aires", DetalleDestino = "/Destinos/Bariloche", IsHotWeek = true },
                        //new Destination { NombreDestio = "Cataratas del Iguazú", ImagenDestino = "~/images/cataratas-del-iguazu.jpg", Precio = 49738, From = "Desde Buenos Aires", DetalleDestino = "/Destinos/Iguazu" },
                        //new Destination { NombreDestio = "Santiago de Chile", ImagenDestino = "~/images/santiago-de-chile.jpg", Precio = 192012, From = "Desde Buenos Aires", DetalleDestino = "/Destinos/Santiago" },
                        //new Destination { NombreDestio = "Punta Cana", ImagenDestino = "~/images/punta-cana.jpg", Precio = 636185, From = "Desde Buenos Aires", DetalleDestino = "/Destinos/PuntaCana", IsHotWeek = true },
                        //new Destination { NombreDestio = "Aruba", ImagenDestino = "~/images/aruba.jpg", Precio = 493911, From = "Desde Buenos Aires", DetalleDestino = "/Destinos/Aruba" },
                        //new Destination { NombreDestio = "Cancún", ImagenDestino = "~/images/cancun.jpg", Precio = 643125, From = "Desde Buenos Aires", DetalleDestino = "/Destinos/Cancun" },
                        //new Destination { NombreDestio = "Nueva York", ImagenDestino = "~/images/nueva-york.jpg", Precio = 743758, From = "Desde Buenos Aires", DetalleDestino = "/Destinos/NuevaYork", IsHotWeek = true }
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
