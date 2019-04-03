using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities.LeagueInfo;
using DAL.Entities.UserData;
using Domain.Exceptions;
using Domain.Mappers.Interfaces;
using Domain.Repositories.Interfaces;
using Domain.Services.Interfaces;
using Domain.Views;
using Microsoft.Extensions.Logging;

namespace Domain.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly ILogger _logger;
        private readonly ISummonerRoleMapper _summonerRoleMapper;
        private readonly ITierDivisionMapper _tierDivisionMapper;
        private readonly ILookupRepository _lookupRepository;
        private readonly ISummonerInfoRepository _summonerInfoRepository;

        public AccountService(ILogger logger, ISummonerRoleMapper summonerRoleMapper, ITierDivisionMapper tierDivisionMapper,
            ILookupRepository lookupRepository, ISummonerInfoRepository summonerInfoRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _summonerRoleMapper = summonerRoleMapper ?? throw new ArgumentNullException(nameof(summonerRoleMapper));
            _tierDivisionMapper = tierDivisionMapper ?? throw new ArgumentNullException(nameof(tierDivisionMapper));
            _lookupRepository = lookupRepository ?? throw new ArgumentNullException(nameof(lookupRepository));
            _summonerInfoRepository = summonerInfoRepository ?? throw new ArgumentNullException(nameof(summonerInfoRepository));
        }

        public async Task<bool> CreateSummonerInfoAsync(SummonerInfoView view, UserEntity user)
        {
            var result = false;
            try
            {
                var summonerRoleId = _summonerRoleMapper.MapFromEnum(view.Role);

                var tierDivisionId = _tierDivisionMapper.MapFromEnum(view.TierDivision);

                var newEntity = new SummonerInfoEntity
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    SummonerName = view.SummonerName,
                    RoleId = summonerRoleId,
                    Tier_DivisionId = tierDivisionId,
                    CurrentLp = view.CurrentLp,
                    IsValidPlayer = false,
                    OpGGUrlLink = view.OpGgUrl
                };

                result = await _summonerInfoRepository.InsertAsync(newEntity);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating summoner info. Please contact support.");
            }

            return result;
        }

        public async Task<bool> UpdateSummonerInfoAsync(SummonerInfoView view, UserEntity user)
        {
            throw new NotImplementedException();
        }

        public Task<SummonerInfoView> GetSummonerViewAsync(UserEntity user)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<SummonerInfoView>> GetRosterSummonerInfosAsync(Guid rosterId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<SummonerInfoView>> GetAllSummonerAsync()
        {
            throw new NotImplementedException();
        }
    }
}
