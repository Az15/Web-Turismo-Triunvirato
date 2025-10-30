namespace Web_Turismo_Triunvirato.Services
{
    public interface IEmailService
    {
        // Método para enviar correos asíncronamente
        Task SendEmailAsync(string toEmail, string subject, string body, string fromEmail = null);
    }
}
