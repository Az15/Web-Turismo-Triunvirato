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




        //public async Task<IActionResult> Encomiendas()
        //{
        //    var companies = await _dbContext.EncomiendaCompanies.ToListAsync();
        //    return View(companies);
        //}

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

                TempData["SuccessMessage"] = "�Promoci�n de vuelo creada exitosamente!";
                return RedirectToAction(nameof(AdminPromotionFlights));
            }
            ViewData["Title"] = "Alta de Promoci�n de Vuelo";
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
    

    // NUEVAS ACCIONES PARA LA GESTI�N DE ENCOMIENDAS

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


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> AltaEncomienda([Bind("Name,ImageUrl,Phone,Email,Address")] EncomiendaCompany company)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _dbContext.Add(company);
        //        await _dbContext.SaveChangesAsync();
        //        TempData["SuccessMessage"] = "�Empresa de encomiendas agregada exitosamente!";
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




    }


}
