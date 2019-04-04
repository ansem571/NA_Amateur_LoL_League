using System;
using Domain.Enums;
using Domain.Mappers.Interfaces;

namespace Domain.Mappers.Implementations
{
    public class TierDivisionMapper : ITierDivisionMapper
    {
        public Guid Map(TierDivisionEnum tierDivision)
        {
            switch (tierDivision)
            {
                case TierDivisionEnum.Unranked:
                    return new Guid("7F1D1868-7C19-4774-B151-B214083D5AB8");
                case TierDivisionEnum.Iron4:
                    return new Guid("66DED5A2-A8EC-490A-9F35-C7A20D90E8A5");
                case TierDivisionEnum.Iron3:
                    return new Guid("8CB52BE4-D2D2-4CB9-968D-7D413E4D0869");
                case TierDivisionEnum.Iron2:
                    return new Guid("9DB85DDA-CC78-49F7-B4C9-23DEC7E4D04A");
                case TierDivisionEnum.Iron1:
                    return new Guid("77BCAFD5-4672-4F3C-A862-28F62EF88CE6");
                case TierDivisionEnum.Bronze4:
                    return new Guid("1E81426D-ADE8-4AF0-96EC-E305116F4D8D");
                case TierDivisionEnum.Bronze3:
                    return new Guid("7F87D4BF-4D01-4073-BFC7-FA76CB510ABE");
                case TierDivisionEnum.Bronze2:
                    return new Guid("13169C1E-5F1B-49E6-98AC-48F6004DA62B");
                case TierDivisionEnum.Bronze1:
                    return new Guid("9BE885B8-DAAD-4032-9628-74B2C0C49609");
                case TierDivisionEnum.Silver4:
                    return new Guid("3B195698-F901-460F-931B-6A2078F77B02");
                case TierDivisionEnum.Silver3:
                    return new Guid("023BD37C-D8F8-4269-BFEF-6E23F96E1AAE");
                case TierDivisionEnum.Silver2:
                    return new Guid("1795AE1B-39F5-46BE-953A-E53C72F04026");
                case TierDivisionEnum.Silver1:
                    return new Guid("3D828C65-DA6D-4609-8659-BDAFD3836EC7");
                case TierDivisionEnum.Gold4:
                    return new Guid("E1DA47BC-4E98-4B8A-AB2B-841AC2A3C7FA");
                case TierDivisionEnum.Gold3:
                    return new Guid("0A6079D0-D325-40F8-B76B-C28C2C3FFEEA");
                case TierDivisionEnum.Gold2:
                    return new Guid("241B18F5-2F11-4C91-9271-021E7FDD41CA");
                case TierDivisionEnum.Gold1:
                    return new Guid("2234361A-8592-4A84-8119-0716618E7268");
                case TierDivisionEnum.Platinum4:
                    return new Guid("F5316B89-83F9-4ED1-9A27-DF8E77AFCBF3");
                case TierDivisionEnum.Platinum3:
                    return new Guid("F0F32C2D-5767-4E04-9ACD-B97D853512A8");
                case TierDivisionEnum.Platinum2:
                    return new Guid("34F5E2FA-4CA3-4B8D-AFCC-00F2D538308A");
                case TierDivisionEnum.Platinum1:
                    return new Guid("81D75B85-C507-4C4C-90F5-3FF908123A79");
                case TierDivisionEnum.Diamond4:
                    return new Guid("D902D288-6242-4F37-A92A-ADDCD2A09821");
                case TierDivisionEnum.Diamond3:
                    return new Guid("8787DBAE-C6A2-43AD-99D3-F31A0DAF5DAA");
                case TierDivisionEnum.Diamond2:
                    return new Guid("72F642A3-4E5D-424D-BA45-5783F73F2F53");
                case TierDivisionEnum.Diamond1:
                    return new Guid("42E07F40-33FE-4C51-A3DA-AF79BD8B9608");
                case TierDivisionEnum.Masters1:
                    return new Guid("4F7B2C04-6B5B-451D-B110-9B11ED0432C4");
                case TierDivisionEnum.Grandmasters1:
                    return new Guid("D026C1F8-674B-4B2B-A821-CDFFF647192A");
                default:
                    throw new ArgumentOutOfRangeException(nameof(tierDivision), tierDivision, null);
            }
        }

        public TierDivisionEnum Map(Guid id)
        {
            var tierDivision = new TierDivisionEnum();
            if (id == new Guid("7F1D1868-7C19-4774-B151-B214083D5AB8"))
            {
                tierDivision = TierDivisionEnum.Unranked;
            }
            else if (id == new Guid("66DED5A2-A8EC-490A-9F35-C7A20D90E8A5"))
            {
                tierDivision = TierDivisionEnum.Iron4;
            }
            else if (id == new Guid("8CB52BE4-D2D2-4CB9-968D-7D413E4D0869"))
            {
                tierDivision = TierDivisionEnum.Iron3;
            }
            else if (id == new Guid("9DB85DDA-CC78-49F7-B4C9-23DEC7E4D04A"))
            {
                tierDivision = TierDivisionEnum.Iron2;
            }
            else if (id == new Guid("77BCAFD5-4672-4F3C-A862-28F62EF88CE6"))
            {
                tierDivision = TierDivisionEnum.Iron1;
            }
            else if (id == new Guid("1E81426D-ADE8-4AF0-96EC-E305116F4D8D"))
            {
                tierDivision = TierDivisionEnum.Bronze4;
            }
            else if (id == new Guid("7F87D4BF-4D01-4073-BFC7-FA76CB510ABE"))
            {
                tierDivision = TierDivisionEnum.Bronze3;
            }
            else if (id == new Guid("13169C1E-5F1B-49E6-98AC-48F6004DA62B"))
            {
                tierDivision = TierDivisionEnum.Bronze2;
            }
            else if (id == new Guid("9BE885B8-DAAD-4032-9628-74B2C0C49609"))
            {
                tierDivision = TierDivisionEnum.Bronze1;
            }
            else if (id == new Guid("3B195698-F901-460F-931B-6A2078F77B02"))
            {
                tierDivision = TierDivisionEnum.Silver4;
            }
            else if (id == new Guid("023BD37C-D8F8-4269-BFEF-6E23F96E1AAE"))
            {
                tierDivision = TierDivisionEnum.Silver3;
            }
            else if (id == new Guid("1795AE1B-39F5-46BE-953A-E53C72F04026"))
            {
                tierDivision = TierDivisionEnum.Silver2;
            }
            else if (id == new Guid("3D828C65-DA6D-4609-8659-BDAFD3836EC7"))
            {
                tierDivision = TierDivisionEnum.Silver1;
            }
            else if (id == new Guid("E1DA47BC-4E98-4B8A-AB2B-841AC2A3C7FA"))
            {
                tierDivision = TierDivisionEnum.Gold4;
            }
            else if (id == new Guid("0A6079D0-D325-40F8-B76B-C28C2C3FFEEA"))
            {
                tierDivision = TierDivisionEnum.Gold3;
            }
            else if (id == new Guid("241B18F5-2F11-4C91-9271-021E7FDD41CA"))
            {
                tierDivision = TierDivisionEnum.Gold2;
            }
            else if (id == new Guid("2234361A-8592-4A84-8119-0716618E7268"))
            {
                tierDivision = TierDivisionEnum.Gold1;
            }
            else if (id == new Guid("F5316B89-83F9-4ED1-9A27-DF8E77AFCBF3"))
            {
                tierDivision = TierDivisionEnum.Platinum4;
            }
            else if (id == new Guid("F0F32C2D-5767-4E04-9ACD-B97D853512A8"))
            {
                tierDivision = TierDivisionEnum.Platinum3;
            }
            else if (id == new Guid("34F5E2FA-4CA3-4B8D-AFCC-00F2D538308A"))
            {
                tierDivision = TierDivisionEnum.Platinum2;
            }
            else if (id == new Guid("81D75B85-C507-4C4C-90F5-3FF908123A79"))
            {
                tierDivision = TierDivisionEnum.Platinum1;
            }
            else if (id == new Guid("D902D288-6242-4F37-A92A-ADDCD2A09821"))
            {
                tierDivision = TierDivisionEnum.Diamond4;
            }
            else if (id == new Guid("8787DBAE-C6A2-43AD-99D3-F31A0DAF5DAA"))
            {
                tierDivision = TierDivisionEnum.Diamond3;
            }
            else if (id == new Guid("72F642A3-4E5D-424D-BA45-5783F73F2F53"))
            {
                tierDivision = TierDivisionEnum.Diamond2;
            }
            else if (id == new Guid("42E07F40-33FE-4C51-A3DA-AF79BD8B9608"))
            {
                tierDivision = TierDivisionEnum.Diamond1;
            }
            else if (id == new Guid("4F7B2C04-6B5B-451D-B110-9B11ED0432C4"))
            {
                tierDivision = TierDivisionEnum.Masters1;
            }
            else if (id == new Guid("D026C1F8-674B-4B2B-A821-CDFFF647192A"))
            {
                tierDivision = TierDivisionEnum.Grandmasters1;
            }

            return tierDivision;
        }
    }
}
