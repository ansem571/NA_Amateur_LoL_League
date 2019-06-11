using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities.LeagueInfo;

namespace Domain.Repositories.Interfaces
{
    public interface IPlayerStatsRepository
    {
        Task<PlayerStatsEntity> GetStatsForSummonerAsync(Guid summonerId);
        Task<IEnumerable<PlayerStatsEntity>> GetStatsForSummonersAsync(IEnumerable<Guid> summonerIds);
        Task<IEnumerable<PlayerStatsEntity>> GetAllStatsAsync();
        Task<bool> InsertAsync(IEnumerable<PlayerStatsEntity> stats);
        Task<bool> UpdateAsync(IEnumerable<PlayerStatsEntity> stats);
        Task<bool> DeleteAsync(IEnumerable<PlayerStatsEntity> stats);
    }
}
