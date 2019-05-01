using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Entities.LeagueInfo;

namespace Domain.Repositories.Interfaces
{
    public interface IDivisionRepository
    {
        Task<IEnumerable<DivisionEntity>> GetAllForSeasonAsync(Guid seasonId);
    }
}
