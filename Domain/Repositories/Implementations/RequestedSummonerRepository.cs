using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Data.Interfaces;
using DAL.Entities.LeagueInfo;
using Domain.Repositories.Interfaces;

namespace Domain.Repositories.Implementations
{
    public class RequestedSummonerRepository : IRequestedSummonerRepository
    {
        private readonly ITableStorageRepository<SummonerRequestEntity> _table;

        public RequestedSummonerRepository(ITableStorageRepository<SummonerRequestEntity> table)
        {
            _table = table ?? throw new ArgumentNullException(nameof(table));
        }

        public async Task<IEnumerable<SummonerRequestEntity>> ReadAllForSummonerAsync(Guid summonerId)
        {
            var entities = (await _table.ReadManyAsync("SummonerId = @summonerId", new { summonerId })).ToList();
            return entities;
        }

        public async Task<IEnumerable<SummonerRequestEntity>> ReadAllAsync()
        {
            return await _table.ReadAllAsync();
        }

        public async Task<bool> CreateAsync(IEnumerable<SummonerRequestEntity> entities)
        {
            var list = entities.ToList();
            if (!list.Any())
            {
                return true;
            }
            return await _table.InsertAsync(list) == list.Count;
        }

        public async Task<bool> UpdateAsync(IEnumerable<SummonerRequestEntity> entities)
        {
            var list = entities.ToList();
            if (!list.Any())
            {
                return true;
            }

            return await _table.UpdateAsync(list);
        }

        public async Task<bool> DeleteAsync(IEnumerable<SummonerRequestEntity> entities)
        {
            var list = entities.ToList();
            if (!list.Any())
            {
                return true;
            }

            return await _table.DeleteAsync(list);
        }
    }
}
