using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL.Entities.LeagueInfo;
using Domain.Enums;
using Domain.Mappers.Interfaces;
using Domain.Views;

namespace Domain.Mappers.Implementations
{
    public class SummonerMapper : ISummonerMapper
    {
        private readonly ISummonerRoleMapper _summonerRoleMapper;
        private readonly ITierDivisionMapper _tierDivisionMapper;
        public SummonerMapper(ISummonerRoleMapper summonerRoleMapper, ITierDivisionMapper tierDivisionMapper)
        {
            _summonerRoleMapper = summonerRoleMapper ?? throw new ArgumentNullException(nameof(summonerRoleMapper));
            _tierDivisionMapper = tierDivisionMapper ?? throw new ArgumentNullException(nameof(tierDivisionMapper));
        }

        public SummonerInfoView Map(SummonerInfoEntity entity)
        {
            return new SummonerInfoView
            {
                SummonerName = entity.SummonerName,
                Role = _summonerRoleMapper.Map(entity.RoleId),
                OffRole = entity.OffRoleId.HasValue ? _summonerRoleMapper.Map(entity.OffRoleId.Value) : SummonerRoleEnum.None,
                TierDivision = _tierDivisionMapper.Map(entity.Tier_DivisionId),
                CurrentLp = entity.CurrentLp,
                OpGgUrl = entity.OpGGUrlLink,
                IsValid = entity.IsValidPlayer
            };
        }

        public IEnumerable<SummonerInfoView> Map(IEnumerable<SummonerInfoEntity> entities)
        {
            return entities.Select(Map);
        }

        public SummonerInfoEntity Map(SummonerInfoView view)
        {
            return new SummonerInfoEntity
            {
                SummonerName = view.SummonerName,
                RoleId = _summonerRoleMapper.Map(view.Role),
                OffRoleId = view.OffRole != SummonerRoleEnum.None ? _summonerRoleMapper.Map(view.OffRole) : default(Guid),
                Tier_DivisionId = _tierDivisionMapper.Map(view.TierDivision),
                OpGGUrlLink = view.OpGgUrl,
                IsValidPlayer = view.IsValid,
                CurrentLp = view.CurrentLp
            };
        }

        public IEnumerable<SummonerInfoEntity> Map(IEnumerable<SummonerInfoView> views)
        {
            return views.Select(Map);
        }
    }
}
