using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Entities.LeagueInfo;

namespace Domain.Repositories.Interfaces
{
    public interface IAlternateAccountRepository
    {
        Task<IEnumerable<AlternateAccountEntity>> ReadAllForSummonerAsync(Guid summonerId);
        Task<bool> CreateAsync(IEnumerable<AlternateAccountEntity> entities);
        Task<bool> UpdateAsync(IEnumerable<AlternateAccountEntity> entities);
        Task<bool> DeleteAsync(IEnumerable<AlternateAccountEntity> entities);
    }
}
