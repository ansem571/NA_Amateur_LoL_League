using DAL.Entities.LeagueInfo;
using Domain.Forms;

namespace Domain.Mappers.Interfaces
{
    public interface IDivisionFormToEntityMapper
    {
        DivisionEntity Map(DivisionForm form);
    }
}
