using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities.UserData;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Web.Extensions;

namespace WebApp
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
                "http://ceal.gg",
                "https://ceal.gg",
            };
            services.AddDistributedMemoryCache();
            services.AddMemoryCache();
            services.AddCors(o => o.AddPolicy("AllowSpecificOrigin", p => p.WithOrigins(origins.ToArray())));
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/logos");

            if (!Directory.Exists("wwwroot/logos"))
            {
                Directory.CreateDirectory(path);
            }

            services.AddSingleton<IFileProvider>(
                new PhysicalFileProvider(path));

            services.AddControllersWithViews();
            services.AddLogging();
            services.AddIdentity<UserEntity, UserRoleEntity>()
                .AddUserManager<UserManager<UserEntity>>()
                .AddRoleManager<RoleManager<UserRoleEntity>>()
                .AddDefaultTokenProviders();
            services.AddMyServices(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
