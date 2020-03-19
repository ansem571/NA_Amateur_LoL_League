using System;
using System.Collections.Generic;
using System.Text;
using Domain.Views;

namespace Domain.Forms
{
    public class PlayoffInputForm
    {
        public List<PlayoffSeedInsertView> Seeds { get; set; }
        public DateTime WeekOf { get; set; }
        public PlayoffFormat BracketFormat { get; set; }
    }
}
