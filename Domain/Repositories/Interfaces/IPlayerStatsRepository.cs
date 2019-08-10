using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities.LeagueInfo;

namespace Domain.Repositories.Interfaces
{
    public interface IPlayerStatsRepository
    {
        Task<PlayerStatsEntity> GetStatsForSummonerAsync(Guid summonerId, Guid? seasonInfoId);
        Task<IEnumerable<PlayerStatsEntity>> GetStatsForSummonersAsync(IEnumerable<Guid> summonerIds, Guid? seasonInfoId);
        Task<IEnumerable<PlayerStatsEntity>> GetAllStatsAsync(Guid? seasonInfoId);
        Task<bool> InsertAsync(IEnumerable<PlayerStatsEntity> stats);
        Task<bool> UpdateAsync(IEnumerable<PlayerStatsEntity> stats);
        Task<bool> DeleteAsync(IEnumerable<PlayerStatsEntity> stats);
    }
}
