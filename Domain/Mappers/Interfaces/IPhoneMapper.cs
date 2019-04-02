using System;
using System.Collections.Generic;
using System.Text;
using DAL.Enums;

namespace Domain.Mappers.Interfaces
{
    public interface IPhoneMapper
    {
        Guid? MapFromEnum(PhoneCarrier carrier);
    }
}
