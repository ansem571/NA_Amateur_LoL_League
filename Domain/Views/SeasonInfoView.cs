using System;
using System.Collections.Generic;

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
        public Guid SeasonInfoId { get; set; }
        public string SeasonName { get; set; }
        public DateTime ClosedRegistrationDate { get; set; }
        public DateTime SeasonStartDate { get; set; }
    }
}
