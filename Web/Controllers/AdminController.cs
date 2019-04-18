using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IAdminService _adminService;
        private readonly ILogger _logger;

        public AdminController(IAccountService accountService, IAdminService adminService, ILogger logger)
        {
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
            _adminService = adminService ?? throw new ArgumentNullException(nameof(adminService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<IActionResult> Index()
        {
            var model = await _adminService.GetSummonersToCreateTeamAsync();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewTeam(IEnumerable<string> names)
        {


            return RedirectToAction("Index");
        }
    }
}