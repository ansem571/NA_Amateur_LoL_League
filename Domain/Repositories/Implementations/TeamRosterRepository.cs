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
    public class TeamRosterRepository : ITeamRosterRepository
    {
        private readonly ITableStorageRepository<TeamRosterEntity> _table;

        public TeamRosterRepository(ITableStorageRepository<TeamRosterEntity> table)
        {
            _table = table ?? throw new ArgumentNullException(nameof(table));
        }

        public async Task<TeamRosterEntity> GetByTeamNameAsync(string teamName)
        {
            return (await _table.ReadManyAsync("TeamName = @teamName", new {teamName})).FirstOrDefault();
        }

        public async Task<TeamRosterEntity> GetByTeamIdAsync(Guid id)
        {
            return await _table.ReadOneAsync(id);
        }

        public async Task<IEnumerable<TeamRosterEntity>> GetAllTeamsAsync()
        {
            return await _table.ReadAllAsync();
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
