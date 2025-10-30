using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace Web_Turismo_Triunvirato.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body, string fromEmail = null)
        {
            // Obtener credenciales del archivo appsettings.json
            var smtpHost = _config["EmailSettings:SmtpHost"];
            var smtpPort = int.Parse(_config["EmailSettings:SmtpPort"]);
            var smtpUser = _config["EmailSettings:SmtpUser"];
            var smtpPass = _config["EmailSettings:SmtpPass"];
            var defaultFrom = fromEmail ?? _config["EmailSettings:DefaultFromEmail"];

            using (var client = new SmtpClient(smtpHost, smtpPort))
            {
                client.EnableSsl = true; // Casi siempre necesario
                client.Credentials = new NetworkCredential(smtpUser, smtpPass);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(defaultFrom, "Tu Agencia de Viajes"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false // O true, si envías HTML
                };
                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
            }
        }
    }

}
