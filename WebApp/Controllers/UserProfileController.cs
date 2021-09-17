using System;
using System.Threading.Tasks;
using Domain.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Web.Controllers
{
    public class UserProfileController: Controller
    {
        private readonly IPlayerProfileService _playerProfileService;
        private readonly ILogger<UserProfileController> _logger;

        public UserProfileController(IPlayerProfileService playerProfileService, ILogger<UserProfileController> logger)
        {
            _playerProfileService = playerProfileService ?? throw new ArgumentNullException(nameof(playerProfileService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<IActionResult> PlayerProfile(Guid userId)
        {
            try
            {
                var view = await _playerProfileService.GetPlayerProfileAsync(userId);
                return View(view);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error on Getting player profile");
                return RedirectToAction(actionName: "Index", controllerName: "Home");
            }
        }
    }
}
