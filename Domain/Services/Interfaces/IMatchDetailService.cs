using System.Threading.Tasks;
using DAL.Entities.LeagueInfo;
using Domain.Views;

namespace Domain.Services.Interfaces
{
    public interface IMatchDetailService
    {
        Task<bool> SendFileData(MatchSubmissionView view, SummonerInfoEntity userPlayer);
    }
}
