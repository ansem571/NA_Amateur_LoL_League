using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities.UserData;
using Domain.Services.Interfaces;
using Domain.Views;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class SummonerController : Controller
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly IAccountService _accountService;

        [TempData]
        public string StatusMessage { get; set; }

        public SummonerController(UserManager<UserEntity> userManager, IAccountService accountService)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                var returnUrl = "http://ceal.gg/Summoner/Index";
                return RedirectToAction("Login", "Account", routeValues: new { returnUrl });
            }         

            var model = await _accountService.GetSummonerViewAsync(user);
            model.StatusMessage = StatusMessage;

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Index(SummonerInfoView model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                var returnUrl = "http://ceal.gg/Summoner/Index";
                return RedirectToAction("Login", "Account", routeValues: new { returnUrl });
            }

            try
            {
                await _accountService.UpdateSummonerInfoAsync(model, user);
            }
            catch (Exception e)
            {
                StatusMessage = e.Message;
                return RedirectToAction(nameof(Index));
            }

            StatusMessage = "Your profile has been updated";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> RequestPlayers()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                var returnUrl = "http://ceal.gg/Summoner/RequestPlayers";
                return RedirectToAction("Login", "Account", routeValues: new { returnUrl });
            }

            var model = await _accountService.GetRequestedSummonersAsync(user);
            model.StatusMessage = StatusMessage;
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RequestPlayers(SummonerRequestView model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                var returnUrl = "http://ceal.gg/Summoner/RequestPlayers";
                return RedirectToAction("Login", "Account", routeValues: new { returnUrl });
            }

            var tempList = new List<RequestedSummoner>();
            foreach (var summoner in model.RequestedSummoners)
            {
                if (!tempList.Select(x => x.SummonerName).Contains(summoner.SummonerName))
                {
                    tempList.Add(summoner);
                }
            }

            model.RequestedSummoners = tempList;

            var result = await _accountService.UpdateSummonerRequestsAsync(user, model);
            if (!result)
            {
                throw new ApplicationException($"Unexpected error occurred updating Summoner Requests for user '{user.Id}'.");
            }

            StatusMessage = "Your requests have been updated";
            return RedirectToAction(nameof(RequestPlayers));
        }
    }
}