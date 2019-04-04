using System;
using Domain.Enums;

namespace Domain.Mappers.Interfaces
{
    public interface ITierDivisionMapper
    {
        Guid Map(TierDivisionEnum tierDivision);
        TierDivisionEnum Map(Guid id);
    }
}
