using System;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities.UserData;
using Domain.Enums;
using Domain.Helpers;
using Xunit;

namespace TestProject
{
    public class MatchDetailTests
    {

        [Fact]
        public async Task TestNames()
        {
            //https://matchhistory.na.leagueoflegends.com/en/#match-details/NA1/3391075957/44347500?tab=overview
            var matchId = 3391075957;
            var version = (await GlobalVariables.ChampsApi.Versions.GetAllAsync()).First();
            var championsTask = GlobalVariables.ChampsApi.Champions.GetAllAsync(version);
            var riotMatchTask = GlobalVariables.Api.Match.GetMatchAsync(RiotSharp.Misc.Region.Na, matchId);

            var riotMatch = await riotMatchTask;
            var champions = await championsTask;
            var championJungle = ChampionsEnum.Nunu;
            var localChampionJungle = new LookupEntity
            {
                Id = Guid.NewGuid(),
                Enum = ChampionsEnum.Nunu.ToString(),
                Category = "Champion",
                Description = "",
                Value = "Nunu & Willump"
            };
            var localChampionTop = new LookupEntity
            {
                Id = Guid.NewGuid(),
                Enum = ChampionsEnum.MonkeyKing.ToString(),
                Category = "Champion",
                Description = "",
                Value = "Wukong"
            };

            var count = 0;
            foreach (var participant in riotMatch.Participants)
            {
                var riotChampion = champions.Keys[participant.ChampionId].ToLowerInvariant();
                count++;
                if (riotChampion == localChampionTop.Enum.ToLowerInvariant() ||
                    riotChampion == localChampionTop.Value.ToLowerInvariant())
                {
                    return;
                }
            }

            foreach (var ban in riotMatch.Teams.SelectMany(x => x.Bans))
            {
                var riotChampion = champions.Keys[ban.ChampionId].ToLowerInvariant();
                try
                {
                    //var ourChampion = GlobalVariables.ChampionEnumCache.Get<string, LookupEntity>(riotChampion);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error getting banned champion: {riotChampion}");
                }
            }
            Assert.True(false);
        }
    }
}
