using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IEmailService
    {
        Task<Response> SendEmailAsync(SendGridMessage sendGridMessage);
    }
}
