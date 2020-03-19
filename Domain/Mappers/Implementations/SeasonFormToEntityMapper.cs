using System;
using DAL.Entities.LeagueInfo;
using Domain.Forms;
using Domain.Mappers.Interfaces;

namespace Domain.Mappers.Implementations
{
    public class SeasonFormToEntityMapper : ISeasonFormToEntityMapper
    {
        public SeasonInfoEntity Map(SeasonInfoForm form)
        {
            return new SeasonInfoEntity
            {
                Id = Guid.NewGuid(),
                SeasonName = form.SeasonName,
                CreatedOn = DateTime.Now,
                ClosedRegistrationDate = form.ClosedRegistrationDate,
                SeasonStartDate = form.SeasonStartDate
            };
        }
    }
}
