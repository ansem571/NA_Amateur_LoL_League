using System;
using System.Collections.Generic;
using System.Text;
using DAL.Entities.LeagueInfo;
using Domain.Views;

namespace Domain.Mappers.Interfaces
{
    public interface ISummonerMapper
    {
        SummonerInfoView Map(SummonerInfoEntity entity);
        IEnumerable<SummonerInfoView> Map(IEnumerable<SummonerInfoEntity> entities);

        SummonerInfoEntity Map(SummonerInfoView view);
        IEnumerable<SummonerInfoEntity> Map(IEnumerable<SummonerInfoView> views);
    }
}
