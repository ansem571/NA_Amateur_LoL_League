using DAL.Entities.LeagueInfo;
using Domain.Forms;

namespace Domain.Mappers.Interfaces
{
    public interface ISeasonFormToEntityMapper
    {
        SeasonInfoEntity Map(SeasonInfoForm form);
    }
}
