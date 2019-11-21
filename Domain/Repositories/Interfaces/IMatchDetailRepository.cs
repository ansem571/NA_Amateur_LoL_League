using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities.LeagueInfo;
using Domain.Helpers;

namespace Domain.Repositories.Interfaces
{
    public interface IMatchDetailRepository
    {
        Task<Dictionary<MatchDetailKey, MatchDetailEntity>> ReadForScheduleId(Guid scheduleId);
        Task<Dictionary<StatsKey, List<MatchDetailEntity>>> GetMatchDetailsForPlayerAsync(IEnumerable<Guid> summonerIds);

        Task<bool> InsertAsync(IEnumerable<MatchDetailEntity> entities);
        Task<bool> DeleteAsync(IEnumerable<Guid> ids);
    }
}
