using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Views
{
    public class PlayoffSeedView
    {
        public string TeamName { get; set; }
        public Guid RosterId { get; set; }
        public string DivisionName { get; set; }
        public Guid DivisionId { get; set; }
        public int Seed { get; set; }
        public PlayoffFormat BracketFormat { get; set; }
    }
}
