using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Data.Interfaces;
using DAL.Entities.LeagueInfo;

namespace Domain.Repositories.Implementations
{
    public interface IMatchMvpRepository
    {
        Task<IEnumerable<MatchMvpEntity>> ReadAllForTeamScheduleId(Guid teamScheduleId);

        Task<bool> CreateAsync(IEnumerable<MatchMvpEntity> entities);
        Task<bool> UpdateAsync(IEnumerable<MatchMvpEntity> entities);
    }
    public class MatchMvpRepository : IMatchMvpRepository
    {
        private readonly ITableStorageRepository<MatchMvpEntity> _table;

        public MatchMvpRepository(ITableStorageRepository<MatchMvpEntity> table)
        {
            _table = table ?? throw new ArgumentNullException(nameof(table));
        }

        public async Task<IEnumerable<MatchMvpEntity>> ReadAllForTeamScheduleId(Guid teamScheduleId)
        {
            var entities = await _table.ReadManyAsync("TeamScheduleId = @teamScheduleId", new {teamScheduleId});

            return entities;
        }

        public async Task<bool> CreateAsync(IEnumerable<MatchMvpEntity> entities)
        {
            entities = entities.ToList();
            if (entities.Any())
            {
                return await _table.InsertAsync(entities) == entities.Count();
            }

            return true;
        }

        public async Task<bool> UpdateAsync(IEnumerable<MatchMvpEntity> entities)
        {
            entities = entities.ToList();
            if (entities.Any())
            {
                return await _table.UpdateAsync(entities);
            }

            return true;
        }
    }
}
