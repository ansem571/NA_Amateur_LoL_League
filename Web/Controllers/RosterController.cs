using System;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities.UserData;
using Domain.Services.Interfaces;
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

        public RosterController(IAccountService accountService, IRosterService rosterService, UserManager<UserEntity> userManager, ILogger logger)
        {
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
            _rosterService = rosterService ?? throw new ArgumentNullException(nameof(rosterService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
            var roster = await _rosterService.GetRosterAsync(rosterId);
            var viewModel = new RosterViewModel
            {
                RosterView = roster
            };
            var user = await _userManager.GetUserAsync(User);
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
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error uploading file");
            }

            StatusMessage = "File Not Selected";
            return RedirectToAction("ViewRosterAsync", new { rosterId });
        }
    }
}