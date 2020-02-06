using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Data.Interfaces;
using DAL.Entities.LeagueInfo;
using Domain.Repositories.Interfaces;

namespace Domain.Repositories.Implementations
{
    public class SeasonInfoRepository : ISeasonInfoRepository
    {
        private readonly ITableStorageRepository<SeasonInfoEntity> _table;

        public SeasonInfoRepository(ITableStorageRepository<SeasonInfoEntity> table)
        {
            _table = table ?? throw new ArgumentNullException(nameof(table));
        }

        public async Task<SeasonInfoEntity> GetActiveSeasonInfoByDateAsync(DateTime date)
        {
            var season = (await _table.ReadManyAsync("SeasonEndDate is null OR SeasonEndDate <= @date",
                new { date }, top: 1)).FirstOrDefault();
            return season;
        }

        public async Task<IEnumerable<SeasonInfoEntity>> GetAllSeasonsAsync()
        {
            return await _table.ReadAllAsync();
        }

        public async Task<bool> CreateSeasonAsync(SeasonInfoEntity season)
        {
            return await _table.InsertAsync(season) == 1;
        }

        public async Task<bool> UpdateSeasonAsync(SeasonInfoEntity season)
        {
            return await _table.UpdateAsync(season);
        }
    }
}