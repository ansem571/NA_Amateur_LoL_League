using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Repositories.Interfaces;
using Domain.Season3Services.Interfaces;
using Domain.Services.Interfaces;
using Domain.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Web.Models.Admin;

namespace Web.ApiControllers
{
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    public class AdminsController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly ILogger _logger;
        private readonly IDivisionService _divisionService;
        private readonly IPlayoffService _playoffService;
        private readonly ISeasonInfoService _seasonInfoService;
        private readonly IUserService _userService;
        private readonly ISummonerInfoRepository _summonerInfoRepository;
        private readonly IRosterService _rosterService;

        public AdminsController(IAdminService adminService, ILogger logger, IDivisionService divisionService, IPlayoffService playoffService, ISeasonInfoService seasonInfoService,
            IUserService userService, ISummonerInfoRepository summonerInfoRepository, IRosterService rosterService)
        {
            _adminService = adminService ?? throw new ArgumentNullException(nameof(adminService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _divisionService = divisionService ?? throw new ArgumentNullException(nameof(divisionService));
            _playoffService = playoffService ?? throw new ArgumentNullException(nameof(playoffService));
            _seasonInfoService = seasonInfoService ?? throw new ArgumentNullException(nameof(seasonInfoService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _summonerInfoRepository = summonerInfoRepository ?? throw new ArgumentNullException(nameof(summonerInfoRepository));
            _rosterService = rosterService ?? throw new ArgumentNullException(nameof(rosterService));
        }

        [HttpGet("GetTeamsToBeCreated")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TeamCreationViewModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetTeamsToBeCreatedAsync()
        {
            try
            {
                var model = await _adminService.GetSummonersToCreateTeamAsync();
                return Ok(model);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("CreateTeam")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateTeamAsync(IEnumerable<Guid> summonerIds)
        {
            await Task.Delay(0);
            throw new NotImplementedException();
        }
    }
}
