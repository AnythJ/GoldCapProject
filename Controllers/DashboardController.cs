using GoldCap.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static GoldCap.Helper;

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
            var thisMonth = _expenseRepository.GetAllExpenses().Where(m => m.Date >= DateTime.Now.AddDays(-30)).OrderByDescending(d => d.Date);
            var lastMonth = _expenseRepository.GetAllExpenses().Where(m => m.Date >= DateTime.Now.AddDays(-60) && m.Date <= DateTime.Now.AddDays(-30));


            #region StatCircles
            int sumExpenses = 0;
            foreach (var item in thisMonth)
            {
                sumExpenses += (int)item.Amount.Value;
            }
            int sumExpensesLastMonth = 0;
            foreach (var item in lastMonth)
            {
                sumExpensesLastMonth += (int)item.Amount.Value;
            }
            var underMonth = sumExpensesLastMonth - sumExpenses;
            var percentage = (Convert.ToDecimal(sumExpenses) / sumExpensesLastMonth) *100;
            int avg = (int)(3.6 * (int)percentage);

            var rightStart = 0;
            var avgRight = 0;
            var avgLeft = 0;

            if (avg >= 0 && avg <= 180)
            {
                rightStart = 0;
                avgRight = 0;
                avgLeft = avg;
            }
            else
            {
                rightStart = 180;
                avgRight = (avg - 180);
                avgLeft = 180;
            }



            ViewBag.PercentageStringLeft = avgLeft + "deg";
            ViewBag.PercentageStringStartRight = rightStart + "deg";
            ViewBag.PercentageStringRight = avgRight + "deg";

            ViewBag.SumLast30 = sumExpenses;
            ViewBag.SumBeforeLast30 = sumExpensesLastMonth;
            ViewBag.UnderMonth = underMonth;
            #endregion


            var x = _expenseRepository.GetCategoryRatios()
                .Where(c => c.CategoryPercentage >= _expenseRepository.GetCategoryRatios()[6].CategoryPercentage);
            if (x != null)
            {
                ViewBag.Categories = x;
            }

            return View(thisMonth);
        }


        public IActionResult Sort(string sortOrder, int id)
        { 
            var model = _expenseRepository.GetAllExpenses().Where(m => m.Date >= DateTime.Now.AddDays(-30));

            if(id > 0 && sortOrder != "default")
            {
                var expense = _expenseRepository.GetExpense(id);
                model = _expenseRepository.GetAllExpenses().Where(e => e.Date.Value.DayOfYear == expense.Date.Value.DayOfYear && e.Date.Value.Year == expense.Date.Value.Year);
            }
            


            ViewBag.AmountSortParm = sortOrder == "Amount" ? "amount_desc" : "Amount";
            ViewBag.CategorySortParm = sortOrder == "Category" ? "category_desc" : "Category";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";


            switch (sortOrder)
            {
                case "Date":
                    model = model.OrderBy(d => d.Date);
                    break;
                case "date_desc":
                    model = model.OrderByDescending(d => d.Date);
                    break;
                case "Category":
                    model = model.OrderBy(d => d.Category);
                    break;
                case "category_desc":
                    model = model.OrderByDescending(d => d.Category);
                    break;
                case "Amount":
                    model = model.OrderBy(d => d.Amount);
                    break;
                case "amount_desc":
                    model = model.OrderByDescending(d => d.Amount);
                    break;
                default:
                    model = model.OrderByDescending(d => d.Date);
                    break;
            }
            return Json(new { html = Helper.RenderRazorViewToString(this, "_ViewAll", model) });
        }

        [HttpGet]
        [NoDirectAccess]
        public IActionResult Details(int id)
        {
            var expenseModel = _expenseRepository.GetExpense(id);
            if (expenseModel == null)
            {
                return NotFound();
            }
            return View("_DetailsModal", expenseModel);

        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _expenseRepository.Delete(id);

            return Json(new { html = Helper.RenderRazorViewToString(this, "RecurringPayments", _expenseRepository.GetAllExpenses()) });
        }

        [HttpGet]
        [NoDirectAccess]
        public IActionResult RecurringPayments()
        {
            var expenseModel = _expenseRepository.GetAllExpenses().Where(d => d.Date.Value.Day >= 23);
            if (expenseModel == null)
            {
                return NotFound();
            }
            return View("RecurringPayments", expenseModel);

        }


        [HttpGet]
        [NoDirectAccess]
        public IActionResult CreateOrEdit(int id = 0)
        {
            ViewBag.CategoryList = _expenseRepository.GetCategoryList();

            if (id == 0)
                return View(new Expense());
            else
            {
                var expenseModel = _expenseRepository.GetExpense(id);
                if (expenseModel == null)
                {
                    return NotFound();
                }
                return View(expenseModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateOrEdit(int id, [Bind("Id,Amount,Category,Description,Date")] Expense expense)
        {
            ViewBag.CategoryList = _expenseRepository.GetCategoryList();
            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    _expenseRepository.Add(expense);
                }
                else
                {
                    _expenseRepository.Update(expense);
                }


                return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "RecurringPayments", _expenseRepository.GetAllExpenses()) });
            }
            return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, "CreateOrEdit", expense) });
        }

        public IActionResult TooltipSort(int id)
        {
            if (id >= 0)
            {
                var expense = _expenseRepository.GetExpense(id);
                var model = _expenseRepository.GetAllExpenses().Where(e => (e.Date.Value.Day == expense.Date.Value.Day
                           && e.Date.Value.Month == expense.Date.Value.Month));


                return Json(new { html = Helper.RenderRazorViewToString(this, "_ViewAll", model) });
            }
            return Json(new { html = Helper.RenderRazorViewToString(this, "_ViewAll", _expenseRepository.GetAllExpenses().Where(m => m.Date >= DateTime.Now.AddDays(-30)).OrderByDescending(d => d.Date)) });
        }

        public JsonResult GetData()
        {
            List<string> newList = new List<string>();
            var ctg = _expenseRepository.GetCategoryRatios();
            foreach (var item in ctg)
            {
                newList.Add(item.CategoryPercentage.ToString());
            }


            DashboardDataModel data = new DashboardDataModel()
            {
                ListLast30 = _expenseRepository.GetSumDayExpense30(),
                CategoryRatios = _expenseRepository.GetCategoryRatios(),
                CategoryCount = _expenseRepository.GetAllCategories().Count(),
                TooltipList = _expenseRepository.GetTooltipList(),
                ExpensesList = _expenseRepository.GetAllExpenses().Where(m => m.Date >= DateTime.Now.AddDays(-30)).OrderBy(e => e.Date).AsEnumerable()
            };

            return Json(data);
        }
    }
}
