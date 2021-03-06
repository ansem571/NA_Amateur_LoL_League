﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Data.Interfaces;
using DAL.Entities.LeagueInfo;
using Domain.Repositories.Interfaces;

namespace Domain.Repositories.Implementations
{
    public class TeamPlayerRepository : ITeamPlayerRepository
    {
        private readonly ITableStorageRepository<TeamPlayerEntity> _table;

        public TeamPlayerRepository(ITableStorageRepository<TeamPlayerEntity> table)
        {
            _table = table ?? throw new ArgumentNullException(nameof(table));
        }

        public async Task<IEnumerable<TeamPlayerEntity>> ReadAllForSeasonAsync(Guid seasonInfoId)
        {
            return await _table.ReadManyAsync("SeasonInfoId = @seasonInfoId", new {seasonInfoId});
        }

        public async Task<IEnumerable<TeamPlayerEntity>> ReadAllForRosterAsync(Guid rosterId)
        {
            return await _table.ReadManyAsync("TeamRosterId = @rosterId", new {rosterId});
        }

        public async Task<Guid?> GetRosterIdForExistingGroupAsync(IEnumerable<Guid> summonerIds, Guid seasonInfoId)
        {
            var player = (await _table.ReadManyAsync("SummonerId in @summonerIds AND SeasonInfoId = @seasonInfoId", new {summonerIds, seasonInfoId})).FirstOrDefault();
            return player?.TeamRosterId;
        }

        public async Task<TeamPlayerEntity> GetBySummonerIdAsync(Guid summonerId, Guid rosterId)
        {
            return (await _table.ReadManyAsync("SummonerId = @summonerId AND TeamRosterId = @rosterId", new {summonerId, rosterId})).FirstOrDefault();
        }

        public async Task<TeamPlayerEntity> GetBySummonerAndSeasonIdAsync(Guid summonerId, Guid seasonInfoId)
        {
            return (await _table.ReadManyAsync("SummonerId = @summonerId AND SeasonInfoId = @seasonInfoId", new { summonerId, seasonInfoId })).FirstOrDefault();
        }

        public async Task<IEnumerable<TeamPlayerEntity>> GetAllRostersForPlayerAsync(Guid summonerId)
        {
            return await _table.ReadManyAsync("SummonerId = @summonerId", new {summonerId});
        }

        public async Task<bool> InsertAsync(IEnumerable<TeamPlayerEntity> entities)
        {
            var list = entities.ToList();
            if (!list.Any())
            {
                return true;
            }

            return await _table.InsertAsync(list) == list.Count;
        }

        public async Task<bool> UpdateAsync(IEnumerable<TeamPlayerEntity> entities)
        {
            var list = entities.ToList();
            if (!list.Any())
            {
                return true;
            }

            return await _table.UpdateAsync(list);
        }

        public async Task<bool> DeleteAsync(IEnumerable<TeamPlayerEntity> entities)
        {
            var list = entities.ToList();
            if (!list.Any())
            {
                return true;
            }

            return await _table.DeleteAsync(list);
        }
    }
}
