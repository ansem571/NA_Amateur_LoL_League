using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Domain.Services.Interfaces;
using Microsoft.Extensions.Logging;
using SendGrid;

namespace Domain.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly ILogger _logger;

        public EmailService(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SendEmailAsync(string to, string messageBody, string subject)
        {
            const string fromEmail = "ansem571@gmail.com";

            var message = new MailMessage(fromEmail, to, subject, messageBody);
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail, "txlzxjycigiwadwm")
            };
            message.IsBodyHtml = true;

            try
            {
                await client.SendMailAsync(message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error sending message");
            }
        }

        public Task SendEmailConfirmationAsync(string email, string link)
        {
            return SendEmailAsync(email, $"Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>", 
                "Confirm your email");
        }
    }
}
