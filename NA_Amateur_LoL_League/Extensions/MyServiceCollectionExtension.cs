using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Databases;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Web.Extensions
{
    public static class MyServiceCollectionExtension
    {

        public static IServiceCollection AddMyServices(this IServiceCollection services, IConfiguration config)
        {
            return services
                .AddDbConnections(config)
                .AddStores();
        }

        public static IServiceCollection AddDbConnections(this IServiceCollection services, IConfiguration config)
        {
            var connString = config["ConnectionStrings:MainDb"];
            services.AddSingleton<IDatabase, Database>(x => new Database(connString));
            return services;
        }

        public static IServiceCollection AddStores(this IServiceCollection services)
        {
            //services.TryAddSingleton();
            return services;
        }
    }
}
