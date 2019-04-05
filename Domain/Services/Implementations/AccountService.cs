using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IAlternateAccountMapper _alternateAccountMapper;
        private readonly IAlternateAccountRepository _alternateAccountRepository;
        private const int AltenateAccountsCount = 3;

        public AccountService(ILogger logger, ISummonerMapper summonerMapper, ILookupRepository lookupRepository,
            ISummonerInfoRepository summonerInfoRepository, IAlternateAccountMapper alternateAccountMapper,
            IAlternateAccountRepository alternateAccountRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _summonerMapper = summonerMapper ?? throw new ArgumentNullException(nameof(summonerMapper));
            _lookupRepository = lookupRepository ?? throw new ArgumentNullException(nameof(lookupRepository));
            _summonerInfoRepository = summonerInfoRepository ?? throw new ArgumentNullException(nameof(summonerInfoRepository));
            _alternateAccountMapper = alternateAccountMapper ??
                                      throw new ArgumentNullException(nameof(alternateAccountMapper));
            _alternateAccountRepository = alternateAccountRepository ??
                                          throw new ArgumentNullException(nameof(alternateAccountRepository));
        }

        public async Task<bool> CreateSummonerInfoAsync(SummonerInfoView view, UserEntity user)
        {
            var result = false;
            try
            {
                var newEntity = _summonerMapper.Map(view);
                newEntity.Id = Guid.NewGuid();
                newEntity.UserId = user.Id;
                newEntity.IsValidPlayer = false;

                result = await _summonerInfoRepository.InsertAsync(newEntity);

                if (view.AlternateAccounts.Any())
                {
                    var alternateAccounts = _alternateAccountMapper.Map(view.AlternateAccounts).ToList();
                    foreach (var alternateAccount in alternateAccounts)
                    {
                        alternateAccount.Id = Guid.NewGuid();
                        alternateAccount.SummonerId = newEntity.Id;
                    }

                    result = await _alternateAccountRepository.CreateAsync(alternateAccounts);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error creating summoner info for: {user.Email}.");
            }

            return result;
        }

        /// <summary>
        /// Will always go through here
        /// </summary>
        /// <param name="view"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> UpdateSummonerInfoAsync(SummonerInfoView view, UserEntity user)
        {
            var result = false;
            try
            {
                view.RemoveEmptyViewsForDb();
                var readEntity = await _summonerInfoRepository.ReadOneByUserIdAsync(user.Id);

                if (readEntity == null)
                {
                    return await CreateSummonerInfoAsync(view, user);
                }

                var summonerInfo = _summonerMapper.Map(view);
                summonerInfo.Id = readEntity.Id;
                summonerInfo.UserId = readEntity.UserId;

                var altAccountTask = UpdateAlternateAccountsAsync(summonerInfo.Id, view.AlternateAccounts);
                var updateSummonerInfoTask = _summonerInfoRepository.UpdateAsync(summonerInfo);
                result = await altAccountTask && await updateSummonerInfoTask;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error updating summoner info for: {user.Email}");
            }

            return result;
        }

        public async Task<bool> UpdateAlternateAccountsAsync(Guid summonerId, IEnumerable<AlternateAccountView> viewList)
        {
            var newList = viewList.ToList();

            var existingAlternateAccounts =
                (await _alternateAccountRepository.ReadAllForSummonerAsync(summonerId)).ToList();

            var kvp = new Dictionary<AlternateAccountView, AlternateAccountEntity>();

            foreach (var entity in existingAlternateAccounts)
            {
                var mapped = _alternateAccountMapper.Map(entity);
                kvp.Add(mapped, entity);
            }

            var createList = new List<AlternateAccountEntity>();
            var updateList = new List<AlternateAccountEntity>();
            var deleteList = new List<AlternateAccountEntity>();

            foreach (var newAltAccount in newList)
            {
                //create
                if (!kvp.TryGetValue(newAltAccount, out var oldEntity))
                {
                    var newEntity = _alternateAccountMapper.Map(newAltAccount);
                    newEntity.Id = Guid.NewGuid();
                    newEntity.SummonerId = summonerId;

                    createList.Add(newEntity);
                }
                //update
                else
                {
                    if (oldEntity.AlternateName != newAltAccount.AlternateName
                        || oldEntity.OpGGUrlLink != newAltAccount.OpGgUrlLink)
                    {
                        oldEntity.AlternateName = newAltAccount.AlternateName;
                        oldEntity.OpGGUrlLink = newAltAccount.OpGgUrlLink;
                        updateList.Add(oldEntity);
                    }
                }
            }

            //delete
            foreach (var oldAltAccount in kvp)
            {
                if (!newList.Contains(oldAltAccount.Key))
                {
                    deleteList.Add(oldAltAccount.Value);
                }
            }

            var createTask = _alternateAccountRepository.CreateAsync(createList);
            var updateTask = _alternateAccountRepository.UpdateAsync(updateList);
            var deleteTask = _alternateAccountRepository.DeleteAsync(deleteList);
            return await createTask && await updateTask && await deleteTask;
        }

        public async Task<SummonerInfoView> GetSummonerViewAsync(UserEntity user)
        {
            var summonerEntity = await _summonerInfoRepository.ReadOneByUserIdAsync(user.Id);
            if (summonerEntity == null)
            {
                return new SummonerInfoView();
            }
            var alternateAccounts = await _alternateAccountRepository.ReadAllForSummonerAsync(summonerEntity.Id);
            var altViews = _alternateAccountMapper.Map(alternateAccounts);

            var summonerInfo = _summonerMapper.Map(summonerEntity);
            summonerInfo.AlternateAccounts = altViews.ToList();
            if (summonerInfo.AlternateAccounts == null)
            {
                summonerInfo.AlternateAccounts = new List<AlternateAccountView>();
            }

            var missingCount = AltenateAccountsCount - summonerInfo.AlternateAccounts.Count;
            summonerInfo.AlternateAccounts.AddRange(AddEmptyAlternateAccountViews(missingCount));
            return summonerInfo;
        }

        public async Task<IEnumerable<SummonerInfoView>> GetRosterSummonerInfosAsync(Guid rosterId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<SummonerInfoView>> GetAllSummonersAsync()
        {
            var summonerEntities = (await _summonerInfoRepository.GetAllSummonersAsync()).ToList();
            var views = new List<SummonerInfoView>();
            foreach (var summoner in summonerEntities)
            {
                var altAccounts = await _alternateAccountRepository.ReadAllForSummonerAsync(summoner.Id);
                var view = _summonerMapper.Map(summoner);
                var altAccountViews = _alternateAccountMapper.Map(altAccounts);
                view.AlternateAccounts = altAccountViews.ToList();

                var missingCount = AltenateAccountsCount - view.AlternateAccounts.Count;
                view.AlternateAccounts.AddRange(AddEmptyAlternateAccountViews(missingCount));

                views.Add(view);
            }
           
            return views;
        }

        private IEnumerable<AlternateAccountView> AddEmptyAlternateAccountViews(int missingCount)
        {
            var list = new List<AlternateAccountView>();
            for (var i = 0; i < missingCount; i++)
            {
                list.Add(new AlternateAccountView());
            }

            return list;
        }
    }
}
