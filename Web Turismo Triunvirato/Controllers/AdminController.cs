using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Web_Turismo_Triunvirato.Services; // Para IPromotionService
using Web_Turismo_Triunvirato.Models;   // Para Promotion y ServiceType
using System.Linq;                     // Para usar LINQ como .Where(), .OrderByDescending(), etc.
using System;                          // Para DateTime

namespace Web_Turismo_Triunvirato.Controllers
{
    // Aseg�rate de que este controlador requiere autenticaci�n con el rol "Admin"
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IPromotionService _promotionService;

        // Constructor: Ahora inyecta IPromotionService
        public AdminController(IPromotionService promotionService)
        {
            _promotionService = promotionService;
        }

        // Acci�n por defecto al entrar al panel de administraci�n (la "pantalla principal")
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

        // La vista de Vuelos ahora puede mostrar informaci�n o enlaces para la administraci�n de promociones de vuelos
        public IActionResult Vuelos()
        {
            ViewData["Title"] = "Administraci�n de Vuelos";
            return View(); // Esta vista contendr� el enlace a AltaPromocion y GestionarPromocionesVuelos
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

        // GET: Admin/AltaPromocion (Muestra el formulario para crear una nueva promoci�n)
        [HttpGet]
        public IActionResult AltaPromocion()
        {
            ViewData["Title"] = "Alta de Promoci�n";
            // Pasa una nueva instancia vac�a de Promotion a la vista para el formulario
            return View(new Promotion());
        }

        // POST: Admin/AltaPromocion (Procesa el env�o del formulario)
        [HttpPost]
        [ValidateAntiForgeryToken] // Protecci�n contra ataques CSRF
        public async Task<IActionResult> AltaPromocion(Promotion promotion)
        {
            if (ModelState.IsValid)
            {
                // Calcula el DiscountPercentage antes de agregar si es necesario,
                // aunque ya lo hacemos en InMemoryPromotionService
                if (promotion.OriginalPrice > 0)
                {
                    promotion.DiscountPercentage = Math.Round(((promotion.OriginalPrice - promotion.OfferPrice) / promotion.OriginalPrice) * 100, 2);
                }

                await _promotionService.AddPromotionAsync(promotion);
                TempData["SuccessMessage"] = "�Promoci�n agregada exitosamente!";
                return RedirectToAction(nameof(ListarPromociones)); // O podr�as redirigir a GestionarPromocionesVuelos si siempre es de vuelos
            }
            // Si el modelo no es v�lido, vuelve a mostrar el formulario con los errores
            ViewData["Title"] = "Alta de Promoci�n"; // Vuelve a establecer el t�tulo si hay errores
            return View(promotion);
        }

        // GET: Admin/ListarPromociones (Muestra todas las promociones - usada como lista general)
        [HttpGet]
        public async Task<IActionResult> ListarPromociones()
        {
            ViewData["Title"] = "Listado de Promociones";
            var promotions = await _promotionService.GetAllPromotionsAsync();
            return View(promotions);
        }

        // GET: Admin/GestionarPromocionesVuelos (Muestra SOLO promociones de Vuelos para edici�n)
        [HttpGet]
        public async Task<IActionResult> GestionarPromocionesVuelos()
        {
            ViewData["Title"] = "Gestionar Promociones de Vuelos";
            // Obtener solo las promociones de tipo Vuelos
            var flightPromotions = await _promotionService.GetPromotionsByTypeAsync(ServiceType.Vuelos);

            // Opcional: Filtrar solo las activas si solo quieres modificar las activas aqu�
            // (La fecha actual es Jueves, 24 de julio de 2025. Se asume que las fechas de las promociones de prueba son anteriores a hoy o cubren el futuro)
            flightPromotions = flightPromotions.Where(p => p.IsActive && p.EndDate >= DateTime.Today).ToList();

            return View(flightPromotions.OrderByDescending(p => p.Id)); // Pasar la lista de promociones a la vista
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
            return View(promotion);
        }

        // POST: Admin/EditarPromocion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarPromocion(Promotion promotion)
        {
            if (ModelState.IsValid)
            {
                // Recalcular el descuento si los precios fueron cambiados
                if (promotion.OriginalPrice > 0)
                {
                    promotion.DiscountPercentage = Math.Round(((promotion.OriginalPrice - promotion.OfferPrice) / promotion.OriginalPrice) * 100, 2);
                }
                await _promotionService.UpdatePromotionAsync(promotion);
                TempData["SuccessMessage"] = "�Promoci�n actualizada exitosamente!";

                // Redirige a la lista espec�fica de vuelos despu�s de editar si el tipo es Vuelos
                if (promotion.ServiceType == ServiceType.Vuelos)
                {
                    return RedirectToAction(nameof(GestionarPromocionesVuelos));
                }
                // Si editas otro tipo de promo (ej: Hotel), redirige a la lista general
                return RedirectToAction(nameof(ListarPromociones));
            }
            ViewData["Title"] = "Editar Promoci�n"; // Vuelve a establecer el t�tulo si hay errores
            return View(promotion);
        }

        // POST: Admin/EliminarPromocion/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarPromocion(int id)
        {
            await _promotionService.DeletePromotionAsync(id);
            TempData["SuccessMessage"] = "�Promoci�n eliminada exitosamente!";
            return RedirectToAction(nameof(ListarPromociones)); // Siempre redirige a la lista general
        }
    }
}