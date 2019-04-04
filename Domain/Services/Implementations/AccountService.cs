using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Entities.LeagueInfo;
using DAL.Entities.UserData;
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
        private readonly ISummonerMapper _summonerMapper;
        private readonly ILookupRepository _lookupRepository;
        private readonly ISummonerInfoRepository _summonerInfoRepository;

        public AccountService(ILogger logger, ISummonerMapper summonerMapper, ILookupRepository lookupRepository, ISummonerInfoRepository summonerInfoRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _summonerMapper = summonerMapper ?? throw new ArgumentNullException(nameof(summonerMapper));
            _lookupRepository = lookupRepository ?? throw new ArgumentNullException(nameof(lookupRepository));
            _summonerInfoRepository = summonerInfoRepository ?? throw new ArgumentNullException(nameof(summonerInfoRepository));
        }

        public async Task<bool> CreateSummonerInfoAsync(SummonerInfoView view, UserEntity user)
        {
            var result = false;
            try
            {
                var newEntity = _summonerMapper.Map(view);
                newEntity.Id = Guid.NewGuid();
                newEntity.UserId = user.Id;

                result = await _summonerInfoRepository.InsertAsync(newEntity);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error creating summoner info for: {user.Email}.");
            }

            return result;
        }

        public async Task<bool> UpdateSummonerInfoAsync(SummonerInfoView view, UserEntity user)
        {
            var result = false;
            try
            {
                var summonerInfo = _summonerMapper.Map(view);

                var readEntity = await _summonerInfoRepository.ReadOneByUserIdAsync(user.Id);

                if (readEntity == null)
                {
                    return await CreateSummonerInfoAsync(view, user);
                }

                summonerInfo.Id = readEntity.Id;
                summonerInfo.UserId = readEntity.UserId;

                result = await _summonerInfoRepository.UpdateAsync(summonerInfo);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error updating summoner info for: {user.Email}");
            }

            return result;
        }

        public async Task<SummonerInfoView> GetSummonerViewAsync(UserEntity user)
        {
            var summonerEntity = await _summonerInfoRepository.ReadOneByUserIdAsync(user.Id);

            return _summonerMapper.Map(summonerEntity);
        }

        public async Task<IEnumerable<SummonerInfoView>> GetRosterSummonerInfosAsync(Guid rosterId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<SummonerInfoView>> GetAllSummonerAsync()
        {
            var entities = await _summonerInfoRepository.GetAllSummonersAsync();

            return _summonerMapper.Map(entities);
        }
    }
}
