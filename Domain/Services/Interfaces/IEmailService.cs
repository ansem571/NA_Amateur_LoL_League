using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string messageBody, string subject);
        Task SendEmailConfirmationAsync(string email, string link);
    }
}
