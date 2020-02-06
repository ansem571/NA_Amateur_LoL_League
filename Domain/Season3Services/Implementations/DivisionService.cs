using System;
using System.Threading.Tasks;
using Domain.Forms;
using Domain.Helpers;
using Domain.Mappers.Interfaces;
using Domain.Repositories.Interfaces;
using Domain.Season3Services.Interfaces;

namespace Domain.Season3Services.Implementations
{
    public class DivisionService : IDivisionService
    {
        private readonly IDivisionRepository _divisionRepository;
        private readonly IDivisionFormToEntityMapper _formToEntityMapper;
        public DivisionService(IDivisionRepository divisionRepository, IDivisionFormToEntityMapper formToEntityMapper)
        {
            _divisionRepository = divisionRepository ?? throw new ArgumentNullException(nameof(divisionRepository));
            _formToEntityMapper = formToEntityMapper ?? throw new ArgumentNullException(nameof(formToEntityMapper));
        }

        public async Task<bool> CreateDivisionAsync(DivisionForm form)
        {
            var entity = _formToEntityMapper.Map(form);

            return await _divisionRepository.CreateDivisionAsync(entity);
        }

        public async Task<bool> UpdateDivisionAsync(Guid divisionId, DivisionForm form)
        {
            var entity = await _divisionRepository.GetByIdAsync(divisionId);


            if (!form.SeasonInfoId.IsNullOrEmpty() && form.SeasonInfoId != entity.SeasonInfoId)
            {
                entity.SeasonInfoId = form.SeasonInfoId;
            }

            if (!form.Name.IsNullOrEmpty() && form.Name != entity.Name)
            {
                entity.Name = form.Name;
            }

            if (!form.UpperLimit.IsNullOrEmpty() && form.UpperLimit != entity.UpperLimit)
            {
                entity.UpperLimit = form.UpperLimit;
            }

            if (!form.LowerLimit.IsNullOrEmpty() && form.LowerLimit != entity.LowerLimit)
            {
                entity.LowerLimit = form.LowerLimit;
            }

            return await _divisionRepository.UpdateDivisionAsync(entity);
        }

        public async Task<bool> DeleteDivisionAsync(Guid divisionId)
        {
            var entity = await _divisionRepository.GetByIdAsync(divisionId);

            return await _divisionRepository.DeleteDivisionAsync(entity);
        }
    }
}
