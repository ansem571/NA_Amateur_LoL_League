using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities.LeagueInfo;

namespace Domain.Repositories.Interfaces
{
    public interface IRequestedSummonerRepository
    {
        Task<IEnumerable<SummonerRequestEntity>> ReadAllForSummonerAsync(Guid summonerId);
        Task<IEnumerable<SummonerRequestEntity>> ReadAllAsync();
        Task<bool> CreateAsync(IEnumerable<SummonerRequestEntity> entities);
        Task<bool> UpdateAsync(IEnumerable<SummonerRequestEntity> entities);
        Task<bool> DeleteAsync(IEnumerable<SummonerRequestEntity> entities);
    }
}
