using DAL.Entities.LeagueInfo;
using Domain.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models.Admin
{
    public class AchievementViewModel
    {
        public IEnumerable<SummonerInfoEntity> Summoners { get; set; }
        public List<UserAchievementForm> Forms { get; set; }
    }
}
