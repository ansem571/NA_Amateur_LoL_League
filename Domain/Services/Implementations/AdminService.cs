using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Mappers.Interfaces;
using Domain.Repositories.Interfaces;
using Domain.Views;
using Microsoft.Extensions.Logging;

namespace Domain.Services.Implementations
{
    public class AdminService
    {
        private readonly ILogger _logger;
        private readonly ISummonerMapper _summonerMapper;
        private readonly IAlternateAccountMapper _alternateAccountMapper;
        private readonly ILookupRepository _lookupRepository;
        private readonly ISummonerInfoRepository _summonerInfoRepository;
        private readonly IAlternateAccountRepository _alternateAccountRepository;
        private readonly IRequestedSummonerRepository _requestedSummonerRepository;
        private readonly ITeamPlayerRepository _teamPlayerRepository;
        private readonly ITeamRosterRepository _teamRosterRepository;

        public AdminService(ILogger logger, ISummonerMapper summonerMapper, IAlternateAccountMapper alternateAccountMapper,
            ILookupRepository lookupRepository, ISummonerInfoRepository summonerInfoRepository,
            IAlternateAccountRepository alternateAccountRepository, IRequestedSummonerRepository requestedSummonerRepository,
            ITeamPlayerRepository teamPlayerRepository, ITeamRosterRepository teamRosterRepository)
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
            _teamPlayerRepository = teamPlayerRepository ??
                                    throw new ArgumentNullException(nameof(teamPlayerRepository));
            _teamRosterRepository = teamRosterRepository ??
                                    throw new ArgumentNullException(nameof(teamRosterRepository));
        }

       

        public async Task<bool> CreateTeamAsync(IEnumerable<string> summonerNames)
        {
            var summoners = await _summonerInfoRepository.GetAllForSummonerNamesAsync(summonerNames);



            return true;
        }
    }
}
