using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Data.Interfaces;
using DAL.Entities.LeagueInfo;

namespace Domain.Repositories.Implementations
{
    public interface IPlayoffSeedRepository
    {
        Task<IEnumerable<PlayoffSeedEntity>> GetAllForSeasonAsync(Guid seasonInfoId);
        Task<bool> InsertAsync(IEnumerable<PlayoffSeedEntity> playoffSeeds);
    }
    public class PlayoffSeedRepository : IPlayoffSeedRepository
    {
        private readonly ITableStorageRepository<PlayoffSeedEntity> _table;

        public PlayoffSeedRepository(ITableStorageRepository<PlayoffSeedEntity> table)
        {
            _table = table ?? throw new ArgumentNullException(nameof(table));
        }

        public async Task<IEnumerable<PlayoffSeedEntity>> GetAllForSeasonAsync(Guid seasonInfoId)
        {
            return await _table.ReadManyAsync("SeasonInfoId = @seasonInfoId", new {seasonInfoId});
        }

        public async Task<bool> InsertAsync(IEnumerable<PlayoffSeedEntity> playoffSeeds)
        {
            playoffSeeds = playoffSeeds.ToList();
            if (!playoffSeeds.Any())
            {
                return true;
            }

            return await _table.InsertAsync(playoffSeeds) == playoffSeeds.Count();
        }
    }
}
