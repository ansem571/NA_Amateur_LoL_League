using System;
using System.Threading.Tasks;
using Domain.Services.Interfaces;
using Domain.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Web.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly ILogger _logger;
        private readonly IScheduleService _scheduleService;
        private readonly IRosterService _rosterService;

        public ScheduleController(ILogger logger, IScheduleService scheduleService, IRosterService rosterService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _scheduleService = scheduleService ?? throw new ArgumentNullException(nameof(scheduleService));
            _rosterService = rosterService ?? throw new ArgumentNullException(nameof(rosterService));
        }

        public async Task<IActionResult> Index()
        {
            var schedules = await _scheduleService.GetAllSchedules();

            return View(schedules);
        }

        public async Task<IActionResult> CreateScheduleAsync()
        {
            try
            {
                var result = await _scheduleService.CreateFullScheduleAsync();
                if (result)
                {
                    return RedirectToAction("Index");
                }
                throw new Exception("come on man");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "ERROR");
            }

            return RedirectToAction("Index");
        }       
        
        public async Task<IActionResult> UpdateMatchScores(Guid scheduleId, string homeTeam, int homeScore, string awayTeam, int awayScore)
        {
            try
            {
                var view = new ScheduleView
                {
                    ScheduleId = scheduleId,
                    HomeTeamScore = homeScore,
                    AwayTeamScore = awayScore
                };
                var homeRosterResultTask = _rosterService.AddToTeamScoreAsync(homeTeam, homeScore, awayScore);
                var awayRosterResultTask = _rosterService.AddToTeamScoreAsync(awayTeam, awayScore, homeScore);
                var scheduleResultTask = _scheduleService.UpdateScheduleAsync(view);
                var result = await homeRosterResultTask && await awayRosterResultTask && await scheduleResultTask;
                if (!result)
                {
                    throw new Exception();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error setting match scores for {scheduleId}.");
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AssignPlayTimeAsync(Guid scheduleId, DateTime playTime)
        {
            try
            {
                var view = new ScheduleView
                {
                    ScheduleId = scheduleId,
                    PlayTime = playTime,
                    HomeTeamScore = -1,
                    AwayTeamScore = -1
                };
                var result = await _scheduleService.UpdateScheduleAsync(view);
                if (!result)
                {
                    throw new Exception();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error setting playtime for {scheduleId}.");
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AssignCastersAsync(Guid scheduleId, string names)
        {
            try
            {
                var view = new ScheduleView
                {
                    ScheduleId = scheduleId,
                    CasterName = names,
                    HomeTeamScore = -1,
                    AwayTeamScore = -1
                };
                var result = await _scheduleService.UpdateScheduleAsync(view);
                if (!result)
                {
                    throw new Exception();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error setting playtime for {scheduleId}.");
            }

            return RedirectToAction("Index");
        }
    }
}
