using System;
using System.Collections.Generic;
using System.Text;
using DAL.Enums;

namespace Domain.Mappers.Interfaces
{
    public interface ITierDivisionMapper
    {
        Guid MapFromEnum(TierDivisionEnum tierDivision);
    }
}
