using Domain.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;

namespace Web.Extensions
{
    public class ErrorHandlingFilter : ExceptionFilterAttribute
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<ErrorHandlingFilter> _logger;

        public ErrorHandlingFilter(IEmailService emailService, ILogger<ErrorHandlingFilter> logger)
        {
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override void OnException(ExceptionContext context)
        {
            var route = context.HttpContext.Request.Path;
            var exception = context.Exception;

            var body = $"Route: {route}\r\n" +
                            $"Exception: {exception.Message}\r\n" +
                            $"Inner Exception: {exception.InnerException?.Message}\r\n" +
                            $"Stack Trace: {exception.StackTrace}";
            _logger.LogInformation(body);
            _emailService.SendEmailAsync("casualesportsamateurleague@gmail.com", body, "Error occured").Wait();
        }
    }
}
