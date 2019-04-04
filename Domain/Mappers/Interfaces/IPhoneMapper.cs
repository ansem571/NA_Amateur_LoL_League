using System;
using Domain.Enums;

namespace Domain.Mappers.Interfaces
{
    public interface IPhoneMapper
    {
        Guid? MapFromEnum(PhoneCarrierEnum carrier);
    }
}
