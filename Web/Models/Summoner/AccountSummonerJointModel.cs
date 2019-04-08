using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Views;
using Web.Models.ManageViewModels;

namespace Web.Models.Summoner
{
    public class AccountSummonerJointModel
    {
        public IndexViewModel ProfileInfo { get; set; }
        public SummonerInfoView SummonerInfo { get; set; }
    }
}
