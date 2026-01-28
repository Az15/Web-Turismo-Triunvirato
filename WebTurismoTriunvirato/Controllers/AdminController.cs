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

            // Cargar mensajes de WhatsApp
            ViewBag.WhatsappMessages = await GetWhatsappMessagesAsync();

            // Importante: ServiceType 3 para Vuelos
            return View("AltaPromotionFlight", new FlightPromotion
            {
                ServiceType = "3",
                IsActive = true
            });
        }

        // Método auxiliar para no repetir código de WhatsApp
        private async Task<List<SelectListItem>> GetWhatsappMessagesAsync()
        {
            return await _dbContext.WhatsappMessages
                .Where(m => m.Is_Active)
                .OrderBy(m => m.Title)
                .Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(),
                    Text = m.Title
                }).ToListAsync();
        }

        [HttpGet]
        public async Task<IActionResult> EditPromotionFlight(int id)
        {
            ViewData["Title"] = "Editar Promoción de Vuelo";

            // 1. Buscamos el vuelo
            var promotion = await _dbContext.FlightPromotions.FindAsync(id);
            if (promotion == null) return NotFound();

            // 2. Buscamos la entidad para saber el ID de Vuelos (ej: 3)
            var entidad = await _dbContext.Entidades.FirstOrDefaultAsync(e => e.Nombre_Tabla == "FlightPromotions");

            if (entidad != null)
            {
                // 3. Cargamos la galería manualmente desde la tabla 'imagenes'
                promotion.ImagenesAdicionales = await _dbContext.Imagen
                    .Where(i => i.Id_Entidad == entidad.Id && i.Id_Objeto == id)
                    .ToListAsync();
            }

            ViewBag.WhatsappMessages = await GetWhatsappMessagesAsync();
            return View("AltaPromotionFlight", promotion);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitPromotionFlight(
            List<IFormFile> ImageFile, // Cambiado a List para recibir la galería
            [Bind("Id,Whatsapp_Id,ServiceType,Description,DestinationName,OriginName,ImageUrl,IsHotWeek,OriginalPrice,OfferPrice,DiscountPercentage,StartDate,EndDate,IsActive,Stars")] FlightPromotion promotion)
        {
            // 1. OBTENER ENTIDAD PARA REFERENCIA
            var entidad = await _dbContext.Entidades.FirstOrDefaultAsync(e => e.Nombre_Tabla == "FlightPromotions");
            if (entidad == null) return BadRequest("No se encontró la configuración de entidad para Vuelos.");

            // 2. PROCESAR IMÁGENES (Subida física)
            List<string> rutasGuardadas = await ProcesarImagenes(ImageFile, entidad.Id);

            if (rutasGuardadas.Any())
            {
                // La primera imagen de la lista se asigna como portada principal
                promotion.ImageUrl = rutasGuardadas[0];
                ModelState.Remove("ImageUrl");
            }
            else
            {
                // Si es alta nueva y no hay fotos, error
                if (promotion.Id == 0)
                {
                    ModelState.AddModelError("ImageUrl", "La imagen es requerida para dar de alta una nueva promoción de Vuelo.");
                }
            }

            if (ModelState.IsValid)
            {
                // Cálculo de descuento
                if (promotion.OriginalPrice > 0)
                {
                    promotion.DiscountPercentage = Math.Round(((promotion.OriginalPrice - promotion.OfferPrice) / promotion.OriginalPrice) * 100, 2);
                }

                promotion.ServiceType = "3"; // Cambiado a 3 que es Vuelos

                // 3. EJECUTAR SP Y CAPTURAR EL ID GENERADO
                // Es vital que AbmFlightPromotionAsync devuelva el ID (int) como vimos antes
                int newId = await _dbContext.AbmFlightPromotionAsync(promotion, "INSERT");

                // 4. INSERTAR EN TABLA 'imagenes' PARA LA GALERÍA
                foreach (var ruta in rutasGuardadas)
                {
                    await _dbContext.InsertarImagenGenericaAsync(ruta, entidad.Id, newId);
                }

                TempData["SuccessMessage"] = "¡Promoción de vuelo y galería creadas exitosamente!";
                return RedirectToAction(nameof(AdminPromotionFlights));
            }

            // Si falló el modelstate, recargamos mensajes de WhatsApp para la vista
            var whatsappMessages = await _dbContext.WhatsappMessages
                .Where(m => m.Is_Active)
                .OrderBy(m => m.Title)
                .ToListAsync();
            ViewBag.WhatsappMessages = whatsappMessages.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Title
            }).ToList();

            ViewData["Title"] = "Alta de Promoción de Vuelo";
            return View("AltaPromotionFlight", promotion);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPromotionFlight(
         int id,
         List<IFormFile> ImageFile,      // Imágenes nuevas (galería)
         List<IFormFile> ReplacedFiles,  // Archivos que reemplazan a otros
         List<string> DeletedImagesUrls, // URLs de imágenes a borrar
         List<string> ReplacedImagesUrls,// URLs de imágenes viejas que serán reemplazadas
         [Bind("Id,ServiceType,Description,Whatsapp_Id,DestinationName,OriginName,ImageUrl,IsHotWeek,OriginalPrice,OfferPrice,DiscountPercentage,StartDate,EndDate,IsActive,Stars")] FlightPromotion promotion
 )
        {
            if (id != promotion.Id) return NotFound();

            // 1. OBTENER ENTIDAD PARA REFERENCIA (Vuelos suele ser Id 3 o buscamos por nombre de tabla)
            var entidad = await _dbContext.Entidades.FirstOrDefaultAsync(e => e.Nombre_Tabla == "FlightPromotions");
            if (entidad == null) return BadRequest("No se encontró la configuración de entidad para Vuelos.");

            // 2. PROCESAR BORRADOS
            if (DeletedImagesUrls != null && DeletedImagesUrls.Any())
            {
                foreach (var url in DeletedImagesUrls)
                {
                    // Usamos el método que ya tienes definido para borrar físicamente y en DB
                    await EliminarImagenPorUrl(url, entidad.Id, promotion.Id);

                    // Si la que borramos era la portada, limpiamos el campo para reasignar luego
                    if (promotion.ImageUrl == url) promotion.ImageUrl = null;
                }
            }

            // 3. PROCESAR REEMPLAZOS (Lápiz)
            if (ReplacedFiles != null && ReplacedFiles.Count > 0 && ReplacedImagesUrls != null)
            {
                for (int i = 0; i < ReplacedFiles.Count; i++)
                {
                    string urlVieja = ReplacedImagesUrls[i];
                    await EliminarImagenPorUrl(urlVieja, entidad.Id, promotion.Id);

                    // Subir la nueva usando tu método ProcesarImagenes (ajusta la carpeta si es necesario)
                    var nuevaRutaLista = await ProcesarImagenes(new List<IFormFile> { ReplacedFiles[i] }, entidad.Id);
                    if (nuevaRutaLista.Any())
                    {
                        string nuevaRuta = nuevaRutaLista[0];
                        await _dbContext.InsertarImagenGenericaAsync(nuevaRuta, entidad.Id, promotion.Id);

                        // Si la reemplazada era la portada, actualizamos el puntero
                        if (promotion.ImageUrl == urlVieja) promotion.ImageUrl = nuevaRuta;
                    }
                }
            }

            // 4. PROCESAR NUEVAS ADICIONES (Botón +)
            List<string> rutasNuevas = await ProcesarImagenes(ImageFile, entidad.Id);
            foreach (var ruta in rutasNuevas)
            {
                await _dbContext.InsertarImagenGenericaAsync(ruta, entidad.Id, promotion.Id);
            }

            // 5. LÓGICA DE SEGURIDAD PARA PORTADA
            if (string.IsNullOrEmpty(promotion.ImageUrl))
            {
                if (rutasNuevas.Any()) promotion.ImageUrl = rutasNuevas[0];
                else
                {
                    // Buscamos en la tabla 'imagenes' qué quedó disponible para este vuelo
                    var restante = await _dbContext.Imagen
                        .FirstOrDefaultAsync(i => i.Id_Entidad == entidad.Id && i.Id_Objeto == promotion.Id);
                    if (restante != null) promotion.ImageUrl = restante.Url;
                }
            }

            // Limpieza de validaciones para archivos que no son parte del modelo persistente
            ModelState.Remove("ImageFile");
            ModelState.Remove("ReplacedFiles");
            if (promotion.ImageUrl != null) ModelState.Remove("ImageUrl");

            if (ModelState.IsValid)
            {
                try
                {
                    // Cálculo de descuento
                    if (promotion.OriginalPrice > 0)
                    {
                        promotion.DiscountPercentage = Math.Round(((promotion.OriginalPrice - promotion.OfferPrice) / promotion.OriginalPrice) * 100, 2);
                    }

                    // Mantenemos el ServiceType coherente para Vuelos
                    promotion.ServiceType = "3";

                    // Llamada al SP de Vuelos
                    await _dbContext.AbmFlightPromotionAsync(promotion, "UPDATE");

                    TempData["SuccessMessage"] = "¡Promoción de vuelo actualizada con éxito!";
                    return RedirectToAction(nameof(AdminPromotionFlights));
                }
                catch (Exception ex)
                {
                    // Aquí podrías loguear el error ex
                    ModelState.AddModelError("", "Ocurrió un error al actualizar la base de datos.");
                }
            }

            // Si falló el ModelState, recargamos los datos necesarios para la vista
            var whatsappMessages = await _dbContext.WhatsappMessages
                .Where(m => m.Is_Active)
                .OrderBy(m => m.Title)
                .ToListAsync();
            ViewBag.WhatsappMessages = whatsappMessages.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Title
            }).ToList();

            ViewData["Title"] = "Editar Promoción de Vuelo";
            return View("AltaPromotionFlight", promotion);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePromotionFlight(int id)
        {
            // 1. Obtener la entidad de referencia para Vuelos
            var entidad = await _dbContext.Entidades.FirstOrDefaultAsync(e => e.Nombre_Tabla == "FlightPromotions");

            if (entidad != null)
            {
                // 2. Buscar todas las imágenes de la galería asociadas en la tabla 'imagenes'
                var imagenes = await _dbContext.Imagen
                    .Where(i => i.Id_Entidad == entidad.Id && i.Id_Objeto == id)
                    .ToListAsync();

                // Borrar archivos físicos del servidor
                foreach (var img in imagenes)
                {
                    var path = Path.Combine(_webHostEnvironment.WebRootPath, img.Url.TrimStart('/'));
                    if (System.IO.File.Exists(path))
                    {
                        try { System.IO.File.Delete(path); } catch { /* Loguear si falla el borrado físico */ }
                    }
                }

                // Opcional: Si el SP no borra en cascada la tabla 'imagenes', 
                // podrías hacer un _dbContext.Imagen.RemoveRange(imagenes) aquí.
            }

            // 3. Ejecutar el SP de borrado para el Vuelo
            // Nota: Pasamos el objeto con el ID y el tipo DELETE
            var flightToDelete = new FlightPromotion { Id = id };
            await _dbContext.AbmFlightPromotionAsync(flightToDelete, "DELETE");

            TempData["SuccessMessage"] = "¡Promoción de vuelo y sus imágenes eliminadas exitosamente!";
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

            ViewBag.WhatsappMessages = whatsappMessages.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Title
            }).ToList();

            ViewData["Title"] = "Editar Promoción de Hotel";

            // Cambiamos FindAsync por una búsqueda que nos permita asegurar que el objeto existe
            var promotion = await _dbContext.HotelPromotions.FindAsync(id);
            if (promotion == null) return NotFound();

            // --- TUTORÍA: Cargamos la galería para Hoteles ---
            // Buscamos el ID de la entidad "HotelPromotions" (que debería ser 4 según tu switch)
            var entidad = await _dbContext.Entidades
                .FirstOrDefaultAsync(e => e.Nombre_Tabla == "HotelPromotions");

            if (entidad != null)
            {
                promotion.ImagenesAdicionales = await _dbContext.Imagen
                    .Where(i => i.Id_Entidad == entidad.Id && i.Id_Objeto == promotion.Id)
                    .ToListAsync();
            }

            return View("AltaPromotionHotel", promotion);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitPromotionHotel(List<IFormFile> ImageFile, [Bind("Id,ServiceType,Description,Whatsapp_Id,DestinationName,HotelName,ImageUrl,IsHotWeek,OriginalPrice,OfferPrice,DiscountPercentage,StartDate,EndDate,Stars,IsActive")] HotelPromotion promotion)
        {
            // 1. Limpieza de validaciones
            ModelState.Remove("ImageFile");
            ModelState.Remove("ImageUrl");
            ModelState.Remove("RenderedWhatsappMessage");

            if (ModelState.IsValid)
            {
                // 2. Obtener Entidad Hoteles (ID: 4)
                var entidad = await _dbContext.Entidades.FirstOrDefaultAsync(e => e.Nombre_Tabla == "HotelPromotions");

                // 3. Procesar colección de imágenes
                List<string> rutas = await ProcesarImagenes(ImageFile, entidad?.Id ?? 4);

                if (rutas.Any())
                {
                    promotion.ImageUrl = rutas[0]; // La primera es la portada
                }
                else
                {
                    ModelState.AddModelError("ImageUrl", "Debe subir al menos una imagen.");
                    await CargarWhatsappMessages();
                    return View("AltaPromotionHotel", promotion);
                }

                // 4. Lógica de Negocio
                if (promotion.OriginalPrice > 0)
                {
                    promotion.DiscountPercentage = Math.Round(((promotion.OriginalPrice - promotion.OfferPrice) / promotion.OriginalPrice) * 100, 2);
                }
                promotion.ServiceType = "4"; // Asegúrate que este sea el ID de Hoteles

                // 5. Guardar y obtener ID
                var idGenerado = await _dbContext.AbmHotelPromotionAsync(promotion, "INSERT");

                // 6. Vincular galería en tabla imágenes
                if (entidad != null && idGenerado > 0)
                {
                    foreach (var ruta in rutas)
                    {
                        await _dbContext.InsertarImagenGenericaAsync(ruta, entidad.Id, idGenerado);
                    }
                }

                TempData["SuccessMessage"] = "¡Hotel creado con éxito!";
                return RedirectToAction(nameof(AdminPromotionHotels));
            }

            await CargarWhatsappMessages();
            return View("AltaPromotionHotel", promotion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPromotionHotel(
            int id,
            List<IFormFile> ImageFile,           // Nuevas imágenes
            List<IFormFile> ReplacedFiles,       // Archivos que reemplazan a otros
            List<string> DeletedImagesUrls,      // URLs marcadas con la "X"
            List<string> ReplacedImagesUrls,     // URLs marcadas con el "Lápiz"
            [Bind("Id,ServiceType,Description,Whatsapp_Id,DestinationName,HotelName,ImageUrl,IsHotWeek,OriginalPrice,OfferPrice,DiscountPercentage,StartDate,EndDate,Stars,IsActive")] HotelPromotion promotion)
        {
            if (id != promotion.Id) return NotFound();

            // 1. Obtener ID de Entidad (4 para Hoteles)
            var entidad = await _dbContext.Entidades.FirstOrDefaultAsync(e => e.Nombre_Tabla == "HotelPromotions");
            int idEntidad = entidad?.Id ?? 4;


            ModelState.Remove("ImageFile");
            ModelState.Remove("ImageUrl");

            if (ModelState.IsValid)
            {
                // --- A. GESTIÓN DE BORRADO ---
                if (DeletedImagesUrls != null && DeletedImagesUrls.Any())
                {
                    foreach (var url in DeletedImagesUrls)
                    {
                        // Borrar registro en BD
                        var imgDb = await _dbContext.Imagen.FirstOrDefaultAsync(i => i.Url == url && i.Id_Entidad == idEntidad);
                        if (imgDb != null) _dbContext.Imagen.Remove(imgDb);

                        // Borrar archivo físico
                        var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, url.TrimStart('/'));
                        if (System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath);
                    }
                }

                // --- B. GESTIÓN DE REEMPLAZO ---
                if (ReplacedFiles != null && ReplacedFiles.Count > 0 && ReplacedImagesUrls != null)
                {
                    for (int i = 0; i < ReplacedFiles.Count; i++)
                    {
                        // Procesar el nuevo archivo (value 4 para carpeta hoteles)
                        var nuevaRutaLista = await ProcesarImagenes(new List<IFormFile> { ReplacedFiles[i] }, idEntidad);
                        if (nuevaRutaLista.Any())
                        {
                            string antiguaUrl = ReplacedImagesUrls[i];
                            string nuevaUrl = nuevaRutaLista[0];

                            // Actualizar en BD
                            var imgDb = await _dbContext.Imagen.FirstOrDefaultAsync(i => i.Url == antiguaUrl && i.Id_Entidad == idEntidad);
                            if (imgDb != null) imgDb.Url = nuevaUrl;

                            // Si la imagen reemplazada era la portada, actualizar el objeto promotion
                            if (promotion.ImageUrl == antiguaUrl) promotion.ImageUrl = nuevaUrl;

                            // Borrar archivo físico viejo
                            var oldPath = Path.Combine(_webHostEnvironment.WebRootPath, antiguaUrl.TrimStart('/'));
                            if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                        }
                    }
                }

                // --- C. NUEVAS IMÁGENES ---
                var nuevasRutas = await ProcesarImagenes(ImageFile, idEntidad);
                foreach (var ruta in nuevasRutas)
                {
                    await _dbContext.InsertarImagenGenericaAsync(ruta, idEntidad, promotion.Id);
                }

                // Si no había portada y subió imágenes nuevas, la primera es portada
                if (string.IsNullOrEmpty(promotion.ImageUrl) && nuevasRutas.Any())
                {
                    promotion.ImageUrl = nuevasRutas[0];
                }

                // 2. Lógica de negocio y SP
                if (promotion.OriginalPrice > 0)
                    promotion.DiscountPercentage = Math.Round(((promotion.OriginalPrice - promotion.OfferPrice) / promotion.OriginalPrice) * 100, 2);

                promotion.ServiceType = "4";

                await _dbContext.AbmHotelPromotionAsync(promotion, "UPDATE");
                await _dbContext.SaveChangesAsync(); // Guardar cambios de la tabla Imagen

                TempData["SuccessMessage"] = "¡Hotel actualizado con éxito!";
                return RedirectToAction(nameof(AdminPromotionHotels));
            }

            await CargarWhatsappMessages();
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
            var entidad = await _dbContext.Entidades.FirstOrDefaultAsync(e => e.Nombre_Tabla == "HotelPromotions");
            int idEntidad = entidad?.Id ?? 4;

            // 1. Buscar todas las imágenes asociadas en la galería
            var imagenes = await _dbContext.Imagen
                .Where(i => i.Id_Entidad == idEntidad && i.Id_Objeto == id)
                .ToListAsync();

            // 2. Borrar archivos físicos
            foreach (var img in imagenes)
            {
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, img.Url.TrimStart('/'));
                if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
            }

            // 3. Llamar al SP (Asegúrate que el SP soporte DELETE o usa ExecuteSqlRaw directamente)
            // Nota: Tu método Abm recibe el objeto, puedes pasar uno nuevo solo con el ID
            await _dbContext.AbmHotelPromotionAsync(new HotelPromotion { Id = id, ServiceType = "4" }, "DELETE");

            TempData["SuccessMessage"] = "¡Promoción y sus imágenes eliminadas!";
            return RedirectToAction(nameof(AdminPromotionHotels));
        }

        // **************** ACCIONES PARA ADMINISTRACIÓN DE PROMOCIONES DE BUSES ****************

        [HttpGet]
        public async Task<IActionResult> AdminPromotionBuses()
        {
            ViewData["Title"] = "Gestionar Promociones de Micros";
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
            ViewData["Title"] = "Alta de Promoción de Micros";
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


            var entidad = await _dbContext.Entidades
         .FirstOrDefaultAsync(e => e.Nombre_Tabla == "BusPromotions");

            if (entidad != null)
            {
                promotion.ImagenesAdicionales = await _dbContext.Imagen
                    .Where(i => i.Id_Entidad == entidad.Id && i.Id_Objeto == promotion.Id)
                    .ToListAsync();
            }

            return View("AltaPromotionBus", promotion);
        }

        // POST: Admin/SubmitPromotionBus (Creación)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitPromotionBus(List<IFormFile> ImageFile, [Bind("Id,ServiceType,Description,Whatsapp_Id,DestinationName,OriginName,ImageUrl,IsHotWeek,OriginalPrice,OfferPrice,DiscountPercentage,StartDate,EndDate,IsActive,BusCompanyName,Category")] BusPromotion promotion)
        {
            // 1. Limpieza de validaciones
            ModelState.Remove("ImageFile");
            ModelState.Remove("ImageUrl");
            ModelState.Remove("RenderedWhatsappMessage");

            if (ModelState.IsValid)
            {

                var entidad = await _dbContext.Entidades.FirstOrDefaultAsync(e => e.Nombre_Tabla == "BusPromotions");
                List<string> rutas = await ProcesarImagenes(ImageFile, entidad?.Id ?? 2);


                if (rutas.Any())
                {
                    promotion.ImageUrl = rutas[0]; // La primera es la portada
                }
                else
                {
                    ModelState.AddModelError("ImageUrl", "Debe subir al menos una imagen.");
                    await CargarWhatsappMessages();
                    return View("AltaPromotionBus", promotion);
                }



                if (promotion.OriginalPrice > 0)
                {
                    promotion.DiscountPercentage = Math.Round(((promotion.OriginalPrice - promotion.OfferPrice) / promotion.OriginalPrice) * 100, 2);
                }
                promotion.ServiceType = "2";

                var idGenerado = await _dbContext.AbmBusPromotionAsync(promotion, "INSERT");

                if (entidad != null && idGenerado > 0)
                {
                    foreach (var ruta in rutas)
                    {
                        await _dbContext.InsertarImagenGenericaAsync(ruta, entidad.Id, idGenerado);
                    }
                }

                TempData["SuccessMessage"] = "¡Promoción de Micro agregada exitosamente!";
                return RedirectToAction(nameof(AdminPromotionBuses));
            }

   
            await CargarWhatsappMessages();

            ViewData["Title"] = "Alta de Promoción de Bus";
            return View("AltaPromotionBus", promotion);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPromotionBus(int id,

            List<IFormFile> ImageFile,           // Nuevas imágenes
            List<IFormFile> ReplacedFiles,       // Archivos que reemplazan a otros
            List<string> DeletedImagesUrls,      // URLs marcadas con la "X"
            List<string> ReplacedImagesUrls,     // URLs marcadas con el "Lápiz"

            [Bind("Id,ServiceType,Description,Whatsapp_Id,DestinationName,OriginName,ImageUrl,IsHotWeek,OriginalPrice,OfferPrice,DiscountPercentage,StartDate,EndDate,IsActive,BusCompanyName,Category")] BusPromotion promotion)
        {
            if (id != promotion.Id)
            {
                return NotFound();
            }

            var entidad = await _dbContext.Entidades.FirstOrDefaultAsync(e => e.Nombre_Tabla == "BusPromotions");
            int idEntidad = entidad?.Id ?? 2;


            // Paso 1: Si no se sube un nuevo archivo, elimina el error de validación para ImageUrl.
            if (promotion.ImageUrl == null)
            {
                ModelState.Remove("ImageFile");
                ModelState.Remove("ImageUrl");
            }


            if (ModelState.IsValid)
            {

                if (DeletedImagesUrls != null && DeletedImagesUrls.Any())
                {
                    foreach (var url in DeletedImagesUrls)
                    {
                        // Borrar registro en BD
                        var imgDb = await _dbContext.Imagen.FirstOrDefaultAsync(i => i.Url == url && i.Id_Entidad == idEntidad);
                        if (imgDb != null) _dbContext.Imagen.Remove(imgDb);

                        // Borrar archivo físico
                        var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, url.TrimStart('/'));
                        if (System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath);
                    }
                }

                // --- B. GESTIÓN DE REEMPLAZO ---
                if (ReplacedFiles != null && ReplacedFiles.Count > 0 && ReplacedImagesUrls != null)
                {
                    for (int i = 0; i < ReplacedFiles.Count; i++)
                    {
                        // Procesar el nuevo archivo (value 4 para carpeta hoteles)
                        var nuevaRutaLista = await ProcesarImagenes(new List<IFormFile> { ReplacedFiles[i] }, idEntidad);
                        if (nuevaRutaLista.Any())
                        {
                            string antiguaUrl = ReplacedImagesUrls[i];
                            string nuevaUrl = nuevaRutaLista[0];

                            // Actualizar en BD
                            var imgDb = await _dbContext.Imagen.FirstOrDefaultAsync(i => i.Url == antiguaUrl && i.Id_Entidad == idEntidad);
                            if (imgDb != null) imgDb.Url = nuevaUrl;

                            // Si la imagen reemplazada era la portada, actualizar el objeto promotion
                            if (promotion.ImageUrl == antiguaUrl) promotion.ImageUrl = nuevaUrl;

                            // Borrar archivo físico viejo
                            var oldPath = Path.Combine(_webHostEnvironment.WebRootPath, antiguaUrl.TrimStart('/'));
                            if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                        }
                    }
                }

                // --- C. NUEVAS IMÁGENES ---
                var nuevasRutas = await ProcesarImagenes(ImageFile, idEntidad);
                foreach (var ruta in nuevasRutas)
                {
                    await _dbContext.InsertarImagenGenericaAsync(ruta, idEntidad, promotion.Id);
                }

                // Si no había portada y subió imágenes nuevas, la primera es portada
                if (string.IsNullOrEmpty(promotion.ImageUrl) && nuevasRutas.Any())
                {
                    promotion.ImageUrl = nuevasRutas[0];
                }


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

                promotion.ServiceType = "2";

                await _dbContext.AbmBusPromotionAsync(promotion, "UPDATE");
                await _dbContext.SaveChangesAsync(); // Guardar cambios de la tabla Imagen

                TempData["SuccessMessage"] = "¡Micro actualizado con éxito!";
                return RedirectToAction(nameof(AdminPromotionBuses));


            }
            await CargarWhatsappMessages();
            return View("AltaPromotionBus", promotion);

        }
       
     
        // POST: Admin/DeletePromotionBus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePromotionBus(int id)
        {

            var entidad = await _dbContext.Entidades.FirstOrDefaultAsync(e => e.Nombre_Tabla == "BusPromotions");
            int idEntidad = entidad?.Id ?? 2;

            var imagenes = await _dbContext.Imagen
         .Where(i => i.Id_Entidad == idEntidad && i.Id_Objeto == id)
         .ToListAsync();

            // 2. Borrar archivos físicos
            foreach (var img in imagenes)
            {
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, img.Url.TrimStart('/'));
                if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
            }

            // 3. Llamar al SP (Asegúrate que el SP soporte DELETE o usa ExecuteSqlRaw directamente)
            // Nota: Tu método Abm recibe el objeto, puedes pasar uno nuevo solo con el ID
            await _dbContext.AbmBusPromotionAsync(new BusPromotion { Id = id, ServiceType = "2" }, "DELETE");




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

        [HttpGet]
        public async Task<IActionResult> AltaPromotionPackage()
        {
            var whatsappMessages = await _dbContext.WhatsappMessages
             .Where(m => m.Is_Active)
             .OrderBy(m => m.Title)
             .ToListAsync();

            ViewBag.WhatsappMessages = whatsappMessages.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Title
            }).ToList();

            ViewData["Title"] = "Alta de Promoción de Paquete";
            return View("AltaPromotionPackage", new PackagePromotion { ServiceType = "3" });
        }

        [HttpGet]
        public async Task<IActionResult> EditPromotionPackage(int? id)
        {
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

            var promotion = await _dbContext.GetPackagePromotionByIdAsync(id.Value);
            if (promotion == null) return NotFound();

            // TUTORÍA: Cargamos la galería para que se vea en la vista de edición
            var entidad = await _dbContext.Entidades
                .FirstOrDefaultAsync(e => e.Nombre_Tabla == "PackagePromotions");

            if (entidad != null)
            {


                // Por esta línea correcta:
                promotion.ImagenesAdicionales = await _dbContext.Imagen
                    .Where(i => i.Id_Entidad == entidad.Id && i.Id_Objeto == promotion.Id)
                    .ToListAsync();

            }

            ViewData["Title"] = "Editar Promoción de Paquete";
            return View("AltaPromotionPackage", promotion);
        }

        //------------------------------------------------------------------------------------------???????????????????????????????????????

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitPromotionPackage(List<IFormFile> ImageFile, [Bind("Id,ServiceType,PackageType,Description,Whatsapp_Id,CompanyName,DestinationName,OriginName,ImageUrl,IsHotWeek,OriginalPrice,OfferPrice,DiscountPercentage,StartDate,EndDate,HotelName,IsActive")] PackagePromotion promotion)
        {

            ModelState.Remove("ImageFile");
            ModelState.Remove("ImageUrl");

            if (ModelState.IsValid)
            {
                var entidad = await _dbContext.Entidades.FirstOrDefaultAsync(e => e.Nombre_Tabla == "PackagePromotions");

                List<string> rutasImagenesNuevas = await ProcesarImagenes(ImageFile,entidad.Id);

                if (rutasImagenesNuevas.Count > 0)
                {
                    promotion.ImageUrl = rutasImagenesNuevas[0];
                }

                if (promotion.OriginalPrice > 0)
                {
                    promotion.DiscountPercentage = Math.Round(((promotion.OriginalPrice - promotion.OfferPrice) / promotion.OriginalPrice) * 100, 2);
                }

                var idGenerado = await _dbContext.AbmPackagePromotionAsync(promotion, "INSERT");

                if (entidad != null && idGenerado > 0 && rutasImagenesNuevas.Count > 0)
                {
                    foreach (var ruta in rutasImagenesNuevas)
                    {
                        await _dbContext.InsertarImagenGenericaAsync(ruta, entidad.Id, idGenerado);
                    }
                }

                TempData["SuccessMessage"] = "¡Promoción de paquete agregada con éxito!";
                return RedirectToAction(nameof(AdminPromotionPackages));
            }

            await CargarViewBagWhatsapp();
            return View("AltaPromotionPackage", promotion);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPromotionPackage(
                int id,
                List<IFormFile> ImageFile,      // Imágenes nuevas (galería)
                List<IFormFile> ReplacedFiles,  // Archivos que reemplazan a otros
                List<string> DeletedImagesUrls, // URLs de imágenes a borrar
                List<string> ReplacedImagesUrls,// URLs de imágenes viejas que serán reemplazadas
                [Bind("Id,ServiceType,PackageType,Description,Whatsapp_Id,CompanyName,DestinationName,OriginName,ImageUrl,IsHotWeek,OriginalPrice,OfferPrice,DiscountPercentage,StartDate,EndDate,HotelName,IsActive")] PackagePromotion promotion
        )
        {
            if (id != promotion.Id) return NotFound();

            // 1. OBTENER ENTIDAD PARA REFERENCIA
            var entidad = await _dbContext.Entidades.FirstOrDefaultAsync(e => e.Nombre_Tabla == "PackagePromotions");
            if (entidad == null) return BadRequest("No se encontró la configuración de entidad.");

            // 2. PROCESAR BORRADOS (Imágenes marcadas con la X)
            if (DeletedImagesUrls != null && DeletedImagesUrls.Any())
            {
                foreach (var url in DeletedImagesUrls)
                {
                    await EliminarImagenPorUrl(url, entidad.Id, promotion.Id);
                    // Si la que borramos era la portada, limpiamos el campo
                    if (promotion.ImageUrl == url) promotion.ImageUrl = null;
                }
            }

            // 3. PROCESAR REEMPLAZOS (Imágenes editadas con el lápiz)
            if (ReplacedFiles != null && ReplacedFiles.Count > 0 && ReplacedImagesUrls != null)
            {
                for (int i = 0; i < ReplacedFiles.Count; i++)
                {
                    // Borrar la vieja
                    string urlVieja = ReplacedImagesUrls[i];
                    await EliminarImagenPorUrl(urlVieja, entidad.Id, promotion.Id);

                    // Subir la nueva (usamos el método que ya tienes pero pasando el archivo individual en una lista)
                    var nuevaRutaLista = await ProcesarImagenes(new List<IFormFile> { ReplacedFiles[i] }, entidad.Id);
                    if (nuevaRutaLista.Any())
                    {
                        string nuevaRuta = nuevaRutaLista[0];
                        await _dbContext.InsertarImagenGenericaAsync(nuevaRuta, entidad.Id, promotion.Id);

                        // Si la reemplazada era la portada, actualizamos el puntero de portada
                        if (promotion.ImageUrl == urlVieja) promotion.ImageUrl = nuevaRuta;
                    }
                }
            }

            // 4. PROCESAR NUEVAS ADICIONES (El contenedor de abajo con el botón +)
            List<string> rutasNuevas = await ProcesarImagenes(ImageFile, entidad.Id);
            foreach (var ruta in rutasNuevas)
            {
                await _dbContext.InsertarImagenGenericaAsync(ruta, entidad.Id, promotion.Id);
            }

            // 5. LÓGICA DE PORTADA: Si nos quedamos sin portada, asignar la primera disponible
            if (string.IsNullOrEmpty(promotion.ImageUrl))
            {
                // Intentamos buscar la primera de las nuevas subidas ahora
                if (rutasNuevas.Any()) promotion.ImageUrl = rutasNuevas[0];
                else
                {
                    // O buscamos en la base de datos lo que queda
                    var restante = await _dbContext.Imagen
                        .FirstOrDefaultAsync(i => i.Id_Entidad == entidad.Id && i.Id_Objeto == promotion.Id);
                    if (restante != null) promotion.ImageUrl = restante.Url;
                }
            }

            ModelState.Remove("ImageFile");
            ModelState.Remove("ReplacedFiles");
            if (promotion.ImageUrl != null) ModelState.Remove("ImageUrl");

            if (ModelState.IsValid)
            {
                try
                {
                    if (promotion.OriginalPrice > 0)
                    {
                        promotion.DiscountPercentage = Math.Round(((promotion.OriginalPrice - promotion.OfferPrice) / promotion.OriginalPrice) * 100, 2);
                    }
                    promotion.ServiceType = "3";

                    await _dbContext.AbmPackagePromotionAsync(promotion, "UPDATE");

                    TempData["SuccessMessage"] = "¡Promoción actualizada con éxito!";
                    return RedirectToAction(nameof(AdminPromotionPackages));
                }
                catch (Exception ex)
                {
                    // Manejar error
                }
            }

            await CargarViewBagWhatsapp();
            return View("AltaPromotionPackage", promotion);
        }

        // NUEVO MÉTODO: Para borrar una imagen específica de la galería vía AJAX o Post
        [HttpPost]
        private async Task EliminarImagenPorUrl(string url, int entidadId, int objetoId)
        {
            var registro = await _dbContext.Imagen
                .FirstOrDefaultAsync(i => i.Url == url && i.Id_Entidad == entidadId && i.Id_Objeto == objetoId);

            if (registro != null)
            {
                // Borrar archivo físico
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, url.TrimStart('/'));
                if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);

                // Borrar registro DB
                _dbContext.Imagen.Remove(registro);
                await _dbContext.SaveChangesAsync();
            }
        }

        //----------------


        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> DeletePromotionPackage(int id)
        {
            // 1. Obtener la entidad de referencia
            var entidad = await _dbContext.Entidades.FirstOrDefaultAsync(e => e.Nombre_Tabla == "PackagePromotions");

            if (entidad != null)
            {
                // 2. Buscar imágenes asociadas para borrar archivos físicos
                var imagenes = await _dbContext.Imagen
                    .Where(i => i.Id_Entidad == entidad.Id && i.Id_Objeto == id)
                    .ToListAsync();

                foreach (var img in imagenes)
                {
                    var path = Path.Combine(_webHostEnvironment.WebRootPath, img.Url.TrimStart('/'));
                    if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                }
            }

            // 3. Ejecutar el SP de borrado (debería borrar en cascada o tendrías que borrar la galería antes en la DB)
            await _dbContext.AbmPackagePromotionAsync(id, "DELETE");

            TempData["SuccessMessage"] = "¡Promoción y sus imágenes eliminadas exitosamente!";
            return RedirectToAction(nameof(AdminPromotionPackages));
        }



        // Reemplaza el bloque incorrecto en el método ProcesarImagenes
        private async Task<List<string>> ProcesarImagenes(List<IFormFile> files, int value)
        {
            List<string> rutas = new List<string>();
            if (files == null || files.Count == 0) return rutas;

            string subCarpeta = value switch
            {
                1 => "Actividades",
                2 => "promocionebuses",
                3 => "promocionesvuelos",
                4 => "promocioneshoteles",
                5 => "promocionespaquetes",
                _ => "otros"
            };

            string rutaRelativaBase = $"/img/{subCarpeta}/";
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img", subCarpeta);

            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    rutas.Add(rutaRelativaBase + uniqueFileName);
                }
            }
            return rutas;
        }



        private async Task CargarViewBagWhatsapp()
        {
            var whatsappMessages = await _dbContext.WhatsappMessages
                .Where(m => m.Is_Active)
                .OrderBy(m => m.Title)
                .ToListAsync();
            ViewBag.WhatsappMessages = whatsappMessages.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Title }).ToList();
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
            // Asegúrate de que la tabla en el DbContext sea 'Activities'
            var activitiesAdmin = await _dbContext.Activities.ToListAsync();
            return View("AdminActivities", activitiesAdmin);
        }

        [HttpGet]
        public async Task<IActionResult> AltaActividad()
        {
            ViewData["Title"] = "Alta de Actividades";
            await CargarWhatsappMessages();
            return View("AltaActividad", new ActivitiesPromotion());
        }

        [HttpGet]
        public async Task<IActionResult> EditActividad(int id) // Corregido el nombre a 'EditActividad'
        {
            await CargarWhatsappMessages();
            ViewData["Title"] = "Editar Actividad";

            // IMPORTANTE: Verifica que estés buscando en la tabla correcta
            var activity = await _dbContext.Activities.FindAsync(id);
            if (activity == null) return NotFound();

            var entidad = await _dbContext.Entidades
                .FirstOrDefaultAsync(e => e.Nombre_Tabla == "ActivitiesPromotion");

            if (entidad != null)
            {
                activity.ImagenesAdicionales = await _dbContext.Imagen
                    .Where(i => i.Id_Entidad == entidad.Id && i.Id_Objeto == activity.Id)
                    .ToListAsync();
            }

            // Usamos la misma vista 'AltaActividad' para editar
            return View("AltaActividad", activity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AltaActividad(List<IFormFile> ImageFile,
            [Bind("Id,Title,Description,Whatsapp_Id,Location,ImageUrl,IsActive")] ActivitiesPromotion Actividad)
        {
            ModelState.Remove("ImageFile");
            ModelState.Remove("ImageUrl");
            ModelState.Remove("RenderedWhatsappMessage");

            if (ModelState.IsValid)
            {
                var entidad = await _dbContext.Entidades.FirstOrDefaultAsync(e => e.Nombre_Tabla == "ActivitiesPromotion");
                List<string> rutas = await ProcesarImagenes(ImageFile, entidad?.Id ?? 1);

                if (rutas.Any())
                {
                    Actividad.ImageUrl = rutas[0];
                }
                else
                {
                    ModelState.AddModelError("ImageUrl", "Debe subir al menos una imagen.");
                    await CargarWhatsappMessages();
                    return View("AltaActividad", Actividad);
                }

                // Guardar mediante Procedimiento Almacenado
                var idGenerado = await _dbContext.AbmActivityAsync(Actividad, "INSERT");

                if (entidad != null && idGenerado > 0)
                {
                    foreach (var ruta in rutas)
                    {
                        await _dbContext.InsertarImagenGenericaAsync(ruta, entidad.Id, idGenerado);
                    }
                }

                TempData["SuccessMessage"] = "¡Actividad creada con éxito!";
                return RedirectToAction(nameof(AdminActivities));
            }
            await CargarWhatsappMessages();
            return View(Actividad);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditActividad(int id,
            List<IFormFile> ImageFile,
            List<IFormFile> ReplacedFiles,
            List<string> DeletedImagesUrls,
            List<string> ReplacedImagesUrls,
            [Bind("Id,Title,Description,Whatsapp_Id,Location,ImageUrl,IsActive")] ActivitiesPromotion Actividad)
        {
            if (id != Actividad.Id) return NotFound();

            var entidad = await _dbContext.Entidades.FirstOrDefaultAsync(e => e.Nombre_Tabla == "ActivitiesPromotion");
            int idEntidad = entidad?.Id ?? 1;

            ModelState.Remove("ImageFile");
            ModelState.Remove("ImageUrl");

            if (ModelState.IsValid)
            {
                // A. GESTIÓN DE BORRADO (Corregido para evitar NullReferenceException)
                if (DeletedImagesUrls != null)
                {
                    // Limpiamos la lista de posibles nulos o vacíos antes de iterar
                    var urlsToProcess = DeletedImagesUrls.Where(u => !string.IsNullOrEmpty(u)).ToList();
                    foreach (var url in urlsToProcess)
                    {
                        var imgDb = await _dbContext.Imagen.FirstOrDefaultAsync(i => i.Url == url && i.Id_Entidad == idEntidad);
                        if (imgDb != null)
                        {
                            _dbContext.Imagen.Remove(imgDb);
                            // Solo borramos el archivo físico si estaba en la BD
                            var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, url.TrimStart('/'));
                            if (System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath);
                        }
                    }
                }

                // B. GESTIÓN DE REEMPLAZO
                if (ReplacedFiles != null && ReplacedFiles.Count > 0 && ReplacedImagesUrls != null)
                {
                    for (int i = 0; i < ReplacedFiles.Count; i++)
                    {
                        if (ReplacedFiles[i] == null) continue;

                        var nuevaRutaLista = await ProcesarImagenes(new List<IFormFile> { ReplacedFiles[i] }, idEntidad);
                        if (nuevaRutaLista.Any())
                        {
                            string antiguaUrl = ReplacedImagesUrls[i];
                            string nuevaUrl = nuevaRutaLista[0];

                            var imgDb = await _dbContext.Imagen.FirstOrDefaultAsync(i => i.Url == antiguaUrl && i.Id_Entidad == idEntidad);
                            if (imgDb != null) imgDb.Url = nuevaUrl;

                            if (Actividad.ImageUrl == antiguaUrl) Actividad.ImageUrl = nuevaUrl;

                            var oldPath = Path.Combine(_webHostEnvironment.WebRootPath, antiguaUrl.TrimStart('/'));
                            if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                        }
                    }
                }

                // C. NUEVAS IMÁGENES
                var nuevasRutas = await ProcesarImagenes(ImageFile, idEntidad);
                if (nuevasRutas != null)
                {
                    foreach (var ruta in nuevasRutas)
                    {
                        await _dbContext.InsertarImagenGenericaAsync(ruta, idEntidad, Actividad.Id);
                    }
                }

                // Actualizar mediante Procedimiento Almacenado
                await _dbContext.AbmActivityAsync(Actividad, "UPDATE");

                // Guardar cambios pendientes en la tabla Imagen (EF Core)
                await _dbContext.SaveChangesAsync();

                TempData["SuccessMessage"] = "¡Actividad actualizada exitosamente!";
                return RedirectToAction(nameof(AdminActivities));
            }

            await CargarWhatsappMessages();
            return View("AltaActividad", Actividad);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteActividad(int id)
        {
            var entidad = await _dbContext.Entidades.FirstOrDefaultAsync(e => e.Nombre_Tabla == "ActivitiesPromotion");
            int idEntidad = entidad?.Id ?? 1;

            // 1. Galería de imágenes
            var imagenes = await _dbContext.Imagen
                .Where(i => i.Id_Entidad == idEntidad && i.Id_Objeto == id)
                .ToListAsync();

            foreach (var img in imagenes)
            {
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, img.Url.TrimStart('/'));
                if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
                _dbContext.Imagen.Remove(img);
            }

            // 2. Ejecutar SP de eliminación
            // IMPORTANTE: Verifica que tu SP acepte "DELETE" y el ID correctamente
            await _dbContext.AbmActivityAsync(new ActivitiesPromotion { Id = id }, "DELETE");

            await _dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "¡Actividad eliminada exitosamente!";
            return RedirectToAction(nameof(AdminActivities));
        }

        
    



    /// <summary>
    /// /////////////////////////////////////////////////////////
    /// </summary>
    /// <returns></returns>

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




        /// -------------------------------------------- -----------



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

