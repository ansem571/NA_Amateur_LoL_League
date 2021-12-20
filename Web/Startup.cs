using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using DAL.Entities.UserData;
using Domain.Helpers;
using Domain.Repositories.Interfaces;
using Domain.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
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
        public IWebHostEnvironment Environment { get; set; }

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
                "http://ceal.gg",
                "https://ceal.gg",
            };
            services.AddDistributedMemoryCache();
            services.AddMemoryCache();

            services.AddIdentity<UserEntity, UserRoleEntity>()
                .AddDefaultTokenProviders();

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/logos");

            if (!Directory.Exists("wwwroot/logos"))
            {
                Directory.CreateDirectory(path);
            }

            services.AddSingleton<IFileProvider>(
                new PhysicalFileProvider(path));

            //Task.Run(() => CreateAdminRole(services)).Wait();
            //Task.Run(() => CreateTribunalRole(services)).Wait();
            //Task.Run(() => CreateModeratorRole(services)).Wait();

            //DeleteBadImages();

            //Task.Run(() => SetupChampionCache(services)).Wait();
            //services
            //    .AddMvc(o =>
            //    {
            //        o.Conventions.Add(new CommaSeparatedQueryStringConvention());
            //        o.ModelMetadataDetailsProviders.Add(new RequiredBindingMetadataProvider());
            //        //o.EnableEndpointRouting = false;
            //    })
            //.AddJsonOptions(o =>
            //{
            //    o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            //    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            //    o.JsonSerializerOptions.IgnoreNullValues = true;
            //});
            services.AddMyServices(Configuration);
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
            });
            var builder = services.BuildServiceProvider();
            var emailService = builder.GetService<IEmailService>();
            services.AddMvc(options =>
            {
                options.SuppressAsyncSuffixInActionNames = false;
                options.Filters.Add(new ErrorHandlingFilter(emailService));
            });
        }

        private async Task SetupChampionCache(IServiceCollection services)
        {
            var builder = services.BuildServiceProvider();
            var lookupRepo = builder.GetService<ILookupRepository>();
            await GlobalVariables.SetupChampionCache(lookupRepo);
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
            var user1 = await userManager.FindByEmailAsync("jadams.macdonnell1@gmail.com"); //me
            var user2 = await userManager.FindByEmailAsync("ansem571@gmail.com"); //me
            var user3 = await userManager.FindByEmailAsync("brandonleekinnaird@gmail.com"); //spanish teacher
            var user4 = await userManager.FindByEmailAsync("morrisonsviewpoint@gmail.com"); //amo
            var user5 = await userManager.FindByEmailAsync("Mike_Salinas112@hotmail.com"); //ultimate ace
            var user6 = await userManager.FindByEmailAsync("brandoncap@live.com"); //aileronroll
            var user7 = await userManager.FindByEmailAsync("brennan.lee.artrip@gmail.com"); //eidocles
            var user8 = await userManager.FindByEmailAsync("christopheringlin@gmail.com"); //ttu phoenix
            var user9 = await userManager.FindByEmailAsync("nolan-ryder2@hotmail.com"); //dragon ryder
            var user10 = await userManager.FindByEmailAsync("lifelongtundra@gmail.com"); //tundra
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
            var user3 = await userManager.FindByEmailAsync("xxmadruenoxx@gmail.com"); // Aseeraa
            var user4 = await userManager.FindByEmailAsync("amajestksniper18@gmail.com"); // we jestin
            var user5 = await userManager.FindByEmailAsync("itsmrkjc@gmail.com"); //cyinite
            var user6 = await userManager.FindByEmailAsync("sime.tesevcic@gmail.com"); //slavic goon
            var user7 = await userManager.FindByEmailAsync("gamingb32@gmail.com"); //GGodzilla123
            var users = new List<UserEntity>
            {
                user1,
                user2,
                user3,
                user4,
                user5,
                user6,
                user7
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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Environment = env;

            app.UseExceptionHandler("/Home/Error");
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            //app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                //endpoints.Map("/health", HealthRequest);
                endpoints.MapRazorPages();
                endpoints.MapDefaultControllerRoute();
                //endpoints.MapControllers();
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                // endpoints.MapControllerRoute("PlayerProfile", "{controller=UserProfile}/{action=PlayerProfile}");
            });
            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller=Home}/{action=Index}/{id?}");
            //    routes.MapRoute(
            //        name: "PlayerProfile",
            //        template: "{controller=UserProfile}/{action=PlayerProfile}");
            //});
        }

        private async Task<int> HealthRequest(HttpContext context)
        {
            await Task.CompletedTask;
            return context.Response.StatusCode = 200;
        }
    }
}
