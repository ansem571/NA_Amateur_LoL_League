using System;
using System.Collections.Generic;
using System.Text;
using Domain.Enums;

namespace Domain.Views
{
    public class UpdateRosterLineupView
    {
        public Guid RosterId { get; set; }
        public Dictionary<Guid, SummonerRoleEnum> Lineup { get; set; }
    }
}
