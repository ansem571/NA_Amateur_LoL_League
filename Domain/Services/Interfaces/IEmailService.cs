using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string messageBody, string subject, IEnumerable<Attachment> attachments = null, List<string> additionalTos = null);
        Task SendTextAsEmailAsync(string to, string messageBody, string subject, PhoneCarrierEnum carrier);
        Task SendEmailConfirmationAsync(string email, string link);
    }
}
