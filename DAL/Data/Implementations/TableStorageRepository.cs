using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using DAL.Data.Interfaces;
using DAL.Databases.Interfaces;
using DAL.Extensions;

namespace DAL.Data.Implementations
{
    public class TableStorageRepository<TEntity> : ITableStorageRepository<TEntity> where TEntity : class, new()
    {
        public IDatabase Database { get; private set; }
        public TableStorageRepository(IDatabase database)
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));
        }

        public async Task<TEntity> ReadOneAsync(object id, IUnitOfWork unitOfWork = null, bool withNoLock = true)
        {
            var conn = unitOfWork?.Connection ?? await Database.CreateOpenConnectionAsync();
            try
            {
                var key = _keyProperties.FirstOrDefault()?.Name ?? "Id";
                var sql = SelectStatement($@"{key.Bracket(Database.GetStartBracket(), Database.GetEndBracket())} = @id", withNoLock: withNoLock);
                return (await conn.QueryAsync<TEntity>(sql, new { id }, commandTimeout: 60 * 5, transaction: unitOfWork?.Transaction)).FirstOrDefault();
            }
            finally
            {
                if (unitOfWork == null)
                {
                    conn.Dispose();
                }
            }
        }

        public async Task<IEnumerable<TEntity>> ReadAllAsync(IUnitOfWork unitOfWork = null, bool withNoLock = true)
        {
            var conn = unitOfWork?.Connection ?? await Database.CreateOpenConnectionAsync();
            try
            {
                var sql = SelectStatement(withNoLock: withNoLock);
                return await conn.QueryAsync<TEntity>(sql, commandTimeout: 60 * 5, transaction: unitOfWork?.Transaction);
            }
            finally
            {
                if (unitOfWork == null)
                {
                    conn.Dispose();
                }
            }
        }

        public async Task<IEnumerable<TEntity>> ReadManyAsync(string where = null, object param = null, string orderBy = null,
            int? top = null, int? page = null, int pageSize = 25, IEnumerable<string> cols = null, IUnitOfWork unitOfWork = null, bool withNoLock = true)
        {
            var conn = unitOfWork?.Connection ?? await Database.CreateOpenConnectionAsync();
            try
            {
                var sql = SelectStatement(where, orderBy, top, page, pageSize, cols, withNoLock);
                return await conn.QueryAsync<TEntity>(sql, param: param, commandTimeout: 60 * 5, transaction: unitOfWork?.Transaction);
            }
            finally
            {
                if (unitOfWork == null)
                {
                    conn.Dispose();
                }
            }
        }

        public async Task<int> InsertAsync(TEntity item, IUnitOfWork unitOfWork = null)
        {
            var conn = unitOfWork?.Connection ?? await Database.CreateOpenConnectionAsync();

            try
            {
                await conn.InsertAsync(item, unitOfWork?.Transaction, 60);
                return 1;
            }
            finally
            {
                if (unitOfWork == null)
                {
                    conn.Dispose();
                }
            }
        }

        public async Task<int> InsertAsync(IEnumerable<TEntity> items, IUnitOfWork unitOfWork = null)
        {
            var conn = unitOfWork?.Connection ?? await Database.CreateOpenConnectionAsync();
            try
            {
                var sbColumns = new StringBuilder();
                var sbParams = new StringBuilder();
                for (var i = 0; i < _writeableProperties.Count; ++i)
                {
                    var property = _writeableProperties[i];

                    sbColumns.Append(property.Name.Bracket(Database.GetStartBracket(), Database.GetEndBracket()));
                    sbParams.Append($"@{property.Name}");

                    if (i < _writeableProperties.Count - 1)
                    {
                        sbColumns.Append(", ");
                        sbParams.Append(", ");
                    }
                }

                var sql = $"insert into {_tableName.Bracket(Database.GetStartBracket(), Database.GetEndBracket())} ({sbColumns}) values ({sbParams})";

                return await conn.ExecuteAsync(sql, items, unitOfWork?.Transaction, 60);
            }
            finally
            {
                if (unitOfWork == null)
                {
                    conn.Dispose();
                }
            }
        }

        public async Task<bool> UpdateAsync(TEntity item, IUnitOfWork unitOfWork = null)
        {
            var conn = unitOfWork?.Connection ?? await Database.CreateOpenConnectionAsync();
            try
            {
                return await conn.UpdateAsync(item, unitOfWork?.Transaction, 60);
            }
            finally
            {
                if (unitOfWork == null)
                {
                    conn.Dispose();
                }
            }
        }

        public async Task<bool> UpdateAsync(IEnumerable<TEntity> items, IUnitOfWork unitOfWork = null)
        {
            var conn = unitOfWork?.Connection ?? await Database.CreateOpenConnectionAsync();
            try
            {
                if (!_keyProperties.Any())
                {
                    throw new Exception("Must specify Key to update records.");
                }

                var sb = new StringBuilder();
                sb.Append($"update {_tableName.Bracket(Database.GetStartBracket(), Database.GetEndBracket())} set ");

                var writableProperties = _writeableProperties.Except(_keyProperties).ToList();
                for (var i = 0; i < writableProperties.Count; ++i)
                {
                    var property = writableProperties[i];
                    sb.Append($"{property.Name.Bracket(Database.GetStartBracket(), Database.GetEndBracket())} = @{property.Name}");
                    if (i < writableProperties.Count - 1)
                    {
                        sb.Append(", ");
                    }
                }
                sb.Append(" where ");
                for (var i = 0; i < _keyProperties.Count; ++i)
                {
                    var key = _keyProperties[i].Name;
                    sb.Append($"{key.Bracket(Database.GetStartBracket(), Database.GetEndBracket())} = @{key}");
                    if (i < _keyProperties.Count - 1)
                    {
                        sb.Append(" and ");
                    }
                }

                return (await conn.ExecuteAsync(sb.ToString(), items, unitOfWork?.Transaction, 60)) > 0;
            }
            finally
            {
                if (unitOfWork == null)
                {
                    conn.Dispose();
                }
            }
        }

        public async Task<bool> DeleteAsync(TEntity item, IUnitOfWork unitOfWork = null)
        {
            var conn = unitOfWork?.Connection ?? await Database.CreateOpenConnectionAsync();
            try
            {
                return await conn.DeleteAsync(item, unitOfWork?.Transaction, 60);
            }
            finally
            {
                if (unitOfWork == null)
                {
                    conn.Dispose();
                }
            }
        }

        public async Task<bool> DeleteAsync(IEnumerable<TEntity> items, IUnitOfWork unitOfWork = null)
        {
            var conn = unitOfWork?.Connection ?? await Database.CreateOpenConnectionAsync();
            try
            {
                return await conn.DeleteAsync(items, unitOfWork?.Transaction, 60);
            }
            finally
            {
                if (unitOfWork == null)
                {
                    conn.Dispose();
                }
            }
        }

        public async Task<bool> DeleteWhereAsync(string where, object param, IUnitOfWork unitOfWork = null)
        {
            var conn = unitOfWork?.Connection ?? await Database.CreateOpenConnectionAsync();
            try
            {
                return (await conn.ExecuteAsync(
                           $@"delete from {_tableName.Bracket(Database.GetStartBracket(), Database.GetEndBracket())} where {where}",
                           param, unitOfWork?.Transaction, 60)) >= 1;
            }
            finally
            {
                if (unitOfWork == null)
                {
                    conn.Dispose();
                }
            }
        }

        public async Task<bool> DeleteAllAsync(IUnitOfWork unitOfWork = null)
        {
            var conn = unitOfWork?.Connection ?? await Database.CreateOpenConnectionAsync();
            try
            {
                return await conn.DeleteAllAsync<TEntity>(unitOfWork?.Transaction);
            }
            finally
            {
                if (unitOfWork == null)
                {
                    conn.Dispose();
                }
            }
        }

        public async Task<int> BulkInsertAsync(IEnumerable<TEntity> items, int? bulkCopyTimeout = null, int batchSize = 5000, SqlBulkCopyOptions? copyOptions = null, IUnitOfWork unitOfWork = null)
        {
            var arrItems = items as TEntity[] ?? items.ToArray();
            var dt = GetDataTable(arrItems);
            dt.TableName = _tableName.Bracket(Database.GetStartBracket(), Database.GetEndBracket());

            return await BulkInsertAsync(dt, bulkCopyTimeout, batchSize, copyOptions, unitOfWork);
        }

        public async Task<int> BulkMergeAsync(IEnumerable<TEntity> items, int? bulkCopyTimeout = null, IUnitOfWork unitOfWork = null)
        {
            var uow = unitOfWork ?? await Database.CreateUnitOfWorkAsync();
            var shouldCommit = false;
            if (uow.Transaction == null)
            {
                uow.BeginTransaction();
                shouldCommit = true;
            }

            try
            {
                if (!(uow.Connection is SqlConnection sqlConn))
                {
                    throw new ArgumentException("Connection must be to a sql database for bulk insert.");
                }

                if (!(uow.Transaction is SqlTransaction sqlTran))
                {
                    throw new ArgumentException("Connection must be to a sql database for bulk insert.");
                }

                using (var cmd = new SqlCommand($"select top 0 * into #tmp from {_tableName.Bracket(Database.GetStartBracket(), Database.GetEndBracket())}", sqlConn, sqlTran))
                {
                    cmd.ExecuteNonQuery();
                }

                var arrItems = items as TEntity[] ?? items.ToArray();
                var dt = GetDataTable(arrItems);
                dt.TableName = "#tmp";

                await BulkInsertAsync(dt, bulkCopyTimeout, unitOfWork: uow);

                using (var cmd = new SqlCommand(GetMergeSqlCommand(), sqlConn, sqlTran))
                {
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = new SqlCommand($"drop table #tmp", sqlConn, sqlTran))
                {
                    cmd.ExecuteNonQuery();
                }

                return dt.Rows.Count;
            }
            finally
            {
                if (shouldCommit)
                {
                    uow.Commit();
                }

                if (unitOfWork == null)
                {
                    uow.Dispose();
                }
            }
        }

        #region helpers

        private readonly string _tableName
            = (typeof(TEntity).GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault() as TableAttribute)?.Name
            ?? typeof(TEntity).Name;

        private readonly List<PropertyInfo> _keyProperties
            = typeof(TEntity).GetProperties().Where(x =>
                    x.GetCustomAttributes(typeof(KeyAttribute), true).FirstOrDefault() is KeyAttribute ||
                    x.GetCustomAttributes(typeof(ExplicitKeyAttribute), true).FirstOrDefault() is ExplicitKeyAttribute)
                .ToList();

        private readonly List<PropertyInfo> _writeableProperties
            = typeof(TEntity).GetProperties().Where(x => (x.GetCustomAttributes(typeof(WriteAttribute), true).FirstOrDefault() as WriteAttribute)?.Write ?? true).ToList();

        private string SelectStatement(string where = null, string orderBy = null, int? top = null, int? page = null, int pagesize = 25, IEnumerable<string> cols = null, bool withNoLock = true)
        {
            var t = top != null ? "top " + top : null;
            var w = where.IsNullOrEmpty() ? null : "where " + where;
            var o = orderBy.IsNullOrEmpty() ? null : "order by " + orderBy;

            var f = page != null ? $"offset {pagesize} * ({page} - 1) rows fetch next {pagesize} rows only" : null;
            if (f != null && o == null)
            {
                o = $"order by {_tableName.Bracket(Database.GetStartBracket(), Database.GetEndBracket())}.Id";
            }

            var strcols = $"{_tableName.Bracket(Database.GetStartBracket(), Database.GetEndBracket())}.*";
            if (cols != null)
            {
                cols = cols.Select(col => $"{_tableName.Bracket(Database.GetStartBracket(), Database.GetEndBracket())}.{col.Bracket(Database.GetStartBracket(), Database.GetEndBracket())}");
                strcols = string.Join(", ", cols);
            }

            return $"select {t} {strcols} from {_tableName.Bracket(Database.GetStartBracket(), Database.GetEndBracket())} {(withNoLock ? " with (nolock)" : string.Empty)} {w} {o} {f}";
        }

        private DataTable GetDataTable(IEnumerable<TEntity> items)
        {
            var dt = GetDataTable();

            var type = typeof(TEntity);
            var properties = type.GetProperties().Where(x => (x.GetCustomAttributes(typeof(WriteAttribute), true).FirstOrDefault() as WriteAttribute)?.Write ?? true).ToList();

            foreach (var item in items)
            {
                var values = new object[properties.Count];
                for (var i = 0; i < properties.Count; ++i) values[i] = properties[i].GetValue(item) ?? DBNull.Value;
                dt.Rows.Add(values);
            }

            return dt;
        }

        private DataTable GetDataTable()
        {
            var type = typeof(TEntity);
            var properties = type.GetProperties().Where(x => (x.GetCustomAttributes(typeof(WriteAttribute), true).FirstOrDefault() as WriteAttribute)?.Write ?? true).ToList();

            var dt = new DataTable(_tableName);

            foreach (var info in properties)
            {
                dt.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
            }

            return dt;
        }

        private string GetMergeSqlCommand()
        {
            var sb = new StringBuilder();
            var joinOn = _keyProperties.Select(x => x.Name);

            sb.AppendLine($"declare @action table (action varchar(50));");
            sb.AppendLine($"merge {_tableName.Bracket(Database.GetStartBracket(), Database.GetEndBracket())} as T");
            sb.AppendLine($"using #tmp as S");
            sb.AppendLine($"on ({string.Join(" and ", joinOn.Select(x => "T." + x.Bracket(Database.GetStartBracket(), Database.GetEndBracket()) + " = S." + x.Bracket(Database.GetStartBracket(), Database.GetEndBracket())))})");
            sb.AppendLine($"when not matched by target");
            sb.AppendLine($"\tthen insert({string.Join(", ", _writeableProperties.Select(x => x.Name.Bracket(Database.GetStartBracket(), Database.GetEndBracket())))}) ");
            sb.AppendLine($"\tvalues ({string.Join(", ", _writeableProperties.Select(x => "S." + x.Name.Bracket(Database.GetStartBracket(), Database.GetEndBracket())))})");
            sb.AppendLine("when matched");
            sb.AppendLine($"\tthen update set {string.Join(", ", _writeableProperties.Select(x => "T." + x.Name.Bracket(Database.GetStartBracket(), Database.GetEndBracket()) + " = S." + x.Name.Bracket(Database.GetStartBracket(), Database.GetEndBracket())))}");
            sb.AppendLine($"output $action into @action; select * from @action;");

            return sb.ToString();
        }

        private async Task<int> BulkInsertAsync(DataTable dataTable, int? bulkCopyTimeout = null, int batchSize = 5000, SqlBulkCopyOptions? copyOptions = null, IUnitOfWork unitOfWork = null)
        {
            var conn = unitOfWork?.Connection ?? await Database.CreateOpenConnectionAsync();
            try
            {
                if (!(conn is SqlConnection sqlConn))
                {
                    throw new ArgumentException("Connection must be a SqlConnection for bulk insert.");
                }

                var sqlTran = unitOfWork?.Transaction as SqlTransaction;
                if (unitOfWork?.Transaction != null && sqlTran == null)
                {
                    throw new ArgumentException("Transaction must be a SqlTransaction for bulk insert.");
                }

                using (var sbc = new SqlBulkCopy(sqlConn, SqlBulkCopyOptions.Default | (copyOptions ?? 0), sqlTran))
                {
                    sbc.DestinationTableName = dataTable.TableName;
                    sbc.BatchSize = batchSize;
                    sbc.BulkCopyTimeout = bulkCopyTimeout ?? sbc.BulkCopyTimeout;
                    foreach (var column in dataTable.Columns)
                    {
                        sbc.ColumnMappings.Add(column.ToString(), column.ToString());
                    }
                    sbc.WriteToServer(dataTable);
                }

                return dataTable.Rows.Count;
            }
            finally
            {
                if (unitOfWork == null)
                {
                    conn.Dispose();
                }
            }
        }

        #endregion
    }
}
