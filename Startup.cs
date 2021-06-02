using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SOCMonitorV2.Controllers;
using SOCMonitorV2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace SOCMonitorV2
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
            services.AddControllersWithViews();

            services.AddDbContext<EmployeeContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("ConnectionStr")));
            services.AddDbContext<TaskContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("ConnectionStr")));
            services.AddDbContext<ShiftContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("ConnectionStr")));
            services.AddDbContext<RoutineContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("ConnectionStr")));

            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(5);//You can set Time   
            });
            services.AddMvc();
            services.AddRazorPages().AddRazorRuntimeCompilation();

            //services.AddIdentity<IdentityUser, IdentityRole>(options =>
            // {
            //     options.Password.RequiredLength = 6;
            //     options.Password.RequiredUniqueChars = 3;
            // }).AddEntityFrameworkStores<AppDbContext>();

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
            }
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Login}/{id?}");
            });
        }
    }
}
