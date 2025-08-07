using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Web_Turismo_Triunvirato.Services;
using Web_Turismo_Triunvirato.Models;
using System.Linq;
using System;

namespace Web_Turismo_Triunvirato.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IPromotionService _promotionService;

        public AdminController(IPromotionService promotionService)
        {
            _promotionService = promotionService;
        }

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

        // Acciones para mostrar las vistas de gesti�n por tipo
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

        // **************** ACCIONES PARA ADMINISTRACI�N DE PROMOCIONES ****************

        // GET: Admin/AltaPromocion (Vuelos)
        [HttpGet]
        public IActionResult AltaPromocion()
        {
            ViewData["Title"] = "Alta de Promoci�n de Vuelo";
            return View("AltaPromocion", new Promotion { ServiceType = ServiceType.Vuelos });
        }

        // POST: Admin/AltaPromocion (Vuelos)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AltaPromocion(Promotion promotion)
        {
            promotion.ServiceType = ServiceType.Vuelos;
            if (ModelState.IsValid)
            {
                if (promotion.OriginalPrice > 0)
                {
                    promotion.DiscountPercentage = Math.Round(((promotion.OriginalPrice - promotion.OfferPrice) / promotion.OriginalPrice) * 100, 2);
                }
                await _promotionService.AddPromotionAsync(promotion);
                TempData["SuccessMessage"] = "�Promoci�n de vuelo agregada exitosamente!";
                return RedirectToAction(nameof(GestionarPromocionesVuelos));
            }
            ViewData["Title"] = "Alta de Promoci�n de Vuelo";
            return View("AltaPromocion", promotion);
        }

        // GET: Admin/AltaPromocionHotel (Hoteles)
        [HttpGet]
        public IActionResult AltaPromocionHotel()
        {
            ViewData["Title"] = "Alta de Promoci�n de Hotel";
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
                await _promotionService.AddPromotionAsync(promotion);
                TempData["SuccessMessage"] = "�Promoci�n de hotel agregada exitosamente!";
                return RedirectToAction(nameof(GestionarPromocionesHoteles));
            }
            ViewData["Title"] = "Alta de Promoci�n de Hotel";
            return View("AltaPromocion", promotion);
        }

        // GET: Admin/AltaPromocionBus (Buses)
        [HttpGet]
        public IActionResult AltaPromocionBus()
        {
            ViewData["Title"] = "Alta de Promoci�n de Bus";
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
                TempData["SuccessMessage"] = "�Promoci�n de bus agregada exitosamente!";
                return RedirectToAction(nameof(GestionarPromocionesBuses));
            }
            ViewData["Title"] = "Alta de Promoci�n de Bus";
            return View("AltaPromocion", promotion);
        }

        // GET: Admin/AltaPromocionPaquete (NUEVA: Paquetes)
        [HttpGet]
        public IActionResult AltaPromocionPaquete()
        {
            ViewData["Title"] = "Alta de Promoci�n de Paquete";
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
                TempData["SuccessMessage"] = "�Promoci�n de paquete agregada exitosamente!";
                return RedirectToAction(nameof(GestionarPromocionesPaquetes));
            }
            ViewData["Title"] = "Alta de Promoci�n de Paquete";
            return View("AltaPromocion", promotion);
        }

        // GET: Admin/GestionarPromocionesVuelos
        [HttpGet]
        public async Task<IActionResult> GestionarPromocionesVuelos()
        {
            ViewData["Title"] = "Gestionar Promociones de Vuelos";
            var flightPromotions = await _promotionService.GetPromotionsByTypeAsync(ServiceType.Vuelos);
            flightPromotions = flightPromotions.Where(p => p.IsActive && p.EndDate >= DateTime.Today).ToList();
            return View("GestionarPromocionesVuelos", flightPromotions.OrderByDescending(p => p.Id));
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


        // GET: Admin/ListarPromociones (Muestra todas las promociones)
        [HttpGet]
        public async Task<IActionResult> ListarPromociones()
        {
            ViewData["Title"] = "Listado de Promociones";
            var promotions = await _promotionService.GetAllPromotionsAsync();
            return View(promotions);
        }

        // GET: Admin/EditarPromocion/{id}
        [HttpGet]
        public async Task<IActionResult> EditarPromocion(int id)
        {
            ViewData["Title"] = "Editar Promoci�n";
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
        public async Task<IActionResult> EditarPromocion(Promotion promotion)
        {
            if (ModelState.IsValid)
            {
                if (promotion.OriginalPrice > 0)
                {
                    promotion.DiscountPercentage = Math.Round(((promotion.OriginalPrice - promotion.OfferPrice) / promotion.OriginalPrice) * 100, 2);
                }
                await _promotionService.UpdatePromotionAsync(promotion);
                TempData["SuccessMessage"] = "�Promoci�n actualizada exitosamente!";

                // Redirige a la lista espec�fica seg�n el tipo
                if (promotion.ServiceType == ServiceType.Vuelos)
                {
                    return RedirectToAction(nameof(GestionarPromocionesVuelos));
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
            ViewData["Title"] = "Editar Promoci�n";
            return View("AltaPromocion", promotion);
        }

        // GET: Admin/BajaPromocion/{id}
        [HttpGet]
        public async Task<IActionResult> BajaPromocion(int id)
        {
            ViewData["Title"] = "Baja de Promoci�n";
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
            TempData["SuccessMessage"] = "�Promoci�n eliminada exitosamente!";

            if (promotion.ServiceType == ServiceType.Vuelos)
            {
                return RedirectToAction(nameof(GestionarPromocionesVuelos));
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