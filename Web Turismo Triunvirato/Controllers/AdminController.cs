using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Web_Turismo_Triunvirato.Services;
using Web_Turismo_Triunvirato.Models;
using System;
using Microsoft.EntityFrameworkCore;
using Web_Turismo_Triunvirato.DataAccess;

namespace Web_Turismo_Triunvirato.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IPromotionService _promotionService;

        private readonly ApplicationDbContext _dbContext;

        public AdminController(IPromotionService promotionService, ApplicationDbContext dbContext)
        {
            _promotionService = promotionService;
            _dbContext = dbContext;
        }

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

        // Acciones para mostrar las vistas de gestión por tipo
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

        // **************** ACCIONES PARA ADMINISTRACIÓN DE PROMOCIONES ****************

        // GET: Admin/AltaPromocion (Vuelos)
        [HttpGet]
        public IActionResult AltaPromocion()
        {
            ViewData["Title"] = "Alta de Promoción de Vuelo";
            return View("AltaPromocion", new FlightPromotion { ServiceType = "Vuelos" });
        }

        // POST: Admin/AltaPromocion (Vuelos)
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> AltaPromocion(FlightPromotion promotion)
        //{
        //    promotion.ServiceType = S;
        //    if (ModelState.IsValid)
        //    {
        //        if (promotion.OriginalPrice > 0)
        //        {
        //            promotion.DiscountPercentage = Math.Round(((promotion.OriginalPrice - promotion.OfferPrice) / promotion.OriginalPrice) * 100, 2);
        //        }
        //        await _promotionService.AddPromotionAsync(promotion);
        //        TempData["SuccessMessage"] = "¡Promoción de vuelo agregada exitosamente!";
        //        return RedirectToAction(nameof(AdminPromotionFlights));
        //    }
        //    ViewData["Title"] = "Alta de Promoción de Vuelo";
        //    return View("AltaPromocion", promotion);
        //}

        // GET: Admin/AltaPromocionHotel (Hoteles)
        [HttpGet]
        public IActionResult AltaPromocionHotel()
        {
            ViewData["Title"] = "Alta de Promoción de Hotel";
            return View("AltaPromocion", new Promotion { ServiceType = ServiceType.Hoteles });
        }

        
        // POST: Admin/AltaPromocionHotel (Hoteles)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AltaPromocionHotel(Promotion promotion)
        {
            promotion.ServiceType = ServiceType.Hoteles;
            if (ModelState.IsValid)
            {
                if (promotion.OriginalPrice > 0)
                {
                    promotion.DiscountPercentage = Math.Round(((promotion.OriginalPrice - promotion.OfferPrice) / promotion.OriginalPrice) * 100, 2);
                }
                await _promotionService.AddPromotionAsync(promotion); // Esta línea ahora usará el nuevo servicio
                TempData["SuccessMessage"] = "¡Promoción de hotel agregada exitosamente!";
                return RedirectToAction(nameof(GestionarPromocionesHoteles));
            }
            ViewData["Title"] = "Alta de Promoción de Hotel";
            return View("AltaPromocion", promotion);
        }

        // GET: Admin/AltaPromocionBus (Buses)
        [HttpGet]
        public IActionResult AltaPromocionBus()
        {
            ViewData["Title"] = "Alta de Promoción de Bus";
            return View("AltaPromocion", new Promotion { ServiceType = ServiceType.Buses });
        }

        // POST: Admin/AltaPromocionBus (Buses)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AltaPromocionBus(Promotion promotion)
        {
            promotion.ServiceType = ServiceType.Buses;
            if (ModelState.IsValid)
            {
                if (promotion.OriginalPrice > 0)
                {
                    promotion.DiscountPercentage = Math.Round(((promotion.OriginalPrice - promotion.OfferPrice) / promotion.OriginalPrice) * 100, 2);
                }
                await _promotionService.AddPromotionAsync(promotion);
                TempData["SuccessMessage"] = "¡Promoción de bus agregada exitosamente!";
                return RedirectToAction(nameof(GestionarPromocionesBuses));
            }
            ViewData["Title"] = "Alta de Promoción de Bus";
            return View("AltaPromocion", promotion);
        }

        // GET: Admin/AltaPromocionPaquete (NUEVA: Paquetes)
        [HttpGet]
        public IActionResult AltaPromocionPaquete()
        {
            ViewData["Title"] = "Alta de Promoción de Paquete";
            return View("AltaPromocion", new Promotion { ServiceType = ServiceType.Paquetes });
        }

        // POST: Admin/AltaPromocionPaquete (NUEVA: Paquetes)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AltaPromocionPaquete(Promotion promotion)
        {
            promotion.ServiceType = ServiceType.Paquetes;
            if (ModelState.IsValid)
            {
                if (promotion.OriginalPrice > 0)
                {
                    promotion.DiscountPercentage = Math.Round(((promotion.OriginalPrice - promotion.OfferPrice) / promotion.OriginalPrice) * 100, 2);
                }
                await _promotionService.AddPromotionAsync(promotion);
                TempData["SuccessMessage"] = "¡Promoción de paquete agregada exitosamente!";
                return RedirectToAction(nameof(GestionarPromocionesPaquetes));
            }
            ViewData["Title"] = "Alta de Promoción de Paquete";
            return View("AltaPromocion", promotion);
        }

        // GET: Admin/GestionarPromocionesVuelos
        [HttpGet]
        public async Task<IActionResult> AdminPromotionFlights()
        {
            ViewData["Title"] = "Gestionar Promociones de Vuelos";
            var flightPromotions = await _dbContext.GetActiveflightpromotionsItemsAsync();
            return View(flightPromotions);
            //var flightPromotions = await _promotionService.GetPromotionsByTypeAsync(ServiceType.Vuelos);
            //flightPromotions = flightPromotions.Where(p => p.IsActive && p.EndDate >= DateTime.Today).ToList();
            //return View("GestionarPromocionesVuelos", flightPromotions.OrderByDescending(p => p.Id));
        }

        // GET: Admin/GestionarPromocionesHoteles
        [HttpGet]
        public async Task<IActionResult> GestionarPromocionesHoteles()
        {
            ViewData["Title"] = "Gestionar Promociones de Hoteles";
            var hotelPromotions = await _promotionService.GetPromotionsByTypeAsync(ServiceType.Hoteles);
            hotelPromotions = hotelPromotions.Where(p => p.IsActive && p.EndDate >= DateTime.Today).ToList();
            return View("GestionarPromocionesHoteles", hotelPromotions.OrderByDescending(p => p.Id));
        }

        // GET: Admin/GestionarPromocionesBuses
        [HttpGet]
        public async Task<IActionResult> GestionarPromocionesBuses()
        {
            ViewData["Title"] = "Gestionar Promociones de Buses";
            var busPromotions = await _promotionService.GetPromotionsByTypeAsync(ServiceType.Buses);
            busPromotions = busPromotions.Where(p => p.IsActive && p.EndDate >= DateTime.Today).ToList();
            return View("GestionarPromocionesBuses", busPromotions.OrderByDescending(p => p.Id));
        }

        // GET: Admin/GestionarPromocionesPaquetes (NUEVA: Paquetes)
        [HttpGet]
        public async Task<IActionResult> GestionarPromocionesPaquetes()
        {
            ViewData["Title"] = "Gestionar Promociones de Paquetes";
            var packagePromotions = await _promotionService.GetPromotionsByTypeAsync(ServiceType.Paquetes);
            packagePromotions = packagePromotions.Where(p => p.IsActive && p.EndDate >= DateTime.Today).ToList();
            return View("GestionarPromocionesPaquetes", packagePromotions.OrderByDescending(p => p.Id));
        }


        // GET: Admin/ListarPromociones (También lo redirigimos a lo mismo)
        [HttpGet]
        public async Task<IActionResult> ListarPromociones()
        {
            ViewData["Title"] = "Listado de Promociones de Vuelos";

            // Llama a tu método del DbContext que usa la Stored Procedure
            var flightPromotions = await _dbContext.GetActiveflightpromotionsItemsAsync();

            // Devuelve la lista de promociones de vuelos a la vista
            return View("AdminPromotionFlights", flightPromotions);
        }


        // GET: Admin/EditarPromocion/{id}
        [HttpGet]
        public async Task<IActionResult> EditarPromocion(int id)
        {
            ViewData["Title"] = "Editar Promoción";
            var promotion = await _promotionService.GetPromotionByIdAsync(id);
            if (promotion == null)
            {
                return NotFound();
            }
            return View("AltaPromocion", promotion);
        }

        // POST: Admin/EditarPromocion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitPromotionFlight(FlightPromotion promotion)
        {
           await _dbContext.AbmFlightPromotionAsync(promotion, null);

            return View("index");

        }

        // GET: Admin/BajaPromocion/{id}
        [HttpGet]
        public async Task<IActionResult> BajaPromocion(int id)
        {
            ViewData["Title"] = "Baja de Promoción";
            var promotion = await _promotionService.GetPromotionByIdAsync(id);
            if (promotion == null)
            {
                return NotFound();
            }
            return View(promotion);
        }

        // POST: Admin/EliminarPromocion/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarPromocion(int id)
        {
            var promotion = await _promotionService.GetPromotionByIdAsync(id);
            if (promotion == null)
            {
                return NotFound();
            }

            await _promotionService.DeletePromotionAsync(id);
            TempData["SuccessMessage"] = "¡Promoción eliminada exitosamente!";

            if (promotion.ServiceType == ServiceType.Vuelos)
            {
                return RedirectToAction(nameof(AdminPromotionFlights));
            }
            else if (promotion.ServiceType == ServiceType.Hoteles)
            {
                return RedirectToAction(nameof(GestionarPromocionesHoteles));
            }
            else if (promotion.ServiceType == ServiceType.Buses)
            {
                return RedirectToAction(nameof(GestionarPromocionesBuses));
            }
            else if (promotion.ServiceType == ServiceType.Paquetes)
            {
                return RedirectToAction(nameof(GestionarPromocionesPaquetes));
            }
            return RedirectToAction(nameof(ListarPromociones));
        }
    }
}