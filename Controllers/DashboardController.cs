using GoldCap.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
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
            var topCategories = _expenseRepository.GetCategoryRatios()
                .Where(c => c.CategoryPercentage >= _expenseRepository.GetCategoryRatios()[6].CategoryPercentage);
            var topCategory = _expenseRepository.GetCategoryRatios().First();


            #region MonthlyAddition

            var allRecurring = _expenseRepository.GetAllRecurring().ToList();
            foreach (var item in allRecurring)
            {
                if (item.Date.Value >= DateTime.Today.AddDays(-365) && item.Date.Value <= DateTime.Now)
                {
                    List<Expense> list = new List<Expense>();
                    var expDate = item.Date;
                    var z = _expenseRepository.GetAllRecurring().Max(e => e.Id);
                    switch (item.Status)
                    {
                        case 0:
                            DateTime finalDate1 = item.Date.Value;
                            for (var i = item.Date.Value; i <= DateTime.Today.AddDays(1); i = i.AddDays(1))
                            {
                                Expense exp = new Expense()
                                {
                                    Amount = item.Amount,
                                    Category = item.Category,
                                    Description = item.Description,
                                    Status = ((StatusName)item.Status).ToString(),
                                    Date = i,
                                    StatusId = z
                                };
                                list.Add(exp);
                                finalDate1 = i.AddDays(1);
                            }
                            item.Date = finalDate1;
                            _expenseRepository.UpdateRecurring(item);
                            break;
                        case 1:
                            DateTime finalDate2 = item.Date.Value;
                            for (var i = item.Date.Value; i <= DateTime.Today.AddDays(1); i = i.AddDays(7))
                            {
                                Expense exp = new Expense()
                                {
                                    Amount = item.Amount,
                                    Category = item.Category,
                                    Description = item.Description,
                                    Status = ((StatusName)item.Status).ToString(),
                                    Date = i,
                                    StatusId = z
                                };
                                list.Add(exp);
                                finalDate2 = i.AddDays(7);
                            }
                            item.Date = finalDate2;
                            _expenseRepository.UpdateRecurring(item);
                            break;
                        case 2:
                            DateTime finalDate3 = item.Date.Value;
                            for (var i = item.Date.Value; i <= DateTime.Today.AddDays(1); i = i.AddMonths(1))
                            {
                                Expense exp = new Expense()
                                {
                                    Amount = item.Amount,
                                    Category = item.Category,
                                    Description = item.Description,
                                    Status = ((StatusName)item.Status).ToString(),
                                    Date = i,
                                    StatusId = z
                                };
                                list.Add(exp);
                                finalDate3 = i.AddMonths(1);
                            }
                            item.Date = finalDate3;
                            _expenseRepository.UpdateRecurring(item);
                            break;
                        case 3:
                            DateTime finalDate4 = item.Date.Value;
                            for (var i = item.Date.Value; i <= DateTime.Today.AddDays(1); i = i.AddYears(1))
                            {
                                Expense exp = new Expense()
                                {
                                    Amount = item.Amount,
                                    Category = item.Category,
                                    Description = item.Description,
                                    Status = ((StatusName)item.Status).ToString(),
                                    Date = i,
                                    StatusId = z
                                };
                                list.Add(exp);
                                finalDate4 = i.AddYears(1);
                            }
                            item.Date = finalDate4;
                            _expenseRepository.UpdateRecurring(item);
                            break;
                        case 4:
                            DateTime finalDate5 = item.Date.Value;
                            List<string> wkdays = new List<string>();
                            wkdays = item.WeekdaysInString.Split(',').ToList();
                            DateTime finalDate6 = DateTime.Now;
                            int zfinal = 0;
                            for (DateTime i = item.Date.Value; i <= DateTime.Now; i = i.AddDays(7 * (int)item.HowOften))
                            {
                                int k = 0;
                                for (int j = 0; j < 7; j++)
                                {
                                    if (wkdays[j] == "True" && k == 0)
                                    {
                                        Expense exp = new Expense()
                                        {
                                            Amount = item.Amount,
                                            Category = item.Category,
                                            Description = item.Description,
                                            Status = ((StatusName)item.Status).ToString(),
                                            Date = i,
                                            StatusId = z
                                        };
                                        k = 1;
                                        var ho = item.HowOften;
                                        if (zfinal == 0)
                                        {
                                            for (DateTime time = item.Date.Value; time <= DateTime.Now; time = time.AddDays(7 * (int)item.HowOften))
                                            {
                                                finalDate5 = finalDate5.AddDays(7 * (int)item.HowOften);
                                            }
                                            zfinal = 1;
                                        }
                                        list.Add(exp);
                                    }
                                    else if (wkdays[j] == "True")
                                    {
                                        Expense exp = new Expense()
                                        {
                                            Amount = item.Amount,
                                            Category = item.Category,
                                            Description = item.Description,
                                            Status = ((StatusName)item.Status).ToString(),
                                            Date = i.AddDays(j),
                                            StatusId = z
                                        };
                                        list.Add(exp);
                                    }
                                }
                            }
                            item.Date = finalDate5;
                            _expenseRepository.UpdateRecurring(item);
                            break;
                        default:
                            break;
                    }

                    _expenseRepository.AddExpenses(list.AsEnumerable());
                }
            }
            #endregion


            #region StatCircles

            #region OfLastMonthCircle
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
            decimal percentage = 0;
            if (sumExpensesLastMonth != 0)
                percentage = (Convert.ToDecimal(sumExpenses) / sumExpensesLastMonth) * 100;

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
                avgRight = (avg - 180) > 180 ? 180 : avg - 180;
                avgLeft = 180;
            }



            ViewBag.PercentageStringLeftU = avgLeft + "deg";
            ViewBag.PercentageStringStartRightU = rightStart + "deg";
            ViewBag.PercentageStringRightU = avgRight + "deg";

            ViewBag.SumLast30 = sumExpenses;
            ViewBag.SumBeforeLast30 = sumExpensesLastMonth;
            ViewBag.UnderMonth = underMonth;
            ViewBag.TooltipPercentage = Decimal.Round(percentage);
            #endregion

            #region CategoryCircle
            var topCate = thisMonth.Where(e => e.Category == topCategory.CategoryName).Sum(e => e.Amount);
            decimal percentageCategory = 0;
            if (sumExpenses != 0)
                percentageCategory = Decimal.Round((Convert.ToDecimal(topCate) / sumExpenses) * 100);

            ViewBag.TopCategory = percentageCategory;
            ViewBag.TopCategoryName = topCategory.CategoryName;
            ViewBag.TopCategoryAmount = topCate;

            int avgC = (int)(3.6 * (int)percentageCategory);

            var rightStartC = 0;
            var avgRightC = 0;
            var avgLeftC = 0;

            if (avgC >= 0 && avgC <= 180)
            {
                rightStartC = 0;
                avgRightC = 0;
                avgLeftC = avgC;
            }
            else
            {
                rightStartC = 180;
                avgRightC = (avgC - 180);
                avgLeftC = 180;
            }

            #region AnimationSpeed
            NumberFormatInfo dotFormat = new NumberFormatInfo();
            dotFormat.NumberDecimalSeparator = ".";

            double dg = avgLeftC + avgRightC;
            double animationProportion = avgLeftC / dg;
            ViewBag.LeftSpeed = (animationProportion * 0.5).ToString(dotFormat) + "s";
            ViewBag.RightSpeed = (0.5 - (animationProportion * 0.5)).ToString(dotFormat) + "s";
            #endregion

            ViewBag.PercentageStringLeftC = avgLeftC + "deg";
            ViewBag.PercentageStringStartRightC = rightStartC + "deg";
            ViewBag.PercentageStringRightC = avgRightC + "deg";
            #endregion
            #endregion



            if (topCategories != null)
            {
                ViewBag.Categories = topCategories;
            }

            return View(thisMonth);
        }


        public IActionResult Sort(string sortOrder, int id)
        {
            var model = _expenseRepository.GetAllExpenses().Where(m => m.Date >= DateTime.Now.AddDays(-30));

            if (id > 0 && sortOrder != "default")
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
        public IActionResult Delete(int id, bool oneOrAll)
        {
            var exp = _expenseRepository.GetRecurring(id);
            if (oneOrAll)
            {
                _expenseRepository.DeleteExpenses(exp);
                _expenseRepository.DeleteRecurring(id);
            }
            _expenseRepository.DeleteRecurring(id);

            return Json(new { html = Helper.RenderRazorViewToString(this, "RecurringPayments", _expenseRepository.GetAllRecurring()) });
        }

        [HttpGet]
        [NoDirectAccess]
        public IActionResult RecurringPayments()
        {
            var expenseModel = _expenseRepository.GetAllRecurring();
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
            string[] weekdays = new string[7] { "Sn", "M", "T", "W", "Th", "F", "S" };
            ViewBag.Weekdays = weekdays;

            return View(new ExpenseRecurringViewModel());

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateOrEdit(int id, [Bind("Id,Amount,Category,Description,Date,Status,HowOften,Weekdays")] ExpenseRecurringViewModel expenseVM)
        {
            string weekdaysToModel = "";
            foreach (var item in expenseVM.Weekdays)
            {
                weekdaysToModel += item.ToString() + ',';
            }
            string[] weekdays = new string[7] { "Sn", "M", "T", "W", "Th", "F", "S" };
            ViewBag.Weekdays = weekdays;
            ExpenseRecurring expense = new ExpenseRecurring()
            {
                Amount = expenseVM.Amount,
                Category = expenseVM.Category,
                Date = expenseVM.Date,
                Description = expenseVM.Description,
                Status = expenseVM.Status,
                Id = expenseVM.Id,
                HowOften = expenseVM.HowOften,
                WeekdaysInString = weekdaysToModel
            };
            ViewBag.CategoryList = _expenseRepository.GetCategoryList();
            if (ModelState.IsValid)
            {
                _expenseRepository.AddRecurring(expense);
                if (expense.Date.Value <= DateTime.Today)
                {
                    List<Expense> list = new List<Expense>();
                    var expDate = expense.Date;
                    var x = _expenseRepository.GetAllRecurring().Max(e => e.Id);
                    switch (expense.Status)
                    {
                        case 0:
                            DateTime finalDate1 = DateTime.Today;
                            for (var i = expense.Date; i <= DateTime.Today.AddDays(1); i = i.Value.AddDays(1))
                            {

                                Expense exp = new Expense()
                                {
                                    Amount = expense.Amount,
                                    Category = expense.Category,
                                    Description = expense.Description,
                                    Status = ((StatusName)expense.Status).ToString(),
                                    Date = expDate,
                                    StatusId = x
                                };
                                list.Add(exp);
                                finalDate1 = exp.Date.Value;
                                expDate = expDate.Value.AddDays(1);
                            }
                            expense.Date = finalDate1.AddDays(1);
                            _expenseRepository.UpdateRecurring(expense);
                            break;
                        case 1:
                            DateTime finalDate2 = DateTime.Today;
                            for (var i = expense.Date; i <= DateTime.Today.AddDays(1); i = i.Value.AddDays(7))
                            {
                                Expense exp = new Expense()
                                {
                                    Amount = expense.Amount,
                                    Category = expense.Category,
                                    Description = expense.Description,
                                    Status = ((StatusName)expense.Status).ToString(),
                                    Date = expDate,
                                    StatusId = x
                                };
                                list.Add(exp);
                                finalDate2 = exp.Date.Value;
                                expDate = expDate.Value.AddDays(7);
                            }
                            expense.Date = finalDate2.AddDays(7);
                            _expenseRepository.UpdateRecurring(expense);
                            break;
                        case 2:
                            DateTime finalDate3 = DateTime.Today;
                            for (var i = expense.Date; i <= DateTime.Today.AddMonths(1); i = i.Value.AddMonths(1))
                            {
                                Expense exp = new Expense()
                                {
                                    Amount = expense.Amount,
                                    Category = expense.Category,
                                    Description = expense.Description,
                                    Status = ((StatusName)expense.Status).ToString(),
                                    Date = expDate,
                                    StatusId = x
                                };
                                list.Add(exp);
                                finalDate3 = exp.Date.Value;
                                expDate = expDate.Value.AddMonths(1);
                            }
                            expense.Date = finalDate3.AddMonths(1);
                            _expenseRepository.UpdateRecurring(expense);
                            break;
                        case 3:
                            DateTime finalDate4 = DateTime.Today;
                            for (var i = expense.Date; i <= DateTime.Today.AddYears(1); i = i.Value.AddYears(1))
                            {
                                Expense exp = new Expense()
                                {
                                    Amount = expense.Amount,
                                    Category = expense.Category,
                                    Description = expense.Description,
                                    Status = ((StatusName)expense.Status).ToString(),
                                    Date = expDate,
                                    StatusId = x
                                };
                                list.Add(exp);
                                finalDate4 = exp.Date.Value;
                                expDate = expDate.Value.AddYears(1);
                            }
                            expense.Date = finalDate4.AddYears(1);
                            _expenseRepository.UpdateRecurring(expense);
                            break;
                        case 4:
                            DateTime finalDate5 = DateTime.Today;
                            for (var i = expense.Date; i <= DateTime.Now; i = i.Value.AddDays(7 * (int)expenseVM.HowOften))
                            {
                                for (int j = 0; j < 7; j++)
                                {
                                    Expense exp = new Expense()
                                    {
                                        Amount = expense.Amount,
                                        Category = expense.Category,
                                        Description = expense.Description,
                                        Status = ((StatusName)expense.Status).ToString(),
                                        Date = expDate,
                                        StatusId = x
                                    };
                                    var d = ((int)i.Value.DayOfWeek) - j;
                                    if (expenseVM.Weekdays[j] == true && i.Value.AddDays(-d) >= expenseVM.Date && i.Value.AddDays(-d) <= DateTime.Today)
                                    {
                                        exp.Date = exp.Date.Value.AddDays(-d);
                                        list.Add(exp);
                                        finalDate5 = exp.Date.Value;
                                    }
                                }
                                expDate = expDate.Value.AddDays(7 * (int)expenseVM.HowOften);
                            }
                            int day = (int)finalDate5.DayOfWeek;
                            for (int k = day + 1; k < 7; k++)
                            {
                                if (expenseVM.Weekdays[k] == true)
                                {
                                    finalDate5 = finalDate5.AddDays(k - day);
                                    break;
                                }
                                else if (k == 6 && expenseVM.Weekdays[k] == false)
                                {
                                    for (int r = 0; r < 7; r++)
                                    {
                                        if (expenseVM.Weekdays[r] == true)
                                        {
                                            finalDate5 = finalDate5.AddDays(r - day + 7);
                                            break;
                                        }
                                    }

                                }
                            }
                            expense.Date = finalDate5;
                            _expenseRepository.UpdateRecurring(expense);
                            break;
                        default:
                            break;
                    }

                    _expenseRepository.AddExpenses(list.AsEnumerable());
                }


                return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "RecurringPayments", _expenseRepository.GetAllRecurring()) });
            }
            return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, "CreateOrEdit", expenseVM) });
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
