using System;
using System.Threading.Tasks;
using Domain.Forms;
using Domain.Mappers.Interfaces;
using Domain.Repositories.Interfaces;
using Domain.Season3Services.Interfaces;

namespace Domain.Season3Services.Implementations
{
    public class SeasonInfoService : ISeasonInfoService
    {
        private readonly ISeasonInfoRepository _seasonInfoRepository;
        private readonly ISeasonFormToEntityMapper _formToEntityMapper;
        public SeasonInfoService(ISeasonInfoRepository seasonInfoRepository, ISeasonFormToEntityMapper formToEntityMapper)
        {
            _seasonInfoRepository = seasonInfoRepository ?? throw new ArgumentNullException(nameof(seasonInfoRepository));
            _formToEntityMapper = formToEntityMapper ?? throw new ArgumentNullException(nameof(formToEntityMapper));
        }

        public async Task<bool> CreateNewSeasonAsync(SeasonInfoForm form)
        {
            var entity = _formToEntityMapper.Map(form);

            return await _seasonInfoRepository.CreateSeasonAsync(entity);
        }

        public async Task<bool> UpdateSeasonEndDateAsync(DateTime endDate)
        {
            var currentSeason = await _seasonInfoRepository.GetActiveSeasonInfoByDateAsync(DateTime.Now);

            currentSeason.SeasonEndDate = endDate;

            return await _seasonInfoRepository.CreateSeasonAsync(currentSeason);
        }
    }
}
