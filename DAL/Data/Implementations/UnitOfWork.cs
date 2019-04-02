using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using DAL.Data.Interfaces;

namespace DAL.Data.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        public IDbTransaction Transaction { get; private set; }
        public IDbConnection Connection { get; }

        public UnitOfWork(IDbConnection connection)
        {
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        #region Transactions

        public IDbTransaction BeginTransaction()
        {
            Transaction = Connection.BeginTransaction();
            return Transaction;
        }

        public void Commit()
        {
            try
            {
                Transaction?.Commit();
            }
            catch
            {
                try
                {
                    Transaction?.Rollback();
                }
                catch
                {
                    /* ignore rollback exception */
                }

                throw;
            }
            finally
            {
                Transaction?.Dispose();
                Transaction = null;
            }
        }

        public void Rollback()
        {
            try
            {
                Transaction?.Rollback();
            }
            finally
            {
                Transaction?.Dispose();
                Transaction = null;
            }
        }

        #endregion

        #region Execute and Query

        public async Task<int> ExecuteAsync(string sql, object param = null, CommandType? commandType = null, int commandTimeout = 30)
        {
            if (string.IsNullOrEmpty(sql))
            {
                return 0;
            }

            return await Connection.ExecuteAsync(sql, param, Transaction, commandTimeout, commandType);
        }

        public async Task<dynamic> QueryAsync(string sql, object param = null, CommandType? commandType = null, int commandTimeout = 30)
        {
            if (string.IsNullOrEmpty(sql))
            {
                return null;
            }

            return await Connection.QueryAsync(sql, param, Transaction, commandTimeout, commandType);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, CommandType? commandType = null, int commandTimeout = 30)
        {
            if (string.IsNullOrEmpty(sql))
            {
                return null;
            }

            return await Connection.QueryAsync<T>(sql, param, Transaction, commandTimeout, commandType);
        }

        #endregion

        public void Dispose()
        {
            if (Transaction != null) Rollback();
            Transaction?.Dispose();
            Connection?.Dispose();
        }
    }
}
