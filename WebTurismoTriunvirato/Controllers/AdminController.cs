using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Web_Turismo_Triunvirato.DataAccess;
using Web_Turismo_Triunvirato.Models;
using Web_Turismo_Triunvirato.Models.ViewModels;
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
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img", "promocionesvuelos");

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

                promotion.ImageUrl = "/img/promocionesvuelos/" + uniqueFileName;
                ModelState.Remove("ImageUrl");
            }
            else
            {
                
                if (promotion.Id == 0)
                {
                    ModelState.AddModelError("ImageUrl", "La imagen es requerida para dar de alta una nueva promoción de Vuelo.");
                }
            }
            

            if (ModelState.IsValid)
            {
                if (promotion.OriginalPrice > 0)
                {
                    promotion.DiscountPercentage = Math.Round(((promotion.OriginalPrice - promotion.OfferPrice) / promotion.OriginalPrice) * 100, 2);
                }

                promotion.ServiceType = "0";

                await _dbContext.AbmFlightPromotionAsync(promotion, "INSERT");

                TempData["SuccessMessage"] = "¡Promoción de vuelo creada exitosamente!";
                return RedirectToAction(nameof(AdminPromotionFlights));
            }

            
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
                if (promotion.ImageUrl == null)
                {
                    ModelState.Remove("ImageUrl");
                }
                if (ImageFile == null)
                {
                    ModelState.Remove("ImageFile");
                }
            }

            if (ModelState.IsValid)
            {
 
                if (ImageFile != null && ImageFile.Length > 0)
                {
                                       
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img", "promocionesvuelos");
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

                    promotion.ImageUrl = "/img/promocionesvuelos/" + uniqueFileName;
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
            return View("AltaPromotionHotel", new HotelPromotion { ServiceType = "1" });
        }


        [HttpGet]
        public async Task<IActionResult> EditPromotionHotel(int id)
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

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img", "promocioneshoteles");
                
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
                
                promotion.ImageUrl = "/img/promocioneshoteles/" + uniqueFileName;

                
                ModelState.Remove("ImageUrl");
            }
            else
            {
                
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

            await CargarWhatsappMessages();
            ViewData["Title"] = "Alta de Promoción de Hotel";
            return View("AltaPromotionHotel", promotion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPromotionHotel(int id, IFormFile ImageFile, [Bind("Id,ServiceType,Description,Whatsapp_Id,DestinationName,HotelName,ImageUrl,IsHotWeek,OriginalPrice,OfferPrice,DiscountPercentage,StartDate,EndDate,Stars,IsActive")] HotelPromotion promotion)
        {
            if (id != promotion.Id)
            {
                return NotFound();
            }
            if (promotion.ImageUrl == null && ImageFile == null) 
            { }
            else
            {

                if (promotion.ImageUrl == null)
                {
                    ModelState.Remove("ImageUrl");
                }
                if (ImageFile == null)
                {
                    ModelState.Remove("ImageFile");
                }
            }

            if (ModelState.IsValid)
            {
  
                if (ImageFile != null && ImageFile.Length > 0)
                {
        
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img","promocioneshoteles");
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

                    promotion.ImageUrl = "/img/promocioneshoteles/" + uniqueFileName;
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


            await CargarWhatsappMessages();
            ViewData["Title"] = "Editar Promoción de Hotel";
            return View("AltaPromotionHotel", promotion);
        }

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


            await _dbContext.AbmHotelPromotionAsync(id, "DELETE");
            TempData["SuccessMessage"] = "¡Promoción de hotel eliminada exitosamente!";
            return RedirectToAction(nameof(AdminPromotionHotels));
        }

        // **************** ACCIONES PARA ADMINISTRACIÓN DE PROMOCIONES DE BUSES ****************

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
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img/promocionesbuses");
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
                promotion.ImageUrl = "/img/promocionesbuses/" + uniqueFileName;
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
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img/promocionesbuses");
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
                promotion.ImageUrl = "/img/promocionesbuses/" + uniqueFileName;
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
        public async Task<IActionResult> AltaPromotionPackage()
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
            ViewData["Title"] = "Alta de Promoción de Paquete";
           
            return View("AltaPromotionPackage", new PackagePromotion { ServiceType = "3" });

        }

        [HttpGet]
        public async Task<IActionResult> EditPromotionPackage(int? id)
        {
            // 1. Carga de mensajes de WhatsApp (esto ya lo hacías bien)
            var whatsappMessages = await _dbContext.WhatsappMessages
              .Where(m => m.Is_Active)
              .OrderBy(m => m.Title)
              .ToListAsync();

            ViewBag.WhatsappMessages = whatsappMessages.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Title
            }).ToList();

            if (id == null) return NotFound();

            // 2. Obtener el paquete base
            var promotion = await _dbContext.GetPackagePromotionByIdAsync(id.Value);
            if (promotion == null) return NotFound();

          
            var entidad = await _dbContext.Entidades
                .FirstOrDefaultAsync(e => e.Nombre_Tabla == "PackagePromotions");

            if (entidad != null)
            {
                
                promotion.ImagenesAdicionales = await _dbContext.Imagenes
                    .Where(i => i.Entidad_Id == entidad.Id && i.Objeto_Id == promotion.Id)
                    .Select(i => i.Url)
                    .ToListAsync();
            }

            ViewData["Title"] = "Editar Promoción de Paquete";
            return View("AltaPromotionPackage", promotion);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitPromotionPackage(List<IFormFile> ImageFile, [Bind("Id,ServiceType,PackageType,Description,Whatsapp_Id,CompanyName,DestinationName,OriginName,ImageUrl,IsHotWeek,OriginalPrice,OfferPrice,DiscountPercentage,StartDate,EndDate,HotelName,IsActive")] PackagePromotion promotion)
        {
            // 1. Lista para las rutas físicas
            List<string> rutasImagenesNuevas = new List<string>();

            // IMPORTANTE: Cambié Collection por List en el parámetro para asegurar compatibilidad
            if (ImageFile != null && ImageFile.Count > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img/promocionespaquetes");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                foreach (var file in ImageFile)
                {
                    if (file.Length > 0)
                    {
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }

                        string rutaRelativa = "/img/promocionespaquetes/" + uniqueFileName;
                        rutasImagenesNuevas.Add(rutaRelativa);
                    }
                }

                // La primera imagen de la lista será la portada (ImageUrl principal)
                if (rutasImagenesNuevas.Count > 0)
                {
                    promotion.ImageUrl = rutasImagenesNuevas[0];
                }

                ModelState.Remove("ImageFile");
                ModelState.Remove("ImageUrl");
            }

            if (ModelState.IsValid)
            {
                // Buscamos el ID de la entidad para "PackagePromotions"
                // Esto asume que tienes la tabla 'entidades' en tu DB y un DbSet<Entidad> en tu DbContext
                var entidad = await _dbContext.Entidades
                    .FirstOrDefaultAsync(e => e.Nombre_Tabla == "PackagePromotions");

                if (entidad != null)
                {
                    promotion.ServiceType = entidad.Id.ToString();
                }

                // Lógica de precios
                if (promotion.OriginalPrice > 0)
                {
                    promotion.DiscountPercentage = Math.Round(((promotion.OriginalPrice - promotion.OfferPrice) / promotion.OriginalPrice) * 100, 2);
                }

                // A. GUARDAR PAQUETE: Llamamos al SP y obtenemos el ID generado
                var idGenerado = await _dbContext.AbmPackagePromotionAsync(promotion, "INSERT");

                // B. GUARDAR GALERÍA: Si se generó el ID y hay fotos nuevas, las guardamos mediante el SP genérico
                if (entidad != null && idGenerado > 0 && rutasImagenesNuevas.Count > 0)
                {
                    foreach (var ruta in rutasImagenesNuevas)
                    {
                        // Usamos el método que ya tienes en el DbContext que llama al SP
                        await _dbContext.InsertarImagenGenericaAsync(ruta, entidad.Id, idGenerado);
                    }
                }

                TempData["SuccessMessage"] = "¡Promoción de paquete agregada con éxito!";
                return RedirectToAction(nameof(AdminPromotionPackages));
            }

            // Si hubo error de validación, recargamos los mensajes de Whatsapp para la vista
            var whatsappMessages = await _dbContext.GetActiveWhatsappMessagesAsync();
            ViewBag.WhatsappMessages = whatsappMessages.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Title }).ToList();

            return View("AltaPromotionPackage", promotion);
        }

        // Método POST para la edición de una promoción de paquete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPromotionPackage(int id,IFormFile ImageFile, [Bind("Id,ServiceType,PackageType,Description,Whatsapp_Id,CompanyName,DestinationName,OriginName,ImageUrl,IsHotWeek,OriginalPrice,OfferPrice,DiscountPercentage,StartDate,EndDate,HotelName,IsActive")] PackagePromotion promotion)
        {
            if (id != promotion.Id)
            {
                return NotFound();
            }

            if (promotion.ImageUrl == null)
            {
                ModelState.Remove("ImageUrl");
            }

            if (ImageFile != null /*&& ImageFile.Length > 0*/)
            {
                // 1. Guarda la nueva imagen en la carpeta de destino.
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img/promocionespaquetes");
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
                promotion.ImageUrl = "/img/promocionespaquetes/" + uniqueFileName;
            }
            else
            {
                // Si no se subió un nuevo archivo, elimina el error de validación de ImageUrl del ModelState
                // para permitir que el resto del modelo se valide.
                ModelState.Remove("ImageFile");
            }

            if (ModelState.IsValid)
            {
                try
                {
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

            ViewData["Title"] = "Editar Promoción de Paquete";
            return View("AltaPromotionPackage", promotion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePromotionPackage(int id)
        {

            await _dbContext.AbmPackagePromotionAsync(id, "DELETE");
            TempData["SuccessMessage"] = "¡Promoción  eliminada exitosamente!";
            return RedirectToAction(nameof(AdminPromotionPackages));
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
        public async Task<IActionResult> EditEncomienda(
     int id,
     IFormFile ImageFile,
    
     [Bind("Id,CompanyName,CompanyUrl,ImageUrl")] EncomiendaCompany company)
        {
            if (id != company.Id)
            {
                return NotFound();
            }

           if (ImageFile == null && !string.IsNullOrEmpty(company.ImageUrl))
            {
                ModelState.Remove("ImageFile");
            }

            if (ModelState.IsValid)
            {
               if (ImageFile != null && ImageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/encomiendas");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImageFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

           
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }
                    if (!string.IsNullOrEmpty(company.ImageUrl))
                    {
                        var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, company.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }
                 
                    company.ImageUrl = "/images/encomiendas/" + uniqueFileName;
                }
                         try
                {
                    _dbContext.Update(company);
                    await _dbContext.SaveChangesAsync();

                    TempData["SuccessMessage"] = "¡Empresa de encomiendas actualizada exitosamente!";
                    return RedirectToAction(nameof(AdminEncomiendas));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Ocurrió un error al actualizar: {ex.Message}");
                    return View("AltaEncomienda", company);
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
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img","actividades");
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

                    Actividad.ImageUrl = "/img/actividades/" + uniqueFileName;
                }

                ViewData["Title"] = "Administrar Actividades";
                //ViewData["Title"] = "Gestionar Promociones de Hoteles";



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
            ViewData["Title"] = "Administrar Actividades";

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
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img","actividades");
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

                        Actividad.ImageUrl = "/img/actividades/" + uniqueFileName;
                    }

                    _dbContext.Update(Actividad);
                    await _dbContext.SaveChangesAsync();
                    TempData["SuccessMessage"] = "¡Actualizada exitosamente!";
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
         //   model.DetailDestinyURL = "/destinations/" + model.Title; // O la lógica que uses para generar esta URL

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

