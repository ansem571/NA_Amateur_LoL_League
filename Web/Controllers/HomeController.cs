using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
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
