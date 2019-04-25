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
    public class TeamCaptainRepository : ITeamCaptainRepository
    {
        private readonly ITableStorageRepository<TeamCaptainEntity> _table;

        public TeamCaptainRepository(ITableStorageRepository<TeamCaptainEntity> table)
        {
            _table = table ?? throw new ArgumentNullException(nameof(table));
        }

        public async Task<TeamCaptainEntity> GetCaptainByRosterId(Guid rosterId)
        {
            var captain = (await _table.ReadManyAsync("TeamRosterId = @rosterId", new {rosterId}, top: 1))
                .FirstOrDefault();

            return captain;
        }

        public async Task<IEnumerable<TeamCaptainEntity>> GetAllTeamCaptainsAsync()
        {
            return await _table.ReadAllAsync();
        }

        public async Task<bool> CreateCaptainAsync(TeamCaptainEntity entity)
        {
            return await _table.InsertAsync(entity) == 1;
        }
    }
}
