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
        [ValidateAntiForgeryToken] // Asegúrate de que tu formulario HTML contenga @Html.AntiForgeryToken()
        public async Task<IActionResult> FormularioConsulta(ConsultaMailViewModel model)
        {
            // --- 1. VALIDACIÓN ---
            if (ModelState.IsValid)
            {
                // NOTA IMPORTANTE: Ya NO llamamos a la BD aquí.
                // Asumimos que model.PackageDetailsSerialized contiene toda la información del paquete.

                // --- 2. ARMADO DEL CUERPO DEL CORREO ---
                // El título del correo lo podemos inferir del detalle serializado o usar un valor genérico.
                // Si el detalle incluye el título, lo parseamos. Si no, usamos el Title del ViewModel (si lo tienes).
                var subject = $"Consulta de Nuevo Cliente | ID Paquete: {model.DestinyId}";

                // Construcción del cuerpo del correo usando el detalle del paquete del frontend (para evitar BD)
                var body = $@"
                Consulta de Cliente a través del sitio web.

                --- DATOS DEL CLIENTE ---
                Nombre: {model.Name}
                Email: {model.Email}
                Teléfono/WhatsApp: {model.Phone}

                --- MENSAJE DEL CLIENTE ---
                {model.Message}

                --- DETALLES DEL PAQUETE (Cargados del Frontend) ---
                {model.PackageDetailsSerialized}
                ";

                try
                {
                    // --- 3. USO DEL SERVICIO DE CORREO ---
                    await _emailService.SendEmailAsync(_agenciaEmail, subject, body);

                    // --- 4. DEVOLVER ÉXITO JSON para AJAX ---
                    return Json(new
                    {
                        success = true,
                        message = "¡Consulta enviada con éxito! Te contactaremos pronto."
                    });
                }
                catch (Exception ex)
                {
                    // Manejo de errores de envío de correo
                    // Lo registramos para debugging
                    // _logger.LogError($"Error al enviar correo: {ex.Message}"); 

                    return Json(new
                    {
                        success = false,
                        message = "Ocurrió un error al procesar tu solicitud. Intenta con WhatsApp."
                    });
                }
            }

            // --- 5. MANEJO DE ERRORES DE VALIDACIÓN (ModelState.IsValid == false) ---

            // Obtener el primer mensaje de error de validación para devolver a AJAX
            var errorMessage = ModelState.Values
                                       .SelectMany(v => v.Errors)
                                       .FirstOrDefault()?.ErrorMessage;

            // Si hay un error, lo devolvemos en formato JSON
            return Json(new
            {
                success = false,
                message = errorMessage ?? "Datos inválidos. Por favor, revisa todos los campos."
            });
        }
    }
}
