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
            ViewBag.Expenses = _expenseRepository.GetAllExpenses().Where(m => m.Date >= DateTime.Now.AddDays(-30)).OrderByDescending(d => d.Date);
            var model = _expenseRepository.GetAllExpenses().Where(m => m.Date >= DateTime.Now.AddDays(-30));

            return View(model);
        }

        
        public JsonResult GetData()
        {
            ViewBag.Expenses = _expenseRepository.GetAllExpenses().Where(m => m.Date >= DateTime.Now.AddDays(-30));
            DashboardDataModel data = new DashboardDataModel()
            {
                Day30 = DateTime.Now.AddDays(-30).Day,
                ListLast30 = _expenseRepository.GetSumDayExpense30()
            };
            
            return Json(data);
        }
    }
}
