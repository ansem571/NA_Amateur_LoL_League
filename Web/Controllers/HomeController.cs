using System;
using System.Diagnostics;
using System.Threading.Tasks;
using DAL.Entities.UserData;
using Domain.Repositories.Interfaces;
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
        private readonly IBlacklistRepository _blacklistRepository;
        private readonly SignInManager<UserEntity> _signInManager;

        public HomeController(UserManager<UserEntity> userManager, IAccountService accountService, IBlacklistRepository blacklistRepository, SignInManager<UserEntity> signInManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _blacklistRepository = blacklistRepository ?? throw new ArgumentNullException(nameof(blacklistRepository));
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> NewHome()
        {
            var seasonInfoTask = _accountService.GetSeasonInfoAsync();
            var userTask = _userManager.GetUserAsync(User);
            var model = new IndexViewModel();

            var user = await userTask;
            if (user != null)
            {
                var blacklisted = await _blacklistRepository.GetByUserIdAsync(user.Id);
                if (blacklisted != null && blacklisted.IsBanned)
                {
                    await _signInManager.SignOutAsync();
                    return RedirectToAction("InvalidUser", "Manage");
                }
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

        public async Task<IActionResult> Index()
        {
            var seasonInfoTask = _accountService.GetSeasonInfoAsync();
            var userTask = _userManager.GetUserAsync(User);
            var model = new IndexViewModel();

            var user = await userTask;
            if (user != null)
            {
                var blacklisted = await _blacklistRepository.GetByUserIdAsync(user.Id);
                if (blacklisted != null && blacklisted.IsBanned)
                {
                    await _signInManager.SignOutAsync();
                    return RedirectToAction("InvalidUser", "Manage");
                }
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
