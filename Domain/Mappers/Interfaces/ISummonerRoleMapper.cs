using System;
using Domain.Enums;

namespace Domain.Mappers.Interfaces
{
    public interface ISummonerRoleMapper
    {
        Guid Map(SummonerRoleEnum role);
        SummonerRoleEnum Map(Guid roleId);
    }
}
