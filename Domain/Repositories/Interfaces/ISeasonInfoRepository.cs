using System;
using System.Threading.Tasks;
using DAL.Entities.LeagueInfo;

namespace Domain.Repositories.Interfaces
{
    public interface ISeasonInfoRepository
    {
        Task<SeasonInfoEntity> GetActiveSeasonInfoByDate(DateTime date);
    }
}
