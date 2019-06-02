using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.Views
{
    public class MatchSubmissionView
    {
        /// <summary>
        /// regular season or playoffs, will be assigned based on date of match week
        /// </summary>
        public string Week { get; set; }
        [Required]
        public string HomeTeamName { get; set; }
        [Required]
        public string AwayTeamName { get; set; }

        public string FileName => $"{HomeTeamName}-{AwayTeamName}";

        //will be 2 to 5 games based on whether in 
        public List<GameInfo> GameInfos { get; set; }

        public string StatusMessage { get; set; }
    }

    public class TeamInfo
    {
        [Required]
        public string PlayerTop { get; set; }
        [Required]
        public string PlayerJungle { get; set; }
        [Required]
        public string PlayerMid { get; set; }
        [Required]
        public string PlayerAdc { get; set; }
        [Required]
        public string PlayerSup { get; set; }

        [Required]
        public string ChampionTop { get; set; }
        [Required]
        public string ChampionJungle { get; set; }
        [Required]
        public string ChampionMid { get; set; }
        [Required]
        public string ChampionAdc { get; set; }
        [Required]
        public string ChampionSup { get; set; }
    }

    public class GameInfo
    {
        [Required]
        public bool BlueSideWinner { get; set; }
        [Required]
        public string ProdraftSpectateLink { get; set; }
        [Required]
        public string MatchHistoryLink { get; set; }

        /// <summary>
        /// For regular season, will just flip sides for game 2
        /// </summary>
        [Required]
        public string TeamWithSideSelection { get; set; }

        [Required]
        public TeamInfo BlueTeam { get; set; }

        [Required]
        public TeamInfo RedTeam { get; set; }

        /// <summary>
        /// Will only be used for games 3,4,5 of series
        /// </summary>
        [Required]
        public bool GamePlayed { get; set; } = true;

        public bool BlueTeamForfeit { get; set; } = false;
        public bool RedTeamForfeit { get; set; } = false;
    }
}
