using System.Threading.Tasks;

namespace ClickHealthBackend.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}