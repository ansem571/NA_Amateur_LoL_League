using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Domain.Views;

namespace Domain.Services.Interfaces
{
    public interface IRosterService
    {
        Task<SeasonInfoView> GetSeasonInfoView();
        Task<IEnumerable<RosterView>> GetAllRosters();
    }
}
