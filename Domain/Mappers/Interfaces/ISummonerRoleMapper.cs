using System;
using System.Collections.Generic;
using System.Text;
using DAL.Enums;

namespace Domain.Mappers.Interfaces
{
    public interface ISummonerRoleMapper
    {
        Guid MapFromEnum(SummonerRoleEnum role);
    }
}
