using System.Collections.Generic;
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

        DetailedSummonerInfoView MapDetailed(SummonerInfoEntity entity, IEnumerable<AlternateAccountEntity> alternates, PlayerStatsView stats = null);
        IEnumerable<DetailedSummonerInfoView> MapDetailed(IEnumerable<SummonerInfoEntity> entities, IEnumerable<AlternateAccountEntity> alternateAccountEntities, IEnumerable<PlayerStatsView> stats);

        SummonerInfoEntity MapDetailed(DetailedSummonerInfoView view);
        IEnumerable<SummonerInfoEntity> MapDetailed(IEnumerable<DetailedSummonerInfoView> views);
    }
}
