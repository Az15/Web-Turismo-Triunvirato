using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Web_Turismo_Triunvirato.Services;
using Web_Turismo_Triunvirato.Models;
using System;
using Microsoft.EntityFrameworkCore;
using Web_Turismo_Triunvirato.DataAccess;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Diagnostics;

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
            ViewData["Title"] = "Panel de Administraci�n";
            return View();
        }

        public IActionResult Perfil()
        {
            ViewData["Title"] = "Perfil del Administrador";
            return View();
        }

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

        public IActionResult packages()
        {
            ViewData["Title"] = "Administraci�n de Paquetes";
            return View();
        }

        public IActionResult Encomiendas()
        {
            ViewData["Title"] = "Administraci�n de Encomiendas";
            return View();
        }

        public IActionResult Activities()
        {
            ViewData["Title"] = "Panel de Administraci�n";
            return View();
        }



        // **************** ACCIONES PARA ADMINISTRACI�N DE PROMOCIONES DE VUELOS ****************

        [HttpGet]
        public async Task<IActionResult> AdminPromotionFlights()
        {
            ViewData["Title"] = "Gestionar Promociones de Vuelos";
            var flightPromotions = await _dbContext.GetActiveflightpromotionsItemsAsync();
            return View("AdminPromotionFlights", flightPromotions);
        }




        [HttpGet]
        public IActionResult AltaPromotionFlight()
        {
            ViewData["Title"] = "Alta de Promoci�n de Vuelo";
            return View("AltaPromotionFlight", new FlightPromotion { ServiceType = "0" });
        }

        [HttpGet]
        public async Task<IActionResult> EditPromotionFlight(int id)
        {
            ViewData["Title"] = "Editar Promoci�n de Vuelo";
            var promotion = await _dbContext.FlightPromotions.FindAsync(id);

            if (promotion == null)
            {
                return NotFound();
            }

            return View("AltaPromotionFlight", promotion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitPromotionFlight([Bind("Id,Whatsapp_Id,ServiceType,Description,DestinationName,OriginName,ImageUrl,IsHotWeek,OriginalPrice,OfferPrice,DiscountPercentage,StartDate,EndDate,IsActive,Stars")] FlightPromotion promotion)
        {
            if (ModelState.IsValid)
            {
                if (promotion.OriginalPrice > 0)
                {
                    promotion.DiscountPercentage = Math.Round(((promotion.OriginalPrice - promotion.OfferPrice) / promotion.OriginalPrice) * 100, 2);
                }
                promotion.ServiceType = "0";

                await _dbContext.AbmFlightPromotionAsync(promotion, "INSERT");

                TempData["SuccessMessage"] = "�Promoci�n de vuelo creada exitosamente!";
                return RedirectToAction(nameof(AdminPromotionFlights));
            }
            ViewData["Title"] = "Alta de Promoci�n de Vuelo";
            return View("AltaPromotionFlight", promotion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPromotionFlight(int id, [Bind("Id,Whatsapp_Id,ServiceType,Description,DestinationName,OriginName,ImageUrl,IsHotWeek,OriginalPrice,OfferPrice,DiscountPercentage,StartDate,EndDate,IsActive,Stars")] FlightPromotion promotion)
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
                    TempData["SuccessMessage"] = "�Promoci�n de vuelo actualizada exitosamente!";
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
            ViewData["Title"] = "Editar Promoci�n de Vuelo";
            return View("AltaPromotionFlight", promotion);
        }

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
            TempData["SuccessMessage"] = "�Promoci�n de vuelo eliminada exitosamente!";
            return RedirectToAction(nameof(AdminPromotionFlights));
        }

        // **************** ACCIONES PARA ADMINISTRACI�N DE PROMOCIONES DE HOTELES ****************

        [HttpGet]
        public async Task<IActionResult> AdminPromotionHotels()
        {
            ViewData["Title"] = "Gestionar Promociones de Hoteles";
            var hotelPromotions = await _dbContext.GetActivePromotionHotelsItemsAsync();
            return View(hotelPromotions);
        }

        [HttpGet]
        public IActionResult AltaPromotionHotel()
        {
            ViewData["Title"] = "Alta de Promoci�n de Hotel";
            return View("AltaPromotionHotel", new HotelPromotion { ServiceType = "1" });
        }

        [HttpGet]
        public async Task<IActionResult> EditPromotionHotel(int id)
        {
            ViewData["Title"] = "Editar Promoci�n de Hotel";
            var promotion = await _dbContext.HotelPromotions.FindAsync(id);
            if (promotion == null)
            {
                return NotFound();
            }
            return View("AltaPromotionHotel", promotion);
        }

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
                TempData["SuccessMessage"] = "�Promoci�n de hotel agregada exitosamente!";
                return RedirectToAction(nameof(AdminPromotionHotels));
            }
            ViewData["Title"] = "Alta de Promoci�n de Hotel";
            return View("AltaPromotionHotel", promotion);
        }

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
                    TempData["SuccessMessage"] = "�Promoci�n de hotel actualizada exitosamente!";
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
            ViewData["Title"] = "Editar Promoci�n de Hotel";
            return View("AltaPromotionHotel", promotion);
        }

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
            TempData["SuccessMessage"] = "�Promoci�n de hotel eliminada exitosamente!";
            return RedirectToAction(nameof(AdminPromotionHotels));
        }

        // **************** ACCIONES PARA ADMINISTRACI�N DE PROMOCIONES DE BUSES ****************

        // GET: Admin/AdminPromotionBuses
        [HttpGet]
        public async Task<IActionResult> AdminPromotionBuses()
        {
            ViewData["Title"] = "Gestionar Promociones de Buses";
            var busPromotions = await _dbContext.GetActivePromotionBusesItemsAsync(); // Asumiendo que este m�todo existe en tu DbContext
            return View("AdminPromotionBuses", busPromotions); // Se asume que tienes una vista llamada AdminPromotionBuses.cshtml
        }

        // GET: Admin/AltaPromotionBus
        [HttpGet]
        public IActionResult AltaPromotionBus()
        {
            ViewData["Title"] = "Alta de Promoci�n de Bus";
            return View("AltaPromotionBus", new BusPromotion { ServiceType = "2" }); // Se asume que tienes un modelo BusPromotion y una vista AltaPromotionBus.cshtml
        }

        // GET: Admin/EditPromotionBus/{id}
        [HttpGet]
        public async Task<IActionResult> EditPromotionBus(int id)
        {
            ViewData["Title"] = "Editar Promoci�n de Bus";
            var promotion = await _dbContext.BusPromotions.FindAsync(id); // Asumiendo que BusPromotions es una propiedad en tu DbContext
            if (promotion == null)
            {
                return NotFound();
            }
            return View("AltaPromotionBus", promotion);
        }

        // POST: Admin/SubmitPromotionBus (Creaci�n)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitPromotionBus([Bind("Id,ServiceType,PackageType,Description,DestinationName,OriginName,ImageUrl,IsHotWeek,OriginalPrice,OfferPrice,DiscountPercentage,StartDate,EndDate,IsActive,AirlineName,HotelName,Stars,BusCompanyName,Category")] BusPromotion promotion)
        {
            if (ModelState.IsValid)
            {
                if (promotion.OriginalPrice > 0)
                {
                    promotion.DiscountPercentage = Math.Round(((promotion.OriginalPrice - promotion.OfferPrice) / promotion.OriginalPrice) * 100, 2);
                }
                promotion.ServiceType = "2";
                await _dbContext.AbmBusPromotionAsync(promotion, "INSERT"); // Asumiendo que AbmBusPromotionAsync existe
                TempData["SuccessMessage"] = "�Promoci�n de bus agregada exitosamente!";
                return RedirectToAction(nameof(AdminPromotionBuses));
            }
            ViewData["Title"] = "Alta de Promoci�n de Bus";
            return View("AltaPromotionBus", promotion);
        }

        // POST: Admin/EditPromotionBus/{id} (Edici�n)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPromotionBus(int id, [Bind("Id,ServiceType,Description,DestinationName,OriginName,ImageUrl,IsHotWeek,OriginalPrice,OfferPrice,DiscountPercentage,StartDate,EndDate,IsActive,BusCompanyName,Category")] BusPromotion promotion)
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
                    promotion.ServiceType = "2";
                    await _dbContext.AbmBusPromotionAsync(promotion, "UPDATE");
                    TempData["SuccessMessage"] = "�Promoci�n de bus actualizada exitosamente!";
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
            ViewData["Title"] = "Editar Promoci�n de Bus";
            return View("AltaPromotionBus", promotion);
        }

        // POST: Admin/DeletePromotionBus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePromotionBus(int id)
        {
            var promotion = await _dbContext.BusPromotions.FindAsync(id);
            if (promotion == null)
            {
                return NotFound();
            }

            await _dbContext.AbmBusPromotionAsync(promotion, "DELETE");
            TempData["SuccessMessage"] = "�Promoci�n de bus eliminada exitosamente!";
            return RedirectToAction(nameof(AdminPromotionBuses));
        }


        // **************** ACCIONES PARA ADMINISTRACI�N DE PROMOCIONES DE PAQUETES ****************

        public async Task<IActionResult> AdminPromotionPackages()
        {
            var promotions = await _dbContext.GetActivePromotionPackagesItemsAsync();
            ViewData["Title"] = "Gesti�n de Promociones de Paquetes";
            return View(promotions);
        }

        // M�todo GET para mostrar la vista de creaci�n de una nueva promoci�n de paquete
        public IActionResult AltaPromotionPackage()
        {
            ViewData["Title"] = "Alta de Promoci�n de Paquete";
            // Pasa un nuevo modelo vac�o a la vista para evitar la excepci�n
            return View(new PackagePromotion());
        }

        // M�todo GET para mostrar la vista de edici�n de una promoci�n de paquete
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
            ViewData["Title"] = "Editar Promoci�n de Paquete";
            return View("AltaPromotionPackage", promotion);
        }

        // M�todo POST para la creaci�n de una promoci�n de paquete
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Se ajusta el Bind para incluir solo los campos que se env�an desde la vista.
        public async Task<IActionResult> SubmitPromotionPackage([Bind("Id,ServiceType,PackageType,Description,CompanyName," +
                                                                        "DestinationName,OriginName,ImageUrl,IsHotWeek,OriginalPrice,OfferPrice,DiscountPercentage,StartDate,EndDate," +
                                                                        "IsActive,HotelName")] PackagePromotion promotion)
        {
            if (ModelState.IsValid)
            {
                // Se calcula el descuento si los precios son v�lidos.
                if (promotion.OriginalPrice > 0)
                {
                    promotion.DiscountPercentage = Math.Round(((promotion.OriginalPrice - promotion.OfferPrice) / promotion.OriginalPrice) * 100, 2);
                }

                // El tipo de servicio para los paquetes es "3".
                promotion.ServiceType = "3";

                // Se llama al m�todo ABM del DbContext para insertar el nuevo paquete.
                await _dbContext.AbmPackagePromotionAsync(promotion, "INSERT");

                TempData["SuccessMessage"] = "�Promoci�n de paquete agregada exitosamente!";
                return RedirectToAction(nameof(AdminPromotionPackages));
            }

            // Si el modelo no es v�lido, se regresa a la vista para mostrar los errores.
            ViewData["Title"] = "Alta de Promoci�n de Paquete";
            return View("AltaPromotionPackage", promotion);
        }

        // M�todo POST para la edici�n de una promoci�n de paquete
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Se ajusta el Bind para incluir solo los campos que se env�an desde la vista.
        public async Task<IActionResult> EditPromotionPackage(int id, [Bind("Id,ServiceType,PackageType,Description,CompanyName,DestinationName,OriginName,ImageUrl,IsHotWeek,OriginalPrice,OfferPrice,DiscountPercentage,StartDate,EndDate,IsActive,HotelName")] PackagePromotion promotion)
        {
            if (id != promotion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Se recalcula el porcentaje de descuento en caso de que los precios se hayan modificado.
                    if (promotion.OriginalPrice > 0)
                    {
                        promotion.DiscountPercentage = Math.Round(((promotion.OriginalPrice - promotion.OfferPrice) / promotion.OriginalPrice) * 100, 2);
                    }

                    // El tipo de servicio para los paquetes es "3".
                    promotion.ServiceType = "3";

                    // Se llama al m�todo ABM del DbContext para actualizar el paquete.
                    await _dbContext.AbmPackagePromotionAsync(promotion, "UPDATE");
                    TempData["SuccessMessage"] = "�Promoci�n de paquete actualizada exitosamente!";
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

            ViewData["Title"] = "Editar Promoci�n de Paquete";
            return View("AltaPromotionPackage", promotion);
        }

        // M�todo POST para la eliminaci�n de una promoci�n de paquete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePromotionPackage(int id)
        {
            var promotion = await _dbContext.PackagePromotions.FindAsync(id);
            if (promotion == null)
            {
                return NotFound();
            }

            // Se llama al m�todo ABM del DbContext para eliminar el paquete.
            await _dbContext.AbmPackagePromotionAsync(promotion, "DELETE");
            TempData["SuccessMessage"] = "�Promoci�n de paquete eliminada exitosamente!";
            return RedirectToAction(nameof(AdminPromotionPackages));
        }
    

    ///////////// NUEVAS ACCIONES PARA LA GESTI�N DE ENCOMIENDAS  ////////////////////////////////////////////

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
        // Se a�ade IFormFile para recibir el archivo subido
        public async Task<IActionResult> AltaEncomienda([Bind("Id,CompanyName,CompanyUrl")] EncomiendaCompany encomienda, IFormFile ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    // La carpeta donde se guardar�n las im�genes. Aseg�rate de que exista.
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/encomiendas");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Genera un nombre de archivo �nico para evitar colisiones
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
    public async Task<IActionResult> EditEncomienda(int id, [Bind("Id,Name,ImageUrl,Phone,Email,Address")] EncomiendaCompany company)
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
                TempData["SuccessMessage"] = "�Empresa de encomiendas actualizada exitosamente!";
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
        TempData["SuccessMessage"] = "�Empresa de encomiendas eliminada exitosamente!";
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
        public IActionResult AltaActividad()
        {
            ViewData["Title"] = "Alta de Actividades";
            return View("AltaActividad", new ActivitiesPromotion());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AltaActividad([Bind("Id,Title,Description,Location,ImageUrl")] ActivitiesPromotion Actividad, IFormFile ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img/Actividades");
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

                _dbContext.Add(Actividad);

                await _dbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Empresa de encomienda creada exitosamente!";
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
            return View("AltaActividad", Actividad);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Se modifica el m�todo de edici�n para aceptar el IFormFile.
        public async Task<IActionResult> EditActividad(int id, [Bind("Id,Title,Description,Location,ImageUrl")] ActivitiesPromotion Actividad, IFormFile ImageFile)
        {
            if (id != Actividad.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Si se subi� una nueva imagen, se guarda y se actualiza la URL.
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        // L�gica para guardar la nueva imagen (similar a AltaActividad).
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img/Actividades");
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

                        // Se borra la imagen anterior si exist�a para no dejar archivos hu�rfanos.
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

                    // Si no se subi� una nueva imagen, se mantiene la URL existente.
                    // Esto se maneja autom�ticamente ya que el campo ImageUrl se incluye en el bind y el campo hidden.
                    _dbContext.Update(Actividad);
                    await _dbContext.SaveChangesAsync();
                    TempData["SuccessMessage"] = "�Empresa de encomiendas actualizada exitosamente!";
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

            // Tambi�n se borra la imagen del servidor al eliminar el registro.
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
            TempData["SuccessMessage"] = "�Actividad eliminada exitosamente!";
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
                    // Llama al m�todo del DbContext con el modelo directamente
                    // La propiedad 'Id' no es necesaria para la inserci�n, ya que es auto-incremental.
                    await _dbContext.CreateWhatsappMessageAsync(
                        model.Title,
                        model.Message_Template,
                        model.Is_Active
                    );

                    return RedirectToAction("AdminWhatsappMessages");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Ocurri� un error al guardar el mensaje: " + ex.Message);
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
                // Llama al m�todo del DbContext para obtener la lista de mensajes
                var messages = await _dbContext.GetAllWhatsappMessagesAsync();
                return View(messages);
            }
            catch (Exception ex)
            {
                // Manejar errores si la llamada al SP falla
                // Considera usar un logger en un proyecto real
                TempData["ErrorMessage"] = "Ocurri� un error al cargar los mensajes: " + ex.Message;
                // Retorna una lista vac�a para evitar errores en la vista
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
                    // Usamos el mismo m�todo de "upsert" que creamos
                    await _dbContext.UpdateWhatsappMessageAsync(model);

                    TempData["SuccessMessage"] = "Mensaje de WhatsApp actualizado exitosamente.";
                    return RedirectToAction("AdminWhatsappMessages");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Ocurri� un error al actualizar el mensaje: " + ex.Message);
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

                // 3. Actualizar el registro en la base de datos usando el SP de actualizaci�n.
                await _dbContext.UpdateWhatsappMessageAsync(message);

                TempData["SuccessMessage"] = "Estado del mensaje actualizado exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ocurri� un error al actualizar el estado: " + ex.Message;
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

        // Acci�n para mostrar el formulario de creaci�n
        [HttpGet]
        public IActionResult CreateCarrouselItem()
        {
            return View();
        }

        // Acci�n para procesar el formulario de creaci�n
        [HttpPost]
        public async Task<IActionResult> CreateCarrouselItem(View_Index_DestinationCarouselItem model, IFormFile imageFile)
        {
            if (ModelState.IsValid && imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img/");
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

        // Acci�n para mostrar la lista de elementos del carrusel
        [HttpGet]
        public async Task<IActionResult> AdminCarrouselItem()
        {
            var items = await _dbContext.View_DestinationCarouselItems.ToListAsync();
            return View(items);
        }

        // Acci�n para mostrar el formulario de creaci�n de destinos
        [HttpGet]
        public IActionResult CreateDestination()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateDestination(View_Index_Destination model, IFormFile imageFile)
        {
            // Verificar si se subi� un archivo de imagen.
            if (imageFile == null || imageFile.Length == 0)
            {
                ModelState.AddModelError("PictureDestiny", "La imagen de destino es obligatoria.");
            }

            // Si hay errores de validaci�n, regresar a la vista.
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // L�gica para guardar la imagen
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img/");

            // Aseg�rate de que la carpeta de destino exista
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
            model.DetailDestinyURL = "/destinations/" + model.Title; // O la l�gica que uses para generar esta URL

            // Guardar los cambios en la base de datos.
            _dbContext.Destinations.Add(model);
            await _dbContext.SaveChangesAsync();

            // Redireccionar al usuario.
            return RedirectToAction("AdminDestination");
        }

        // Acci�n para mostrar la lista de destinos/promociones
        [HttpGet]
        public async Task<IActionResult> AdminDestination()
        {
            var items = await _dbContext.Destinations.ToListAsync();
            return View(items);
        }


        // GET: Muestra el formulario de edici�n con los datos cargados del carrusel
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

        // POST: Procesa los cambios del formulario de edici�n del carrusel
        [HttpPost]
        public async Task<IActionResult> EditCarrouselItem(View_Index_DestinationCarouselItem model, IFormFile imageFile)
        {
            // Carga el elemento existente del DbContext
            var existingItem = await _dbContext.View_DestinationCarouselItems.FindAsync(model.Id);
            if (existingItem == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Si se subi� una nueva imagen, la guardamos y actualizamos la URL
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

                // Actualiza los dem�s campos
                existingItem.Title = model.Title;
                existingItem.AltText = model.AltText;
                existingItem.LinkUrl = model.LinkUrl;
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

        // POST: Procesa los cambios del formulario de edici�n de destino
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
                // Si se subi� una nueva imagen, la guardamos y actualizamos la URL
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

                // Actualiza los dem�s campos
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
            // pero tu modelo original s� la tiene. Si la usas en la tabla, la puedes agregar aqu�.
            // item.IsActive = !item.IsActive; // Descomentar si la propiedad existe.

            item.IsHotWeek = !item.IsHotWeek; // Usaremos esta propiedad como ejemplo para activar/desactivar
            _dbContext.Entry(item).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("AdminDestination");
        }
    }
}