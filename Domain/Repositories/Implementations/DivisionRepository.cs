using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Data.Interfaces;
using DAL.Entities.LeagueInfo;
using Domain.Repositories.Interfaces;

namespace Domain.Repositories.Implementations
{
    public class DivisionRepository : IDivisionRepository
    {
        private readonly ITableStorageRepository<DivisionEntity> _table;

        public DivisionRepository(ITableStorageRepository<DivisionEntity> table)
        {
            _table = table ?? throw new ArgumentNullException(nameof(table));
        }

        public async Task<IEnumerable<DivisionEntity>> GetAllForSeasonAsync(Guid seasonId)
        {
            return await _table.ReadManyAsync("SeasonInfoId = @seasonId", new {seasonId});
        }
    }
}
