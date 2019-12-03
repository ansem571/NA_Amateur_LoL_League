using System;
using System.Collections.Generic;
using System.Text;
using Domain.Enums;

namespace Domain.Views
{
    public class UpdateRosterLineupView
    {
        public Guid RosterId { get; set; }
        public Dictionary<Guid, SummonerRoleTuple> Lineup { get; set; }
        public string StatusMessage { get; set; }
    }

    public class SummonerRoleTuple
    {
        public string SummonerName { get; set; }
        public SummonerRoleEnum TeamRole { get; set; }
    }
}
