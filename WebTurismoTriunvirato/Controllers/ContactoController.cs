using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web_Turismo_Triunvirato.DataAccess;
using Web_Turismo_Triunvirato.Models.ViewModels;
using Web_Turismo_Triunvirato.Services;

namespace Web_Turismo_Triunvirato.Controllers
{
    public class ContactoController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IEmailService _emailService;
        private readonly string _agenciaEmail = "correo_de_tu_agencia@tudominio.com"; // Email destino

        public ContactoController(ApplicationDbContext dbContext, IEmailService emailService)
        {
            _dbContext = dbContext;
            _emailService = emailService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FormularioConsulta(ConsultaMailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var subject = $"Nueva Reserva de Paquete | ID: {model.DestinyId} | {model.Name}";

                // --- 1. CONSTRUCCIÓN DINÁMICA DE PASAJEROS ---
                string pasajerosHtml = "";
                if (model.Pasajeros != null && model.Pasajeros.Count > 0)
                {
                    pasajerosHtml = "\n--- DATOS DE LOS PASAJEROS ---";
                    for (int i = 0; i < model.Pasajeros.Count; i++)
                    {
                        pasajerosHtml += $"\nPasajero #{i + 1}: {model.Pasajeros[i].NombreCompleto} - DNI: {model.Pasajeros[i].Dni}";
                    }
                }

                // --- 2. CUERPO DEL MAIL ---
                var body = $@"
        Nueva solicitud de reserva recibida.

        --- CONTACTO PRINCIPAL ---
        Nombre: {model.Name}
        Email: {model.Email}
        Teléfono: {model.Phone}
        {pasajerosHtml}

        --- MENSAJE ADICIONAL ---
        {model.Message}

        --- DETALLES TÉCNICOS DE LA SOLICITUD ---
        {model.PackageDetailsSerialized}
        ";

                try
                {
                    await _emailService.SendEmailAsync(_agenciaEmail, subject, body);

                    return Json(new { success = true, message = "Reserva enviada con éxito. Un agente te contactará pronto." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Error al enviar el correo. Por favor, intenta por WhatsApp." });
                }
            }

            // Manejo de errores de validación...
            var errorMessage = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
            return Json(new { success = false, message = errorMessage ?? "Revisa los datos ingresados." });
        }
    }
}
