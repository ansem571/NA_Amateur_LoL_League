using System.Collections.Generic;
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
