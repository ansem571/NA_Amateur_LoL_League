using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Web.Models.Admin;

namespace Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly ILogger _logger;
        private readonly IGoogleDriveService _googleDriveService;

        public AdminController(IAdminService adminService, ILogger logger, IGoogleDriveService googleDriveService)
        {
            _adminService = adminService ?? throw new ArgumentNullException(nameof(adminService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _googleDriveService = googleDriveService ?? throw new ArgumentNullException(nameof(googleDriveService));
        }

        [TempData]
        public string StatusMessage { get; set; }

        public IActionResult Index()
        {
            return View(model: StatusMessage);
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

        public async Task<IActionResult> UpdateTeamTierScoresAsync()
        {
            try
            {
                var result = await _adminService.UpdateRosterTierScoreAsync();
                if (!result)
                {
                    throw new Exception("Failed to update roster tier scores");
                }

                StatusMessage = "Updated Official Rosters tier scores";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                StatusMessage = e.Message;
                _logger.LogError(e, StatusMessage);
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> CreateGoogleDriveFoldersAsync()
        {
            try
            {
                _googleDriveService.SetupCredentials();

                var result = await _googleDriveService.CreateFolders();
                if (!result)
                {
                    throw new Exception("Failed to create folders on google drive");
                }

                StatusMessage = "Created Google Drive Folders For Season 0";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                StatusMessage = e.Message;
                _logger.LogError(e, StatusMessage);
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public ViewResult UploadStatsAsync()
        {
            return View(model: StatusMessage);
        }

        [HttpPost]
        public async Task<IActionResult> UploadStatsAsync(IEnumerable<IFormFile> files)
        {
            try
            {
                var result = await _adminService.UploadPlayerStatsAsync(files);
                if (result)
                {
                    StatusMessage = "Successfully uploaded player stats";
                }
                else
                {
                    throw new Exception("Look at your logic again Ryan");
                }
            }
            catch (Exception e)
            {
                StatusMessage = $"Failed to upload player stats, {e.Message}";
            }

            return View(model: StatusMessage);
        }
    }
}