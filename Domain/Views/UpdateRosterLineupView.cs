using System;
using System.Collections.Generic;
using System.Text;
using Domain.Enums;

namespace Domain.Views
{
    public class UpdateRosterLineupView
    {
        public List<PlayerTeamRoleView> Lineup { get; set; }
    }

    public class PlayerTeamRoleView
    {
        public Guid SummonerInfoId { get; set; }
        public SummonerRoleEnum Role { get; set; }
    }
}
