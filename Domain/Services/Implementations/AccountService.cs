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
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace Domain.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly string DefaultText = "--Select a Summoner Name--";

        private readonly ILogger _logger;
        private readonly ISummonerMapper _summonerMapper;
        private readonly IAlternateAccountMapper _alternateAccountMapper;
        private readonly ILookupRepository _lookupRepository;
        private readonly ISummonerInfoRepository _summonerInfoRepository;
        private readonly IAlternateAccountRepository _alternateAccountRepository;
        private readonly IRequestedSummonerRepository _requestedSummonerRepository;
        private const int AltenateAccountsCount = 3;
        private const int RequestingSummonerCount = 6;

        public AccountService(ILogger logger, ISummonerMapper summonerMapper, IAlternateAccountMapper alternateAccountMapper,
            ILookupRepository lookupRepository, ISummonerInfoRepository summonerInfoRepository,
            IAlternateAccountRepository alternateAccountRepository, IRequestedSummonerRepository requestedSummonerRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _summonerMapper = summonerMapper ?? throw new ArgumentNullException(nameof(summonerMapper));
            _alternateAccountMapper = alternateAccountMapper ??
                                      throw new ArgumentNullException(nameof(alternateAccountMapper));

            _lookupRepository = lookupRepository ?? throw new ArgumentNullException(nameof(lookupRepository));
            _summonerInfoRepository = summonerInfoRepository ?? throw new ArgumentNullException(nameof(summonerInfoRepository));
            _alternateAccountRepository = alternateAccountRepository ??
                                          throw new ArgumentNullException(nameof(alternateAccountRepository));
            _requestedSummonerRepository = requestedSummonerRepository ??
                                           throw new ArgumentNullException(nameof(requestedSummonerRepository));
        }

        public async Task<bool> CreateSummonerInfoAsync(SummonerInfoView view, UserEntity user)
        {
            var result = false;
            try
            {
                var newEntity = _summonerMapper.Map(view);
                newEntity.Id = Guid.NewGuid();
                newEntity.UserId = user.Id;
                newEntity.IsValidPlayer = true;

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
                var emptyInfo = new SummonerInfoView();
                emptyInfo.AlternateAccounts.AddRange(AddEmptyAlternateAccountViews(AltenateAccountsCount));
                return emptyInfo;
            }
            var alternateAccounts = await _alternateAccountRepository.ReadAllForSummonerAsync(summonerEntity.Id);
            var altViews = _alternateAccountMapper.Map(alternateAccounts);

            var summonerInfo = _summonerMapper.Map(summonerEntity);
            summonerInfo.AlternateAccounts = altViews.ToList();

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

        public async Task<SummonerRequestView> GetRequestedSummonersAsync(UserEntity user)
        {
            var summoners = (await _summonerInfoRepository.GetAllSummonersAsync()).ToList();
            var summonerEntity = summoners.First(x => x.UserId == user.Id);
            summoners.Remove(summonerEntity);

            var requestedSummonerEntities = (await _requestedSummonerRepository.ReadAllForSummonerAsync(summonerEntity.Id)).ToList();
            var view = new SummonerRequestView
            {
                SummonerName = summonerEntity.SummonerName,
                SummonerNames = summoners.Select(x => x.SummonerName).ToList(),
                Names = GetSelectListItems(summoners.Select(x => x.SummonerName))
            };
            if (!requestedSummonerEntities.Any())
            {
                view.RequestedSummoners.AddRange(AddEmptyRequestedSummonerViews(RequestingSummonerCount));
            }
            else
            {
                var requestedSummoners = new List<RequestedSummoner>();

                foreach (var entity in requestedSummonerEntities)
                {
                    var summoner = summoners.First(x => x.Id == entity.SummonerRequestedId);
                    requestedSummoners.Add(new RequestedSummoner
                    {
                        SummonerName = summoner.SummonerName,
                        IsSub = entity.IsSub
                    });
                }

                view.RequestedSummoners = requestedSummoners;

                var missingCount = RequestingSummonerCount - view.RequestedSummoners.Count;
                view.RequestedSummoners.AddRange(AddEmptyRequestedSummonerViews(missingCount));
            }
            SetIsSubToElements(view);
            return view;
        }

        public async Task<bool> UpdateSummonerRequestsAsync(UserEntity user, SummonerRequestView view)
        {
            SetIsSubToElements(view);
            var summoners = (await _summonerInfoRepository.GetAllSummonersAsync()).ToDictionary(x => x.Id, x => x);
            var summonerEntity = summoners.First(x => x.Value.UserId == user.Id);
            summoners.Remove(summonerEntity.Key);

            var requestedSummonerEntities = (await _requestedSummonerRepository.ReadAllForSummonerAsync(summonerEntity.Key)).ToList();

            //pulled from db
            var kvp = new Dictionary<string, SummonerRequestEntity>();
            foreach (var requestEntity in requestedSummonerEntities)
            {
                if (summoners.TryGetValue(requestEntity.SummonerRequestedId, out var summoner))
                {
                    kvp.TryAdd(summoner.SummonerName, requestEntity);
                }
            }

            var createList = new List<SummonerRequestEntity>();
            var updateList = new List<SummonerRequestEntity>();
            var deleteList = new List<SummonerRequestEntity>();

            //new list
            foreach (var requestedSummoner in view.RequestedSummoners)
            {
                //skip these
                if (requestedSummoner.SummonerName == null || requestedSummoner.SummonerName == DefaultText)
                {
                    continue;
                }
                //insert
                if (!kvp.TryGetValue(requestedSummoner.SummonerName, out var oldSummonerRequest))
                {
                    var newEntity = new SummonerRequestEntity
                    {
                        Id = Guid.NewGuid(),
                        SummonerId = summonerEntity.Key,
                        SummonerRequestedId = summoners.First(x => x.Value.SummonerName == requestedSummoner.SummonerName).Key,
                        IsSub = requestedSummoner.IsSub
                    };
                    createList.Add(newEntity);
                }
                //update
                else
                {
                    oldSummonerRequest.SummonerRequestedId =
                        summoners.First(x => x.Value.SummonerName == requestedSummoner.SummonerName).Key;
                    oldSummonerRequest.IsSub = requestedSummoner.IsSub;
                    updateList.Add(oldSummonerRequest);
                }
            }

            //delete
            foreach (var oldEntity in kvp)
            {
                if (!view.RequestedSummoners.Select(x => x.SummonerName).Contains(oldEntity.Key))
                {
                    deleteList.Add(oldEntity.Value);
                }
            }

            var createTask = _requestedSummonerRepository.CreateAsync(createList);
            var updateTask = _requestedSummonerRepository.UpdateAsync(updateList);
            var deleteTask = _requestedSummonerRepository.DeleteAsync(deleteList);
            return await createTask && await updateTask && await deleteTask;
        }

        #region private helpers
        private IEnumerable<AlternateAccountView> AddEmptyAlternateAccountViews(int missingCount)
        {
            var list = new List<AlternateAccountView>();
            for (var i = 0; i < missingCount; i++)
            {
                list.Add(new AlternateAccountView());
            }

            return list;
        }

        private IEnumerable<RequestedSummoner> AddEmptyRequestedSummonerViews(int missingCount)
        {
            var list = new List<RequestedSummoner>();
            for (var i = 0; i < missingCount; i++)
            {
                list.Add(new RequestedSummoner());
            }

            return list;
        }

        private List<SelectListItem> GetSelectListItems(IEnumerable<string> summonerNames)
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem
            {
                Text = DefaultText,
                Value = ""
            });

            foreach (var summonerName in summonerNames)
            {
                list.Add(new SelectListItem
                {
                    Text = summonerName,
                    Value = summonerName
                });
            }

            return list;
        }

        private static void SetIsSubToElements(SummonerRequestView view)
        {
            try
            {
                view.RequestedSummoners[4].IsSub = true;
                view.RequestedSummoners[5].IsSub = true;
            }
            catch (Exception)
            {
                //ignore
            }
        }
        #endregion
    }
}
