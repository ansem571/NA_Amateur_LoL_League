using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DAL.Entities.UserData;
using Domain.Helpers;
using Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using RiotSharp.Caching;
using Swashbuckle.AspNetCore.Swagger;
using Web.Extensions;

namespace Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var origins = new List<string>
            {
                "http://localhost",
                "http://casuauleal.com",
                "https://casuauleal.com",
                "http://www.casualeal.com",
                "https://www.casuauleal.com",
            };
            services
                .AddMemoryCache()
                .AddMyServices(Configuration)
                .AddCors(options =>
                {
                    services.AddCors(o => o.AddPolicy("AllowSpecificOrigin", p => p.WithOrigins(origins.ToArray())));
                })
                .AddSwaggerGen(c =>
                {
                    var assemblyName = Assembly.GetExecutingAssembly().GetName();
                    var version =
                        $"v{assemblyName.Version.Major}.{assemblyName.Version.Minor}.{assemblyName.Version.Build}";
                    c.SwaggerDoc(version,
                        new Info
                        {
                            Title = $"{assemblyName.Name} {version}",
                            Version = version
                        });
                });

            services.AddIdentity<UserEntity, UserRoleEntity>()
                .AddDefaultTokenProviders();

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/logos");

            if (!Directory.Exists("wwwroot/logos"))
            {
                Directory.CreateDirectory(path);
            }

            services.AddSingleton<IFileProvider>(
                new PhysicalFileProvider(path));

            Task.Run(() => CreateAdminRole(services)).Wait();
            Task.Run(() => CreateTribunalRole(services)).Wait();
            Task.Run(() => CreateModeratorRole(services)).Wait();

            DeleteBadImages();

            Task.Run(() => SetupChampionCache(services)).Wait();
            services.AddMvc(o =>
                {
                    o.Conventions.Add(new CommaSeparatedQueryStringConvention());
                    o.ModelMetadataDetailsProviders.Add(new RequiredBindingMetadataProvider());
                })
            .AddJsonOptions(o =>
            {
                o.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                o.SerializerSettings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
                o.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });

        }

        private async Task SetupChampionCache(IServiceCollection services)
        {
            var builder = services.BuildServiceProvider();
            var lookupRepo = builder.GetService<ILookupRepository>();
            var logger = builder.GetService<ILogger>();
            GlobalVariables.ChampionEnumCache = new Cache();
            await GlobalVariables.SetupChampionCache(lookupRepo);
            var thread = new Thread(() => GlobalVariables.UpdateCache(lookupRepo, logger));
            thread.Start();
        }

        private async Task CreateAdminRole(IServiceCollection services)
        {
            var builder = services.BuildServiceProvider();
            var roleManager = builder.GetService<RoleManager<UserRoleEntity>>();
            var userManager = builder.GetService<UserManager<UserEntity>>();

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                var role = new UserRoleEntity
                {
                    Name = "Admin"
                };
                await roleManager.CreateAsync(role);
            }

            //Add any other user who will NEED Admin privileges 
            var user = await userManager.FindByEmailAsync("jadams.macdonnell1@gmail.com");
            if (!await userManager.IsInRoleAsync(user, "Admin"))
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }

        private async Task CreateTribunalRole(IServiceCollection services)
        {
            var builder = services.BuildServiceProvider();
            var roleManager = builder.GetService<RoleManager<UserRoleEntity>>();
            var userManager = builder.GetService<UserManager<UserEntity>>();

            if (!await roleManager.RoleExistsAsync("Tribunal"))
            {
                var role = new UserRoleEntity
                {
                    Name = "Tribunal"
                };
                await roleManager.CreateAsync(role);
            }
            //Add any other user who will NEED Admin privileges 
            var user1 = await userManager.FindByEmailAsync("jadams.macdonnell1@gmail.com");
            var user2 = await userManager.FindByEmailAsync("ansem571@gmail.com");
            var user3 = await userManager.FindByEmailAsync("josiahrosendahl@gmail.com");
            var user4 = await userManager.FindByEmailAsync("gwrobinson2@gmail.com");
            var user5 = await userManager.FindByEmailAsync("michael.spindel05@gmail.com");
            var user6 = await userManager.FindByEmailAsync("scatter.catt@gmail.com");
            var user7 = await userManager.FindByEmailAsync("shadow2097@gmail.com");
            var user8 = await userManager.FindByEmailAsync("morrisonsviewpoint@gmail.com");
            var user9 = await userManager.FindByEmailAsync("gman.mcgee@gmail.com");
            var user10 = await userManager.FindByEmailAsync("Mike_Salinas112@hotmail.com");
            var users = new List<UserEntity>
            {
                user1,
                user2,
                user3,
                user4,
                user5,
                user6,
                user7,
                user8,
                user9,
                user10
            };
            foreach (var user in users)
            {
                if (!await userManager.IsInRoleAsync(user, "Tribunal"))
                {
                    await userManager.AddToRoleAsync(user, "Tribunal");
                }
            }
        }


        private async Task CreateModeratorRole(IServiceCollection services)
        {
            var builder = services.BuildServiceProvider();
            var roleManager = builder.GetService<RoleManager<UserRoleEntity>>();
            var userManager = builder.GetService<UserManager<UserEntity>>();

            if (!await roleManager.RoleExistsAsync("Moderator"))
            {
                var role = new UserRoleEntity
                {
                    Name = "Moderator"
                };
                await roleManager.CreateAsync(role);
            }

            var user1 = await userManager.FindByEmailAsync("jadams.macdonnell1@gmail.com");
            var user2 = await userManager.FindByEmailAsync("ansem571@gmail.com");
            var users = new List<UserEntity>
            {
                user1,
                user2
            };
            foreach (var user in users)
            {
                if (!await userManager.IsInRoleAsync(user, "Moderator"))
                {
                    await userManager.AddToRoleAsync(user, "Moderator");
                }
            }
        }

        private void DeleteBadImages()
        {
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\logos");

            var files = Directory.GetFiles(folder);
            var tempList = new List<string>(files);
            foreach (var file in tempList)
            {
                if (file.EndsWith(".png") || file.EndsWith(".jpg")
                                          || file.EndsWith(".jpeg") || file.EndsWith(".gif"))
                {
                    //skip
                }
                else
                {
                    File.Delete(file);
                }
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            app.Map("/health", lapp => lapp.Run(async ctx => ctx.Response.StatusCode = 200));
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: "PlayerProfile",
                    template: "{controller=UserProfile}/{action=PlayerProfile}");
            });
            app.UseCors("CorsPolicy");

            app
                .UseSwagger()
                .UseSwaggerUI(c =>
                {
                    var assemblyName = Assembly.GetExecutingAssembly().GetName();
                    var version =
                        $"v{assemblyName.Version.Major}.{assemblyName.Version.Minor}.{assemblyName.Version.Build}";
                    c.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"{assemblyName.Name} {version}");
                });
        }
    }
}
