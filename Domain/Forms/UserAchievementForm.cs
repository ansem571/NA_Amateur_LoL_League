using System;

namespace Domain.Forms
{
    public class UserAchievementForm
    {
        public Guid UserId { get; set; }
        public string Achievement { get; set; }
        public DateTime Date { get; set; }
        public string TeamName { get; set; }
        public string AgainstTeam { get; set; }
    }
}
