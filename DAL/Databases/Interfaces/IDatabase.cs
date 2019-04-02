using System.Data;
using System.Threading.Tasks;
using DAL.Data.Interfaces;

namespace DAL.Databases.Interfaces
{
    public interface IDatabase
    {
        IDbConnection CreateConnection();
        Task<IDbConnection> CreateOpenConnectionAsync();
        Task<IUnitOfWork> CreateUnitOfWorkAsync();

        char GetStartBracket();
        char GetEndBracket();
    }
}
