using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.Mappers.Interfaces;
using Domain.Repositories.Interfaces;
using Domain.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Domain.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly ILogger _logger;
        private readonly ILookupRepository _lookupRepository;
        private readonly IPhoneMapper _phoneMapper;

        public EmailService(ILogger logger, ILookupRepository lookupRepository, IPhoneMapper phoneMapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _lookupRepository = lookupRepository ?? throw new ArgumentNullException(nameof(lookupRepository));
            _phoneMapper = phoneMapper ?? throw new ArgumentNullException(nameof(phoneMapper));
        }

        public async Task SendEmailAsync(string to, string messageBody, string subject, IEnumerable<Attachment> attachments = null)
        {
            const string fromEmail = "casualesportsamateurleague@gmail.com";

            var message = new MailMessage(fromEmail, to, subject, messageBody);
            if (attachments != null)
            {
                foreach (var attachment in attachments)
                {
                    message.Attachments.Add(attachment);
                }
            }
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail, "minxkypicbyguvzv")
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

        public async Task SendTextAsEmailAsync(string to, string messageBody, string subject, PhoneCarrierEnum carrier)
        {
            const string fromEmail = "ansem571@gmail.com";
            var carrierId = _phoneMapper.MapFromEnum(carrier);
            
            //cant confirm email
            if (carrierId == null)
            {
                _logger.LogInformation($"Carrier does not exist in our lookup table, therefore, we cannot send a confirmation: {carrier.ToString()}");
                return;
            }

            //will have a value at this point
            var lookupEntity = await _lookupRepository.GetLookupEntity(carrierId.Value);
            to += lookupEntity.Value;

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
