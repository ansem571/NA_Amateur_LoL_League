using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities.LeagueInfo;

namespace Domain.Repositories.Interfaces
{
    public interface ISeasonInfoRepository
    {
        Task<SeasonInfoEntity> GetActiveSeasonInfoByDate(DateTime date);
    }
}
