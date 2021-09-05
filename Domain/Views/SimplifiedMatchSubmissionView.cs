using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.Views
{
    public class SimplifiedMatchSubmissionView
    {
        public string Week { get; set; }
        [Required]
        public string HomeTeamName { get; set; }
        [Required]
        public string AwayTeamName { get; set; }

        public string FileName => $"{HomeTeamName}-{AwayTeamName}";
        public string StatusMessage { get; set; }
        //will be 2 to 5 games based on whether in 
        public List<GameDetail> GameDetails { get; set; }
        public Guid ScheduleId { get; set; }
        public List<string> ValidPlayers { get; set; }
    }

    public class GameDetail
    {
        [Required]
        public bool BlueSideWinner { get; set; }
        [Required]
        public string TeamOnBlueSide { get; set; }
        [Required]
        public string ProdraftSpectateLink { get; set; }
        [Required]
        public IFormFile PostGameScreenshot { get; set; }
        public string MatchReplayUrl { get; set; }
        public string BlueMvp { get; set; }
        public string RedMvp { get; set; }

        public string HonoraryBlueOppMvp { get; set; }
        public string HonoraryRedOppMvp { get; set; }
        public int GameNum { get; set; }

        /// <summary>
        /// Will only be used for games 3,4,5 of series
        /// </summary>
        [Required]
        public bool GamePlayed { get; set; } = true;

        public bool HomeTeamForfeit { get; set; } = false;
        public bool AwayTeamForfeit { get; set; } = false;
    }
}
