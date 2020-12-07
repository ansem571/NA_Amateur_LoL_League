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
        Task<IEnumerable<SummonerInfoEntity>> GetAllValidSummonersAsync();
        Task<IEnumerable<SummonerInfoEntity>> GetAllForSummonerIdsAsync(IEnumerable<Guid> summonerIds);
        Task<IEnumerable<SummonerInfoEntity>> GetAllForSummonerNamesAsync(IEnumerable<string> summonerNames);
        Task<IEnumerable<SummonerInfoEntity>> GetAllAcademyPlayers();
        Task<SummonerInfoEntity> GetSummonerByDiscordHandleAsync(string discordHandle);
        Task<bool> InsertAsync(SummonerInfoEntity entity);
        Task<bool> UpdateAsync(IEnumerable<SummonerInfoEntity> entities);
    }
}
