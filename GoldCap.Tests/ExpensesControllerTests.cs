using GoldCap.Controllers;
using GoldCap.Models;
using GoldCap.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;

namespace GoldCap.Tests
{
    public class ExpensesControllerTests : IClassFixture<CustomWebApplicationFactory<GoldCap.Startup>>
    {
        private DbContextOptions<AppDbContext> dbContextOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public ExpensesControllerTests(CustomWebApplicationFactory<GoldCap.Startup> factory)
        {
            _factory = factory;
            _httpContextAccessor = new HttpContextAccessor();
            _httpContextAccessor.HttpContext = new DefaultHttpContext();
            _httpContextAccessor.HttpContext.User.AddIdentity(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "TestsLogin") }));

            var serverVersion = new MySqlServerVersion(new Version(5, 6, 50));
            string connStr = "Server=127.0.0.1;Port=3306;Database=GoldCapTestDb;Uid=root;Pwd=admin;SslMode=None;AllowPublicKeyRetrieval=True";

            dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseMySql(connStr, serverVersion)
                .Options;
        }

        [Fact]
        public async Task Get_CreateOrEditTestWithCorrectId_ReturnsViewResultWithModelWithTheSameIdAsPassedParameter()
        {
            var unitOfWork = Utilities.CreateUnitOfWork(dbContextOptions, _httpContextAccessor);
            ExpensesController controller = new ExpensesController(unitOfWork, _httpContextAccessor);

            int id = 10;
            var result = await controller.CreateOrEdit(id) as ViewResult;
            var model = (Expense)result.Model;

            Assert.Equal(id, model.Id);
        }

        [Fact]
        public async Task Get_CreateOrEditWithInCorrectId_ReturnsViewResultWithModelWithTheSameIdAsPassedParameter()
        {
            var unitOfWork = Utilities.CreateUnitOfWork(dbContextOptions, _httpContextAccessor);
            ExpensesController controller = new ExpensesController(unitOfWork, _httpContextAccessor);

            int id = 1000;
            var result = await controller.CreateOrEdit(id) as ViewResult;

            Assert.Null(result);
        }

       
    }
}
