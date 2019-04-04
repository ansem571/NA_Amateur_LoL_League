using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Entities.LeagueInfo;

namespace Domain.Repositories.Interfaces
{
    public interface ISummonerInfoRepository
    {
        Task<SummonerInfoEntity> ReadOneBySummonerIdAsync(Guid summonerId);
        Task<SummonerInfoEntity> ReadOneByUserIdAsync(Guid userId);
        Task<IEnumerable<SummonerInfoEntity>> GetAllSummonersAsync();
        Task<bool> InsertAsync(SummonerInfoEntity entity);
        Task<bool> UpdateAsync(SummonerInfoEntity entity);
    }
}
