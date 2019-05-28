using System;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities.UserData;
using Domain.Services.Interfaces;
using Domain.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Web.Models.Roster;

namespace Web.Controllers
{
    public class RosterController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IRosterService _rosterService;
        private readonly UserManager<UserEntity> _userManager;
        private readonly ILogger _logger;
        private readonly IScheduleService _scheduleService;
        private readonly IGoogleDriveService _googleDriveService;

        public RosterController(IAccountService accountService, IRosterService rosterService, UserManager<UserEntity> userManager,
            ILogger logger, IScheduleService scheduleService, IGoogleDriveService googleDriveService)
        {
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
            _rosterService = rosterService ?? throw new ArgumentNullException(nameof(rosterService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _scheduleService = scheduleService ?? throw new ArgumentNullException(nameof(scheduleService));
            _googleDriveService = googleDriveService ?? throw new ArgumentNullException(nameof(googleDriveService));
        }

        [TempData]
        public string StatusMessage { get; set; }

        //Views all summoners
        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewBag.SummonerName = string.IsNullOrEmpty(sortOrder) ? "summoner_desc" : "";
            ViewBag.Role = sortOrder == "Role" ? "role_desc" : "Role";
            ViewBag.TierDivision = sortOrder == "TierDivision" ? "tierDivision_desc" : "TierDivision";
            ViewBag.TeamName = sortOrder == "TeamName" ? "teamName_desc" : "TeamName";
            ViewBag.ESub = sortOrder == "ESub" ? "ESub_desc" : "ESub";

            var model = await _accountService.GetFpSummonerView();

            switch (sortOrder)
            {
                case "summoner_desc":
                    {
                        model.SummonerInfos = model.SummonerInfos.OrderByDescending(x => x.SummonerName).ToList();
                        break;
                    }
                case "role_desc":
                    {
                        model.SummonerInfos = model.SummonerInfos.OrderByDescending(x => x.Role).ThenBy(x => x.OffRole).ToList();
                        break;
                    }
                case "Role":
                    {
                        model.SummonerInfos = model.SummonerInfos.OrderBy(x => x.Role).ThenBy(x => x.OffRole).ToList();
                        break;
                    }
                case "tierDivision_desc":
                    {
                        model.SummonerInfos = model.SummonerInfos.OrderByDescending(x => x.TierDivision).ToList();
                        break;
                    }
                case "TierDivision":
                    {
                        model.SummonerInfos = model.SummonerInfos.OrderBy(x => x.TierDivision).ToList();
                        break;
                    }
                case "teamName_desc":
                    {
                        model.SummonerInfos = model.SummonerInfos.OrderByDescending(x => x.TeamName).ToList();
                        break;
                    }
                case "TeamName":
                    {
                        model.SummonerInfos = model.SummonerInfos.OrderBy(x => x.TeamName).ToList();
                        break;
                    }
                case "ESub":
                    {
                        model.SummonerInfos = model.SummonerInfos.OrderBy(x => x.IsEsubOnly).ToList();
                        break;
                    }
                case "ESub_desc":
                    {
                        model.SummonerInfos = model.SummonerInfos.OrderByDescending(x => x.IsEsubOnly).ToList();
                        break;
                    }
                default:
                    model.SummonerInfos = model.SummonerInfos.OrderBy(x => x.SummonerName).ToList();
                    break;
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> TeamCreationViewAsync()
        {
            var model = await _accountService.GetRequestedPlayersAsync();

            return View("TeamCreationView", model);
        }

        [HttpGet]
        public async Task<IActionResult> ViewAllRostersAsync()
        {
            var model = await _rosterService.GetSeasonInfoView();
            model.StatusMessage = StatusMessage;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ViewRosterAsync(Guid rosterId)
        {
            var rosterTask = _rosterService.GetRosterAsync(rosterId);
            var userTask = _userManager.GetUserAsync(User);
            var scheduleTask = _scheduleService.GetTeamSchedule(rosterId);

            var roster = await rosterTask;
            var user = await userTask;
            var viewModel = new RosterViewModel
            {
                RosterView = roster,
                ScheduleLineup = await scheduleTask
            };
            if (user == null)
            {
                return View(viewModel);
            }

            var summoner = await _accountService.GetSummonerViewAsync(user);
            viewModel.IsCaptain = summoner.SummonerName == roster.Captain;
            viewModel.StatusMessage = StatusMessage;
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTeamNameAsync(string newTeamName, Guid rosterId)
        {
            try
            {

                var result = await _rosterService.UpdateTeamNameAsync(newTeamName, rosterId);

                if (result)
                {
                    return RedirectToAction("ViewRosterAsync", new { rosterId });
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating Team Name");
            }

            StatusMessage = "Error updating Team Name";
            return RedirectToAction("ViewRosterAsync", new { rosterId });
        }

        [HttpPost]
        public async Task<IActionResult> UploadLogoAsync(IFormFile file, Guid rosterId)
        {
            try
            {

                var result = await _rosterService.SaveFileAsync(file, rosterId);

                if (result.result)
                {
                    return RedirectToAction("ViewRosterAsync", new { rosterId });
                }

                throw new Exception();
            }
            catch (Exception e)
            {
                StatusMessage = "Error uploading file";
                _logger.LogError(e, StatusMessage);
            }

            return RedirectToAction("ViewRosterAsync", new { rosterId });
        }

        [HttpPost]
        public async Task<IActionResult> SetPlayerAsSubAsync(string summonerName, Guid rosterId)
        {
            if (!ModelState.IsValid)
            {
                StatusMessage = "Contact Ansem571 about setting player as sub";
                return RedirectToAction("ViewRosterAsync", new { rosterId });
            }
            try
            {
                var result = await _rosterService.SetPlayerAsSubAsync(summonerName, rosterId);
                if (result)
                {
                    return RedirectToAction("ViewRosterAsync", new { rosterId });
                }

                throw new Exception();
            }
            catch (Exception e)
            {
                StatusMessage = $"Failed to set {summonerName} as sub";
                _logger.LogError(e, StatusMessage);
            }

            return RedirectToAction("ViewRosterAsync", new { rosterId });
        }

        [HttpGet]
        public IActionResult SendMatchDataAsync(int weekNumber, string hometeam, string awayteam)
        {
            var view = new MatchSubmissionView();
            view.Week = $"Week {weekNumber}";
            view.HomeTeamName = hometeam;
            view.AwayTeamName = awayteam;
            return View(view);
        }

        [HttpPost]
        public async Task<IActionResult> SendMatchDataAsync(MatchSubmissionView view)
        {
            try
            {
                view.GameInfos = view.GameInfos.Where(x => x.GamePlayed).ToList();
                if (!view.GameInfos.Any() || view.GameInfos.Count < 2)
                {
                    throw new Exception("View was not setup right");
                }

                var result = await _googleDriveService.SendFileData(view);

                if (!result)
                {
                    throw new Exception($"Failed to send match data for: {view.FileName}");
                }

                StatusMessage = "Successfully submitted match data. Stats will be reflected soon.";
            }
            catch (Exception e)
            {
                StatusMessage = e.Message;
                _logger.LogError(e, StatusMessage);
            }

            view.StatusMessage = StatusMessage;
            return View(view);
        }

        public async Task<IActionResult> StandingsAsync()
        {
            try
            {
                var standingsSortedRoster = await _scheduleService.SetupStandings();

                return View(standingsSortedRoster);
            }
            catch (Exception e)
            {
                StatusMessage = e.Message;
                _logger.LogError(e, e.Message);
                return RedirectToAction("ViewAllRostersAsync");
            }
        }
    }
}