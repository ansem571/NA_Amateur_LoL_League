using System;

namespace Domain.Forms
{
    public class DivisionForm
    {
        public Guid SeasonInfoId { get; set; }
        public string Name { get; set; }
        public int LowerLimit { get; set; }
        public int UpperLimit { get; set; }
    }
}
