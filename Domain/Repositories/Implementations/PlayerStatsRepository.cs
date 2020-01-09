using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Data.Interfaces;
using DAL.Entities.LeagueInfo;
using Domain.Helpers;
using Domain.Repositories.Interfaces;

namespace Domain.Repositories.Implementations
{
    public class PlayerStatsRepository : IPlayerStatsRepository
    {
        private readonly ITableStorageRepository<PlayerStatsEntity> _table;

        public PlayerStatsRepository(ITableStorageRepository<PlayerStatsEntity> table)
        {
            _table = table ?? throw new ArgumentNullException(nameof(table));
        }

        public async Task<Dictionary<StatsKey, List<PlayerStatsEntity>>> GetStatsAsync(IEnumerable<StatsKey> keys)
        {
            keys = keys.ToList();
            var entities = await _table.ReadManyAsync("SummonerId in @summonerIds AND SeasonInfoId in @seasonIds", new
            {
                summonerIds = keys.Select(x => x.SummonerId),
                seasonIds = keys.Select(x => x.SeasonId)
            });


            var dictionary = entities.Where(x => x.SeasonInfoId.HasValue).GroupBy(x => (x.SummonerId, x.SeasonInfoId))
                .ToDictionary(x =>
                    new StatsKey(x.Key.Item1, x.Key.Item2.Value), x => x.ToList());
            return dictionary;
        }

        public async Task<PlayerStatsEntity> GetStatsForSummonerAsync(Guid summonerId, Guid? seasonInfoId)
        {
            var entity = (await _table.ReadManyAsync("SummonerId = @summonerId AND SeasonInfoId = @seasonInfoId", new { summonerId, seasonInfoId }, top: 1))
                .FirstOrDefault();
            return entity;
        }

        public async Task<IEnumerable<PlayerStatsEntity>> GetStatsForSummonersAsync(IEnumerable<Guid> summonerIds, Guid? seasonInfoId)
        {
            var entities = await _table.ReadManyAsync("SummonerId in @summonerIds AND SeasonInfoId = @seasonInfoId", new { summonerIds, seasonInfoId });
            return entities;
        }

        public async Task<IEnumerable<PlayerStatsEntity>> GetAllStatsAsync(Guid? seasonInfoId)
        {
            return await _table.ReadManyAsync("SeasonInfoId = @seasonInfoId", new { seasonInfoId });
        }

        public async Task<bool> InsertAsync(IEnumerable<PlayerStatsEntity> stats)
        {
            stats = stats.ToList();
            if (!stats.Any())
            {
                return true;
            }

            return await _table.InsertAsync(stats) == stats.Count();
        }

        public async Task<bool> UpdateAsync(IEnumerable<PlayerStatsEntity> stats)
        {
            stats = stats.ToList();
            if (!stats.Any())
            {
                return true;
            }

            return await _table.UpdateAsync(stats);
        }

        public async Task<bool> DeleteAsync(IEnumerable<PlayerStatsEntity> stats)
        {
            stats = stats.ToList();
            if (!stats.Any())
            {
                return true;
            }

            return await _table.DeleteAsync(stats);
        }
    }
}
