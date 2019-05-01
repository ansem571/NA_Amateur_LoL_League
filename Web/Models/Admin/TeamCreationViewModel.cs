using System;
using System.Collections.Generic;
using Domain.Views;

namespace Web.Models.Admin
{
    public class TeamCreationViewModel
    {
        public SummonerTeamCreationView AllSummoners { get; set; }
        public string SelectedSummonersJoint { get; set; }
        public IEnumerable<Guid> SelectedSummoners => Setup();
        public string StatusMessage { get; set; }

        public IEnumerable<Guid> Setup()
        {
            var collection = new List<Guid>();
            var split = SelectedSummonersJoint.Split(",");
            foreach (var s in split)
            {
                var attempt = Guid.TryParse(s, out var result);
                if (!attempt)
                {
                    throw new Exception($"{s} could not parse to guid");
                }
                collection.Add(result);
            }

            return collection;
        }
    }
}
