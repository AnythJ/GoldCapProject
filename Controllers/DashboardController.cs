using GoldCap.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoldCap.Controllers
{
    public class DashboardController : Controller
    {
        private IExpenseRepository _expenseRepository;

        public DashboardController(IExpenseRepository expenseRepository)
        {
            _expenseRepository = expenseRepository;
        }
        public IActionResult Index()
        {
            ViewBag.Expenses = _expenseRepository.GetAllExpenses().Where(m => m.Date >= DateTime.Now.AddDays(-30));
            var model = _expenseRepository.GetAllExpenses().Where(m => m.Date >= DateTime.Now.AddDays(-30));

            ViewBag.Date30 = DateTime.Now.AddDays(-30).Day;
            int[] daysArray = Helper.Last30DaysArray();
            ViewBag.daysArray = daysArray;

            return View(model);
        }

        
        public JsonResult GetData()
        {
            ViewBag.Expenses = _expenseRepository.GetAllExpenses().Where(m => m.Date >= DateTime.Now.AddDays(-30));
            DashboardDataModel data = new DashboardDataModel()
            {
                ExpensesLast30Days = _expenseRepository.GetAllExpenses().Where(m => m.Date >= DateTime.Now.AddDays(-30)),
                Last30Days = Helper.Last30DaysArray(),
                Day30 = DateTime.Now.AddDays(-30).Day,
                ExpensesLast30DaysArray = _expenseRepository.GetAllExpenses().Where(m => m.Date >= DateTime.Now.AddDays(-30)).ToArray(),
                summed30 = _expenseRepository.GetSumDayExpense30()
            };
            
            return Json(data);
        }
    }
}
