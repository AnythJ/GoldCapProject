using GoldCap.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace GoldCap.Tests
{
    public class UnitOfWorkTests : IClassFixture<CustomWebApplicationFactory<GoldCap.Startup>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<GoldCap.Startup> _factory;
        private DbContextOptions<AppDbContext> dbContextOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UnitOfWorkTests(CustomWebApplicationFactory<GoldCap.Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            _httpContextAccessor = new HttpContextAccessor();
            _httpContextAccessor.HttpContext = new DefaultHttpContext();
            _httpContextAccessor.HttpContext.User.AddIdentity(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "TestsLogin") }));

            var serverVersion = new MySqlServerVersion(new Version(5, 6, 50));
            string connStr = "Server=127.0.0.1;Port=3306;Database=GoldCapTestDb;Uid=root;Pwd=admin;SslMode=None;AllowPublicKeyRetrieval=True";

            dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseMySql(connStr, serverVersion)
                .Options;
        }


        [Theory]
        [InlineData(10, "None", 0)]
        [InlineData(50, "None", 1)]
        [InlineData(99.99, "None", 2)]
        public async Task AddExpense_SingleObject_AfterAdditionDBContextShouldContainTheSameObject(decimal amount, string category, int daysAgo)
        {
            // Arrange
            var unitOfWork = Utilities.CreateUnitOfWork(dbContextOptions, _httpContextAccessor);
            Expense expense = Utilities.CreateTestExpense(_httpContextAccessor, amount, category, daysAgo);

            // Act
            Expense addedExpense = unitOfWork.ExpenseRepository.Add(expense);

            // Assert
            Assert.NotEqual(0, addedExpense.Id);
        }

        [Theory]
        [InlineData("None")]
        [InlineData("Daily")]
        [InlineData("Food")]
        public async Task AddCategory_SingleObject_AfterAdditionDBContextShouldContainTheSameObject(string categoryName)
        {
            // Arrange
            var unitOfWork = Utilities.CreateUnitOfWork(dbContextOptions, _httpContextAccessor);
            Category category = Utilities.CreateTestCategory(_httpContextAccessor, categoryName);

            // Act
            Category addedCategory = unitOfWork.CategoryRepository.Add(category);

            // Assert
            Assert.NotEqual(0, addedCategory.Id);
        }

        [Theory]
        [InlineData(1000, 3, "First income")]
        [InlineData(2000, 2, "Second income")]
        [InlineData(3000, 1, "Third income")]

        public async Task AddIncome_SingleObject_AfterAdditionDBContextShouldContainTheSameObject(decimal amount, int monthsSinceFirstPaycheck, string description)
        {
            // Arrange
            var unitOfWork = Utilities.CreateUnitOfWork(dbContextOptions, _httpContextAccessor);
            Income income = Utilities.CreateTestIncome(_httpContextAccessor, amount, monthsSinceFirstPaycheck, description);

            // Act
            Income addedIncome = unitOfWork.IncomeRepository.Add(income);

            // Assert
            Assert.NotEqual(0, addedIncome.Id);
        }

        [Fact]
        public async Task DeleteIncome_SingleObject_AfterDeletionDBContextShouldNotContainTheSameObject()
        {
            // Arrange
            var unitOfWork = Utilities.CreateUnitOfWork(dbContextOptions, _httpContextAccessor);
            Income income = Utilities.CreateTestIncome(_httpContextAccessor, 1000, 1, "Income to be removed");

            // Act
            Income addedIncome = unitOfWork.IncomeRepository.Add(income);
            bool removed = await unitOfWork.IncomeRepository.DeleteAsync(addedIncome.Id);

            // Assert
            Assert.True(removed);
        }

        

    }
}