using System;
using System.Threading.Tasks;
using Domain.Forms;

namespace Domain.Season3Services.Interfaces
{
    public interface ISeasonInfoService
    {
        Task<bool> CreateNewSeasonAsync(SeasonInfoForm form);
        Task<bool> UpdateSeasonEndDateAsync(DateTime endDate);
    }
}
