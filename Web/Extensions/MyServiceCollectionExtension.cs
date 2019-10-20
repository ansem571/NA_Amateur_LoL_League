using DAL.Data.Interfaces;
using DAL.Databases.Implementations;
using DAL.Databases.Interfaces;
using DAL.Entities.LeagueInfo;
using DAL.Entities.Logging;
using DAL.Entities.UserData;
using DAL.Stores;
using DAL.Stores.TSR;
using Domain.Mappers.Implementations;
using Domain.Mappers.Interfaces;
using Domain.Repositories.Implementations;
using Domain.Repositories.Interfaces;
using Domain.Services.Implementations;
using Domain.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Web.Extensions
{
    public static class MyServiceCollectionExtension
    {

        public static IServiceCollection AddMyServices(this IServiceCollection services, IConfiguration config)
        {
            return services
                .AddDbConnections(config)
                .AddStores()
                .AddMappers()
                .AddRepositories()
                .AddServices()
                .AddLogService();
        }

        public static IServiceCollection AddLogService(this IServiceCollection services)
        {
            return services.AddSingleton<ILogger, DbLogService>(x =>
            {
                var logStore = x.GetService<ITableStorageRepository<MessageLogEntity>>();
                var mapper = x.GetService<ILogLevelMapper>();
                var dbLogger = new DbLogService(logStore, mapper, "Ca.S.U.L.");
                return dbLogger;
            });
        }

        public static IServiceCollection AddDbConnections(this IServiceCollection services, IConfiguration config)
        {
            var connString = config.GetConnectionString("MainDb");
            services.AddSingleton<IDatabase, SqlDatabase>(x => new SqlDatabase(connString));
            return services;
        }

        public static IServiceCollection AddStores(this IServiceCollection services)
        {
            //For identity purposes
            services.TryAddSingleton<IUserStore<UserEntity>, UserStore>();
            services.TryAddSingleton<IRoleStore<UserRoleEntity>, RoleStore>();

            services.TryAddSingleton<ITableStorageRepository<MessageLogEntity>, MessageLogStore>();
            services.TryAddSingleton<ITableStorageRepository<UserRoleRelationEntity>, UserRoleStore>();
            services.TryAddSingleton<ITableStorageRepository<LookupEntity>, LookupStore>();
            services.TryAddSingleton<ITableStorageRepository<SummonerInfoEntity>, SummonerInfoStore>();
            services.TryAddSingleton<ITableStorageRepository<AlternateAccountEntity>, AlternateAccountStore>();
            services.TryAddSingleton<ITableStorageRepository<SummonerRequestEntity>, SummonerRequestStore>();
            services.TryAddSingleton<ITableStorageRepository<TeamRosterEntity>, TeamRosterStore>();
            services.TryAddSingleton<ITableStorageRepository<TeamPlayerEntity>, TeamPlayerStore>();
            services.TryAddSingleton<ITableStorageRepository<TeamCaptainEntity>, TeamCaptainStore>();
            services.TryAddSingleton<ITableStorageRepository<SeasonInfoEntity>, SeasonInfoStore>();
            services.TryAddSingleton<ITableStorageRepository<DivisionEntity>, DivisionStore>();
            services.TryAddSingleton<ITableStorageRepository<PlayerStatsEntity>, PlayerStatsStore>();
            services.TryAddSingleton<ITableStorageRepository<ScheduleEntity>, ScheduleStore>();
            services.TryAddSingleton<ITableStorageRepository<BlacklistEntity>, BlacklistStore>();
            services.TryAddSingleton<ITableStorageRepository<GoogleDriveFolderEntity>, GoogleDriveFolderStore>();
            services.TryAddSingleton<ITableStorageRepository<AchievementEntity>, AchievementStore>();
            return services;
        }

        public static IServiceCollection AddMappers(this IServiceCollection services)
        {
            services.TryAddSingleton<ILogLevelMapper, LogLevelMapper>();
            services.TryAddSingleton<IPhoneMapper, PhoneMapper>();
            services.TryAddSingleton<ISummonerRoleMapper, SummonerRoleMapper>();
            services.TryAddSingleton<ITierDivisionMapper, TierDivisionMapper>();
            services.TryAddSingleton<ISummonerMapper, SummonerMapper>();
            services.TryAddSingleton<IAlternateAccountMapper, AlternateAccountMapper>();
            services.TryAddSingleton<IPlayerStatsMapper, PlayerStatsMapper>();
            services.TryAddSingleton<IScheduleMapper, ScheduleMapper>();
            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.TryAddSingleton<ILookupRepository, LookupRepository>();
            services.TryAddSingleton<ISummonerInfoRepository, SummonerInfoRepository>();
            services.TryAddSingleton<IAlternateAccountRepository, AlternateAccountRepository>();
            services.TryAddSingleton<IRequestedSummonerRepository, RequestedSummonerRepository>();
            services.TryAddSingleton<ITeamRosterRepository, TeamRosterRepository>();
            services.TryAddSingleton<ITeamPlayerRepository, TeamPlayerRepository>();
            services.TryAddSingleton<ITeamCaptainRepository, TeamCaptainRepository>();
            services.TryAddSingleton<ISeasonInfoRepository, SeasonInfoRepository>();
            services.TryAddSingleton<IDivisionRepository, DivisionRepository>();
            services.TryAddSingleton<IPlayerStatsRepository, PlayerStatsRepository>();
            services.TryAddSingleton<IScheduleRepository, ScheduleRepository>();
            services.TryAddSingleton<IBlacklistRepository, BlacklistRepository>();
            services.TryAddSingleton<IGoogleDriveFolderRepository, GoogleDriveFolderRepository>();
            services.TryAddSingleton<IAchievementRepository, AchievementRepository>();
            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.TryAddSingleton<IAccountService, AccountService>();
            services.TryAddSingleton<IEmailService, EmailService>();
            services.TryAddSingleton<IAdminService, AdminService>();
            services.TryAddSingleton<IRosterService, RosterService>();
            services.TryAddSingleton<IScheduleService, ScheduleService>();
            services.TryAddSingleton<IGoogleDriveService, GoogleDriveService>();
            services.TryAddSingleton<IPlayerProfileService, PlayerProfileService>();
            return services;
        }
    }
}
