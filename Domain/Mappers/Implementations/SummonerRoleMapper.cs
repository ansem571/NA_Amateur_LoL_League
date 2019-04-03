using System;
using System.Collections.Generic;
using System.Text;
using DAL.Enums;
using Domain.Mappers.Interfaces;

namespace Domain.Mappers.Implementations
{
    public class SummonerRoleMapper : ISummonerRoleMapper
    {
        public Guid MapFromEnum(SummonerRoleEnum role)
        {
            switch (role)
            {
                case SummonerRoleEnum.Top:
                    {
                        return new Guid("D28AF391-3DF2-4318-8EF3-CA0A36A076D9");
                    }
                case SummonerRoleEnum.Jungle:
                    {
                        return new Guid("A3D13850-280E-4ED5-B430-BAE0541A7433");
                    }
                case SummonerRoleEnum.Mid:
                    {
                        return new Guid("0D915E96-4890-4910-82E5-5C349937B9AD");
                    }
                case SummonerRoleEnum.Adc:
                    {
                        return new Guid("C76E5F4B-43A0-4C19-975F-F41E48CF6DB3");
                    }
                case SummonerRoleEnum.Sup:
                    {
                        return new Guid("3A2502EB-F104-43B9-9FC1-2FC189C27F0E");
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(role), role, null);
            }
        }
    }
}
