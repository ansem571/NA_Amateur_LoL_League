using System;
using System.Diagnostics;
using System.Threading.Tasks;
using DAL.Entities.UserData;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Web.Models.ManageViewModels;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<UserEntity> _userManager;

        public HomeController(UserManager<UserEntity> userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var model = new IndexViewModel();

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
            return View(model);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Mission statement";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Contact information" +
                                  "\r Please attempt to contact a member of the Court before sending an email to support. " +
                                  "\r If it is an issue regarding payment, go ahead and contact support.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
