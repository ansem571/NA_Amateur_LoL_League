using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL.Entities.LeagueInfo;
using Domain.Mappers.Interfaces;
using Domain.Views;

namespace Domain.Mappers.Implementations
{
    public class AlternateAccountMapper: IAlternateAccountMapper
    {
        public AlternateAccountView Map(AlternateAccountEntity entity)
        {
            return new AlternateAccountView
            {
                AlternateName = entity.AlternateName,
                OpGgUrlLink = entity.OpGGUrlLink
            };
        }

        public AlternateAccountEntity Map(AlternateAccountView view)
        {
            return new AlternateAccountEntity
            {
                AlternateName = view.AlternateName,
                OpGGUrlLink = view.OpGgUrlLink
            };
        }

        public IEnumerable<AlternateAccountView> Map(IEnumerable<AlternateAccountEntity> entities)
        {
            return entities.Select(Map);
        }

        public IEnumerable<AlternateAccountEntity> Map(IEnumerable<AlternateAccountView> views)
        {
            return views.Select(Map);
        }
    }
}
