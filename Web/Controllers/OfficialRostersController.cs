using DAL.Entities.UserData;
using Domain.Repositories.Interfaces;
using Domain.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Controllers
{
    public class OfficialRostersController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IRosterService _rosterService;
        private readonly UserManager<UserEntity> _userManager;
        private readonly ILogger _logger;
        private readonly IScheduleService _scheduleService;
        private readonly IMatchDetailService _matchDetailService;
        private readonly ISummonerInfoRepository _summonerInfoRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IGameInfoService _gameInfoService;

        public OfficialRostersController(IAccountService accountService, IRosterService rosterService, UserManager<UserEntity> userManager,
            ILogger logger, IScheduleService scheduleService, IMatchDetailService matchDetailService,
            ISummonerInfoRepository summonerInfoRepository, IScheduleRepository scheduleRepository,
            IGameInfoService gameInfoService)
        {
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
            _rosterService = rosterService ?? throw new ArgumentNullException(nameof(rosterService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _scheduleService = scheduleService ?? throw new ArgumentNullException(nameof(scheduleService));
            _matchDetailService = matchDetailService ?? throw new ArgumentNullException(nameof(matchDetailService));
            _summonerInfoRepository = summonerInfoRepository ?? throw new ArgumentNullException(nameof(summonerInfoRepository));
            _scheduleRepository = scheduleRepository ?? throw new ArgumentNullException(nameof(scheduleRepository));
            _gameInfoService = gameInfoService ?? throw new ArgumentNullException(nameof(gameInfoService));
        }
        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> Index()
        {
            var model = await _rosterService.GetSeasonInfoView();
            model.StatusMessage = StatusMessage;
            return View(model);
        }
    }
}
