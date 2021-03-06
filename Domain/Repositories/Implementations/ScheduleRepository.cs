﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Data.Interfaces;
using DAL.Entities.LeagueInfo;
using Domain.Repositories.Interfaces;

namespace Domain.Repositories.Implementations
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly ITableStorageRepository<ScheduleEntity> _table;

        public ScheduleRepository(ITableStorageRepository<ScheduleEntity> table)
        {
            _table = table ??
                     throw new ArgumentNullException(nameof(table));
        }

        public async Task<ScheduleEntity> GetScheduleAsync(Guid id)
        {
            return await _table.ReadOneAsync(id);
        }

        public async Task<IEnumerable<ScheduleEntity>> GetAllAsync(Guid? seasonInfoId)
        {
            return await _table.ReadManyAsync("SeasonInfoId = @seasonInfoId", new { seasonInfoId });
        }

        public async Task<IEnumerable<ScheduleEntity>> GetAllUpdatedMatchesAsync(Guid seasonInfoId)
        {
            return await _table.ReadManyAsync(
                "(HomeTeamWins is not null AND HomeTeamWins > 0) or (AwayTeamWins is not null AND AwayTeamWins > 0) AND SeasonInfoId = @seasonInfoId", new { seasonInfoId });
        }

        public async Task<bool> InsertAsync(IEnumerable<ScheduleEntity> entities)
        {
            entities = entities.ToList();
            return !entities.Any() || await _table.InsertAsync(entities) == entities.Count();
        }

        public async Task<bool> UpdateAsync(IEnumerable<ScheduleEntity> entities)
        {
            entities = entities.ToList();
            return !entities.Any() || await _table.UpdateAsync(entities);
        }

        public async Task<IEnumerable<ScheduleEntity>> GetAllPlayoffMatchesAsync(Guid seasonInfoId)
        {
            return await _table.ReadManyAsync("IsPlayoffMatch = 1 AND SeasonInfoId = @seasonInfoId",
                new {seasonInfoId});
        }
    }
}
