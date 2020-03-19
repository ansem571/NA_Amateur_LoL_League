using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<DivisionEntity> GetByIdAsync(Guid id)
        {
            return await _table.ReadOneAsync(id);
        }

        public async Task<IEnumerable<DivisionEntity>> GetAllForSeasonAsync(Guid seasonId)
        {
            return await _table.ReadManyAsync("SeasonInfoId = @seasonId", new {seasonId});
        }

        public async Task<DivisionEntity> GetDivisionByTeamScoreAsync(int teamTierScore, Guid seasonInfoId)
        {
            var division = (await _table.ReadManyAsync("LowerLimit < @teamTierScore AND UpperLimit > @teamTierScore AND SeasonInfoId = @seasonInfoId",
                new {teamTierScore, seasonInfoId})).FirstOrDefault();
            return division;
        }

        public async Task<bool> CreateDivisionAsync(DivisionEntity entity)
        {
            return await _table.InsertAsync(entity) == 1;
        }

        public async Task<bool> UpdateDivisionAsync(DivisionEntity entity)
        {
            return await _table.UpdateAsync(entity);
        }

        public async Task<bool> DeleteDivisionAsync(DivisionEntity entity)
        {
            return await _table.DeleteAsync(entity);
        }
    }
}
