using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using DAL.Entities.UserData;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace DAL.Stores
{
    public class RoleStore : IRoleStore<UserRoleEntity>
    {
        private readonly string _connectionString;

        public RoleStore(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MainDb");
        }

        public async Task<IdentityResult> CreateAsync(UserRoleEntity role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                role.Id = Guid.NewGuid();
                await connection.QueryAsync($@"INSERT INTO [UserRole] ([Id], [Name], [NormalizedName])
                    VALUES (@{nameof(UserRoleEntity.Id)}, @{nameof(UserRoleEntity.Name)}, @{nameof(UserRoleEntity.NormalizedName)});", role, commandTimeout: 60 * 5);
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(UserRoleEntity role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                await connection.ExecuteAsync($@"UPDATE [UserRole] SET
                    [Name] = @{nameof(UserRoleEntity.Name)},
                    [NormalizedName] = @{nameof(UserRoleEntity.NormalizedName)}
                    WHERE [Id] = @{nameof(UserRoleEntity.Id)}", role);
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(UserRoleEntity role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                await connection.ExecuteAsync($"DELETE FROM [UserRole] WHERE [Id] = @{nameof(UserRoleEntity.Id)}", role);
            }

            return IdentityResult.Success;
        }

        public Task<string> GetRoleIdAsync(UserRoleEntity role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Id.ToString());
        }

        public Task<string> GetRoleNameAsync(UserRoleEntity role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Name);
        }

        public Task SetRoleNameAsync(UserRoleEntity role, string roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
            return Task.FromResult(0);
        }

        public Task<string> GetNormalizedRoleNameAsync(UserRoleEntity role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.NormalizedName);
        }

        public Task SetNormalizedRoleNameAsync(UserRoleEntity role, string normalizedName, CancellationToken cancellationToken)
        {
            role.NormalizedName = normalizedName;
            return Task.FromResult(0);
        }

        public async Task<UserRoleEntity> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                return await connection.QuerySingleOrDefaultAsync<UserRoleEntity>($@"SELECT * FROM [UserRole]
                    WHERE [Id] = @{nameof(roleId)}", new { roleId });
            }
        }

        public async Task<UserRoleEntity> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                return await connection.QuerySingleOrDefaultAsync<UserRoleEntity>($@"SELECT * FROM [UserRole]
                    WHERE [NormalizedName] = @{nameof(normalizedRoleName)}", new { normalizedRoleName });
            }
        }

        public void Dispose()
        {
            // Nothing to dispose.
        }
    }
}
