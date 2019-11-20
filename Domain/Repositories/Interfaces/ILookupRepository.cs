using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Entities.UserData;

namespace Domain.Repositories.Interfaces
{
    public interface ILookupRepository
    {
        Task<LookupEntity> GetLookupEntity(Guid id);
        Task<IEnumerable<LookupEntity>> GetLookupEntitiesByCategory(string category);
    }
}
