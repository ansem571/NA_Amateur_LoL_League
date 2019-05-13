using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Web.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly ILogger _logger;
        private readonly IScheduleService _scheduleService;

        public ScheduleController(ILogger logger, IScheduleService scheduleService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _scheduleService = scheduleService ?? throw new ArgumentNullException(nameof(scheduleService));
        }

        public async Task<IActionResult> Index()
        {
            var schedules = await _scheduleService.GetAllSchedules();

            return View(schedules);
        }

        public async Task<IActionResult> CreateScheduleAsync()
        {
            try
            {
                var result = await _scheduleService.CreateFullScheduleAsync();
                if (result)
                {
                    return RedirectToAction("Index");
                }
                throw new Exception("come on man");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "ERROR");
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AssignPlayTimeAsync(Guid scheduleId, DateTime playTime)
        {
            return null;
        }

        public async Task<IActionResult> AssignCastersAsync(Guid scheduleId, string names)
        {
            var splitNames = names.Split(",");
            return null;
        }
    }
}
