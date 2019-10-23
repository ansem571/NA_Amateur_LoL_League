using System;
using System.Collections.Generic;
using System.Text;
using DAL.Entities.LeagueInfo;

namespace Domain.Views
{
    public class AchievementView : AchievementEntity
    {
        public string Season { get; set; }

        public AchievementView(AchievementEntity entity)
        {
            Id = entity.Id;
            UserId = entity.UserId;
            Achievement = entity.Achievement;
            AchievedDate = entity.AchievedDate;
            AchievedTeam = entity.AchievedTeam;
        }
    }
}
