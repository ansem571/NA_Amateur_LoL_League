using System;
using System.Threading.Tasks;
using Domain.Forms;

namespace Domain.Season3Services.Interfaces
{
    public interface IDivisionService
    {
        Task<bool> CreateDivisionAsync(DivisionForm form);
        Task<bool> UpdateDivisionAsync(Guid divisionId, DivisionForm form);
        Task<bool> DeleteDivisionAsync(Guid divisionId);
    }
}
