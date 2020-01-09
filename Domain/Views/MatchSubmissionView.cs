using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Domain.Enums;
using Domain.Helpers;

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

        public Guid ScheduleId { get; set; }
        public List<string> ValidPlayers { get; set; }
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
        public ChampionsEnum ChampionTop { get; set; }
        [Required]
        public ChampionsEnum ChampionJungle { get; set; }
        [Required]
        public ChampionsEnum ChampionMid { get; set; }
        [Required]
        public ChampionsEnum ChampionAdc { get; set; }
        [Required]
        public ChampionsEnum ChampionSup { get; set; }

        internal string LocalChampionTop => GlobalVariables.ChampionCache.Get<string, string>(ChampionTop.ToString());
        internal string LocalChampionJungle => GlobalVariables.ChampionCache.Get<string, string>(ChampionJungle.ToString());
        internal string LocalChampionMid => GlobalVariables.ChampionCache.Get<string, string>(ChampionMid.ToString());
        internal string LocalChampionAdc => GlobalVariables.ChampionCache.Get<string, string>(ChampionAdc.ToString());
        internal string LocalChampionSupport => GlobalVariables.ChampionCache.Get<string, string>(ChampionSup.ToString());
    }

    public class GameInfoPlayer
    {
        public bool IsBlue { get; }
        public string PlayerName { get; set; }

        public GameInfoPlayer() { }

        public GameInfoPlayer(bool isBlue, string playerName)
        {
            IsBlue = isBlue;
            PlayerName = playerName;
        }
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

        public string BlueMvp { get; set; }
        public string RedMvp { get; set; }

        /// <summary>
        /// Will only be used for games 3,4,5 of series
        /// </summary>
        [Required]
        public bool GamePlayed { get; set; } = true;

        public bool HomeTeamForfeit { get; set; } = false;
        public bool AwayTeamForfeit { get; set; } = false;

        internal GameInfoPlayer PlayerName(string riotChampionName)
        {
            //Blue team check
            if (BlueTeam.LocalChampionTop == riotChampionName)
            {
                return new GameInfoPlayer(true, BlueTeam.PlayerTop);
            }
            if (BlueTeam.LocalChampionJungle == riotChampionName)
            {
                return new GameInfoPlayer(true, BlueTeam.PlayerJungle);
            }
            if (BlueTeam.LocalChampionMid == riotChampionName)
            {
                return new GameInfoPlayer(true, BlueTeam.PlayerMid);
            }
            if (BlueTeam.LocalChampionAdc == riotChampionName)
            {
                return new GameInfoPlayer(true, BlueTeam.PlayerAdc);
            }
            if (BlueTeam.LocalChampionSupport == riotChampionName)
            {
                return new GameInfoPlayer(true, BlueTeam.PlayerSup);
            }

            //Red team check
            if (RedTeam.LocalChampionTop == riotChampionName)
            {
                return new GameInfoPlayer(false, RedTeam.PlayerTop);
            }
            if (RedTeam.LocalChampionJungle == riotChampionName)
            {
                return new GameInfoPlayer(false, RedTeam.PlayerJungle);
            }
            if (RedTeam.LocalChampionMid == riotChampionName)
            {
                return new GameInfoPlayer(false, RedTeam.PlayerMid);
            }
            if (RedTeam.LocalChampionAdc == riotChampionName)
            {
                return new GameInfoPlayer(false, RedTeam.PlayerAdc);
            }
            if (RedTeam.LocalChampionSupport == riotChampionName)
            {
                return new GameInfoPlayer(false, RedTeam.PlayerSup);
            }

            return null;
        }
    }
}
