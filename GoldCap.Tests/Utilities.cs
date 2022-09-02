using GoldCap.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GoldCap.Tests
{
    public static class Utilities
    {
        public static void ClearRecordsInTestDatabase(AppDbContext context)
        {
            context.Database.ExecuteSqlRaw("SET FOREIGN_KEY_CHECKS = 0; TRUNCATE TABLE expenses");
            context.Database.ExecuteSqlRaw("SET FOREIGN_KEY_CHECKS = 0; TRUNCATE TABLE recurringexpenses");
            context.Database.ExecuteSqlRaw("SET FOREIGN_KEY_CHECKS = 0; TRUNCATE TABLE incomes");
            context.Database.ExecuteSqlRaw("SET FOREIGN_KEY_CHECKS = 0; TRUNCATE TABLE categories");
        }

        public static UnitOfWork CreateUnitOfWork(DbContextOptions<AppDbContext> dbContextOptions, IHttpContextAccessor _httpContextAccessor)
        {
            AppDbContext context = new AppDbContext(dbContextOptions);

            return new UnitOfWork(context, new LoggerFactory(), _httpContextAccessor);
        }

        public static Expense CreateTestExpense(IHttpContextAccessor _httpContextAccessor, decimal amount, string category, int daysAgo, string? description = null, string? status = null, int recurringId = 0, int id = 0)
        {
            string login = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            Expense expense = new()
            {
                Id = id,
                Amount = amount,
                Category = category,
                Date = DateTime.Now.AddDays(-daysAgo),
                ExpenseManagerLogin = login,
                StatusId = recurringId,
                Status = status
            };

            return expense;
        }

        public static Tuple<List<Expense>, ExpenseRecurring> CreateTestRecurringExpense(IHttpContextAccessor _httpContextAccessor, int howMany, decimal amount, string category)
        {
            List<Expense> list = new();
            ExpenseRecurring expenseRecurring = new()
            {
                Amount = amount,
                Category = category,
                Date = DateTime.Now.AddDays(-howMany),
                ExpenseManagerLogin = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name),
                HowOften = 1,
                Status = 0
            };

            for (int i = 0; i < howMany; i++)
            {
                list.Add(CreateTestExpense(_httpContextAccessor, amount, category, howMany - i));
            }

            return Tuple.Create(list, expenseRecurring);
        }

        public static List<Category> CreateTestCategories(IHttpContextAccessor _httpContextAccessor, Array categories)
        {
            List<Category> list = new List<Category>();

            for (int i = 0; i < categories.Length; i++)
            {
                list.Add(CreateTestCategory(_httpContextAccessor, (string)categories.GetValue(i).ToString()));
            }

            return list;
        }

        public static Category CreateTestCategory(IHttpContextAccessor _httpContextAccessor, string categoryName)
        {
            Category category = new()
            {
                ExpenseManagerLogin = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name),
                Name = categoryName
            };

            return category;
        }

        public static Income CreateTestIncome(IHttpContextAccessor _httpContextAccessor, decimal amount, int monthsSinceFirstPaycheck, string? description = null)
        {
            string login = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            Income income = new()
            {
                Amount = amount,
                Description = description,
                Date = DateTime.Now.AddMonths(-monthsSinceFirstPaycheck),
                ExpenseManagerLogin = login,
                FirstPaycheckDate = DateTime.Now.AddMonths(monthsSinceFirstPaycheck)
            };

            return income;
        }

        public static void GenerateData(AppDbContext context, IHttpContextAccessor _httpContextAccessor)
        {
            Array categories = Enum.GetValues(typeof(Ctg));
            Expense expense = CreateTestExpense(_httpContextAccessor, 10, categories.GetValue(new Random().Next(categories.Length - 1)).ToString(), 5, id: 10);
            (List<Expense> list, ExpenseRecurring recurringExpense) = CreateTestRecurringExpense(_httpContextAccessor, 5, 10, categories.GetValue(new Random().Next(categories.Length - 1)).ToString());

            List<Category> categoryList = CreateTestCategories(_httpContextAccessor, categories);


            context.Expenses.AddRange(list);
            context.RecurringExpenses.Add(recurringExpense);

            context.Expenses.Add(expense);

            context.Categories.AddRange(categoryList);

            context.SaveChanges();
        }
    }
}
