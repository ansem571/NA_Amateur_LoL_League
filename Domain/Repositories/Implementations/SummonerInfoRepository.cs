using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Data.Interfaces;
using DAL.Entities.LeagueInfo;
using Domain.Repositories.Interfaces;

namespace Domain.Repositories.Implementations
{
    public class SummonerInfoRepository : ISummonerInfoRepository
    {
        private readonly ITableStorageRepository<SummonerInfoEntity> _table;

        public SummonerInfoRepository(ITableStorageRepository<SummonerInfoEntity> table)
        {
            _table = table ?? throw new ArgumentNullException(nameof(table));
        }

        public async Task<SummonerInfoEntity> ReadOneBySummonerIdAsync(Guid summonerId)
        {
            return await _table.ReadOneAsync(summonerId);
        }

        public async Task<SummonerInfoEntity> ReadOneByUserIdAsync(Guid userId)
        {
            return (await _table.ReadManyAsync("UserId = @userId", new {userId})).FirstOrDefault();
        }

        public async Task<IEnumerable<SummonerInfoEntity>> GetAllSummonersAsync()
        {
            return await _table.ReadAllAsync();
        }

        public async Task<IEnumerable<SummonerInfoEntity>> GetAllValidSummonersAsync()
        {
            return await _table.ReadManyAsync("IsValidPlayer = 1 or IsSubOnly = 1");
        }

        public async Task<IEnumerable<SummonerInfoEntity>> GetAllAcademyPlayers()
        {
            return await _table.ReadManyAsync("IsAcademyPlayer = 1");
        }

        public async Task<IEnumerable<SummonerInfoEntity>> GetAllForSummonerIdsAsync(IEnumerable<Guid> summonerIds)
        {
            return await _table.ReadManyAsync("Id in @summonerIds", new {summonerIds});
        }

        public async Task<IEnumerable<SummonerInfoEntity>> GetAllForSummonerNamesAsync(IEnumerable<string> summonerNames)
        {
            return await _table.ReadManyAsync("SummonerName in @summonerNames", new { summonerNames });
        }

        public async Task<bool> InsertAsync(SummonerInfoEntity entity)
        {
            return await _table.InsertAsync(entity) == 1;
        }

        public async Task<bool> UpdateAsync(IEnumerable<SummonerInfoEntity> entities)
        {
            entities = entities.ToList();
            if (!entities.Any())
            {
                return true;
            }
            return await _table.UpdateAsync(entities);
        }

        public async Task<SummonerInfoEntity> GetSummonerByDiscordHandleAsync(string discordHandle)
        {
            return (await _table.ReadManyAsync("DiscordHandle = @discordHandle", new { discordHandle })).FirstOrDefault();
        }
    }
}
