using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Data.Interfaces;
using DAL.Entities.LeagueInfo;
using Domain.Repositories.Interfaces;

namespace Domain.Repositories.Implementations
{
    public class TeamPlayerRepository : ITeamPlayerRepository
    {
        private readonly ITableStorageRepository<TeamPlayerEntity> _table;

        public TeamPlayerRepository(ITableStorageRepository<TeamPlayerEntity> table)
        {
            _table = table ?? throw new ArgumentNullException(nameof(table));
        }

        public async Task<IEnumerable<TeamPlayerEntity>> ReadAllAsync()
        {
            return await _table.ReadAllAsync();
        }

        public async Task<IEnumerable<TeamPlayerEntity>> ReadAllForRosterAsync(Guid rosterId)
        {
            return await _table.ReadManyAsync("TeamRosterId = @rosterId", new {rosterId});
        }

        public async Task<Guid?> GetRosterIdForExistingGroupAsync(IEnumerable<Guid> summonerIds)
        {
            var player = (await _table.ReadManyAsync("SummonerId in @summonerIds", new {summonerIds})).FirstOrDefault();
            return player?.TeamRosterId;
        }

        public async Task<TeamPlayerEntity> GetBySummonerIdAsync(Guid summonerId)
        {
            return (await _table.ReadManyAsync("SummonerId = @summonerId", new {summonerId})).FirstOrDefault();
        }

        public async Task<bool> InsertAsync(IEnumerable<TeamPlayerEntity> entities)
        {
            var list = entities.ToList();
            if (!list.Any())
            {
                return true;
            }

            return await _table.InsertAsync(list) == list.Count;
        }

        public async Task<bool> UpdateAsync(IEnumerable<TeamPlayerEntity> entities)
        {
            var list = entities.ToList();
            if (!list.Any())
            {
                return true;
            }

            return await _table.UpdateAsync(list);
        }

        public async Task<bool> DeleteAsync(IEnumerable<TeamPlayerEntity> entities)
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
