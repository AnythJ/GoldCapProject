using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace GoldCap.Models
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly AppDbContext _context;
        private readonly ILogger _logger;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        public IExpenseRepository ExpenseRepository { get; private set; }
        public ICategoryRepository CategoryRepository { get; private set; }
        public IRecurringRepository RecurringRepository { get; private set; }
        public IIncomeRepository IncomeRepository { get; private set; }

        public UnitOfWork(AppDbContext context, ILoggerFactory loggerFactory, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = loggerFactory.CreateLogger("logs");
            _httpContextAccessor = httpContextAccessor;

            ExpenseRepository = new ExpenseRepository(context, _httpContextAccessor);
            CategoryRepository = new CategoryRepository(context, _httpContextAccessor);
            RecurringRepository = new RecurringRepository(context, _httpContextAccessor);
            IncomeRepository = new IncomeRepository(context, _httpContextAccessor);
        }

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
