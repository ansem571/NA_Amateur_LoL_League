using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services.Interfaces
{
    public interface IAdminService
    {
        Task<IEnumerable<string>> GetSummonersToCreateTeamAsync();
        Task<bool> CreateNewTeamAsync(IEnumerable<string> summonerNames);
        Task<bool> RemovePlayerFromRosterAsync(string name, Guid rosterId);
    }
}
