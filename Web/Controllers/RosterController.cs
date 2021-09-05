using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities.UserData;
using Domain.Helpers;
using Domain.Repositories.Interfaces;
using Domain.Services.Interfaces;
using Domain.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RiotSharp;
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
        private readonly IMatchDetailService _matchDetailService;
        private readonly ISummonerInfoRepository _summonerInfoRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IGameInfoService _gameInfoService;

        public RosterController(IAccountService accountService, IRosterService rosterService, UserManager<UserEntity> userManager,
            ILogger logger, IScheduleService scheduleService, IMatchDetailService matchDetailService, 
            ISummonerInfoRepository summonerInfoRepository, IScheduleRepository scheduleRepository,
            IGameInfoService gameInfoService)
        {
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
            _rosterService = rosterService ?? throw new ArgumentNullException(nameof(rosterService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _scheduleService = scheduleService ?? throw new ArgumentNullException(nameof(scheduleService));
            _matchDetailService = matchDetailService ?? throw new ArgumentNullException(nameof(matchDetailService));
            _summonerInfoRepository = summonerInfoRepository ?? throw new ArgumentNullException(nameof(summonerInfoRepository));
            _scheduleRepository = scheduleRepository ?? throw new ArgumentNullException(nameof(scheduleRepository));
            _gameInfoService = gameInfoService ?? throw new ArgumentNullException(nameof(gameInfoService));
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
            ViewBag.AcademyPlayer = sortOrder == "AcademyPlayer" ? "AcademyPlayer_desc" : "AcademyPlayer";

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
                case "AcademyPlayer":
                    {
                        model.SummonerInfos = model.SummonerInfos.OrderBy(x => x.IsAcademyPlayer).ToList();
                        break;
                    }
                case "AcademyPlayer_desc":
                    {
                        model.SummonerInfos = model.SummonerInfos.OrderByDescending(x => x.IsAcademyPlayer).ToList();
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

            var roster = await rosterTask;
            var user = await userTask;
            var viewModel = new RosterViewModel
            {
                RosterView = roster,
                ScheduleLineup = roster.Schedule
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
        public async Task<IActionResult> SendMatchDetailsAsync(int weekNumber, Guid scheduleId)
        {
            var scheduleTask = _scheduleRepository.GetScheduleAsync(scheduleId);
            var schedule = await scheduleTask;
            var matchInfoTask = _gameInfoService.GetGameInfoForMatch(scheduleId);
            var matchInfo = await matchInfoTask;
            var playersList = await _accountService.GetAllValidPlayers(matchInfo.HomeTeam, matchInfo.AwayTeam);
            var view = new SimplifiedMatchSubmissionView
            {
                Week = !schedule.IsPlayoffMatch ? $"Week {weekNumber}" : $"Playoff {weekNumber}",
                HomeTeamName = matchInfo.HomeTeam,
                AwayTeamName = matchInfo.AwayTeam,
                ScheduleId = scheduleId,
                ValidPlayers = playersList,
                GameDetails = matchInfo.GameDetails
            };
            return View(view);
        }

        [HttpPost]
        public async Task<IActionResult> SendMatchDetailsAsync(SimplifiedMatchSubmissionView view)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                view.GameDetails = view.GameDetails.Where(x => x.GamePlayed || x.AwayTeamForfeit || x.HomeTeamForfeit)
                    .ToList();
                var isNullCheck = false;
                if (user == null)
                {
                    throw new Exception("You were logged out unexpectedly. Im sorry.");
                }
                var userPlayer = await _summonerInfoRepository.ReadOneByUserIdAsync(user.Id);
                foreach (var gameInfo in view.GameDetails)
                {
                    isNullCheck = Properties<GameDetail>.HasEmptyProperties(gameInfo);
                    if(isNullCheck)
                    {
                        break;
                    }
                }

                if (!view.GameDetails.Any() || view.GameDetails.Count < 2 || isNullCheck)
                {
                    throw new Exception("Form was not setup right");
                }


                var result = await _matchDetailService.SendFileData(view, userPlayer);

                if (!result)
                {
                    throw new Exception($"Failed to send match data for: {view.FileName}");
                }

                StatusMessage = "Successfully submitted match data. Stats will be reflected soon.";
            }
            catch (RiotSharpException e)
            {
                StatusMessage = $"{e.Message} Note. This is an error with Riot's Api. Please contact Ansem571 to let him know";
                _logger.LogError(e, StatusMessage);
            }
            catch (Exception e)
            {
                StatusMessage = e.Message;
                _logger.LogError(e, StatusMessage);
            }

            var playersList = await _accountService.GetAllValidPlayers(view.HomeTeamName, view.AwayTeamName);
            view.ValidPlayers = playersList;
            view.StatusMessage = StatusMessage;
            return View(view);
        }

        //[HttpGet]
        //public async Task<IActionResult> SendMatchDataAsync(int weekNumber, Guid scheduleId)
        //{
        //    var scheduleTask = _scheduleRepository.GetScheduleAsync(scheduleId);
        //    var schedule = await scheduleTask;
        //    var matchInfoTask = _gameInfoService.GetGameInfoForMatch(scheduleId);
        //    var matchInfo = await matchInfoTask;
        //    var playersList = await _accountService.GetAllValidPlayers(matchInfo.HomeTeam, matchInfo.AwayTeam);
        //    var view = new MatchSubmissionView
        //    {
        //        Week = !schedule.IsPlayoffMatch ? $"Week {weekNumber}" : $"Playoff {weekNumber}",
        //        HomeTeamName = matchInfo.HomeTeam,
        //        AwayTeamName = matchInfo.AwayTeam,
        //        ScheduleId = scheduleId,
        //        ValidPlayers = playersList,
        //        GameInfos = matchInfo.GameInfos
        //    };
        //    return View(view);
        //}

        //[HttpPost]
        //public async Task<IActionResult> SendMatchDataAsync(MatchSubmissionView view)
        //{
        //    try
        //    {
        //        var user = await _userManager.GetUserAsync(User);
        //        view.GameInfos = view.GameInfos.Where(x => x.GamePlayed || x.AwayTeamForfeit || x.HomeTeamForfeit)
        //            .ToList();
        //        var isNullCheck = false;
        //        if (user == null)
        //        {
        //            throw new Exception("You were logged out unexpectedly. Im sorry.");
        //        }
        //        var userPlayer = await _summonerInfoRepository.ReadOneByUserIdAsync(user.Id);
        //        foreach (var gameInfo in view.GameInfos)
        //        {
        //            isNullCheck = Properties<GameInfo>.HasEmptyProperties(gameInfo);
        //            if (isNullCheck)
        //            {
        //                if (gameInfo.HomeTeamForfeit || gameInfo.AwayTeamForfeit)
        //                {
        //                    isNullCheck = false;
        //                    continue;
        //                }

        //                break;
        //            }

        //            isNullCheck = Properties<TeamInfo>.HasEmptyProperties(gameInfo.BlueTeam);
        //            if (isNullCheck)
        //            {
        //                if (gameInfo.HomeTeamForfeit || gameInfo.AwayTeamForfeit)
        //                {
        //                    isNullCheck = false;
        //                    continue;
        //                }

        //                break;
        //            }

        //            isNullCheck = Properties<TeamInfo>.HasEmptyProperties(gameInfo.RedTeam);
        //            if (isNullCheck)
        //            {
        //                if (gameInfo.HomeTeamForfeit || gameInfo.AwayTeamForfeit)
        //                {
        //                    isNullCheck = false;
        //                    continue;
        //                }

        //                break;
        //            }

        //            if (gameInfo.BlueTeam.IsDefault() || gameInfo.RedTeam.IsDefault())
        //            {
        //                throw new Exception("You did not assign a player to their champion");
        //            }
        //        }

        //        if (!view.GameInfos.Any() || view.GameInfos.Count < 2 || isNullCheck)
        //        {
        //            throw new Exception("Form was not setup right");
        //        }


        //        var result = await _googleDriveService.SendFileData(view, userPlayer);

        //        if (!result)
        //        {
        //            throw new Exception($"Failed to send match data for: {view.FileName}");
        //        }

        //        StatusMessage = "Successfully submitted match data. Stats will be reflected soon.";
        //    }
        //    catch (RiotSharpException e)
        //    {
        //        StatusMessage = $"{e.Message} Note. This is an error with Riot's Api. Please contact Ansem571 to let him know";
        //        _logger.LogError(e, StatusMessage);
        //    }
        //    catch (Exception e)
        //    {
        //        StatusMessage = e.Message;
        //        _logger.LogError(e, StatusMessage);
        //    }

        //    var playersList = await _accountService.GetAllValidPlayers(view.HomeTeamName, view.AwayTeamName);
        //    view.ValidPlayers = playersList;
        //    view.StatusMessage = StatusMessage;
        //    return View(view);
        //}

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

        [HttpGet]
        public async Task<IActionResult> UpdateRosterLineup(Guid rosterId)
        {
            var roster = await _rosterService.GetRosterAsync(rosterId);
            var view = new UpdateRosterLineupView
            {
                RosterId = rosterId,
                Lineup = new Dictionary<Guid, SummonerRoleTuple>()
            };
            foreach (var player in roster.Players)
            {
                if (player.Id == Guid.Empty)
                    continue;
                view.Lineup.Add(player.Id, new SummonerRoleTuple
                {
                    SummonerName = player.SummonerName,
                    TeamRole = player.TeamRole
                });
            }
            return View(view);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRosterLineup(UpdateRosterLineupView view)
        {

            try
            {
                var result = await _rosterService.UpdateRosterLineupAsync(view);
                if (!result)
                {
                    throw new Exception("Error updating roster lineup, consult with WebDude");
                }

                StatusMessage = "Successfully updated RosterLineup";
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating RosterLineup");
                StatusMessage = e.Message;
            }

            view.StatusMessage = StatusMessage;
            return View(view);
        }
    }
}