using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Entities.LeagueInfo;

namespace Domain.Repositories.Interfaces
{
    public interface ITeamRosterRepository
    {
        Task<TeamRosterEntity> GetByTeamNameAsync(string teamName, Guid? seasonInfoId);
        Task<TeamRosterEntity> GetByTeamIdAsync(Guid id);
        Task<IEnumerable<TeamRosterEntity>> GetAllTeamsAsync();
        Task<IEnumerable<TeamRosterEntity>> GetAllTeamsAsync(Guid? seasonInfoId);
        Task<TeamRosterEntity> GetTeamAsync(Guid seasonId, IEnumerable<Guid> summonerId);

        Task<bool> CreateAsync(TeamRosterEntity entity);
        Task<bool> UpdateAsync(TeamRosterEntity entity);
    }
}
