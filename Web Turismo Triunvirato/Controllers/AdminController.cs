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

        public IActionResult Encomiendas()
        {
            ViewData["Title"] = "Administración de Encomiendas";
            return View();
        }




        //public async Task<IActionResult> Encomiendas()
        //{
        //    var companies = await _dbContext.EncomiendaCompanies.ToListAsync();
        //    return View(companies);
        //}

        // **************** ACCIONES PARA ADMINISTRACIÓN DE PROMOCIONES DE VUELOS ****************

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
            ViewData["Title"] = "Alta de Promoción de Vuelo";
            return View("AltaPromotionFlight", new FlightPromotion { ServiceType = "0" });
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

            return View("AltaPromotionFlight", promotion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitPromotionFlight([Bind("Id,ServiceType,Description,DestinationName,OriginName,ImageUrl,IsHotWeek,OriginalPrice,OfferPrice,DiscountPercentage,StartDate,EndDate,IsActive,Stars")] FlightPromotion promotion)
        {
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
        public IActionResult AltaPromotionHotel()
        {
            ViewData["Title"] = "Alta de Promoción de Hotel";
            return View("AltaPromotionHotel", new HotelPromotion { ServiceType = "1" });
        }

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
        public IActionResult AltaPromotionBus()
        {
            ViewData["Title"] = "Alta de Promoción de Bus";
            return View("AltaPromotionBus", new BusPromotion { ServiceType = "2" }); // Se asume que tienes un modelo BusPromotion y una vista AltaPromotionBus.cshtml
        }

        // GET: Admin/EditPromotionBus/{id}
        [HttpGet]
        public async Task<IActionResult> EditPromotionBus(int id)
        {
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
                TempData["SuccessMessage"] = "¡Promoción de bus agregada exitosamente!";
                return RedirectToAction(nameof(AdminPromotionBuses));
            }
            ViewData["Title"] = "Alta de Promoción de Bus";
            return View("AltaPromotionBus", promotion);
        }

        // POST: Admin/EditPromotionBus/{id} (Edición)
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
            ViewData["Title"] = "Editar Promoción de Bus";
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
        public async Task<IActionResult> SubmitPromotionPackage([Bind("Id,ServiceType,PackageType,Description,CompanyName," +
                                                                        "DestinationName,OriginName,ImageUrl,IsHotWeek,OriginalPrice,OfferPrice,DiscountPercentage,StartDate,EndDate," +
                                                                        "IsActive,HotelName")] PackagePromotion promotion)
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
        // Se ajusta el Bind para incluir solo los campos que se envían desde la vista.
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

                    // Se llama al método ABM del DbContext para actualizar el paquete.
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

            ViewData["Title"] = "Editar Promoción de Paquete";
            return View("AltaPromotionPackage", promotion);
        }

        // Método POST para la eliminación de una promoción de paquete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePromotionPackage(int id)
        {
            var promotion = await _dbContext.PackagePromotions.FindAsync(id);
            if (promotion == null)
            {
                return NotFound();
            }

            // Se llama al método ABM del DbContext para eliminar el paquete.
            await _dbContext.AbmPackagePromotionAsync(promotion, "DELETE");
            TempData["SuccessMessage"] = "¡Promoción de paquete eliminada exitosamente!";
            return RedirectToAction(nameof(AdminPromotionPackages));
        }
    

    // NUEVAS ACCIONES PARA LA GESTIÓN DE ENCOMIENDAS

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


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> AltaEncomienda([Bind("Name,ImageUrl,Phone,Email,Address")] EncomiendaCompany company)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _dbContext.Add(company);
        //        await _dbContext.SaveChangesAsync();
        //        TempData["SuccessMessage"] = "¡Empresa de encomiendas agregada exitosamente!";
        //        return RedirectToAction(nameof(AdminEncomiendas));
        //    }
        //    ViewData["Title"] = "Alta de Empresa de Encomiendas";
        //    return View("AltaEncomienda", company);
        //}

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




    }


}
