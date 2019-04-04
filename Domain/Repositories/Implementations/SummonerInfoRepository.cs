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
            return await _table.ReadManyAsync("IsValidPlayer = 1");
        }

        public async Task<IEnumerable<SummonerInfoEntity>> GetAllForSummonerIdsAsync(IEnumerable<Guid> summonerIds)
        {
            return await _table.ReadManyAsync("Id in @summonerIds", new {summonerIds});
        }

        public async Task<bool> InsertAsync(SummonerInfoEntity entity)
        {
            return await _table.InsertAsync(entity) == 1;
        }

        public async Task<bool> UpdateAsync(SummonerInfoEntity entity)
        {
            return await _table.UpdateAsync(entity);
        }
    }
}
