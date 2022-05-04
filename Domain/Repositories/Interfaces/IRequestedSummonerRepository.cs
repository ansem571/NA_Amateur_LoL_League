using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Data.Interfaces;
using DAL.Entities.LeagueInfo;

namespace Domain.Repositories.Interfaces
{
    public interface IRequestedSummonerRepository
    {
        Task<IEnumerable<SummonerRequestEntity>> ReadAllForSummonerAsync(Guid summonerId, Guid seasonInfoId);
        Task<IEnumerable<SummonerRequestEntity>> ReadAllForSeasonAsync(Guid seasonInfoId);

        Task<bool> CreateAsync(IEnumerable<SummonerRequestEntity> entities, IUnitOfWork uow = null);
        Task<bool> UpdateAsync(IEnumerable<SummonerRequestEntity> entities, IUnitOfWork uow = null);
        Task<bool> DeleteAsync(IEnumerable<SummonerRequestEntity> entities, IUnitOfWork uow = null);
    }
}
