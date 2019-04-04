using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DAL.Data.Implementations;
using DAL.Data.Interfaces;
using DAL.Databases.Interfaces;

namespace DAL.Databases.Implementations
{
    public class SqlDatabase : IDatabase
    {
        private readonly string _connString;

        public SqlDatabase(string connString)
        {
            _connString = connString ?? throw new ArgumentNullException(nameof(connString));
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connString);
        }

        public async Task<IDbConnection> CreateOpenConnectionAsync()
        {
            var conn = new SqlConnection(_connString);
            await conn.OpenAsync();
            return conn;
        }

        public async Task<IUnitOfWork> CreateUnitOfWorkAsync()
        {
            var conn = await CreateOpenConnectionAsync();
            var uow = new UnitOfWork(conn);
            return uow;
        }

        public char GetStartBracket() => '[';
        public char GetEndBracket() => ']';
    }
}
