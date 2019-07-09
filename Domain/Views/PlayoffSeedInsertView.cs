using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Views
{
    public class PlayoffSeedInsertView
    {
        public Guid RosterId { get; set; }
        public Guid DivisionId { get; set; }
        public int Seed { get; set; }
        public PlayoffFormat BracketFormat { get; set; }
    }

    public enum PlayoffFormat
    {
        Standard,
        Gauntlet
    }
}
