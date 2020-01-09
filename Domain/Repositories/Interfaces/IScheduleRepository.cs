using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Entities.LeagueInfo;

namespace Domain.Repositories.Interfaces
{
    public interface IScheduleRepository
    {
        Task<ScheduleEntity> GetScheduleAsync(Guid id);
        Task<IEnumerable<ScheduleEntity>> GetAllAsync(Guid? seasonInfoId);
        Task<IEnumerable<ScheduleEntity>> GetAllUpdatedMatchesAsync();
        Task<bool> InsertAsync(IEnumerable<ScheduleEntity> entities);
        Task<bool> UpdateAsync(IEnumerable<ScheduleEntity> entities);
    }
}
