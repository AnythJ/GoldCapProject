using GoldCap.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GoldCap.Tests
{
    public class CustomWebApplicationFactory<TStartup>
    : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbContextOptions<AppDbContext>));

                services.Remove(descriptor);
                services.AddDbContextPool<AppDbContext>(options =>
                {
                    var serverVersion = new MySqlServerVersion(new Version(5, 6, 50));
                    string connStr = "Server=127.0.0.1;Port=3306;Database=GoldCapTestDb;Uid=root;Pwd=admin;SslMode=None;AllowPublicKeyRetrieval=True";

                    options.UseMySql(connStr, serverVersion);
                });

                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<AppDbContext>();
                    var logger = scopedServices
                        .GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                    db.Database.EnsureCreated();

                    try
                    {
                        IHttpContextAccessor httpContextAccessor = scopedServices.GetRequiredService<IHttpContextAccessor>();
                        httpContextAccessor.HttpContext = new DefaultHttpContext();
                        httpContextAccessor.HttpContext.User.AddIdentity(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "TestsLogin") }));
                      
                        Utilities.ClearRecordsInTestDatabase(db);
                        Utilities.GenerateData(db, httpContextAccessor);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred seeding the " +
                            "database with test messages. Error: {Message}", ex.Message);
                    }
                }
            });
        }
    }
}
