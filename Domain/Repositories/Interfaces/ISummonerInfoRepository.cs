using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities.LeagueInfo;

namespace Domain.Repositories.Interfaces
{
    public interface ISummonerInfoRepository
    {
        Task<SummonerInfoEntity> ReadOneBySummonerIdAsync(Guid summonerId);
        Task<SummonerInfoEntity> ReadOneByUserIdAsync(Guid userId);
        Task<bool> InsertAsync(SummonerInfoEntity entity);
    }
}
