using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
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
            var strBuilder = new StringBuilder();
            strBuilder.AppendFormat($"You are almost done creating your account.", Environment.NewLine);
            strBuilder.AppendFormat($"Please confirm your account by clicking this <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>.", 
                                    Environment.NewLine, Environment.NewLine);
            strBuilder.Append(GetAdditionalText());
            
            return SendEmailAsync(email, strBuilder.ToString(), 
                "Confirm your email");
        }

        public static string GetAdditionalText()
        {
            var strBuilder = new StringBuilder();
            strBuilder.AppendFormat("Once you have confirmed your email. You must input your Summoner Information.<br />" +

                                    "You can do this by using the Navigation bar to the 'My SummonerInfo' tab, or by clicking the 'Update Summoner Info' " +
                                    "button on your 'Profile' page. Please make sure to update this information as best/often as you can. This will be used " +
                                    "to determine which division you will be placed in for competition as well as validating e-sub status.<br /><br />" +


                                    "After inputting your Summoner Information, you may then navigate to the 'Register For Season' tab on the Nav bar.<br />" +

                                    "This is a one time purchase for a player for the upcoming season. You must agree to our rules before making the purchase. " +
                                    "When you click on the payment link, the website will redirect you to Paypal to make the payment. The entry fee for any " +
                                    "season is $10 USD. Once you have made the payment, be sure to click the 'Return to Merchant' button. This will return " +
                                    "you back to our website and auto-validate the player without further actions needed. If you wish to check that you have " +
                                    "been validated. Use the Nav bar to go to the 'Team Creation' tab, if you are there, you are registered for the season.<br />" +

                                    "If you failed to click 'Return to Merchant'. Please contact the WebDude in our discord server. If you need to pay for " +
                                    "additional members, please contact the WebDude in our discord server. If you have any additional questions not related to " +
                                    "the website, please contact any of the Moderators or Tribunal members. We are more than happy to assist you.");

            return strBuilder.ToString();
        }
    }
}
