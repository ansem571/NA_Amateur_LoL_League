using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domain.Views
{
    public class SeasonInfoView
    {
        public SeasonInfoViewPartial SeasonInfo { get; set; }
        public IEnumerable<RosterView> Rosters { get; set; }      
        public string StatusMessage { get; set; }
    }

    public class SeasonInfoViewPartial
    {
        public string SeasonName { get; set; }
        public DateTime ClosedRegistrationDate { get; set; }
        public DateTime SeasonStartDate { get; set; }
    }
}
