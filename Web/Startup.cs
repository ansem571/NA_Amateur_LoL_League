using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities.UserData;
using Domain.Enums;
using Domain.Helpers;
using Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using RiotSharp.Caching;
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
            services
                .AddMemoryCache()
                .AddMyServices(Configuration);

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

            DeleteBadImages();

            Task.Run(() => SetupChampionCache(services)).Wait();

            services.AddMvc();
        }

        private async Task SetupChampionCache(IServiceCollection services)
        {
            var builder = services.BuildServiceProvider();
            var lookupRepo = builder.GetService<ILookupRepository>();
            GlobalVariables.ChampionCache = new Cache();
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
        }
    }
}
