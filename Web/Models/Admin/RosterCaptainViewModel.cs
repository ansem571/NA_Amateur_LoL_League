using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities.LeagueInfo;
using Domain.Views;

namespace Web.Models.Admin
{
    public class RosterCaptainViewModel
    {
        public IEnumerable<RosterView> Rosters { get; set; }
        public TeamCaptainView Captain { get; set; }
        public string StatusMessage { get; set; }
    }
}
