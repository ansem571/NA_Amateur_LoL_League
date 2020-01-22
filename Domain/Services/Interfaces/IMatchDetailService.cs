using System.Threading.Tasks;
using DAL.Entities.UserData;
using Domain.Views;

namespace Domain.Services.Interfaces
{
    public interface IMatchDetailService
    {
        Task<bool> SendFileData(MatchSubmissionView view, UserEntity user);
    }
}
