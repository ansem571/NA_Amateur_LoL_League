using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace DAL.Entities.LeagueInfo
{
    [Table("Achievement")]
    public class AchievementEntity
    {
        [ExplicitKey]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Achievement { get; set; }
        public DateTime AchievedDate { get; set; }
        public string AchievedTeam { get; set; }

        public override bool Equals(object obj)
        {
            return obj is AchievementEntity entity &&
                   UserId.Equals(entity.UserId) &&
                   Achievement == entity.Achievement &&
                   AchievedDate == entity.AchievedDate &&
                   AchievedTeam == entity.AchievedTeam;
        }

        public override int GetHashCode()
        {
            var hashCode = -252758580;
            hashCode = hashCode * -1521134295 + EqualityComparer<Guid>.Default.GetHashCode(UserId);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Achievement);
            hashCode = hashCode * -1521134295 + AchievedDate.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(AchievedTeam);
            return hashCode;
        }
    }
}
