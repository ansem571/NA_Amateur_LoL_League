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
    public class AlternateAccountRepository : IAlternateAccountRepository
    {
        private readonly ITableStorageRepository<AlternateAccountEntity> _table;

        public AlternateAccountRepository(ITableStorageRepository<AlternateAccountEntity> table)
        {
            _table = table ?? throw new ArgumentNullException(nameof(table));
        }

        public async Task<IEnumerable<AlternateAccountEntity>> ReadAllForSummonerAsync(Guid summonerId)
        {
            return await _table.ReadManyAsync("SummonerId = @summonerId", new {summonerId});
        }

        public async Task<bool> CreateAsync(IEnumerable<AlternateAccountEntity> entities)
        {
            var entitiesList = entities.ToList();
            if (!entitiesList.Any())
            {
                return true;
            }

            return await _table.InsertAsync(entitiesList) == entitiesList.Count;
        }

        public async Task<bool> UpdateAsync(IEnumerable<AlternateAccountEntity> entities)
        {
            var entitiesList = entities.ToList();
            if (!entitiesList.Any())
            {
                return true;
            }

            return await _table.UpdateAsync(entitiesList);
        }

        public async Task<bool> DeleteAsync(IEnumerable<AlternateAccountEntity> entities)
        {
            var entitiesList = entities.ToList();
            if (!entitiesList.Any())
            {
                return true;
            }

            return await _table.DeleteAsync(entitiesList);
        }
    }
}
