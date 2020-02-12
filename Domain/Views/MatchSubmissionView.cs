using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DAL.Entities.UserData;
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

        public bool IsDefault()
        {
            var defaultValue = "Home Team Players";
            return PlayerTop.Contains(defaultValue) || PlayerJungle.Contains(defaultValue) || PlayerMid.Contains(defaultValue) ||
                   PlayerAdc.Contains(defaultValue) || PlayerSup.Contains(defaultValue);
        }

        public LookupEntity LocalChampionTop => GlobalVariables.ChampionEnumCache.Get<string, LookupEntity>(ChampionTop.ToString().ToLowerInvariant());
        public LookupEntity LocalChampionJungle => GlobalVariables.ChampionEnumCache.Get<string, LookupEntity>(ChampionJungle.ToString().ToLowerInvariant());
        public LookupEntity LocalChampionMid => GlobalVariables.ChampionEnumCache.Get<string, LookupEntity>(ChampionMid.ToString().ToLowerInvariant());
        public LookupEntity LocalChampionAdc => GlobalVariables.ChampionEnumCache.Get<string, LookupEntity>(ChampionAdc.ToString().ToLowerInvariant());
        public LookupEntity LocalChampionSupport => GlobalVariables.ChampionEnumCache.Get<string, LookupEntity>(ChampionSup.ToString().ToLowerInvariant());
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
        public int GameNum { get; set; }

        /// <summary>
        /// Will only be used for games 3,4,5 of series
        /// </summary>
        [Required]
        public bool GamePlayed { get; set; } = true;

        public bool HomeTeamForfeit { get; set; } = false;
        public bool AwayTeamForfeit { get; set; } = false;

        public GameInfoPlayer PlayerName(string riotChampionName)
        {
            //Blue team check
            if (BlueTeam.LocalChampionTop.Value.ToLowerInvariant() == riotChampionName || BlueTeam.LocalChampionTop.Enum.ToLowerInvariant() == riotChampionName)
            {
                return new GameInfoPlayer(true, BlueTeam.PlayerTop);
            }
            if (BlueTeam.LocalChampionJungle.Value.ToLowerInvariant() == riotChampionName || BlueTeam.LocalChampionJungle.Enum.ToLowerInvariant() == riotChampionName)
            {
                return new GameInfoPlayer(true, BlueTeam.PlayerJungle);
            }
            if (BlueTeam.LocalChampionMid.Value.ToLowerInvariant() == riotChampionName || BlueTeam.LocalChampionMid.Enum.ToLowerInvariant() == riotChampionName)
            {
                return new GameInfoPlayer(true, BlueTeam.PlayerMid);
            }
            if (BlueTeam.LocalChampionAdc.Value.ToLowerInvariant() == riotChampionName || BlueTeam.LocalChampionAdc.Enum.ToLowerInvariant() == riotChampionName)
            {
                return new GameInfoPlayer(true, BlueTeam.PlayerAdc);
            }
            if (BlueTeam.LocalChampionSupport.Value.ToLowerInvariant() == riotChampionName || BlueTeam.LocalChampionSupport.Enum.ToLowerInvariant() == riotChampionName)
            {
                return new GameInfoPlayer(true, BlueTeam.PlayerSup);
            }

            //Red team check
            if (RedTeam.LocalChampionTop.Value.ToLowerInvariant() == riotChampionName || RedTeam.LocalChampionTop.Enum.ToLowerInvariant() == riotChampionName)
            {
                return new GameInfoPlayer(false, RedTeam.PlayerTop);
            }
            if (RedTeam.LocalChampionJungle.Value.ToLowerInvariant() == riotChampionName || RedTeam.LocalChampionJungle.Enum.ToLowerInvariant() == riotChampionName)
            {
                return new GameInfoPlayer(false, RedTeam.PlayerJungle);
            }
            if (RedTeam.LocalChampionMid.Value.ToLowerInvariant() == riotChampionName || RedTeam.LocalChampionMid.Enum.ToLowerInvariant() == riotChampionName)
            {
                return new GameInfoPlayer(false, RedTeam.PlayerMid);
            }
            if (RedTeam.LocalChampionAdc.Value.ToLowerInvariant() == riotChampionName || RedTeam.LocalChampionAdc.Enum.ToLowerInvariant() == riotChampionName)
            {
                return new GameInfoPlayer(false, RedTeam.PlayerAdc);
            }
            if (RedTeam.LocalChampionSupport.Value.ToLowerInvariant() == riotChampionName || RedTeam.LocalChampionSupport.Enum.ToLowerInvariant() == riotChampionName)
            {
                return new GameInfoPlayer(false, RedTeam.PlayerSup);
            }

            return null;
        }
    }
}
