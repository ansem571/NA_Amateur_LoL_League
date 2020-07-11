using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Entities.LeagueInfo;

namespace Domain.Repositories.Interfaces
{
    public interface ITeamCaptainRepository
    {
        Task<TeamCaptainEntity> GetCaptainByRosterId(Guid rosterId);
        Task<IEnumerable<TeamCaptainEntity>> GetAllTeamCaptainsAsync();
        Task<bool> CreateCaptainAsync(TeamCaptainEntity entity);
        Task<bool> DeleteCaptainAsync(TeamCaptainEntity entity);
    }
}
