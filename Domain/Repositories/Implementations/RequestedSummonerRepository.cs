using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<IEnumerable<SummonerRequestEntity>> ReadAllForSummonerAsync(Guid summonerId, Guid seasonInfoId)
        {
            var entities = (await _table.ReadManyAsync("SummonerId = @summonerId AND SeasonInfoId = @seasonInfoId", new { summonerId, seasonInfoId })).ToList();
            return entities;
        }

        public async Task<IEnumerable<SummonerRequestEntity>> ReadAllForSeasonAsync(Guid seasonInfoId)
        {
            var entities = (await _table.ReadManyAsync("SeasonInfoId = @seasonInfoId", new { seasonInfoId })).ToList();
            return entities;
        }

        public async Task<bool> CreateAsync(IEnumerable<SummonerRequestEntity> entities, IUnitOfWork uow = null)
        {
            var list = entities.ToList();
            if (!list.Any())
            {
                return true;
            }
            return await _table.InsertAsync(list, uow) == list.Count;
        }

        public async Task<bool> UpdateAsync(IEnumerable<SummonerRequestEntity> entities, IUnitOfWork uow = null)
        {
            var list = entities.ToList();
            if (!list.Any())
            {
                return true;
            }

            return await _table.UpdateAsync(list, uow);
        }

        public async Task<bool> DeleteAsync(IEnumerable<SummonerRequestEntity> entities, IUnitOfWork uow = null)
        {
            var list = entities.ToList();
            if (!list.Any())
            {
                return true;
            }

            return await _table.DeleteAsync(list, uow);
        }
    }
}
