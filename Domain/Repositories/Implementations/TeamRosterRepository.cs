using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Data.Interfaces;
using DAL.Entities.LeagueInfo;
using Domain.Repositories.Interfaces;

namespace Domain.Repositories.Implementations
{
    public class TeamRosterRepository : ITeamRosterRepository
    {
        private readonly ITableStorageRepository<TeamRosterEntity> _table;

        public TeamRosterRepository(ITableStorageRepository<TeamRosterEntity> table)
        {
            _table = table ?? throw new ArgumentNullException(nameof(table));
        }

        public async Task<TeamRosterEntity> GetByTeamNameAsync(string teamName, Guid? seasonInfoId)
        {
            var seasonInfoNotNull = seasonInfoId == null ? "AND SeasonInfoId is null" : "AND SeasonInfoId = @seasonInfoId";
            return (await _table.ReadManyAsync($"TeamName = @teamName {seasonInfoNotNull}", new { teamName, seasonInfoId })).FirstOrDefault();
        }

        public async Task<TeamRosterEntity> GetByTeamIdAsync(Guid id)
        {
            return await _table.ReadOneAsync(id);
        }
        public async Task<IEnumerable<TeamRosterEntity>> GetAllTeamsAsync()
        {
            return await _table.ReadAllAsync();
        }

        public async Task<IEnumerable<TeamRosterEntity>> GetAllTeamsAsync(Guid? seasonInfoId)
        {
            return await _table.ReadManyAsync(seasonInfoId.HasValue ? "SeasonInfoId = @seasonInfoId" : "SeasonInfoId is null", new { seasonInfoId });
        }

        public async Task<TeamRosterEntity> GetTeamAsync(Guid seasonId, IEnumerable<Guid> id)
        {
            return (await _table.ReadManyAsync("SeasonInfoId = @seasonId AND Id in @id", new {seasonId, id})).FirstOrDefault();
        }

        public async Task<bool> CreateAsync(TeamRosterEntity entity)
        {
            return await _table.InsertAsync(entity) == 1;
        }

        public async Task<bool> UpdateAsync(TeamRosterEntity entity)
        {
            return await _table.UpdateAsync(entity);
        }
    }
}
