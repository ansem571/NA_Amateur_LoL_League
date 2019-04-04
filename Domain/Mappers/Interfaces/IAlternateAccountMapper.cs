using System;
using System.Collections.Generic;
using System.Text;
using DAL.Entities.LeagueInfo;
using Domain.Views;

namespace Domain.Mappers.Interfaces
{
    public interface IAlternateAccountMapper
    {
        AlternateAccountView Map(AlternateAccountEntity entity);
        AlternateAccountEntity Map(AlternateAccountView view);

        IEnumerable<AlternateAccountView> Map(IEnumerable<AlternateAccountEntity> entities);
        IEnumerable<AlternateAccountEntity> Map(IEnumerable<AlternateAccountView> views);
    }
}
