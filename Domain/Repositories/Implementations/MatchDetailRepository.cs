﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Data.Interfaces;
using DAL.Entities.LeagueInfo;
using Domain.Helpers;
using Domain.Repositories.Interfaces;

namespace Domain.Repositories.Implementations
{
    public class MatchDetailRepository : IMatchDetailRepository
    {
        private readonly ITableStorageRepository<MatchDetailEntity> _table;

        public MatchDetailRepository(ITableStorageRepository<MatchDetailEntity> table)
        {
            _table = table ?? throw new ArgumentNullException(nameof(table));
        }

        public async Task<Dictionary<MatchDetailKey, MatchDetailEntity>> ReadForScheduleId(Guid scheduleId)
        {
            var entities = (await _table.ReadManyAsync("TeamScheduleId = @scheduleId", new {scheduleId})).ToList();
            var dictionary = entities.GroupBy(x => (x.TeamScheduleId, x.Game, x.PlayerId))
                .ToDictionary(x => new MatchDetailKey
                {
                    ScheduleId = x.Key.Item1,
                    Game = x.Key.Item2,
                    PlayerId = x.Key.Item3
                }, x => x.First());

            return dictionary;
        }

        public async Task<bool> InsertAsync(IEnumerable<MatchDetailEntity> entities)
        {
            entities = entities.ToList();
            if (!entities.Any())
            {
                return true;
            }

            return await _table.InsertAsync(entities) == entities.Count();
        }

        public async Task<bool> DeleteAsync(IEnumerable<Guid> ids)
        {
            ids = ids.ToList();
            if (!ids.Any())
            {
                return true;
            }

            var result = await _table.DeleteWhereAsync("Id in @ids", new {ids});
            return result;
        }
    }
}