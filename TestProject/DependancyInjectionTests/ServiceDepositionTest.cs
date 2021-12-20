using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Web;

namespace RepeatableTests.ConfigTests
{    
     /// <summary>
     /// The purpose of the Service Deposition Test is to force the Dependency Injection engine to
     /// materialize all referenced Services.
     /// 
     /// In particular it makes sure that all Service References are reconciled and that all Config
     /// strings are filled.
     /// </summary>
    public class ServiceDepositionTest
    {
        [Fact(Skip = "dev")]
        public void DepositDev() => Deposit(AppSettingsLinks.Dev);

        [Fact]
        public void DepositStaging() => Deposit(AppSettingsLinks.Staging);

        [Fact]
        public void DepositProd() => Deposit(AppSettingsLinks.Prod);

        private void Deposit(string jsonfile)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile(jsonfile)
                .Build();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IConfiguration>(config);
            serviceCollection.AddSingleton<ILoggerFactory>(new LoggerFactory());
            var startup = new Startup(config);
            startup.ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            foreach (var service in serviceCollection)
            {
                if (service.ServiceType.FullName.StartsWith("Microsoft.")
                    || service.ServiceType.FullName.StartsWith("Swashbuckle.AspNetCore.Swagger"))
                {
                    continue;
                }

                serviceProvider.GetService(service.ServiceType);
            }
        }
    }
}
