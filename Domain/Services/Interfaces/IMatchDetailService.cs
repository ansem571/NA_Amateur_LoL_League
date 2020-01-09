using System.Threading.Tasks;
using Domain.Views;

namespace Domain.Services.Interfaces
{
    public interface IMatchDetailService
    {
        Task<bool> SendFileData(MatchSubmissionView view);
    }
}
