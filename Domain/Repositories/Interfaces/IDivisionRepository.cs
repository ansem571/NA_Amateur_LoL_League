using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Entities.LeagueInfo;

namespace Domain.Repositories.Interfaces
{
    public interface IDivisionRepository
    {
        Task<DivisionEntity> GetByIdAsync(Guid id);
        Task<IEnumerable<DivisionEntity>> GetAllForSeasonAsync(Guid seasonId);
        Task<DivisionEntity> GetDivisionByTeamScoreAsync(int teamTierScore, Guid seasonInfoId);
        Task<bool> CreateDivisionAsync(DivisionEntity entity);
        Task<bool> UpdateDivisionAsync(DivisionEntity entity);
        Task<bool> DeleteDivisionAsync(DivisionEntity entity);
    }
}
