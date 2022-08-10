using GoldCap.Models;
using GoldCap.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static GoldCap.Helper;

namespace GoldCap.Controllers
{
    public class DashboardController : Controller
    {
        private IExpenseRepository _expenseRepository;
        private ICategoryRepository _categoryRepository;
        private IRecurringRepository _recurringRepository;
        private IIncomeRepository _incomeRepository;

        public DashboardController(IExpenseRepository expenseRepository, ICategoryRepository categoryRepository, IRecurringRepository recurringRepository, IIncomeRepository incomeRepository)
        {
            _expenseRepository = expenseRepository;
            _categoryRepository = categoryRepository;
            _recurringRepository = recurringRepository;
            _incomeRepository = incomeRepository;
        }

        public async Task<IActionResult> Index(int period = 30)
        {
            DateTime lastmonthFirstDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1);
            DateTime lastmonthLastDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddDays(-1);
            var totalExpensesForUser = await _expenseRepository.GetAllAsync();
            var totalCategoriesRatio = _expenseRepository.GetCategoryRatios(period);

            var expensesFromLastMonth = totalExpensesForUser.Where(m => m.Date >= lastmonthFirstDay && m.Date <= lastmonthLastDay);

            var expensesFromThisPeriod = totalExpensesForUser.Where(m => m.Date >= DateTime.Now.AddDays(-period)).OrderByDescending(d => d.Date);
            var categoriesWithAbove0Ratio = totalCategoriesRatio.Where(c => c.CategoryPercentage > 0);
            var topCategory = totalCategoriesRatio != null ? totalCategoriesRatio.FirstOrDefault() : null;

            #region Monthly income and expenses addition
            var userIncomes = _incomeRepository.GetAll(User.FindFirstValue(ClaimTypes.Name)).ToList();
            decimal totalIncome = await DashboardHelper.GetAndUpdateIncomes(userIncomes, _expenseRepository, _incomeRepository, User.FindFirstValue(ClaimTypes.Name));
            
            if (period == 365) totalIncome *= 12;

            var allRecurringExpenses = _recurringRepository.GetAll().ToList();

            DashboardHelper.UpdateRecurringExpensesOnIndex(allRecurringExpenses, _expenseRepository, _recurringRepository, _incomeRepository, User.FindFirstValue(ClaimTypes.Name));

            #endregion

            #region StatCircles

            int sumExpenses = 0;
            foreach (var item in expensesFromThisPeriod) { sumExpenses += (int)item.Amount.Value; }

            #region LastPeriodStatCircle
            int sumExpensesLastPeriod = 0;
            foreach (var item in expensesFromLastMonth) { sumExpensesLastPeriod += (int)item.Amount.Value; }

            StatCircle lastPeriodCircle = DashboardHelper.CreateLastPeriodStatCircle(sumExpenses, sumExpensesLastPeriod, period);
            #endregion

            #region IncomeStatCircle
            StatCircle incomeCircle = DashboardHelper.CreateIncomeStatCircle(totalIncome, sumExpenses);
            #endregion

            #region CategoryStatCircle
            var topCate = topCategory != null ? expensesFromThisPeriod.Where(e => e.Category == topCategory.CategoryName).Sum(e => e.Amount) : 0;
           
            StatCircle cateCircle = DashboardHelper.CreateCategoryStatCircle(sumExpenses, topCate, topCategory);
            #endregion

            List<StatCircle> circleStats = new();
            circleStats.Add(lastPeriodCircle);
            circleStats.Add(cateCircle);
            circleStats.Add(incomeCircle);

            #endregion

            #region StatPills
            List<StatPill> pillsStats = new();
            #region FirstPill
            var highestExpense = totalExpensesForUser.Where(m => m.Date >= DateTime.Now.AddDays(-period)).OrderByDescending(d => d.Amount).FirstOrDefault();

            StatPill firstPill = new();
            if (highestExpense != null)
            {
                firstPill.AmountDecimal = highestExpense.Amount > 100 ? Math.Round((decimal)highestExpense.Amount, 0) : Math.Round((decimal)highestExpense.Amount, 1);
                firstPill.Category = highestExpense.Category;
                firstPill.Date = highestExpense.Date;
                firstPill.Percentage = (sumExpensesLastPeriod != 0) ? Decimal.Round((Convert.ToDecimal(highestExpense.Amount) / sumExpensesLastPeriod) * 100, 1) : 0;
                firstPill.DatetimeString = highestExpense.Date.Value.ToString("dd/M/yyyy hh:mm");
            }
            #endregion

            #region SecondPill
            var lastExpense = expensesFromThisPeriod.FirstOrDefault();
            decimal averageAmount = expensesFromThisPeriod.Count() != 0 ? sumExpenses / expensesFromThisPeriod.Count() : 0;
            decimal aboveOrBelow = averageAmount != 0 ? (decimal)lastExpense.Amount / averageAmount : 0;

            StatPill secondPill = new();
            if (lastExpense != null)
            {
                secondPill.AmountDecimal = lastExpense.Amount > 100 ? Math.Round((decimal)lastExpense.Amount, 0) : Math.Round((decimal)lastExpense.Amount, 1);
                secondPill.Category = lastExpense.Category;
                secondPill.DatetimeString = lastExpense.Date.Value.DayOfWeek + ", " + lastExpense.Date.Value.Day + " " + lastExpense.Date.Value.ToString("MMMM", CultureInfo.InvariantCulture);
                secondPill.Percentage = Decimal.Round((aboveOrBelow - 1) * 100, 1);
            }
            #endregion

            #region ThirdPill
            var lowestExpense = totalExpensesForUser.Where(m => m.Date >= DateTime.Now.AddDays(-period)).OrderBy(d => d.Amount).FirstOrDefault();
            StatPill thirdPill = new();
            if (lowestExpense != null)
            {
                thirdPill.AmountDecimal = lowestExpense.Amount > 100 ? Math.Round((decimal)lowestExpense.Amount, 0) : Math.Round((decimal)lowestExpense.Amount, 1);
                thirdPill.Category = lowestExpense.Category;
                thirdPill.Date = lowestExpense.Date;
                thirdPill.Percentage = (sumExpensesLastPeriod != 0) ? Decimal.Round((Convert.ToDecimal(lowestExpense.Amount) / sumExpensesLastPeriod) * 100, 1) : 0;
                thirdPill.DatetimeString = lowestExpense.Date.Value.ToString("dd/M/yyyy hh:mm");
            }
            #endregion

            pillsStats.Add(firstPill);
            pillsStats.Add(secondPill);
            pillsStats.Add(thirdPill);
            #endregion

            #region NotificationsBoxData
            int k = 0;
            List<Expense> notificationList = new();
            List<ExpenseRecurring> firstFiveIncomingExpenses = allRecurringExpenses.OrderBy(e => e.Date).Take(3).ToList();
            foreach (var item in firstFiveIncomingExpenses)
            {
                if (k == 4) break;

                Expense newExpense = Helper.CreateExpenseFromRecurring(item, item.Date.Value);
                notificationList.Add(newExpense);
                k++;
            }
            #endregion

            DashboardViewModel dashboardViewModel = new()
            {
                PillsStats = pillsStats,
                Expenses = expensesFromThisPeriod,
                CirclesStats = circleStats,
                Period = period,
                NotificationList = notificationList
            };
            
            if (categoriesWithAbove0Ratio != null)
            {
                dashboardViewModel.Categories = categoriesWithAbove0Ratio;
            }

            return View(dashboardViewModel);
        }


        public async Task<IActionResult> Sort(string sortOrder, int id, string categoryName = null, int period = 30)
        {
            var expenses = await _expenseRepository.GetAllAsync();
            var model = expenses.Where(m => m.Date >= DateTime.Now.AddDays(-period));

            if (id > 0 && sortOrder != "default")
            {
                var expense = await _expenseRepository.GetAsync(id);
                model = expenses.Where(e => e.Date.Value.DayOfYear == expense.Date.Value.DayOfYear && e.Date.Value.Year == expense.Date.Value.Year);
            }
            else if (categoryName != null)
            {
                var withPercentage = categoryName.Split(' ');
                withPercentage = withPercentage.Take(withPercentage.Count() - 1).ToArray();
                categoryName = String.Join(' ', withPercentage).Trim();
                model = expenses.Where(e => e.Category == categoryName && e.Date >= DateTime.Today.AddDays(-period));
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
        public async Task<IActionResult> Details(int id)
        {
            var expenseModel = await _expenseRepository.GetAsync(id);
            if (expenseModel == null)
            {
                return NotFound();
            }
            return View("_DetailsModal", expenseModel);

        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, bool oneOrAll)
        {
            var exp = await _recurringRepository.GetAsync(id);
            if (oneOrAll)
            {
                await _expenseRepository.DeleteExpensesAsync(exp);
                await _recurringRepository.DeleteAsync(id);
            }
            await _recurringRepository.DeleteAsync(id);

            return Json(new { html = Helper.RenderRazorViewToString(this, "RecurringPayments", _recurringRepository.GetAll()) });
        }

        [HttpPost, ActionName("DeleteIncome")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteIncome(int id)
        {
            var userLogin = User.FindFirstValue(ClaimTypes.Name);
            var inc = _incomeRepository.GetAll(userLogin).FirstOrDefault();
            await _incomeRepository.DeleteAsync(id);

            return Json(new { html = Helper.RenderRazorViewToString(this, "IncomeList", _incomeRepository.GetAll(userLogin)) });
        }

        [HttpGet]
        [NoDirectAccess]
        public IActionResult RecurringPayments()
        {
            var expenseModel = _recurringRepository.GetAll();
            if (expenseModel == null)
            {
                return NotFound();
            }
            return View("RecurringPayments", expenseModel);

        }

        [HttpGet]
        [NoDirectAccess]
        public IActionResult IncomeList()
        {
            var userLogin = User.FindFirstValue(ClaimTypes.Name);
            var incomeModel = _incomeRepository.GetAll(userLogin);
            if (incomeModel == null)
            {
                return View("IncomeList");
            }
            return View("IncomeList", incomeModel);

        }

        [HttpGet]
        [NoDirectAccess]
        public IActionResult CreateOrEdit(int id = 0, int preStatus = 0)
        {
            ViewBag.CategoryList = _categoryRepository.GetAll().ToList().OrderBy(c => c.Name);
            string[] weekdays = new string[7] { "Sn", "M", "T", "W", "Th", "F", "S" };
            ViewBag.Weekdays = weekdays;

            ExpenseRecurringViewModel model = new();
            model.Status = preStatus;

            return View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrEdit(int id, [Bind("Id,Amount,Category,Description,Date,Status,HowOften,Weekdays")] ExpenseRecurringViewModel expenseVM)
        {
            if (expenseVM.Status == 4 && expenseVM.HowOften == null) ModelState.AddModelError("HowOften", "Number needed");

            if (expenseVM.Status == 4 && !expenseVM.Weekdays.Any(e => e == true)) ModelState.AddModelError("Weekdays", "Select at least one day");

            string weekdaysToModel = "";
            foreach (var item in expenseVM.Weekdays)
            {
                weekdaysToModel += item.ToString() + ',';
            }
            string[] weekdays = new string[7] { "Sn", "M", "T", "W", "Th", "F", "S" };
            ViewBag.Weekdays = weekdays;
            ViewBag.CategoryList = _categoryRepository.GetAll().ToList().OrderBy(c => c.Name);
            ExpenseRecurring expenseRecurring = new()
            {
                Amount = expenseVM.Amount,
                Category = expenseVM.Category,
                Date = expenseVM.Date,
                Description = expenseVM.Description,
                Status = expenseVM.Status,
                HowOften = expenseVM.HowOften,
                WeekdaysInString = weekdaysToModel,
                ExpenseManagerLogin = User.FindFirstValue(ClaimTypes.Name)
            };

            if (ModelState.IsValid)
            {
                await _recurringRepository.AddAsync(expenseRecurring);
                var lastAddedRecurring = await _recurringRepository.UpdateAsync(expenseRecurring);
                await _recurringRepository.UpdateAsync(expenseRecurring);

                if (expenseRecurring.Date.Value <= DateTime.Today)
                {
                    List<Expense> list = new();
                    var expDate = expenseRecurring.Date;
                    var recurringId = lastAddedRecurring.Id;
                    switch (expenseRecurring.Status) //Adding expenses depending on which status was selected for recurring expense
                    {
                        case 0:
                            DateTime nextDate0 = DateTime.Today;
                            for (var i = expenseRecurring.Date; i <= DateTime.Now; i = i.Value.AddDays(1))
                            {
                                Expense expense = Helper.CreateExpenseFromRecurring(expenseRecurring, expDate.Value);
                                expense.ExpenseManagerLogin = User.FindFirstValue(ClaimTypes.Name);
                                expense.StatusId = recurringId;

                                list.Add(expense);
                                nextDate0 = expense.Date.Value;
                                expDate = expDate.Value.AddDays(1);
                            }
                            expenseRecurring.Date = nextDate0.AddDays(1);
                            await _recurringRepository.UpdateAsync(expenseRecurring);
                            break;
                        case 1:
                            DateTime nextDate1 = DateTime.Today;
                            for (var i = expenseRecurring.Date; i <= DateTime.Now; i = i.Value.AddDays(7))
                            {
                                Expense expense = Helper.CreateExpenseFromRecurring(expenseRecurring, expDate.Value);
                                expense.ExpenseManagerLogin = User.FindFirstValue(ClaimTypes.Name);
                                expense.StatusId = recurringId;

                                list.Add(expense);
                                nextDate1 = expense.Date.Value;
                                expDate = expDate.Value.AddDays(7);
                            }
                            expenseRecurring.Date = nextDate1.AddDays(7);
                            await _recurringRepository.UpdateAsync(expenseRecurring);
                            break;
                        case 2:
                            DateTime nextDate2 = DateTime.Today;
                            for (var i = expenseRecurring.Date; i <= DateTime.Now; i = i.Value.AddMonths(1))
                            {
                                Expense expense = Helper.CreateExpenseFromRecurring(expenseRecurring, expDate.Value);
                                expense.ExpenseManagerLogin = User.FindFirstValue(ClaimTypes.Name);
                                expense.StatusId = recurringId;

                                list.Add(expense);
                                nextDate2 = expense.Date.Value;
                                expDate = expDate.Value.AddMonths(1);
                            }
                            expenseRecurring.Date = nextDate2.AddMonths(1);
                            await _recurringRepository.UpdateAsync(expenseRecurring);
                            break;
                        case 3:
                            DateTime nextDate3 = DateTime.Today;
                            for (var i = expenseRecurring.Date; i <= DateTime.Now; i = i.Value.AddYears(1))
                            {
                                Expense expense = Helper.CreateExpenseFromRecurring(expenseRecurring, expDate.Value);
                                expense.ExpenseManagerLogin = User.FindFirstValue(ClaimTypes.Name);
                                expense.StatusId = recurringId;

                                list.Add(expense);
                                nextDate3 = expense.Date.Value;
                                expDate = expDate.Value.AddYears(1);
                            }
                            expenseRecurring.Date = nextDate3.AddYears(1);
                            await _recurringRepository.UpdateAsync(expenseRecurring);
                            break;
                        case 4: //For custom status, it iterates from expense date, day by day and checks if day of the week is the same as chosen during creation
                            DateTime nextDate4 = DateTime.Today;
                            for (var i = expenseRecurring.Date; i <= DateTime.Now.AddDays(7); i = i.Value.AddDays(1))
                            {
                                if (expenseVM.Weekdays[Convert.ToInt32(i.Value.DayOfWeek)])
                                {
                                    if (i <= DateTime.Now)
                                    {
                                        Expense expense = Helper.CreateExpenseFromRecurring(expenseRecurring, i.Value);
                                        expense.ExpenseManagerLogin = User.FindFirstValue(ClaimTypes.Name);
                                        expense.StatusId = recurringId;
                                        list.Add(expense);
                                        nextDate4 = expense.Date.Value;
                                    }


                                    if (i > DateTime.Now)
                                    {
                                        nextDate4 = i.Value;
                                        break;
                                    }
                                }
                            }
                            expenseRecurring.Date = nextDate4;
                            await _recurringRepository.UpdateAsync(expenseRecurring);
                            break;
                        default:
                            break;
                    }

                    await _expenseRepository.AddRangeAsync(list.AsEnumerable());
                }


                return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "RecurringPayments", _recurringRepository.GetAll()) });
            }
            return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, "CreateOrEdit", expenseVM) });
        }

        public async Task<IActionResult> TooltipSort(int id, string categoryName, int period) //Sorting list after tooltip action in charts
        {
            var expenses = await _expenseRepository.GetAllAsync();
            if (id >= 0)
            {
                var expense = await _expenseRepository.GetAsync(id);
                var model =expenses.Where(e => (e.Date.Value.Day == expense.Date.Value.Day
                           && e.Date.Value.Month == expense.Date.Value.Month));

                return Json(new { html = Helper.RenderRazorViewToString(this, "_ViewAll", model) });
            }
            else if (categoryName == null)
            {
                return Json(new { html = Helper.RenderRazorViewToString(this, "_ViewAll", expenses.Where(m => m.Date >= DateTime.Now.AddDays(-period)).OrderByDescending(d => d.Date)) });
            }
            else
            {
                var withPercentage = categoryName.Split(' ');
                withPercentage = withPercentage.Take(withPercentage.Count() - 1).ToArray();
                string str = String.Join(' ', withPercentage).Trim();
                return Json(new { html = Helper.RenderRazorViewToString(this, "_ViewAll", expenses.Where(m => m.Date >= DateTime.Now.AddDays(-period) && m.Category == str).OrderByDescending(d => d.Date)) });
            }
        }

        [HttpGet]
        [NoDirectAccess]
        public IActionResult CreateOrEditIncome(int id = 0)
        {
            return View(new Income());
        }

        // POST: Expenses/CreateOrEdit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrEditIncome(int id, [Bind("Id,Amount,Description,Date")] Income income) //Creation of income, currently without edit possibility
        {
            if (ModelState.IsValid)
            {
                var userLogin = User.FindFirstValue(ClaimTypes.Name);
                income.ExpenseManagerLogin = userLogin;
                income.FirstPaycheckDate = income.Date;
                int howManyMonthsToAdd = 0;

                if (income.Date.Value <= DateTime.Now)
                {
                    if (income.Date.Value.Month == DateTime.Now.Month)
                    {
                        if (income.Date.Value < DateTime.Now) howManyMonthsToAdd = 1;
                        else howManyMonthsToAdd = 0;
                    }
                    else
                    {
                        howManyMonthsToAdd = DateTime.Now.Month - income.Date.Value.Month;

                        if (income.Date.Value.Day < DateTime.Now.Day) howManyMonthsToAdd++;
                    }

                }


                income.Date = income.Date.Value.AddMonths(howManyMonthsToAdd);
                await _incomeRepository.AddAsync(income);
                var expenses = await _expenseRepository.GetAllAsync();
                return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAll", expenses.OrderByDescending(e => e.Date)) });
            }
            return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, "CreateOrEditIncome", income) });
        }
        public async Task<JsonResult> GetData(int period = 30)
        {
            List<string> newList = new();
            var ctg = _expenseRepository.GetCategoryRatios(period);
            foreach (var item in ctg)
            {
                newList.Add(item.CategoryPercentage.ToString());
            }
            var expenses = await _expenseRepository.GetAllAsync();
            DashboardDataModel data = new()
            {
                ListLast30 = _expenseRepository.GetSumOfExpensesInEachDayInLast30Days(period),
                CategoryRatios = ctg,
                CategoryCount = _categoryRepository.GetAll().ToList().Count(),
                TooltipList = _expenseRepository.GetTooltipList(period),
                ExpensesList = expenses.Where(m => m.Date >= DateTime.Now.AddDays(-period)).OrderBy(e => e.Date).AsEnumerable()
            };

            return Json(data);
        }
    }
}
