using GoldCap.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoldCap
{
    public class Startup
    {
        private IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var serverVersion = new MySqlServerVersion(new Version(5, 6, 50));

            services.AddDbContextPool<AppDbContext>(x =>
            {

                var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                string connStr;

                if (env == "Development")
                {
                    connStr = _config.GetConnectionString("ExpenseDBConnection");
                }
                else
                {
                    var connUrl = Environment.GetEnvironmentVariable("CLEARDB_DATABASE_URL");

                    connUrl = connUrl.Replace("mysql://", string.Empty);
                    var userPassSide = connUrl.Split("@")[0];
                    var hostSide = connUrl.Split("@")[1];

                    var connUser = userPassSide.Split(":")[0];
                    var connPass = userPassSide.Split(":")[1];
                    var connHost = hostSide.Split("/")[0];
                    var connDb = hostSide.Split("/")[1].Split("?")[0];


                    connStr = $"server={connHost};Uid={connUser};Pwd={connPass};Database={connDb};SSL Mode=None";
                }

                x.UseMySql(connStr, serverVersion);

            });


            services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>();

            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });
            services.AddScoped(typeof(IGeneralRepository<>), typeof(GeneralRepository<>));
            services.AddTransient<IExpenseRepository, ExpenseRepository>();
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<IRecurringRepository, RecurringRepository>();
            services.AddTransient<IIncomeRepository, IncomeRepository>();
            services.AddHttpContextAccessor();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc(routes => {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=home}/{action=index}/{id?}");
            });
            //app.UseMvcWithDefaultRoute();
        }
    }
}
