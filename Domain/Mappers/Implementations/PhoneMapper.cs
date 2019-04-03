using System;
using System.Collections.Generic;
using System.Text;
using DAL.Enums;
using Domain.Mappers.Interfaces;

namespace Domain.Mappers.Implementations
{
    public class PhoneMapper : IPhoneMapper
    {
        public Guid? MapFromEnum(PhoneCarrierEnum carrier)
        {
            switch (carrier)
            {
                case PhoneCarrierEnum.ATT:
                    return new Guid("B8872D4D-89F9-443D-9031-C0DF96E849E0");
                case PhoneCarrierEnum.Verizon:
                    return new Guid("2E38A098-24BC-4C33-97FB-E4F3CFDB79A0");
                case PhoneCarrierEnum.Sprint:
                    return new Guid("5A3B69FA-ACC1-4512-BC72-D2AB5231ABBC");
                case PhoneCarrierEnum.TMobile:
                    return new Guid("B619049D-6F60-4008-AB32-CC75B7B830E2");
                case PhoneCarrierEnum.VirginMobile:
                    return new Guid("1E041013-759E-494A-9C24-30EEEC1A29E3");
                case PhoneCarrierEnum.Nextel:
                    return new Guid("7BD3EC35-7D0C-4DE3-9E99-86925C662817");
                case PhoneCarrierEnum.Boost:
                    return new Guid("4BB66480-A5B8-43EA-BEE2-E91E7EBF5666");
                case PhoneCarrierEnum.Alltel:
                    return new Guid("18F841B4-D5F7-413A-B291-4BCD80ACBF2A");
                case PhoneCarrierEnum.EE:
                    return new Guid("A7EE7A89-D169-4150-A3E3-6FB4238167D5");
                case PhoneCarrierEnum.Unknown:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException(nameof(carrier), carrier, null);
            }
        }
    }
}
