using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Entities.LeagueInfo;

namespace Domain.Repositories.Interfaces
{
    public interface ITeamPlayerRepository
    {
        Task<IEnumerable<TeamPlayerEntity>> ReadAllForRosterAsync(Guid rosterId);
        Task<Guid?> GetRosterIdForExistingGroupAsync(IEnumerable<Guid> summonerIds);
        Task<TeamPlayerEntity> GetBySummonerIdAsync(Guid summonerId);
        Task<bool> CreateAsync(IEnumerable<TeamPlayerEntity> entities);
        Task<bool> InsertAsync(IEnumerable<TeamPlayerEntity> entities);
        Task<bool> DeleteAsync(IEnumerable<TeamPlayerEntity> entities);
    }
}
