using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Data.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IDbConnection Connection { get; }

        IDbTransaction Transaction { get; }
        IDbTransaction BeginTransaction();
        void Commit();
        void Rollback();

        Task<int> ExecuteAsync(string sql, object param = null, CommandType? commandType = null, int commandTimeout = 30);
        Task<dynamic> QueryAsync(string sql, object param = null, CommandType? commandType = null, int commandTimeout = 30);
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, CommandType? commandType = null, int commandTimeout = 30);
    }
}
