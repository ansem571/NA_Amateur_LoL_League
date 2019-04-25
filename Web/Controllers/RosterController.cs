using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Services.Interfaces;
using Domain.Views;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class RosterController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IRosterService _rosterService;

        public RosterController(IAccountService accountService, IRosterService rosterService)
        {
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
            _rosterService = rosterService ?? throw new ArgumentNullException(nameof(rosterService));
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
    }
}