using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Web_Turismo_Triunvirato.DataAccess;
using Web_Turismo_Triunvirato.Models;
using Web_Turismo_Triunvirato.Services;

namespace Web_Turismo_Triunvirato.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IPromotionService _promotionService;
        private readonly ApplicationDbContext _dbContext;
        
        public AdminController(IPromotionService promotionService, ApplicationDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            _promotionService = promotionService;
            _dbContext = dbContext;
            _webHostEnvironment = webHostEnvironment;
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

        public IActionResult packages()
        {
            ViewData["Title"] = "Administración de Paquetes";
            return View();
        }

        [HttpGet]
        public IActionResult Encomiendas()
        {
            ViewData["Title"] = "Administración de Encomiendas";
            return View();
        }

        public IActionResult Activities()
        {
            ViewData["Title"] = "Panel de Administración";
            return View();
        }



        // **************** ACCIONES PARA ADMINISTRACIÓN DE PROMOCIONES DE VUELOS ****************

        [HttpGet]
        public async Task<IActionResult> AdminPromotionFlights()
        {
            ViewData["Title"] = "Gestionar Promociones de Vuelos";
            var flightPromotions = await _dbContext.GetActiveflightpromotionsItemsAsync();
            return View("AdminPromotionFlights", flightPromotions);
        }




        [HttpGet]
        public async Task<IActionResult> AltaPromotionFlight()
        {
            ViewData["Title"] = "Alta de Promoción de Vuelo";
            // Obtener la lista de mensajes de WhatsApp activos
            var whatsappMessages = await _dbContext.WhatsappMessages
                .Where(m => m.Is_Active)
                .OrderBy(m => m.Title)
                .ToListAsync();

            // Crear una lista de SelectListItem para el dropdown
            var whatsappList = whatsappMessages.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Title
            }).ToList();

            // Pasar la lista al ViewBag. El nombre de la variable de ViewBag puede ser cualquiera.
            ViewBag.WhatsappMessages = whatsappList;

            return View("AltaPromotionFlight", new FlightPromotion { ServiceType = "0"});
        }

        [HttpGet]
        public async Task<IActionResult> EditPromotionFlight(int id)
        {
            ViewData["Title"] = "Editar Promoción de Vuelo";
            var promotion = await _dbContext.FlightPromotions.FindAsync(id);

            if (promotion == null)
            {
                return NotFound();
            }

            // Obtener la lista de mensajes de WhatsApp activos
            var whatsappMessages = await _dbContext.WhatsappMessages
                .Where(m => m.Is_Active)
                .OrderBy(m => m.Title)
                .ToListAsync();

            // Crear una lista de SelectListItem para el dropdown
            var whatsappList = whatsappMessages.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Title
            }).ToList();

            // Pasar la lista al ViewBag. El nombre de la variable de ViewBag puede ser cualquiera.
            ViewBag.WhatsappMessages = whatsappList;

            return View("AltaPromotionFlight", promotion);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitPromotionFlight(IFormFile ImageFile,[Bind("Id,Whatsapp_Id,ServiceType,Description,DestinationName,OriginName,ImageUrl,IsHotWeek,OriginalPrice,OfferPrice,DiscountPercentage,StartDate,EndDate,IsActive,Stars")] FlightPromotion promotion)
        {
            // === Lógica de Subida de Imagen ===
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img", "PromocionesVuelos");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(fileStream); 
                    }

                // 🛑 CRÍTICO: URL de la DB en minúsculas para consistencia en Linux
                promotion.ImageUrl = "/img/PromocionesVuelos/" + uniqueFileName;
                ModelState.Remove("ImageUrl");
            }
            else
            {
                // Si es un nuevo registro y no hay imagen, lanza error de validación
                if (promotion.Id == 0)
                {
                    ModelState.AddModelError("ImageUrl", "La imagen es requerida para dar de alta una nueva promoción de Vuelo.");
                }
            }
            // ===================================

            if (ModelState.IsValid)
            {
                // ... (Tu lógica de cálculo de descuento) ...
                promotion.ServiceType = "0";

                await _dbContext.AbmFlightPromotionAsync(promotion, "INSERT");

                TempData["SuccessMessage"] = "¡Promoción de vuelo creada exitosamente!";
                return RedirectToAction(nameof(AdminPromotionFlights));
            }

            // ... (Tu lógica para recargar ViewBag.WhatsappMessages si falla el ModelState) ...
            ViewData["Title"] = "Alta de Promoción de Vuelo";
            return View("AltaPromotionFlight", promotion);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPromotionFlight(int id, IFormFile ImageFile, [Bind("Id,ServiceType,Description,Whatsapp_Id,DestinationName,OriginName,ImageUrl,IsHotWeek,OriginalPrice,OfferPrice,DiscountPercentage,StartDate,EndDate,IsActive,Stars")] FlightPromotion promotion)
        {
            if (id != promotion.Id)
            {
                return NotFound();
            }


            if (promotion.ImageUrl == null && ImageFile == null) { }
            else
            {              
                // Paso 1: Si no se sube un nuevo archivo, elimina el error de validación para ImageUrl.
                if (promotion.ImageUrl == null)
                {
                    ModelState.Remove("ImageUrl");
                }
                if (ImageFile == null)
                {
                    ModelState.Remove("ImageFile");
                }
            }


            // Ahora, si el ModelState es válido, puedes continuar.
            if (ModelState.IsValid)
            {
                // Lógica para manejar la subida de la nueva imagen
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    //var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "/img/PromocionesVuelos");
                    
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img", "PromocionesVuelos");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }

                    promotion.ImageUrl = "/img/PromocionesVuelos/" + uniqueFileName;
                }

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

            // Si el ModelState no es válido por otras razones, vuelve a cargar los datos necesarios
            // para mostrar la vista con los errores de validación.
            var whatsappMessages = await _dbContext.WhatsappMessages
                .Where(m => m.Is_Active)
                .OrderBy(m => m.Title)
                .ToListAsync();
            var whatsappList = whatsappMessages.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Title
            }).ToList();
            ViewBag.WhatsappMessages = whatsappList;
            ViewData["Title"] = "Editar Promoción de Vuelo";
            return View("AltaPromotionFlight", promotion);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePromotionFlight(int id)
        {
            //var promotion = await _dbContext.FlightPromotions.FindAsync(id);
            //if (promotion == null)
            //{
            //    return NotFound();
            //}

            await _dbContext.AbmFlightPromotionAsync(id, "DELETE");
            TempData["SuccessMessage"] = "¡Promoción de vuelo eliminada exitosamente!";
            return RedirectToAction(nameof(AdminPromotionFlights));
        }

        // **************** ACCIONES PARA ADMINISTRACIÓN DE PROMOCIONES DE HOTELES ****************

        [HttpGet]
        public async Task<IActionResult> AdminPromotionHotels()
        {
            ViewData["Title"] = "Gestionar Promociones de Hoteles";
            var hotelPromotions = await _dbContext.GetActivePromotionHotelsItemsAsync();
            return View(hotelPromotions);
        }

        [HttpGet]
        public async Task<IActionResult> AltaPromotionHotel()
        {
            ViewData["Title"] = "Alta de Promoción de Hotel";
            // Obtener la lista de mensajes de WhatsApp activos
            var whatsappMessages = await _dbContext.WhatsappMessages
                .Where(m => m.Is_Active)
                .OrderBy(m => m.Title)
                .ToListAsync();

            // Crear una lista de SelectListItem para el dropdown
            var whatsappList = whatsappMessages.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Title
            }).ToList();

            // Pasar la lista al ViewBag. El nombre de la variable de ViewBag puede ser cualquiera.
            ViewBag.WhatsappMessages = whatsappList;
            return View("AltaPromotionHotel", new HotelPromotion { ServiceType = "1" });
        }


        [HttpGet]
        public async Task<IActionResult> EditPromotionHotel(int id)
        {
            var whatsappMessages = await _dbContext.WhatsappMessages
               .Where(m => m.Is_Active)
               .OrderBy(m => m.Title)
               .ToListAsync();

            // Crear una lista de SelectListItem para el dropdown
            var whatsappList = whatsappMessages.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Title
            }).ToList();

            // Pasar la lista al ViewBag. El nombre de la variable de ViewBag puede ser cualquiera.
            ViewBag.WhatsappMessages = whatsappList;
            ViewData["Title"] = "Editar Promoción de Hotel";
            var promotion = await _dbContext.HotelPromotions.FindAsync(id);
            if (promotion == null)
            {
                return NotFound();
            }
            return View("AltaPromotionHotel", promotion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitPromotionHotel(IFormFile ImageFile, [Bind("Id,ServiceType,Description,Whatsapp_Id,DestinationName,HotelName,ImageUrl,IsHotWeek,OriginalPrice,OfferPrice,DiscountPercentage,StartDate,EndDate,Stars,IsActive")] HotelPromotion promotion)
        {
            ModelState.Remove("RenderedWhatsappMessage");

            // === INICIO DE LÓGICA DE IMAGEN (MOVIDA Y CORREGIDA) ===

            // 1. Manejar la subida de la imagen si se proporcionó un archivo
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img", "PromocionesHoteles");
                //var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "/img/PromocionesHoteles");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(fileStream);
                }
                // Asignar la nueva URL al modelo ANTES de la validación
                promotion.ImageUrl = "/img/PromocionesHoteles/" + uniqueFileName;

                // Quitar ImageUrl del ModelState si se subió el archivo
                ModelState.Remove("ImageUrl");
            }
            else
            {
                // Si es una creación (INSERT), la imagen es obligatoria.
                if (string.IsNullOrEmpty(promotion.ImageUrl))
                {
                    ModelState.AddModelError("ImageUrl", "La imagen es requerida para dar de alta una nueva promoción.");
                }
            }

          
            if (ImageFile == null)
            {
                ModelState.Remove("ImageFile");
            }

            
            if (ModelState.IsValid)
            {
                if (promotion.OriginalPrice > 0)
                {
                    promotion.DiscountPercentage = Math.Round(((promotion.OriginalPrice - promotion.OfferPrice) / promotion.OriginalPrice) * 100, 2);
                }
                promotion.ServiceType = "1";

                // ERROR DE SINTAXIS CORREGIDO: se eliminó la coma
                await _dbContext.AbmHotelPromotionAsync(promotion, "INSERT");

                TempData["SuccessMessage"] = "¡Promoción creada exitosamente!";
                return RedirectToAction(nameof(AdminPromotionHotels));
            }


            // Si el modelo NO es válido, vuelve a cargar la lista de mensajes de WhatsApp
            // Aquí asumo que 'CargarWhatsappMessages' es un método que alimenta al ViewBag
            // Si no existe, reemplázalo con el bloque de carga de WhatsApp que usas en tus otros métodos GET.
            await CargarWhatsappMessages();
            ViewData["Title"] = "Alta de Promoción de Hotel";
            return View("AltaPromotionHotel", promotion);
        }


        // Para el método de edición, el approach es similar, pero con ModelState.Remove
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPromotionHotel(int id, IFormFile ImageFile, [Bind("Id,ServiceType,Description,Whatsapp_Id,DestinationName,HotelName,ImageUrl,IsHotWeek,OriginalPrice,OfferPrice,DiscountPercentage,StartDate,EndDate,Stars,IsActive")] HotelPromotion promotion)
        {
            if (id != promotion.Id)
            {
                return NotFound();
            }


            if (promotion.ImageUrl == null && ImageFile == null) { }
            else
            {
                // Paso 1: Si no se sube un nuevo archivo, elimina el error de validación para ImageUrl.
                if (promotion.ImageUrl == null)
                {
                    ModelState.Remove("ImageUrl");
                }
                if (ImageFile == null)
                {
                    ModelState.Remove("ImageFile");
                }
            }

            //ModelState.Remove("RenderedWhatsappMessage");
            if (ModelState.IsValid)
            {
                // Lógica para manejar la subida de la nueva imagen
                if (ImageFile != null && ImageFile.Length > 0)
                {
        
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "/img/PromocionesHoteles");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);


                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }

                    promotion.ImageUrl = "/img/PromocionesHoteles/" + uniqueFileName;
                }

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


            var whatsappMessages = await _dbContext.WhatsappMessages
         .Where(m => m.Is_Active)
         .OrderBy(m => m.Title)
         .ToListAsync();
            var whatsappList = whatsappMessages.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Title
            }).ToList();
            ViewBag.WhatsappMessages = whatsappList;

            // Si el modelo es inválido, vuelve a cargar la lista de mensajes de WhatsApp
            await CargarWhatsappMessages();
            ViewData["Title"] = "Editar Promoción de Hotel";
            return View("AltaPromotionHotel", promotion);
        }


        // Método auxiliar para cargar la lista de mensajes de WhatsApp
        private async Task CargarWhatsappMessages()
        {
            var whatsappMessages = await _dbContext.WhatsappMessages
                .Where(m => m.Is_Active)
                .OrderBy(m => m.Title)
                .ToListAsync();

            var whatsappList = whatsappMessages.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Title
            }).ToList();

            ViewBag.WhatsappMessages = whatsappList;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePromotionHotel(int id)
        {
            //var promotion = await _dbContext.HotelPromotions.FindAsync(id);
            //if (promotion == null)
            //{
            //    return NotFound();
            //}

            await _dbContext.AbmHotelPromotionAsync(id, "DELETE");
            TempData["SuccessMessage"] = "¡Promoción de hotel eliminada exitosamente!";
            return RedirectToAction(nameof(AdminPromotionHotels));
        }

        // **************** ACCIONES PARA ADMINISTRACIÓN DE PROMOCIONES DE BUSES ****************

        // GET: Admin/AdminPromotionBuses
        [HttpGet]
        public async Task<IActionResult> AdminPromotionBuses()
        {
            ViewData["Title"] = "Gestionar Promociones de Buses";
            var busPromotions = await _dbContext.GetActivePromotionBusesItemsAsync(); // Asumiendo que este método existe en tu DbContext
            return View("AdminPromotionBuses", busPromotions); // Se asume que tienes una vista llamada AdminPromotionBuses.cshtml
        }

        // GET: Admin/AltaPromotionBus
        [HttpGet]
        public async Task<IActionResult> AltaPromotionBus()
        {
            var whatsappMessages = await _dbContext.WhatsappMessages
              .Where(m => m.Is_Active)
              .OrderBy(m => m.Title)
              .ToListAsync();

            // Crear una lista de SelectListItem para el dropdown
            var whatsappList = whatsappMessages.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Title
            }).ToList();

            // Pasar la lista al ViewBag. El nombre de la variable de ViewBag puede ser cualquiera.
            ViewBag.WhatsappMessages = whatsappList;
            ViewData["Title"] = "Alta de Promoción de Bus";
            return View("AltaPromotionBus", new BusPromotion { ServiceType = "2" }); // Se asume que tienes un modelo BusPromotion y una vista AltaPromotionBus.cshtml
        }

        // GET: Admin/EditPromotionBus/{id}
        [HttpGet]
        public async Task<IActionResult> EditPromotionBus(int id)
        {
            var whatsappMessages = await _dbContext.WhatsappMessages
              .Where(m => m.Is_Active)
              .OrderBy(m => m.Title)
              .ToListAsync();

            // Crear una lista de SelectListItem para el dropdown
            var whatsappList = whatsappMessages.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Title
            }).ToList();

            // Pasar la lista al ViewBag. El nombre de la variable de ViewBag puede ser cualquiera.
            ViewBag.WhatsappMessages = whatsappList;

            ViewData["Title"] = "Editar Promoción de Bus";
            var promotion = await _dbContext.BusPromotions.FindAsync(id); // Asumiendo que BusPromotions es una propiedad en tu DbContext
            if (promotion == null)
            {
                return NotFound();
            }
            return View("AltaPromotionBus", promotion);
        }

        // POST: Admin/SubmitPromotionBus (Creación)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitPromotionBus(IFormFile ImageFile, [Bind("Id,ServiceType,Description,Whatsapp_Id,DestinationName,OriginName,ImageUrl,IsHotWeek,OriginalPrice,OfferPrice,DiscountPercentage,StartDate,EndDate,IsActive,BusCompanyName,Category")] BusPromotion promotion)
        {

            if(promotion.ImageUrl == null)
            {
                ModelState.Remove("ImageUrl");
            }

            if (ImageFile != null )
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "/img/PromocionesBuses");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(fileStream);
                }
                promotion.ImageUrl = "/img/PromocionesBuses/" + uniqueFileName;
            }
            else
            {
                if(promotion.ImageUrl != null)
                {

                    ModelState.Remove("ImageFile");
                }
                else
                {
                    ModelState.AddModelError("ImageUrl", "La imagen es requerida para dar de alta una nueva promoción.");   
                }
            }

            if (ModelState.IsValid)
            {
                if (promotion.OriginalPrice > 0)
                {
                    promotion.DiscountPercentage = Math.Round(((promotion.OriginalPrice - promotion.OfferPrice) / promotion.OriginalPrice) * 100, 2);
                }
                promotion.ServiceType = "2";
                // Asumiendo que AbmBusPromotionAsync existe para INSERT
                await _dbContext.AbmBusPromotionAsync(promotion, "INSERT");
                TempData["SuccessMessage"] = "¡Promoción de bus agregada exitosamente!";
                return RedirectToAction(nameof(AdminPromotionBuses));
            }
            var whatsappMessages = await _dbContext.WhatsappMessages
                .Where(m => m.Is_Active)
                .OrderBy(m => m.Title)
                .ToListAsync();

            var whatsappList = whatsappMessages.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Title
            }).ToList();

            ViewBag.WhatsappMessages = whatsappList;
            ViewData["Title"] = "Alta de Promoción de Bus";
            return View("AltaPromotionBus", promotion);
        }

        // POST: Admin/EditPromotionBus/{id} (Edición)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPromotionBus(int id, IFormFile ImageFile, [Bind("Id,ServiceType,Description,Whatsapp_Id,DestinationName,OriginName,ImageUrl,IsHotWeek,OriginalPrice,OfferPrice,DiscountPercentage,StartDate,EndDate,IsActive,BusCompanyName,Category")] BusPromotion promotion)
        {
            if (id != promotion.Id)
            {
                return NotFound();
            }

                // Paso 1: Si no se sube un nuevo archivo, elimina el error de validación para ImageUrl.
                if (promotion.ImageUrl == null)
                {
                    ModelState.Remove("ImageUrl");
                }
            

            // Paso 1: Manejar la subida de la nueva imagen
            if (ImageFile != null /*&& ImageFile.Length > 0*/)
            {
                // 1. Guarda la nueva imagen en la carpeta de destino.
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img/PromocionesBuses");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(fileStream);
                }

                // 2. Actualiza la propiedad ImageUrl del modelo con la nueva ruta.
                promotion.ImageUrl = "/img/PromocionesBuses/" + uniqueFileName;
            }
            else
            {
                // Si no se subió un nuevo archivo, elimina el error de validación de ImageUrl del ModelState
                // para permitir que el resto del modelo se valide.
                ModelState.Remove("ImageFile");
            }

            // Ahora, si el ModelState es válido, puedes continuar.
            if (ModelState.IsValid)
            {
                try
                {
                    if (promotion.OriginalPrice > 0)
                    {
                        promotion.DiscountPercentage = Math.Round(((promotion.OriginalPrice - promotion.OfferPrice) / promotion.OriginalPrice) * 100, 2);
                    }
                    promotion.ServiceType = "2";
                    await _dbContext.AbmBusPromotionAsync(promotion, "UPDATE");
                    TempData["SuccessMessage"] = "¡Promoción de bus actualizada exitosamente!";
                    return RedirectToAction(nameof(AdminPromotionBuses));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _dbContext.BusPromotions.AnyAsync(e => e.Id == promotion.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // Si el ModelState no es válido, vuelve a cargar la lista de WhatsApp y la vista
            var whatsappMessages = await _dbContext.WhatsappMessages
                .Where(m => m.Is_Active)
                .OrderBy(m => m.Title)
                .ToListAsync();
            var whatsappList = whatsappMessages.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Title
            }).ToList();
            ViewBag.WhatsappMessages = whatsappList;
            ViewData["Title"] = "Editar Promoción de Bus";
            return View("AltaPromotionBus", promotion);
        }

        // POST: Admin/DeletePromotionBus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePromotionBus(int id)
        {
        
            await _dbContext.AbmBusPromotionAsync(id, "DELETE");
            TempData["SuccessMessage"] = "¡Promoción de bus eliminada exitosamente!";
            return RedirectToAction(nameof(AdminPromotionBuses));
        }



        // **************** ACCIONES PARA ADMINISTRACIÓN DE PROMOCIONES DE PAQUETES ****************

        public async Task<IActionResult> AdminPromotionPackages()
        {
            var promotions = await _dbContext.GetActivePromotionPackagesItemsAsync();
            ViewData["Title"] = "Gestión de Promociones de Paquetes";
            return View(promotions);
        }

        // Método GET para mostrar la vista de creación de una nueva promoción de paquete
        public IActionResult AltaPromotionPackage()
        {
            ViewData["Title"] = "Alta de Promoción de Paquete";
            // Pasa un nuevo modelo vacío a la vista para evitar la excepción
            return View(new PackagePromotion());
        }

        // Método GET para mostrar la vista de edición de una promoción de paquete
        public async Task<IActionResult> EditPromotionPackage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var promotion = await _dbContext.GetPackagePromotionByIdAsync(id.Value);
            if (promotion == null)
            {
                return NotFound();
            }
            ViewData["Title"] = "Editar Promoción de Paquete";
            return View("AltaPromotionPackage", promotion);
        }

        // Método POST para la creación de una promoción de paquete
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Se ajusta el Bind para incluir solo los campos que se envían desde la vista.
        public async Task<IActionResult> SubmitPromotionPackage([Bind("Id,ServiceType,PackageType,Description,Whatsapp_Id,CompanyName,DestinationName,OriginName,ImageUrl,IsHotWeek,OriginalPrice,OfferPrice,DiscountPercentage,StartDate,EndDate,HotelName,IsActive")] PackagePromotion promotion)
        {
            if (ModelState.IsValid)
            {
                // Se calcula el descuento si los precios son válidos.
                if (promotion.OriginalPrice > 0)
                {
                    promotion.DiscountPercentage = Math.Round(((promotion.OriginalPrice - promotion.OfferPrice) / promotion.OriginalPrice) * 100, 2);
                }

                // El tipo de servicio para los paquetes es "3".
                promotion.ServiceType = "3";

                // Se llama al método ABM del DbContext para insertar el nuevo paquete.
                await _dbContext.AbmPackagePromotionAsync(promotion, "INSERT");

                TempData["SuccessMessage"] = "¡Promoción de paquete agregada exitosamente!";
                return RedirectToAction(nameof(AdminPromotionPackages));
            }

            // Si el modelo no es válido, se regresa a la vista para mostrar los errores.
            ViewData["Title"] = "Alta de Promoción de Paquete";
            return View("AltaPromotionPackage", promotion);
        }

        // Método POST para la edición de una promoción de paquete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPromotionPackage(
           int id,
           IFormFile ImageFile, // <-- ARCHIVO RECIBIDO POR SEPARADO
           [Bind("Id,ServiceType,PackageType,Description,Whatsapp_Id,CompanyName,DestinationName,OriginName,ImageUrl,IsHotWeek,OriginalPrice,OfferPrice,DiscountPercentage,StartDate,EndDate,HotelName,IsActive")] PackagePromotion promotion)
        {
            if (id != promotion.Id)
            {
                return NotFound();
            }

            // Si no se sube un nuevo archivo, eliminamos el error de validación para ImageUrl y ImageFile
            if (promotion.ImageUrl == null)
            {
                ModelState.Remove("ImageUrl");
            }
            if (ImageFile == null)
            {
                ModelState.Remove("ImageFile");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Lógica para manejar la subida de la nueva imagen
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "/img/Paquetes"); // Ruta
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImageFile.FileName);
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(fileStream);
                        }

                        // Se actualiza la URL del modelo con la nueva imagen
                        promotion.ImageUrl = "/img/Paquetes/" + uniqueFileName;
                    }
                    else
                    {
                        // Si NO se subió un archivo nuevo, debemos cargar la URL antigua
                        // 1. Obtenemos el registro existente SÓLO para su ImageUrl
                        var existingPromotion = await _dbContext.GetPackagePromotionByIdAsync(id);
                        if (existingPromotion != null)
                        {
                            // 2. Mantenemos la URL anterior
                            promotion.ImageUrl = existingPromotion.ImageUrl;
                        }
                        // Si el registro no existe, el error será manejado por el Abm más abajo.
                    }

                    if (promotion.OriginalPrice > 0)
                    {
                        promotion.DiscountPercentage = Math.Round(((promotion.OriginalPrice - promotion.OfferPrice) / promotion.OriginalPrice) * 100, 2);
                    }
                    promotion.ServiceType = "3"; // Tipo de servicio de paquetes

                    await _dbContext.AbmPackagePromotionAsync(promotion, "UPDATE");
                    TempData["SuccessMessage"] = "¡Promoción de paquete actualizada exitosamente!";
                    return RedirectToAction(nameof(AdminPromotionPackages));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _dbContext.PackagePromotions.AnyAsync(e => e.Id == promotion.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al actualizar la promoción o al guardar la imagen: " + ex.Message);
                }
            }

            // Si el modelo es inválido, vuelve a cargar la vista
            // await CargarWhatsappMessages(); 
            ViewData["Title"] = "Editar Promoción de Paquete";
            return View("AltaPromotionPackage", promotion);
        }


        ///////////// NUEVAS ACCIONES PARA LA GESTIÓN DE ENCOMIENDAS  ////////////////////////////////////////////

        [HttpGet]
    public async Task<IActionResult> AdminEncomiendas()
    {
        ViewData["Title"] = "Administrar Empresas de Encomiendas";
        var encomiendaCompanies = await _dbContext.EncomiendaCompanies.ToListAsync();
        return View("AdminEncomiendas", encomiendaCompanies);
    }

    [HttpGet]
    public IActionResult AltaEncomienda()
    {
        ViewData["Title"] = "Alta de Empresa de Encomiendas";
        return View("AltaEncomienda", new EncomiendaCompany());
    }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Se añade IFormFile para recibir el archivo subido
        public async Task<IActionResult> AltaEncomienda([Bind("Id,CompanyName,CompanyUrl")] EncomiendaCompany encomienda, IFormFile ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    // La carpeta donde se guardarán las imágenes. Asegúrate de que exista.
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/encomiendas");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Genera un nombre de archivo único para evitar colisiones
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Guarda el archivo en el servidor
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }

                    // Actualiza la propiedad ImageUrl del modelo con la ruta relativa
                    encomienda.ImageUrl = "/images/encomiendas/" + uniqueFileName;
                }

                _dbContext.Add(encomienda);
                await _dbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Empresa de encomienda creada exitosamente!";
                return RedirectToAction(nameof(AdminEncomiendas));
            }

            return View(encomienda);
        }

        [HttpGet]
    public async Task<IActionResult> EditEncomienda(int id)
    {
        ViewData["Title"] = "Editar Empresa de Encomiendas";
        var company = await _dbContext.EncomiendaCompanies.FindAsync(id);
        if (company == null)
        {
            return NotFound();
        }
        return View("AltaEncomienda", company);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditEncomienda(int id, [Bind("Id,CompanyName,CompanyUrl")] EncomiendaCompany company)
    {
        if (id != company.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _dbContext.Update(company);
                await _dbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "¡Empresa de encomiendas actualizada exitosamente!";
                return RedirectToAction(nameof(AdminEncomiendas));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _dbContext.EncomiendaCompanies.AnyAsync(e => e.Id == company.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
        ViewData["Title"] = "Editar Empresa de Encomiendas";
        return View("AltaEncomienda", company);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteEncomienda(int id)
    {
        var company = await _dbContext.EncomiendaCompanies.FindAsync(id);
        if (company == null)
        {
            return NotFound();
        }

        _dbContext.EncomiendaCompanies.Remove(company);
        await _dbContext.SaveChangesAsync();
        TempData["SuccessMessage"] = "¡Empresa de encomiendas eliminada exitosamente!";
        return RedirectToAction(nameof(AdminEncomiendas));
    }

        /////////////////////////////////////// ACTIVIDADES ////////////////////////////////////////////////////////////

        [HttpGet]
        public async Task<IActionResult> AdminActivities()
        {
            ViewData["Title"] = "Administrar Actividades";
            var activitiesAdmin = await _dbContext.Activities.ToListAsync();
            return View("AdminActivities", activitiesAdmin);
        }

        [HttpGet]
        public async Task<IActionResult> AltaActividad()
        {
            ViewData["Title"] = "Alta de Actividades";
            // Obtener la lista de mensajes de WhatsApp activos
            var whatsappMessages = await _dbContext.WhatsappMessages
                .Where(m => m.Is_Active)
                .OrderBy(m => m.Title)
                .ToListAsync();

            // Crear una lista de SelectListItem para el dropdown
            var whatsappList = whatsappMessages.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Title
            }).ToList();

            // Pasar la lista al ViewBag. El nombre de la variable de ViewBag puede ser cualquiera.
            ViewBag.WhatsappMessages = whatsappList;
            return View("AltaActividad", new ActivitiesPromotion());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AltaActividad([Bind("Id,Title, Description,Whatsapp_Id,Location,ImageUrl,IsActive")] ActivitiesPromotion Actividad, IFormFile ImageFile)
        {
            if (ImageFile != null && Actividad.ImageUrl == null)
            {
                ModelState.Remove("ImageUrl");
            }

            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "/img/Actividades");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }

                    Actividad.ImageUrl = "/img/Actividades/" + uniqueFileName;
                }
                ViewData["Title"] = "Gestionar Promociones de Hoteles";

                _dbContext.Add(Actividad);

                await _dbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "La actividad se creo con exito!";
                return RedirectToAction(nameof(AdminActivities));
            }

            return View(Actividad);
        }

        [HttpGet]
        public async Task<IActionResult> EditActividad(int id)
        {
            ViewData["Title"] = "Editar Actividades";
            var Actividad = await _dbContext.Activities.FindAsync(id);
            if (Actividad == null)
            {
                return NotFound();
            }
            ViewData["Title"] = "Gestionar Promociones de Hoteles";

            // Obtener la lista de mensajes de WhatsApp activos
            var whatsappMessages = await _dbContext.WhatsappMessages
                .Where(m => m.Is_Active)
                .OrderBy(m => m.Title)
                .ToListAsync();

            // Crear una lista de SelectListItem para el dropdown
            var whatsappList = whatsappMessages.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Title
            }).ToList();

            // Pasar la lista al ViewBag. El nombre de la variable de ViewBag puede ser cualquiera.
            ViewBag.WhatsappMessages = whatsappList;
            return View("AltaActividad", Actividad);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Se modifica el método de edición para aceptar el IFormFile.
        public async Task<IActionResult> EditActividad(int id, [Bind("Id,Title, Description,Whatsapp_Id,Location,ImageUrl,IsActive")] ActivitiesPromotion Actividad, IFormFile ImageFile)
        {
            if (id != Actividad.Id)
            {
                return NotFound();
            }
            if (ImageFile == null && Actividad.ImageUrl != null)
            {
                ModelState.Remove("imageFile");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Si se subió una nueva imagen, se guarda y se actualiza la URL.
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        // Lógica para guardar la nueva imagen (similar a AltaActividad).
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "/img/Actividades");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(fileStream);
                        }

                        // Se borra la imagen anterior si existía para no dejar archivos huérfanos.
                        if (!string.IsNullOrEmpty(Actividad.ImageUrl))
                        {
                            string oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, Actividad.ImageUrl.TrimStart('/'));
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        // Se actualiza la URL de la imagen del modelo.
                        Actividad.ImageUrl = "/img/Actividades/" + uniqueFileName;
                    }

                    // Si no se subió una nueva imagen, se mantiene la URL existente.
                    // Esto se maneja automáticamente ya que el campo ImageUrl se incluye en el bind y el campo hidden.
                    _dbContext.Update(Actividad);
                    await _dbContext.SaveChangesAsync();
                    TempData["SuccessMessage"] = "¡Empresa de encomiendas actualizada exitosamente!";
                    return RedirectToAction(nameof(AdminActivities));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _dbContext.Activities.AnyAsync(e => e.Id == Actividad.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            // Obtener la lista de mensajes de WhatsApp activos
            var whatsappMessages = await _dbContext.WhatsappMessages
                .Where(m => m.Is_Active)
                .OrderBy(m => m.Title)
                .ToListAsync();

            // Crear una lista de SelectListItem para el dropdown
            var whatsappList = whatsappMessages.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Title
            }).ToList();

            // Pasar la lista al ViewBag. El nombre de la variable de ViewBag puede ser cualquiera.
            ViewBag.WhatsappMessages = whatsappList;
            ViewData["Title"] = "Editar Actividad";
            return View("AltaActividad", Actividad);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteActividad(int id)
        {
            var Actividad = await _dbContext.Activities.FindAsync(id);
            if (Actividad == null)
            {
                return NotFound();
            }

            // También se borra la imagen del servidor al eliminar el registro.
            if (!string.IsNullOrEmpty(Actividad.ImageUrl))
            {
                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, Actividad.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _dbContext.Activities.Remove(Actividad);
            await _dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "¡Actividad eliminada exitosamente!";
            return RedirectToAction(nameof(AdminActivities));
        }

        [HttpGet]
        public IActionResult Whatsapp()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreateWhatsappMessage()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateWhatsappMessage(WhatsappMessage model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Llama al método del DbContext con el modelo directamente
                    // La propiedad 'Id' no es necesaria para la inserción, ya que es auto-incremental.
                    await _dbContext.CreateWhatsappMessageAsync(
                        model.Title,
                        model.Message_Template,
                        model.Is_Active
                    );

                    return RedirectToAction("AdminWhatsappMessages");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Ocurrió un error al guardar el mensaje: " + ex.Message);
                    return View(model);
                }
            }

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> AdminWhatsappMessages()
        {
            try
            {
                // Llama al método del DbContext para obtener la lista de mensajes
                var messages = await _dbContext.GetAllWhatsappMessagesAsync();
                return View(messages);
            }
            catch (Exception ex)
            {
                // Manejar errores si la llamada al SP falla
                // Considera usar un logger en un proyecto real
                TempData["ErrorMessage"] = "Ocurrió un error al cargar los mensajes: " + ex.Message;
                // Retorna una lista vacía para evitar errores en la vista
                return View(new List<WhatsappMessage>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditWhatsappMessage(int id)
        {
            var message = await _dbContext.GetWhatsappMessageByIdAsync(id);
            if (message == null)
            {
                return NotFound();
            }
            return View(message);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditWhatsappMessage(WhatsappMessage model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Usamos el mismo método de "upsert" que creamos
                    await _dbContext.UpdateWhatsappMessageAsync(model);

                    TempData["SuccessMessage"] = "Mensaje de WhatsApp actualizado exitosamente.";
                    return RedirectToAction("AdminWhatsappMessages");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Ocurrió un error al actualizar el mensaje: " + ex.Message);
                    return View(model);
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleWhatsappMessageStatus(int id)
        {
            try
            {
                // 1. Obtener el mensaje de la base de datos usando el ID.
                var message = await _dbContext.GetWhatsappMessageByIdAsync(id);

                if (message == null)
                {
                    TempData["ErrorMessage"] = "Mensaje no encontrado.";
                    return RedirectToAction("AdminWhatsappMessages");
                }

                // 2. Cambiar el estado del mensaje.
                message.Is_Active = !message.Is_Active;

                // 3. Actualizar el registro en la base de datos usando el SP de actualización.
                await _dbContext.UpdateWhatsappMessageAsync(message);

                TempData["SuccessMessage"] = "Estado del mensaje actualizado exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ocurrió un error al actualizar el estado: " + ex.Message;
            }

            // 4. Redirigir de vuelta a la lista de mensajes.
            return RedirectToAction("AdminWhatsappMessages");
        }
        ////fin/////
        ///

        [HttpGet]
        public IActionResult PaginaPrincipal()
        {
            return View();
        }

        // Acción para mostrar el formulario de creación
        [HttpGet]
        public IActionResult CreateCarrouselItem()
        {
            return View();
        }

        // Acción para procesar el formulario de creación
        [HttpPost]
        public async Task<IActionResult> CreateCarrouselItem(View_Index_DestinationCarouselItem model, IFormFile imageFile)
        {
            if (ModelState.IsValid && imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "/img/");
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                model.ImageUrl = "/img/" + uniqueFileName;
                _dbContext.View_DestinationCarouselItems.Add(model);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction("AdminCarrouselItem");
            }
            return View(model);
        }

        // Acción para mostrar la lista de elementos del carrusel
        [HttpGet]
        public async Task<IActionResult> AdminCarrouselItem()
        {
            var items = await _dbContext.View_DestinationCarouselItems.ToListAsync();
            return View(items);
        }

        // Acción para mostrar el formulario de creación de destinos
        [HttpGet]
        public IActionResult CreateDestination()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateDestination(View_Index_Destination model, IFormFile imageFile)
        {
            // Verificar si se subió un archivo de imagen.
            if (imageFile == null || imageFile.Length == 0)
            {
                ModelState.AddModelError("PictureDestiny", "La imagen de destino es obligatoria.");
            }

            // Si hay errores de validación, regresar a la vista.
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Lógica para guardar la imagen
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "/img/");

            // Asegúrate de que la carpeta de destino exista
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            // Asignar los valores del modelo
            model.PictureDestiny = "/img/" + uniqueFileName;
            model.DetailDestinyURL = "/destinations/" + model.Title; // O la lógica que uses para generar esta URL

            // Guardar los cambios en la base de datos.
            _dbContext.Destinations.Add(model);
            await _dbContext.SaveChangesAsync();

            // Redireccionar al usuario.
            return RedirectToAction("AdminDestination");
        }

        // Acción para mostrar la lista de destinos/promociones
        [HttpGet]
        public async Task<IActionResult> AdminDestination()
        {
            var items = await _dbContext.Destinations.ToListAsync();
            return View(items);
        }


        // GET: Muestra el formulario de edición con los datos cargados del carrusel
        [HttpGet]
        public async Task<IActionResult> EditCarrouselItem(int id)
        {
            var item = await _dbContext.View_DestinationCarouselItems.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        // POST: Procesa los cambios del formulario de edición del carrusel
        [HttpPost]
        public async Task<IActionResult> EditCarrouselItem(View_Index_DestinationCarouselItem model, IFormFile imageFile)
        {
            // Carga el elemento existente del DbContext
            var existingItem = await _dbContext.View_DestinationCarouselItems.FindAsync(model.Id);

            if (existingItem == null)
            {
                return NotFound();
            }

            if(imageFile == null && model.ImageUrl != null)
            {
                ModelState.Remove("imageFile");
            }


            if (ModelState.IsValid)
            {
                // Si se subió una nueva imagen, la guardamos y actualizamos la URL
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/carousel");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Opcional: Elimina la imagen antigua
                    if (!string.IsNullOrEmpty(existingItem.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, existingItem.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    existingItem.ImageUrl = "/images/carousel/" + uniqueFileName;
                }

                // Actualiza los demás campos
                existingItem.Title = model.Title;
                existingItem.AltText = model.AltText;
                //existingItem.LinkUrl = model.LinkUrl;
                existingItem.IsActive = model.IsActive;

                _dbContext.Entry(existingItem).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return RedirectToAction("AdminCarrouselItem");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleCarrouselItemStatus(int id)
        {
            var item = await _dbContext.View_DestinationCarouselItems.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            item.IsActive = !item.IsActive; // Invierte el estado
            _dbContext.Entry(item).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("AdminCarrouselItem");
        }

        public async Task<IActionResult> EditDestination(int id)
                {
                    var promotion = await _dbContext.Destinations.FindAsync(id);
                    if (promotion == null)
                    {
                        return NotFound();
                    }
                    return View(promotion);
                }

        // POST: Procesa los cambios del formulario de edición de destino
        [HttpPost]
        public async Task<IActionResult> EditDestination(View_Index_Destination model, IFormFile imageFile)
        {
            var existingPromotion = await _dbContext.Destinations.FindAsync(model.Id);
            if (existingPromotion == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Si se subió una nueva imagen, la guardamos y actualizamos la URL
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/destinations");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Opcional: Elimina la imagen antigua
                    if (!string.IsNullOrEmpty(existingPromotion.PictureDestiny))
                    {
                        var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, existingPromotion.PictureDestiny.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    existingPromotion.PictureDestiny = "/images/destinations/" + uniqueFileName;
                }

                // Actualiza los demás campos
                existingPromotion.Title = model.Title;
                existingPromotion.Price = model.Price;
                existingPromotion.From = model.From;
                existingPromotion.TripType = model.TripType;
                existingPromotion.IsHotWeek = model.IsHotWeek;

                _dbContext.Entry(existingPromotion).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return RedirectToAction("AdminDestination");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleDestinationStatus(int id)
        {
            var item = await _dbContext.Destinations.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            // Nota: El modelo View_Index_Destination no tiene la propiedad IsActive, 
            // pero tu modelo original sí la tiene. Si la usas en la tabla, la puedes agregar aquí.
            // item.IsActive = !item.IsActive; // Descomentar si la propiedad existe.

            item.IsHotWeek = !item.IsHotWeek; // Usaremos esta propiedad como ejemplo para activar/desactivar
            _dbContext.Entry(item).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("AdminDestination");
        }
    }
}
