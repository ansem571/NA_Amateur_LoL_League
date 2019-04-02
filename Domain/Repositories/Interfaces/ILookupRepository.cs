﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities.UserData;

namespace Domain.Repositories.Interfaces
{
    public interface ILookupRepository
    {
        Task<LookupEntity> GetLookupEntity(Guid id);
    }
}
