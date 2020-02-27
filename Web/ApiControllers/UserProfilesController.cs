using System;
using System.Threading.Tasks;
using Domain.Exceptions;
using Domain.Services.Interfaces;
using Domain.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web.ApiControllers
{
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    public class UserProfilesController : Controller
    {
        private readonly IPlayerProfileService _playerProfileService;

        public UserProfilesController(IPlayerProfileService playerProfileService)
        {
            _playerProfileService = playerProfileService ?? throw new ArgumentNullException(nameof(playerProfileService));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PlayerProfileView))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserProfileAsync(Guid userId)
        {
            try
            {
                var result = await _playerProfileService.GetPlayerProfileAsync(userId);
                return Ok(result);
            }
            catch (SummonerInfoException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}