namespace Web_Turismo_Triunvirato.Services
{
    public interface IEmailService
    {
        // M�todo para enviar correos as�ncronamente
        Task SendEmailAsync(string toEmail, string subject, string body, string fromEmail = null);
    }
}
