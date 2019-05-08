using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using DAL.Entities.LeagueInfo;
using DAL.Entities.UserData;
using Domain.Exceptions;
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
        private readonly ISummonerInfoRepository _summonerInfoRepository;
        private readonly IAlternateAccountRepository _alternateAccountRepository;
        private readonly IRequestedSummonerRepository _requestedSummonerRepository;
        private readonly ITeamPlayerRepository _teamPlayerRepository;
        private readonly ITeamRosterRepository _teamRosterRepository;
        private readonly ISeasonInfoRepository _seasonInfoRepository;
        private const int AltenateAccountsCount = 3;
        private const int RequestingSummonerCount = 6;
        private const int TeamRosterMaxCount = 7;

        public AccountService(ILogger logger, ISummonerMapper summonerMapper, IAlternateAccountMapper alternateAccountMapper,
            ISummonerInfoRepository summonerInfoRepository, IAlternateAccountRepository alternateAccountRepository,
            IRequestedSummonerRepository requestedSummonerRepository, ITeamPlayerRepository teamPlayerRepository, 
            ITeamRosterRepository teamRosterRepository, ISeasonInfoRepository seasonInfoRepository)
        {
            _logger = logger ?? 
                      throw new ArgumentNullException(nameof(logger));
            _summonerMapper = summonerMapper ?? 
                              throw new ArgumentNullException(nameof(summonerMapper));
            _alternateAccountMapper = alternateAccountMapper ??
                                      throw new ArgumentNullException(nameof(alternateAccountMapper));        
            _summonerInfoRepository = summonerInfoRepository ?? 
                                      throw new ArgumentNullException(nameof(summonerInfoRepository));
            _alternateAccountRepository = alternateAccountRepository ??
                                          throw new ArgumentNullException(nameof(alternateAccountRepository));
            _requestedSummonerRepository = requestedSummonerRepository ??
                                           throw new ArgumentNullException(nameof(requestedSummonerRepository));
            _teamPlayerRepository = teamPlayerRepository ??
                                    throw new ArgumentNullException(nameof(teamPlayerRepository));
            _teamRosterRepository = teamRosterRepository ??
                                    throw new ArgumentNullException(nameof(teamRosterRepository));
            _seasonInfoRepository = seasonInfoRepository ??
                                    throw new ArgumentNullException(nameof(seasonInfoRepository));
        }

        public async Task<bool> CreateSummonerInfoAsync(SummonerInfoView view, UserEntity user)
        {
            try
            {
                var date = DateTime.Now;
                var seasonInfo = await _seasonInfoRepository.GetActiveSeasonInfoByDate(date);

                var newEntity = _summonerMapper.Map(view);
                newEntity.Id = Guid.NewGuid();
                newEntity.UserId = user.Id;
                newEntity.IsValidPlayer = seasonInfo.ClosedRegistrationDate > date;

                var result = await _summonerInfoRepository.InsertAsync(newEntity);

                if (result && view.AlternateAccounts.Any())
                {
                    var alternateAccounts = _alternateAccountMapper.Map(view.AlternateAccounts).ToList();
                    foreach (var alternateAccount in alternateAccounts)
                    {
                        alternateAccount.Id = Guid.NewGuid();
                        alternateAccount.SummonerId = newEntity.Id;
                    }

                    result = await _alternateAccountRepository.CreateAsync(alternateAccounts);
                }

                return result;
            }
            catch (Exception e)
            {
                var message = $"Error creating summoner info for: {user.Email}";
                _logger.LogError(e, message);
                throw new SummonerInfoException(message, e);
            }
        }

        /// <summary>
        /// Will always go through here
        /// </summary>
        /// <param name="view"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> UpdateSummonerInfoAsync(SummonerInfoView view, UserEntity user)
        {
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
                summonerInfo.IsValidPlayer = readEntity.IsValidPlayer;

                var altAccountTask = UpdateAlternateAccountsAsync(summonerInfo.Id, view.AlternateAccounts);
                var updateSummonerInfoTask = _summonerInfoRepository.UpdateAsync(summonerInfo);
                return await altAccountTask && await updateSummonerInfoTask;
            }
            catch (SummonerInfoException)
            {
                throw;
            }
            catch (Exception e)
            {
                var message = $"Error updating summoner info for: {user.Email}";
                _logger.LogError(e, message);
                throw new Exception(message, e);
            }
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

        public async Task<SummonerRequestView> GetRequestedSummonersAsync(UserEntity user)
        {
            var summoners = (await _summonerInfoRepository.GetAllSummonersAsync()).ToList();
            var summonerEntity = summoners.First(x => x.UserId == user.Id);
            summoners.Remove(summonerEntity);

            var requestedSummonerEntities = (await _requestedSummonerRepository.ReadAllForSummonerAsync(summonerEntity.Id)).ToList();
            var view = new SummonerRequestView
            {
                SummonerName = summonerEntity.SummonerName,
                SummonerNames = summoners.Select(x => x.SummonerName).OrderBy(x => x).ToList(),
                Names = GetSelectListItems(summoners.Select(x => x.SummonerName).OrderBy(x => x))
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

        public async Task<FpSummonerView> GetFpSummonerView()
        {
            var summoners = (await _summonerInfoRepository.GetAllValidSummonersAsync()).ToDictionary(x => x.Id, x => x);
            var teams = (await _teamRosterRepository.GetAllTeamsAsync()).ToDictionary(x => x.Id, x => x);

            var usedSummoners = new Dictionary<Guid, SummonerInfoEntity>();
            var fpSummonerView = new FpSummonerView();

            foreach (var team in teams)
            {
                var players = await _teamPlayerRepository.ReadAllForRosterAsync(team.Key);
                foreach (var player in players)
                {
                    if (summoners.TryGetValue(player.SummonerId, out var summoner) && !usedSummoners.TryGetValue(player.SummonerId, out _))
                    {
                        usedSummoners.Add(player.SummonerId, summoner);
                        var mapped = _summonerMapper.Map(summoner);
                        fpSummonerView.SummonerInfos.Add(new FpSummonerInfo
                        {
                            RosterId = team.Key,
                            SummonerName = summoner.SummonerName,
                            Role = mapped.Role,
                            OffRole = mapped.OffRole,
                            TierDivision = mapped.TierDivision,
                            OpGgUrl = summoner.OpGGUrlLink,
                            TeamName = team.Value.TeamName
                        });
                    }
                    else
                    {
                        _logger.LogError($"Dupe player alert for: {summoner?.SummonerName ?? player.SummonerId.ToString()}");
                    }
                }
            }

            var remainingSummoners = summoners.Except(usedSummoners).Concat(usedSummoners.Except(summoners));

            foreach (var remainingSummoner in remainingSummoners)
            {
                var mapped = _summonerMapper.Map(remainingSummoner.Value);
                fpSummonerView.SummonerInfos.Add(new FpSummonerInfo
                {
                    SummonerName = mapped.SummonerName,
                    Role = mapped.Role,
                    OffRole = mapped.OffRole,
                    TierDivision = mapped.TierDivision,
                    OpGgUrl = mapped.OpGgUrl,
                    TeamName = "Unassigned"
                });
            }

            return fpSummonerView;
        }

        public async Task<SeasonInfoViewPartial> GetSeasonInfoAsync()
        {
            var seasonInfo = await _seasonInfoRepository.GetActiveSeasonInfoByDate(DateTime.Now);

            var partialSeasonView = new SeasonInfoViewPartial
            {
                ClosedRegistrationDate = seasonInfo.ClosedRegistrationDate,
                SeasonStartDate = seasonInfo.SeasonStartDate,
                SeasonName = seasonInfo.SeasonName
            };
            return partialSeasonView;
        }

        public async Task<List<RequestedPlayersView>> GetRequestedPlayersAsync()
        {
            var summoners = (await _summonerInfoRepository.GetAllValidSummonersAsync()).ToDictionary(x => x.Id, x => x);
            var requests = (await _requestedSummonerRepository.ReadAllAsync()).GroupBy(x => x.SummonerId)
                .ToDictionary(y => y.Key, y => y.ToList());
            var players = (await _teamPlayerRepository.ReadAllAsync()).ToList();

            var onRosters = new List<string>();
            var usedSummoners = new List<Guid>();
            var tempTeams = new List<RequestedPlayersView>();
            foreach (var requestedList in requests.Values)
            {
                var summoner = summoners[requestedList.First().SummonerId];
                if (!usedSummoners.Contains(summoner.Id))
                {
                    usedSummoners.Add(summoner.Id);
                }

                var onRoster = players.FirstOrDefault(x => x.SummonerId == summoner.Id);
                if (onRoster != null)
                {
                    onRosters.Add(summoner.SummonerName);
                }
                var summonerMapped = _summonerMapper.Map(summoner);
                var partial = new PartialSummonerView
                {
                    SummonerName = summonerMapped.SummonerName,
                    RoleForTeam = summonerMapped.Role,
                    Rank = summonerMapped.TierDivision
                };

                var existingGroup = tempTeams.FirstOrDefault(x => x.Summoners.Contains(partial));
                if (existingGroup == null)
                {
                    existingGroup = new RequestedPlayersView
                    {
                        Summoners = new List<PartialSummonerView>
                        {
                            partial
                        },
                        Id = Guid.NewGuid()
                    };
                }

                foreach (var requestEntity in requestedList)
                {
                    summoners.TryGetValue(requestEntity.SummonerRequestedId, out var requestedSummoner);
                    if (requestedSummoner == null)
                    {
                        continue;
                    }
                    if (!usedSummoners.Contains(requestedSummoner.Id))
                    {
                        usedSummoners.Add(requestedSummoner.Id);
                    }
                    onRoster = players.FirstOrDefault(x => x.SummonerId == requestedSummoner.Id);
                    if (onRoster != null)
                    {
                        onRosters.Add(requestedSummoner.SummonerName);
                    }
                    var requestedMapped = _summonerMapper.Map(requestedSummoner);
                    var requestedPartail = new PartialSummonerView
                    {
                        SummonerName = requestedMapped.SummonerName,
                        RoleForTeam = requestedMapped.Role,
                        Rank = requestedMapped.TierDivision
                    };

                    var view = tempTeams.FirstOrDefault(x => x.Summoners.Contains(requestedPartail));
                    if (view == null)
                    {
                        existingGroup.Summoners.Add(requestedPartail);
                    }
                    else
                    {
                        if (view == existingGroup)
                        {
                            //Do nothing
                        }
                        else
                        {
                            view.Summoners.AddRange(existingGroup.Summoners);
                            existingGroup = view;
                        }
                    }
                }

                var oldGroup = tempTeams.FirstOrDefault(x => x.Id == existingGroup.Id);
                if (oldGroup == null)
                {
                    existingGroup.Summoners = existingGroup.Summoners.Distinct().ToList();
                    tempTeams.Add(existingGroup);
                }
                else
                {
                    oldGroup.Summoners.AddRange(existingGroup.Summoners);
                    oldGroup.Summoners = oldGroup.Summoners.Distinct().ToList();
                }
            }

            foreach (var usedSummoner in usedSummoners)
            {
                summoners.Remove(usedSummoner);
            }

            foreach (var summonerInfoEntity in summoners.Values)
            {
                var summonerMapped = _summonerMapper.Map(summonerInfoEntity);
                tempTeams.Add(new RequestedPlayersView
                {
                    Summoners = new List<PartialSummonerView>
                    {
                        new PartialSummonerView
                        {
                            SummonerName = summonerMapped.SummonerName,
                            RoleForTeam = summonerMapped.Role,
                            Rank = summonerMapped.TierDivision
                        }
                    }
                });
            }

            var tempList = new List<RequestedPlayersView>(tempTeams);
            foreach (var tempTeam in tempList)
            {
                var summonerNames = tempTeam.Summoners.Select(x => x.SummonerName).ToList();
                var match = summonerNames.Intersect(onRosters);
                if (match.Any())
                {
                    tempTeams.Remove(tempTeam);
                }

                tempTeam.CleanupList();
                var missingCount = TeamRosterMaxCount - tempTeam.Summoners.Count;
                if (missingCount > 0)
                {
                    tempTeam.Summoners.AddRange(AddEmptyPartialViews(missingCount));
                }

            }

            return tempTeams;
        }

        #region private helpers
        private static IEnumerable<AlternateAccountView> AddEmptyAlternateAccountViews(int missingCount)
        {
            var list = new List<AlternateAccountView>();
            for (var i = 0; i < missingCount; i++)
            {
                list.Add(new AlternateAccountView());
            }

            return list;
        }

        private static IEnumerable<PartialSummonerView> AddEmptyPartialViews(int missingCount)
        {
            var list = new List<PartialSummonerView>();
            for (var i = 0; i < missingCount; i++)
            {
                list.Add(new PartialSummonerView());
            }

            return list;
        }

        private static IEnumerable<RequestedSummoner> AddEmptyRequestedSummonerViews(int missingCount)
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
