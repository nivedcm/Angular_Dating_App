using API.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace API.Services
{
    public class EmailService : IEmailService
    {
        public async Task<Response> SendEmailAsync(SendGridMessage message)
        {
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_APIKEY");
            var client = new SendGridClient(apiKey);
            //var msg = new SendGridMessage()
            //{
            //    From = new EmailAddress("dateradar@dateradar.com", "Date Radar"),
            //    Subject = "Hello World from the SendGrid CSharp SDK!",
            //    PlainTextContent = "Hello, Email!",
            //    HtmlContent = "<strong>Hello, Email!</strong>"
            //};
            message.AddTo(new EmailAddress("dateradar@dateradar.com", "Date Radar"));
            var response = await client.SendEmailAsync(message);
            return response;
        }
    }
}
