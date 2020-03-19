using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Entities.LeagueInfo;

namespace Domain.Repositories.Interfaces
{
    public interface ISeasonInfoRepository
    {
        Task<SeasonInfoEntity> GetActiveSeasonInfoByDateAsync(DateTime date);
        Task<IEnumerable<SeasonInfoEntity>> GetAllSeasonsAsync();
        Task<bool> CreateSeasonAsync(SeasonInfoEntity season);
        Task<bool> UpdateSeasonAsync(SeasonInfoEntity season);
    }
}
