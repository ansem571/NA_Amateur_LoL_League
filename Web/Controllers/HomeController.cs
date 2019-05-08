﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using DAL.Entities.UserData;
using Domain.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Web.Models.Home;
using Web.Models.ManageViewModels;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly IAccountService _accountService;

        public HomeController(UserManager<UserEntity> userManager, IAccountService accountService)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> Index()
        {
            var seasonInfoTask = _accountService.GetSeasonInfoAsync();
            var userTask = _userManager.GetUserAsync(User);
            var model = new IndexViewModel();

            var user = await userTask;
            if (user != null)
            {
                model = new IndexViewModel
                {
                    Username = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    IsEmailConfirmed = user.ConfirmedEmail,
                    StatusMessage = StatusMessage
                };
            }
            var seasonInfo = await seasonInfoTask;

            var viewModel = new HomeViewModel
            {
                IndexViewModel = model,
                SeasonInfo = seasonInfo
            };
            return View(viewModel);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Mission statement";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Contact information" +
                                  "\r Please attempt to contact a member of the Tribunal before sending an email to support. " +
                                  "\r If it is an issue regarding payment, go ahead and contact support.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
