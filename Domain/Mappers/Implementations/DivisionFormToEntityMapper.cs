using System;
using DAL.Entities.LeagueInfo;
using Domain.Forms;
using Domain.Mappers.Interfaces;

namespace Domain.Mappers.Implementations
{
    public class DivisionFormToEntityMapper : IDivisionFormToEntityMapper
    {
        public DivisionEntity Map(DivisionForm form)
        {
            return new DivisionEntity
            {
                Id = Guid.NewGuid(),
                SeasonInfoId = form.SeasonInfoId,
                Name = form.Name,
                UpperLimit = form.UpperLimit,
                LowerLimit = form.LowerLimit
            };
        }
    }
}
