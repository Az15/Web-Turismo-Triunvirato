using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Web_Turismo_Triunvirato.Services;
using Web_Turismo_Triunvirato.Models;
using System;
using Microsoft.EntityFrameworkCore;
using Web_Turismo_Triunvirato.DataAccess;
using System.Linq;

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

        // **************** ACCIONES PARA ADMINISTRACIÓN DE PROMOCIONES DE VUELOS ****************

        // GET: Admin/AdminPromotionFlights
        [HttpGet]
        public async Task<IActionResult> AdminPromotionFlights()
        {
            ViewData["Title"] = "Gestionar Promociones de Vuelos";
            var flightPromotions = await _dbContext.GetActiveflightpromotionsItemsAsync();
            return View(flightPromotions);
        }

        // GET: Admin/AltaPromocion (Vuelos)
        [HttpGet]
        public IActionResult AltaPromocion()
        {
            ViewData["Title"] = "Alta de Promoción de Vuelo";
            return View("AltaPromocionFlight", new FlightPromotion { ServiceType = "0" });
        }

        // GET: Admin/EditPromotionFlight/{id}
        [HttpGet]
        public async Task<IActionResult> EditPromotionFlight(int id)
        {
            ViewData["Title"] = "Editar Promoción de Vuelo";
            var promotion = await _dbContext.GetPromotionByIdAsync(id);

            if (promotion == null)
            {
                return NotFound();
            }

            return View("AltaPromocionFlight", promotion);
        }

        // POST: Admin/SubmitPromotionFlight (Para la creación de una nueva promoción de vuelo)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitPromotionFlight(FlightPromotion promotion)
        {
            if (ModelState.IsValid)
            {
                if (promotion.OriginalPrice > 0)
                {
                    promotion.DiscountPercentage = Math.Round(((promotion.OriginalPrice - promotion.OfferPrice) / promotion.OriginalPrice) * 100, 2);
                }

                await _dbContext.AbmFlightPromotionAsync(promotion, "INSERT");

                TempData["SuccessMessage"] = "¡Promoción de vuelo creada exitosamente!";
                return RedirectToAction(nameof(AdminPromotionFlights));
            }
            ViewData["Title"] = "Alta de Promoción de Vuelo";
            return View("AltaPromocionFlight", promotion);
        }

        // POST: Admin/EditPromotionFlight/{id} (Guarda los cambios)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPromotionFlight(int id, [Bind("Id,ServiceType,Description,DestinationName,OriginName,ImageUrl,IsHotWeek,OriginalPrice,OfferPrice,DiscountPercentage,StartDate,EndDate,IsActive,Stars")] FlightPromotion promotion)
        {
            if (id != promotion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (promotion.OriginalPrice > 0)
                    {
                        promotion.DiscountPercentage = Math.Round(((promotion.OriginalPrice - promotion.OfferPrice) / promotion.OriginalPrice) * 100, 2);
                    }
                    promotion.ServiceType = "0";
                    await _dbContext.AbmFlightPromotionAsync(promotion, "UPDATE");
                    TempData["SuccessMessage"] = "¡Promoción de vuelo actualizada exitosamente!";
                    return RedirectToAction(nameof(AdminPromotionFlights));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _dbContext.FlightPromotions.AnyAsync(e => e.Id == promotion.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewData["Title"] = "Editar Promoción de Vuelo";
            return View("AltaPromocionFlight", promotion);
        }

        // POST: Admin/DeletePromotionFlight
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePromotionFlight(int id)
        {
            var promotion = await _dbContext.FlightPromotions.FindAsync(id);
            if (promotion == null)
            {
                return NotFound();
            }

            await _dbContext.AbmFlightPromotionAsync(promotion, "DELETE");
            TempData["SuccessMessage"] = "¡Promoción de vuelo eliminada exitosamente!";
            return RedirectToAction(nameof(AdminPromotionFlights));
        }

        // **************** ACCIONES PARA ADMINISTRACIÓN DE PROMOCIONES DE HOTELES ****************

        // GET: Admin/AdminPromotionHotels
        [HttpGet]
        public async Task<IActionResult> AdminPromotionHotels()
        {
            ViewData["Title"] = "Gestionar Promociones de Hoteles";
            var hotelPromotions = await _dbContext.GetActivePromotionHotelsItemsAsync();
            return View(hotelPromotions);
        }

        // GET: Admin/AltaPromotionHotel
        [HttpGet]
        public IActionResult AltaPromotionHotel()
        {
            ViewData["Title"] = "Alta de Promoción de Hotel";
            return View("AltaPromotionHotel", new HotelPromotion { ServiceType = "1" });
        }

        // GET: Admin/EditPromotionHotel/{id}
        [HttpGet]
        public async Task<IActionResult> EditPromotionHotel(int id)
        {
            ViewData["Title"] = "Editar Promoción de Hotel";
            var promotion = await _dbContext.HotelPromotions.FindAsync(id);
            if (promotion == null)
            {
                return NotFound();
            }
            return View("AltaPromotionHotel", promotion);
        }

        // POST: Admin/SubmitPromotionHotel (Creación)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitPromotionHotel([Bind("ServiceType,Description,DestinationName,HotelName,ImageUrl,IsHotWeek,OriginalPrice,OfferPrice,StartDate,EndDate,IsActive,Stars")] HotelPromotion promotion)
        {
            if (ModelState.IsValid)
            {
                if (promotion.OriginalPrice > 0)
                {
                    promotion.DiscountPercentage = Math.Round(((promotion.OriginalPrice - promotion.OfferPrice) / promotion.OriginalPrice) * 100, 2);
                }
                promotion.ServiceType = "1";
                await _dbContext.AbmHotelPromotionAsync(promotion, "INSERT");
                TempData["SuccessMessage"] = "¡Promoción de hotel agregada exitosamente!";
                return RedirectToAction(nameof(AdminPromotionHotels));
            }
            ViewData["Title"] = "Alta de Promoción de Hotel";
            return View("AltaPromotionHotel", promotion);
        }

        // POST: Admin/EditPromotionHotel/{id} (Edición)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPromotionHotel(int id, [Bind("Id,ServiceType,Description,DestinationName,HotelName,ImageUrl,IsHotWeek,OriginalPrice,OfferPrice,DiscountPercentage,StartDate,EndDate,IsActive,Stars")] HotelPromotion promotion)
        {
            if (id != promotion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (promotion.OriginalPrice > 0)
                    {
                        promotion.DiscountPercentage = Math.Round(((promotion.OriginalPrice - promotion.OfferPrice) / promotion.OriginalPrice) * 100, 2);
                    }
                    promotion.ServiceType = "1";
                    await _dbContext.AbmHotelPromotionAsync(promotion, "UPDATE");
                    TempData["SuccessMessage"] = "¡Promoción de hotel actualizada exitosamente!";
                    return RedirectToAction(nameof(AdminPromotionHotels));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _dbContext.HotelPromotions.AnyAsync(e => e.Id == promotion.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewData["Title"] = "Editar Promoción de Hotel";
            return View("AltaPromotionHotel", promotion);
        }

        // POST: Admin/DeletePromotionHotel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePromotionHotel(int id)
        {
            var promotion = await _dbContext.HotelPromotions.FindAsync(id);
            if (promotion == null)
            {
                return NotFound();
            }

            await _dbContext.AbmHotelPromotionAsync(promotion, "DELETE");
            TempData["SuccessMessage"] = "¡Promoción de hotel eliminada exitosamente!";
            return RedirectToAction(nameof(AdminPromotionHotels));
        }

        // ... Puedes agregar más acciones para Buses, Paquetes, etc.
    }
}