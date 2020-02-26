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
            var matchId = 3267470125;
            var version = (await GlobalVariables.ChampsApi.Versions.GetAllAsync()).First();
            var championsTask = GlobalVariables.ChampsApi.Champions.GetAllAsync(version);
            var riotMatchTask = GlobalVariables.Api.Match.GetMatchAsync(RiotSharp.Misc.Region.Na, matchId);

            var riotMatch = await riotMatchTask;
            var champions = await championsTask;
            var localChampionJungle = new LookupEntity
            {
                Id = Guid.NewGuid(),
                Enum = ChampionsEnum.Nunu.ToString(),
                Category = "Champion",
                Description = "",
                Value = "Nunu & Willump"
            }
            ;

            foreach (var participant in riotMatch.Participants)
            {
                var riotChampion = champions.Keys[participant.ChampionId].ToLowerInvariant();

                if (riotChampion == localChampionJungle.Enum.ToLowerInvariant() ||
                    riotChampion == localChampionJungle.Value.ToLowerInvariant())
                {
                    return;
                }
            }
            Assert.True(false);
        }
    }
}
