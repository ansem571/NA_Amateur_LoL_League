using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities.LeagueInfo;
using Domain.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Web.Models.Admin;

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

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CreateTeamAsync()
        {
            var model = await _adminService.GetSummonersToCreateTeamAsync();

            var viewModel = new TeamCreationViewModel
            {
                AllSummoners = model,
                SelectedSummonersJoint = "",
                StatusMessage = StatusMessage
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeamAsync(TeamCreationViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    StatusMessage = "ModelState is invalid";
                    return RedirectToAction("CreateTeamAsync");
                }

                var result = await _adminService.CreateNewTeamAsync(viewModel.SelectedSummoners);

                if (result)
                {
                    StatusMessage = "You have created a new team";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error on creating team, da fuck Ryan");
            }
            
            StatusMessage = "Error creating team";
            return RedirectToAction("CreateTeamAsync");
        }

        [HttpGet]
        public async Task<IActionResult> CreateCaptainAsync()
        {
            var rosters = await _adminService.GetAllRosters();
            var model = new RosterCaptainViewModel
            {
                Rosters = rosters,
                StatusMessage = StatusMessage
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCaptainAsync(RosterCaptainViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    StatusMessage = "ModelState is invalid";
                    return RedirectToAction("CreateCaptainAsync");
                }

                var result = await _adminService.AssignTeamCaptain(viewModel.Captain);

                if (result)
                {
                    StatusMessage = "You have assigned the new captain";
                    return RedirectToAction("CreateCaptainAsync");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "error on assigning captain, da fuck Ryan");
            }
            
            StatusMessage = "Error assigning captain";
            return RedirectToAction("Index");
        }
    }
}