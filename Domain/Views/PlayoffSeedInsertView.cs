using System;

namespace Domain.Views
{
    public class PlayoffSeedInsertView
    {
        public Guid RosterId { get; set; }
        public Guid DivisionId { get; set; }
        public int Seed { get; set; }
    }

    public enum PlayoffFormat
    {
        Standard,
        Gauntlet
    }
}
