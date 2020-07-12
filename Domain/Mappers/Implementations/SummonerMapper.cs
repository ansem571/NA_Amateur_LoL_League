using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IAlternateAccountMapper _alternateAccountMapper;
        public SummonerMapper(ISummonerRoleMapper summonerRoleMapper, ITierDivisionMapper tierDivisionMapper, IAlternateAccountMapper alternateAccountMapper)
        {
            _summonerRoleMapper = summonerRoleMapper ??
                                  throw new ArgumentNullException(nameof(summonerRoleMapper));
            _tierDivisionMapper = tierDivisionMapper ??
                                  throw new ArgumentNullException(nameof(tierDivisionMapper));
            _alternateAccountMapper = alternateAccountMapper ??
                                      throw new ArgumentNullException(nameof(alternateAccountMapper));
        }

        public SummonerInfoView Map(SummonerInfoEntity entity)
        {
            return new SummonerInfoView
            {
                SummonerName = entity.SummonerName,
                Role = _summonerRoleMapper.Map(entity.RoleId),
                OffRole = entity.OffRoleId.HasValue ? _summonerRoleMapper.Map(entity.OffRoleId.Value) : SummonerRoleEnum.None,
                TierDivision = _tierDivisionMapper.Map(entity.Tier_DivisionId),
                PreviousSeasonTierDivision = entity.PreviousSeasonRankId != null 
                    ? _tierDivisionMapper.Map(entity.PreviousSeasonRankId.Value) 
                    : default(TierDivisionEnum?),
                CurrentLp = entity.CurrentLp,
                OpGgUrl = entity.OpGGUrlLink,
                IsValid = entity.IsValidPlayer,
                IsSubOnly = entity.IsSubOnly ?? false,
                IsAcademyPlayer = entity.IsAcademyPlayer
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
                OffRoleId = view.OffRole != SummonerRoleEnum.None ? _summonerRoleMapper.Map(view.OffRole) : default(Guid?),
                Tier_DivisionId = _tierDivisionMapper.Map(view.TierDivision),
                PreviousSeasonRankId = view.PreviousSeasonTierDivision != null
                    ? _tierDivisionMapper.Map(view.PreviousSeasonTierDivision.Value)
                    : default(Guid?),
                OpGGUrlLink = view.OpGgUrl,
                IsValidPlayer = view.IsValid,
                CurrentLp = view.CurrentLp,
                IsSubOnly = view.IsSubOnly,
                TeamRoleId = view.TeamRole != SummonerRoleEnum.None ? _summonerRoleMapper.Map(view.TeamRole) : default(Guid?),
                IsAcademyPlayer = view.IsAcademyPlayer
            };
        }

        public IEnumerable<SummonerInfoEntity> Map(IEnumerable<SummonerInfoView> views)
        {
            return views.Select(Map);
        }

        public DetailedSummonerInfoView MapDetailed(SummonerInfoEntity entity, IEnumerable<AlternateAccountEntity> alternates, PlayerStatsView stats = null)
        {
            return new DetailedSummonerInfoView
            {
                Id = entity.Id,
                UserId = entity.UserId,
                SummonerName = entity.SummonerName,
                AlternateAccounts = alternates != null ? _alternateAccountMapper.Map(alternates).ToList() : new List<AlternateAccountView>(),
                Role = _summonerRoleMapper.Map(entity.RoleId),
                OffRole = entity.OffRoleId.HasValue ? _summonerRoleMapper.Map(entity.OffRoleId.Value) : SummonerRoleEnum.None,
                TierDivision = _tierDivisionMapper.Map(entity.Tier_DivisionId),
                PreviousSeasonTierDivision = entity.PreviousSeasonRankId != null
                    ? _tierDivisionMapper.Map(entity.PreviousSeasonRankId.Value)
                    : default(TierDivisionEnum?),
                CurrentLp = entity.CurrentLp,
                OpGgUrl = entity.OpGGUrlLink,
                IsValid = entity.IsValidPlayer,
                PlayerStats = stats,
                IsSubOnly = entity.IsSubOnly ?? false,
                TeamRole = entity.TeamRoleId == null ? _summonerRoleMapper.Map(entity.RoleId) : _summonerRoleMapper.Map(entity.TeamRoleId.Value),
                IsAcademyPlayer = entity.IsAcademyPlayer
            };
        }

        public IEnumerable<DetailedSummonerInfoView> MapDetailed(IEnumerable<SummonerInfoEntity> entities, IEnumerable<AlternateAccountEntity> alternateAccountEntities, IEnumerable<PlayerStatsView> stats)
        {
            stats = stats.ToList();
            alternateAccountEntities = alternateAccountEntities.ToList();
            var list = new List<DetailedSummonerInfoView>();
            foreach (var entity in entities)
            {
                var playerStats = stats.FirstOrDefault(x => x.SummonerId == entity.Id);
                var alternates = alternateAccountEntities.Where(x => x.SummonerId == entity.Id);
                var view = MapDetailed(entity, alternates, playerStats);
                list.Add(view);
            }

            return list;
        }

        public SummonerInfoEntity MapDetailed(DetailedSummonerInfoView view)
        {
            return new SummonerInfoEntity
            {
                Id = view.Id,
                SummonerName = view.SummonerName,
                RoleId = _summonerRoleMapper.Map(view.Role),
                OffRoleId = view.OffRole != SummonerRoleEnum.None ? _summonerRoleMapper.Map(view.OffRole) : default(Guid),
                Tier_DivisionId = _tierDivisionMapper.Map(view.TierDivision),
                PreviousSeasonRankId = view.PreviousSeasonTierDivision != null
                    ? _tierDivisionMapper.Map(view.PreviousSeasonTierDivision.Value)
                    : default(Guid?),
                OpGGUrlLink = view.OpGgUrl,
                IsValidPlayer = view.IsValid,
                CurrentLp = view.CurrentLp,
                IsSubOnly = view.IsSubOnly,
                IsAcademyPlayer = view.IsAcademyPlayer
            };
        }

        public IEnumerable<SummonerInfoEntity> MapDetailed(IEnumerable<DetailedSummonerInfoView> views)
        {
            return views.Select(MapDetailed);
        }
    }
}
