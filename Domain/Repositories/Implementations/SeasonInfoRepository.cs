using System;
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

        public async Task<SeasonInfoEntity> GetActiveSeasonInfoByDate(DateTime date)
        {
            var season = (await _table.ReadManyAsync("SeasonEndDate is null OR SeasonEndDate <= @date",
                new { date }, top: 1)).FirstOrDefault();
            return season;
        }
    }
}