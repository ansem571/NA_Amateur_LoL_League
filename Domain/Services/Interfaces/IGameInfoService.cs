using Domain.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services.Interfaces
{
    public interface IGameInfoService
    {
        Task<MatchInfo> GetGameInfoForMatch(Guid scheduleId);
    }
}
