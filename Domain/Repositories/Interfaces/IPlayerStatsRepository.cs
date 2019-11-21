using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities.LeagueInfo;
using Domain.Helpers;

namespace Domain.Repositories.Interfaces
{
    public interface IPlayerStatsRepository
    {
        Task<Dictionary<StatsKey, List<PlayerStatsEntity>>> GetStatsAsync(IEnumerable<StatsKey> keys);

        [Obsolete("Use GetStatsAsync", true)]
        Task<PlayerStatsEntity> GetStatsForSummonerAsync(Guid summonerId, Guid? seasonInfoId);
        [Obsolete("Use GetStatsAsync", true)]
        Task<IEnumerable<PlayerStatsEntity>> GetStatsForSummonersAsync(IEnumerable<Guid> summonerIds, Guid? seasonInfoId);
        [Obsolete("Use GetStatsAsync", true)]
        Task<IEnumerable<PlayerStatsEntity>> GetAllStatsAsync(Guid? seasonInfoId);

        Task<bool> InsertAsync(IEnumerable<PlayerStatsEntity> stats);
        Task<bool> UpdateAsync(IEnumerable<PlayerStatsEntity> stats);
        Task<bool> DeleteAsync(IEnumerable<PlayerStatsEntity> stats);
    }
}
