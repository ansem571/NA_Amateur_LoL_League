using Domain.Views;
using System.Collections.Generic;
using Web.Models.ManageViewModels;

namespace Web.Models.Summoner
{
    public class AccountSummonerJointModel
    {
        public IndexViewModel ProfileInfo { get; set; }
        public SummonerInfoView SummonerInfo { get; set; }
        public IEnumerable<string> DiscordNames { get; set; }
        public string ReferedByDiscordHandle { get; set; }

        public string DefaultReferedByDiscordHandle => "Select User by their Discord Handle";


    }
}
