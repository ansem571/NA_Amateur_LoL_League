using System.IO;
using System.Threading.Tasks;
using DAL.Entities.UserData;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
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

            services.AddSingleton<IFileProvider>(
                new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/logos")));

            Task.Run(() => CreateAdminRole(services)).Wait();

            services.AddMvc();
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
            });
        }
    }
}
