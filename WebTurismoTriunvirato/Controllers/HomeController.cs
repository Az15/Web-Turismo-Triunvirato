using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System; // Para DateTime
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Web_Turismo_Triunvirato.DataAccess;
using Web_Turismo_Triunvirato.Models;
using Web_Turismo_Triunvirato.Models.ViewModels;
using Web_Turismo_Triunvirato.Services;
using Web_Turismo_Triunvirato.Services;
using Web_Turismo_Triunvirato.ViewModels; // Asegúrate de tenerlo si usas otros ViewModels, si no, puedes quitarlo

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
        public async Task<IActionResult> Index()
        {
            // ... (Lógica de autenticación/redirección) ...

            var Carousel = await _dbContext.GetCarouselItemsAsync();
            var Destinys = await _dbContext.GetHotDestinyItemsAsync();

            // Diccionario para almacenar en caché las plantillas de WhatsApp ya cargadas (WhatsappId -> WhatsappMessage)
            var whatsappTemplatesCache = new Dictionary<int, WhatsappMessage>();

            // --- 1. Procesar Carrousel ---
            foreach (var item in Carousel)
            {
                // Asumo que el objeto item tiene una propiedad "WhatsappId"
                int itemWhatsappId = item.Whatsapp_Id; // **Asegúrate de que esta propiedad exista en tu modelo**

                if (itemWhatsappId > 0)
                {
                    WhatsappMessage template;

                    // Intentar obtener la plantilla de la caché
                    if (!whatsappTemplatesCache.TryGetValue(itemWhatsappId, out template))
                    {
                        // Si no está en caché, buscar en la BD y agregar a la caché
                        template = await _dbContext.GetWhatsappMessageByIdAsync(itemWhatsappId);
                        if (template != null)
                        {
                            whatsappTemplatesCache.Add(itemWhatsappId, template);
                        }
                    }

                    // Renderizar el mensaje si la plantilla fue encontrada
                    if (template != null)
                    {
                        item.RenderedWhatsappMessage = WhatsappMessage.RenderWts(template.Message_Template, item);
                    }
                    else
                    {
                        item.RenderedWhatsappMessage = "¡Descubre este destino increíble con nosotros!";
                    }
                }
                else
                {
                    item.RenderedWhatsappMessage = "No se encontro un mensaje para esta ubicación";
                }
            }

            
            // --- 2. Procesar Destinys ---
            foreach (var item in Destinys)
            {
                // Asumo que el objeto item tiene una propiedad "WhatsappId"
                int itemWhatsappId = item.Whatsapp_Id; // **Asegúrate de que esta propiedad exista en tu modelo**

                if (itemWhatsappId > 0)
                {
                    WhatsappMessage template;

                    // Intentar obtener la plantilla de la caché
                    if (!whatsappTemplatesCache.TryGetValue(itemWhatsappId, out template))
                    {
                        // Si no está en caché, buscar en la BD y agregar a la caché
                        template = await _dbContext.GetWhatsappMessageByIdAsync(itemWhatsappId);
                        if (template != null)
                        {
                            whatsappTemplatesCache.Add(itemWhatsappId, template);
                        }
                    }

                    // Renderizar el mensaje si la plantilla fue encontrada
                    if (template != null)
                    {
                        item.RenderedWhatsappMessage = WhatsappMessage.RenderWts(template.Message_Template, item);
                      
                    }
                    else
                    {
                                                item.RenderedWhatsappMessage = "¡Descubre este destino increíble con nosotros!";
                    }
                }
                else
                {
                    item.RenderedWhatsappMessage = "No se encontro un mensaje para esta ubicación";
                }
            }

            var collection_Index = new View_Index_Collection
            {
                DestinationCarrousel = Carousel,
                PopularDestinations = Destinys
            };





            return View(collection_Index);
        }

        [HttpGet]
        public async Task<IActionResult> Flights()
        {
            var flightPromotions = await _dbContext.GetViewFlightpromotionsItemsAsync();

            int whatsappMessageId = flightPromotions.FirstOrDefault()?.Whatsapp_Id ?? 1;
            var whatsappTemplate = await _dbContext.GetWhatsappMessageByIdAsync(whatsappMessageId);

            foreach (var promotion in flightPromotions)
            {
                // --- LÓGICA DE IMÁGENES PARA VUELOS ---
                var todasLasImagenes = await _dbContext.GetImagenesByEntidadAsync(promotion.entidad, promotion.Id);

                if (todasLasImagenes != null && todasLasImagenes.Any())
                {
                    var imagenesOrdenadas = todasLasImagenes.OrderBy(i => i.Id).ToList();

                    // Portada (Card exterior)
                    promotion.ImageUrl = imagenesOrdenadas[0].Url;

                    // Lista completa para el Modal (incluye la primera)
                    promotion.ImagenesAdicionales = imagenesOrdenadas;
                }

                // --- LÓGICA DE WHATSAPP ---
                var dataObject = new
                {
                    origin = promotion.OriginName,
                    destination = promotion.DestinationName,
                    price = promotion.OfferPrice.ToString("C0", new System.Globalization.CultureInfo("es-AR"))
                };

                promotion.RenderedWhatsappMessage = WhatsappMessage.RenderWts(whatsappTemplate.Message_Template, dataObject);
            }

            ViewData["Title"] = "Vuelos";
            return View(flightPromotions);
        }


        [HttpGet]
        public  IActionResult RutaAtlantica() 
        {
            return View(); 
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
            var hotelPromotions = await _dbContext.GetViewHotelspromotionsItemsAsync();

            int whatsappMessageId = hotelPromotions.FirstOrDefault()?.Whatsapp_Id ?? 1;
            var whatsappTemplate = await _dbContext.GetWhatsappMessageByIdAsync(whatsappMessageId);

            foreach (var promotion in hotelPromotions)
            {
                // --- NUEVA LÓGICA DE IMÁGENES ---
                // Llamamos al SP de MySQL pasándole la entidad y el ID del hotel
                var todasLasImagenes = await _dbContext.GetImagenesByEntidadAsync(promotion.entidad, promotion.Id);

                if (todasLasImagenes != null && todasLasImagenes.Any())
                {
                    var imagenesOrdenadas = todasLasImagenes.OrderBy(i => i.Id).ToList();

                    // La primera imagen es la de portada (index 0)
                    promotion.ImageUrl = imagenesOrdenadas[0].Url;

                    // Guardamos todas para la galería del modal (incluyendo la principal)
                    promotion.ImagenesAdicionales = imagenesOrdenadas;
                }

                // --- LÓGICA DE WHATSAPP ---
                var dataObject = new
                {
                    HotelName = promotion.HotelName, // Corregido: Usamos el nombre del hotel real
                    DestinationName = promotion.DestinationName,
                    OfferPrice = promotion.OfferPrice.ToString("C0", new CultureInfo("es-AR")),
                };

                promotion.RenderedWhatsappMessage = WhatsappMessage.RenderWts(whatsappTemplate.Message_Template, dataObject);
            }

            return View(hotelPromotions);
        }

        [HttpGet]
        public async Task<IActionResult> Bus()
        {
            var busPromotions = await _dbContext.GetViewBusspromotionsItemsAsync();

            var firstBusPromotion = busPromotions.FirstOrDefault();
            int whatsappMessageId = firstBusPromotion?.Whatsapp_Id ?? 1;

            var whatsappTemplate = await _dbContext.GetWhatsappMessageByIdAsync(whatsappMessageId);

            foreach (var promotion in busPromotions)
            {
                // --- LÓGICA DE GALERÍA PARA BUSES ---
                var todasLasImagenes = await _dbContext.GetImagenesByEntidadAsync(promotion.entidad, promotion.Id);

                if (todasLasImagenes != null && todasLasImagenes.Any())
                {
                    var imagenesOrdenadas = todasLasImagenes.OrderBy(i => i.Id).ToList();

                    // Imagen principal de la Card
                    promotion.ImageUrl = imagenesOrdenadas[0].Url;

                    // Galería completa para el modal
                    promotion.ImagenesAdicionales = imagenesOrdenadas;
                }

                // --- LÓGICA DE WHATSAPP ---
                var dataObject = new
                {
                    originName = promotion.OriginName,
                    destinationName = promotion.DestinationName,
                    offerPrice = promotion.OfferPrice.ToString("C0", new System.Globalization.CultureInfo("es-AR"))
                };

                promotion.RenderedWhatsappMessage = Web_Turismo_Triunvirato.Models.WhatsappMessage.RenderWts(whatsappTemplate.Message_Template, dataObject);
            }

            return View(busPromotions);
        }

        [HttpGet]
        public async Task<IActionResult> TravelPackages()
        {
            var packagePromotions = await _dbContext.GetActivePromotionPackagesItemsAsync();

            // Obtener el template de WhatsApp una sola vez
            int whatsappMessageId = packagePromotions.FirstOrDefault()?.Whatsapp_Id ?? 1;
            var whatsappTemplate = await _dbContext.GetWhatsappMessageByIdAsync(whatsappMessageId);

            foreach (var promotion in packagePromotions)
            {
                // 1. Llamar al SP. Pasamos los valores del objeto actual.
                // Asumo que promotion.entidad ya viene cargado con el ID de entidad de "Paquetes"
                var todasLasImagenes = await _dbContext.GetImagenesByEntidadAsync(promotion.entidad, promotion.Id);

                if (todasLasImagenes != null && todasLasImagenes.Any())
                {
                    // Ordenamos por ID para que el orden sea predecible
                    var imagenesOrdenadas = todasLasImagenes.OrderBy(i => i.Id).ToList();

                    // La primera imagen (posición 0) es la destacada
                    promotion.ImageUrl = imagenesOrdenadas.First().Url;

                    // El resto (a partir de la posición 1) van a la galería
                    promotion.ImagenesAdicionales = imagenesOrdenadas.ToList();
                }

                // 2. Lógica de WhatsApp...
                var dataObject = new
                {
                    promotion.Description,
                    promotion.DestinationName,
                    OfferPrice = promotion.OfferPrice.ToString("C0", new System.Globalization.CultureInfo("es-AR")),
                    promotion.PackageType
                };
                promotion.RenderedWhatsappMessage = WhatsappMessage.RenderWts(whatsappTemplate.Message_Template, dataObject);
            }

            return View(packagePromotions);
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
        public async Task<IActionResult> Activities()
        {
            var activeActivities = await _dbContext.GetActiveActivitiesAsync();

            int whatsappMessageId = activeActivities.FirstOrDefault()?.Whatsapp_Id ?? 1;
            var whatsappTemplate = await _dbContext.GetWhatsappMessageByIdAsync(whatsappMessageId);

            foreach (var activity in activeActivities)
            {
                // --- CARGA DE GALERÍA ---
                var todasLasImagenes = await _dbContext.GetImagenesByEntidadAsync(activity.entidad, activity.Id);

                if (todasLasImagenes != null && todasLasImagenes.Any())
                {
                    var imagenesOrdenadas = todasLasImagenes.OrderBy(i => i.Id).ToList();
                    activity.ImageUrl = imagenesOrdenadas[0].Url; // Portada
                    activity.ImagenesAdicionales = imagenesOrdenadas; // Galería completa
                }

                // --- RENDERIZADO WHATSAPP ---
                var dataObject = new
                {
                    Title = activity.Title,
                    Location = activity.Location,
                    Description = activity.Description
                };
                activity.RenderedWhatsappMessage = WhatsappMessage.RenderWts(whatsappTemplate.Message_Template, dataObject);
            }

            return View(activeActivities);
        }

        [HttpGet]
        public IActionResult TravelAssistance()
        {
            return View();
        }

    }
}
