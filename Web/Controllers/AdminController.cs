﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities.UserData;
using Domain.Forms;
using Domain.Repositories.Interfaces;
using Domain.Season3Services.Interfaces;
using Domain.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Web.Models.Admin;

namespace Web.Controllers
{
    [Authorize(Roles = "Admin, Tribunal, Moderator")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly ILogger _logger;
        private readonly IPlayoffService _playoffService;
        private readonly IUserService _userService;
        private readonly ISummonerInfoRepository _summonerInfoRepository;
        private readonly IRosterService _rosterService;
        private readonly UserManager<UserEntity> _userManager;

        public AdminController(IAdminService adminService, ILogger logger, IPlayoffService playoffService,
            IUserService userService, ISummonerInfoRepository summonerInfoRepository, IRosterService rosterService, UserManager<UserEntity> userManager)
        {
            _adminService = adminService ?? throw new ArgumentNullException(nameof(adminService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _playoffService = playoffService ?? throw new ArgumentNullException(nameof(playoffService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _summonerInfoRepository = summonerInfoRepository ?? throw new ArgumentNullException(nameof(summonerInfoRepository));
            _rosterService = rosterService ?? throw new ArgumentNullException(nameof(rosterService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        [TempData]
        public string StatusMessage { get; set; }

        [Authorize(Roles = "Admin, Tribunal, Moderator")]
        public IActionResult Index()
        {
            return View(model: StatusMessage);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteOldLogosAsync(bool allLogos)
        {
            var result = await _rosterService.DeleteOldLogos(allLogos);

            StatusMessage = result ? "Deleted Logos" : "Failed Logo Deletion, look at logs";

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Tribunal")]
        public async Task<IActionResult> CreateTeamAsync()
        {
            var model = await _adminService.GetSummonersToCreateTeamAsync();

            var viewModel = new TeamCreationViewModel
            {
                AllSummoners = model,
                SelectedSummonersJoint = "",
                StatusMessage = StatusMessage,
                TeamTierScore = 0
            };
            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Tribunal")]
        public async Task<IActionResult> CreateTeamAsync(TeamCreationViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    StatusMessage = "ModelState is invalid";
                    return RedirectToAction("CreateTeamAsync");
                }

                var result = await _adminService.CreateNewTeamAsync(viewModel.SelectedSummoners, viewModel.TeamTierScore);

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

        [HttpPost]
        [Authorize(Roles = "Admin, Tribunal")]
        public async Task<IActionResult> AddNewAchievementsAsync()
        {
            var players = await _summonerInfoRepository.GetAllSummonersAsync();

            var viewModel = new AchievementViewModel
            {
                Summoners = players,
                Forms = new List<UserAchievementForm>()
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Tribunal")]
        public async Task<IActionResult> AddNewAchievementsAsync(AchievementViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                StatusMessage = "ModelState is invalid";
                return RedirectToAction("AddNewAchievementsAsync");
            }

            var result = await _adminService.InsertAchievement(viewModel.Forms);

            if (result)
            {
                StatusMessage = "You have added new achievement(s)";
                return RedirectToAction("AddNewAchievementsAsync");
            }

            StatusMessage = "Error adding achievement(s)";
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Tribunal, Moderator")]
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
        [Authorize(Roles = "Admin, Tribunal, Moderator")]
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

        [Authorize(Roles = "Admin, Tribunal")]
        public async Task<IActionResult> UpdateTeamTierScoresAsync()
        {
            try
            {
                await Task.Delay(10);
                StatusMessage = "Fuck you Karen";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                await Task.Delay(10);
                StatusMessage = e.Message;
                _logger.LogError(e, StatusMessage);
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Tribunal")]
        public ViewResult UploadStatsAsync()
        {
            return View(model: StatusMessage);
        }

        [Obsolete]
        [HttpPost]
        [Authorize(Roles = "Admin, Tribunal")]
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

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BanUser()
        {
            var players = await _summonerInfoRepository.GetAllSummonersAsync();

            return View(players);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BanUser(Guid userId)
        {
            try
            {
                var result = await _userService.UpdateBannedUserAsync(userId);
                StatusMessage = result ? "Successfully updated user" : "Failed to update ban status";
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error sending ");
                StatusMessage = "Failed to update ban status";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Tribunal")]
        public async Task<IActionResult> CreatePlayoffSeeds()
        {
            var model = await _rosterService.GetSeasonInfoView();
            var viewModel = new PlayoffInputView
            {
                PlayoffInputForm = new PlayoffInputForm(),
                SeasonInfoView = model
            };
            viewModel.SeasonInfoView.StatusMessage = StatusMessage;
            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Tribunal")]
        public async Task<IActionResult> CreatePlayoffSeeds(PlayoffInputView view)
        {
            try
            {
                var form = view.PlayoffInputForm;
                form.Seeds = form.Seeds.Where(seed => seed.DivisionId != Guid.Empty || seed.RosterId != Guid.Empty).ToList();
                var result = await _playoffService.SetupPlayoffSchedule(form.Seeds, form.WeekOf, form.BracketFormat);
                StatusMessage = result ? "Successfully created playoff seeds" : "Failed to created playoff seeds";

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error sending ");
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GoToSwagger()
        {
            var url = Request.GetDisplayUrl();
            var swaggerUrl = url.Replace("Admin/GoToSwagger", "swagger/index.html");


            return Redirect(swaggerUrl);
        }
    }
}