using Domain.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Web.Extensions
{
    public class ErrorHandlingFilter : ExceptionFilterAttribute
    {
        private readonly IEmailService _emailService;

        public ErrorHandlingFilter(IEmailService emailService)
        {
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        public override void OnException(ExceptionContext context)
        {
            var route = context.HttpContext.Request.Path;
            var exception = context.Exception;

            var body = $"Route: {route}\r\n" +
                            $"Exception: {exception.Message}\r\n" +
                            $"Inner Exception: {exception.InnerException?.Message}\r\n" +
                            $"Stack Trace: {exception.StackTrace}";
            _emailService.SendEmailAsync("casualesportsamateurleague@gmail.com", body, "Error occured").Wait();
        }
    }
}
