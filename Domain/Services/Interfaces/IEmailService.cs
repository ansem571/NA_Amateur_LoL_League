using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DAL.Enums;

namespace Domain.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string messageBody, string subject);
        Task SendTextAsEmailAsync(string to, string messageBody, string subject, PhoneCarrierEnum carrier);
        Task SendEmailConfirmationAsync(string email, string link);
    }
}
