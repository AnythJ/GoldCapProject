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

        public DashboardController(IExpenseRepository expenseRepository)
        {
            _expenseRepository = expenseRepository;
        }

        public static Tuple<Income, Income> CreateIncomeInPeriod(Income item)
        {
            Income income = new Income
            {
                Amount = item.Amount,
                Description = item.Description,
                Id = item.Id,
                ExpenseManagerLogin = item.ExpenseManagerLogin,
                FirstPaycheckDate = item.FirstPaycheckDate
            };
            DateTime nextIncomeDate = item.Date.Value;
            for (var i = item.Date.Value; i <= DateTime.Today.AddDays(1); i = i.AddMonths(1))
            {
                income.Date = i;
                nextIncomeDate = i.AddMonths(1);
            }
            item.Date = nextIncomeDate;

            return Tuple.Create(item, income);
        }



        public IActionResult Index(int period = 30)
        {
            var lastmonthFirstDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1);
            var lastmonthLastDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddDays(-1);
            var expensesFromLastMonth = _expenseRepository.GetAllExpenses().Where(m => m.Date >= lastmonthFirstDay && m.Date <= lastmonthLastDay);

            var expensesFromThisPeriod = _expenseRepository.GetAllExpenses().Where(m => m.Date >= DateTime.Now.AddDays(-period)).OrderByDescending(d => d.Date);
            var categoriesWithAbove0Ratio = _expenseRepository.GetCategoryRatios(period).Where(c => c.CategoryPercentage > 0);
            var topCategory = _expenseRepository.GetCategoryRatios(period) != null ? _expenseRepository.GetCategoryRatios(period).FirstOrDefault() : null;


            #region MonthlyIncomeAddition
            var userIncomes = _expenseRepository.GetIncome(User.FindFirstValue(ClaimTypes.Name)).ToList();
            int totalIncome = 0;
            foreach (var item in userIncomes)
            {
                if (item.FirstPaycheckDate.Value <= DateTime.Now)
                {
                    totalIncome += (int)item.Amount;
                }

                if (item is not null && item.Date.Value <= DateTime.Now)
                {
                    Income income = new Income()
                    {
                        Amount = item.Amount,
                        Description = item.Description,
                        Id = item.Id,
                        ExpenseManagerLogin = item.ExpenseManagerLogin,
                        FirstPaycheckDate = item.FirstPaycheckDate
                    };
                    DateTime nextIncomeDate = item.Date.Value;
                    for (var i = item.Date.Value; i <= DateTime.Today.AddDays(1); i = i.AddMonths(1))
                    {
                        income.Date = i;
                        nextIncomeDate = i.AddMonths(1);
                    }

                    item.Date = nextIncomeDate;
                    _expenseRepository.UpdateIncome(income);
                }



            }
            if (period == 365) totalIncome *= 12; //Handles if period == year, it doesn't cover week period, since it isn't really needed

            var allRecurringExpenses = _expenseRepository.GetAllRecurring().ToList();

            foreach (var item in allRecurringExpenses)
            {
                if (item.Date.Value >= DateTime.Today.AddDays(-365) && item.Date.Value <= DateTime.Now)
                {
                    List<Expense> list = new List<Expense>();
                    switch (item.Status) //Handles all 4 recurring statuses: weekly, monthly, yearly and custom, for first 3, it just adds amount of days
                    {
                        case 0:
                            DateTime nextDate0 = item.Date.Value;
                            for (var i = item.Date.Value; i <= DateTime.Now; i = i.AddDays(1))
                            {
                                Expense expense = Helper.CreateExpenseFromRecurring(item, i);
                                expense.ExpenseManagerLogin = User.FindFirstValue(ClaimTypes.Name);
                                list.Add(expense);
                                nextDate0 = i.AddDays(1);
                            }
                            item.Date = nextDate0;
                            _expenseRepository.UpdateRecurring(item);
                            break;
                        case 1:
                            DateTime nextDate1 = item.Date.Value;
                            for (var i = item.Date.Value; i <= DateTime.Now; i = i.AddDays(7))
                            {
                                Expense expense = Helper.CreateExpenseFromRecurring(item, i);
                                expense.ExpenseManagerLogin = User.FindFirstValue(ClaimTypes.Name);
                                list.Add(expense);
                                nextDate1 = i.AddDays(7);
                            }
                            item.Date = nextDate1;
                            _expenseRepository.UpdateRecurring(item);
                            break;
                        case 2:
                            DateTime nextDate2 = item.Date.Value;
                            for (var i = item.Date.Value; i <= DateTime.Now; i = i.AddMonths(1))
                            {
                                Expense expense = Helper.CreateExpenseFromRecurring(item, i);
                                expense.ExpenseManagerLogin = User.FindFirstValue(ClaimTypes.Name);
                                list.Add(expense);
                                nextDate2 = i.AddMonths(1);
                            }
                            item.Date = nextDate2;
                            _expenseRepository.UpdateRecurring(item);
                            break;
                        case 3:
                            DateTime nextDate3 = item.Date.Value;
                            for (var i = item.Date.Value; i <= DateTime.Now; i = i.AddYears(1))
                            {
                                Expense expense = Helper.CreateExpenseFromRecurring(item, i);
                                expense.ExpenseManagerLogin = User.FindFirstValue(ClaimTypes.Name);
                                list.Add(expense);
                                nextDate3 = i.AddYears(1);
                            }
                            item.Date = nextDate3;
                            _expenseRepository.UpdateRecurring(item);
                            break;
                        case 4:
                            /// <summary>
                            /// Iterates from the latest date to todays date checking if day of the week is true in wkdays, that was provided by user when creating
                            /// </summary>
                            DateTime nextDate4 = DateTime.Today;
                            List<string> wkdays = item.WeekdaysInString.Split(',').ToList();
                            for (var i = item.Date; i <= DateTime.Now.AddDays(7); i = i.Value.AddDays(1))
                            {

                                if (wkdays[Convert.ToInt32(i.Value.DayOfWeek)] == "True")
                                {
                                    if (i < DateTime.Now)
                                    {
                                        Expense expense = Helper.CreateExpenseFromRecurring(item, i.Value);
                                        expense.ExpenseManagerLogin = User.FindFirstValue(ClaimTypes.Name);
                                        expense.StatusId = item.Id;
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

                            item.Date = nextDate4;
                            _expenseRepository.UpdateRecurring(item);
                            break;
                        default:
                            break;
                    }

                    _expenseRepository.AddExpenses(list.AsEnumerable()); //Adds created expenses in period from start date to actual date as a list
                }
            }
            #endregion

            #region StatCircles

            #region LastPeriodCircle
            int sumExpenses = 0;
            foreach (var item in expensesFromThisPeriod) { sumExpenses += (int)item.Amount.Value; }

            int sumExpensesLastPeriod = 0;

            foreach (var item in expensesFromLastMonth) { sumExpensesLastPeriod += (int)item.Amount.Value; }

            var underPeriod = sumExpensesLastPeriod - sumExpenses;
            decimal percentage = 0;
            if (sumExpensesLastPeriod != 0) percentage = (Convert.ToDecimal(sumExpenses) / sumExpensesLastPeriod) * 100;


            #region UnderCircleTurnAnimationCalculations
            int degrees = (int)(3.6 * (int)percentage);
            var degreesStartRight = 0;
            var degreesRight = 0;
            var degreesLeft = 0;
            if (degrees > 0 && degrees <= 180)
            {
                degreesStartRight = 0;
                degreesRight = 0;
                degreesLeft = degrees;
            }
            else
            {
                degreesStartRight = 180;
                degreesRight = (degrees - 180) > 180 ? 180 : ((degrees - 180) == -180 ? 180 : (degrees - 180));
                degreesLeft = 180;
            }
            #endregion

            #region UnderCircleCreation
            StatCircle underCircle = new StatCircle()
            {
                PercentageLeft = degreesLeft + "deg",
                PercentageStartRight = degreesStartRight + "deg",
                PercentageRight = degreesRight + "deg",

                SumLast30Days = sumExpenses,
                SumBeforeLast30Days = sumExpensesLastPeriod,
                UnderMonthAmount = Math.Abs(underPeriod),
                TooltipPercentage = Decimal.Round(percentage),
                AdditionalString = underPeriod >= 0 ? "under" : "above"
            };

            switch (period)
            {
                case 7:
                    underCircle.PeriodName = "week";
                    break;
                case 30:
                    underCircle.PeriodName = "month";
                    break;
                case 365:
                    underCircle.PeriodName = "year";
                    break;
                    #endregion
            }
            #endregion

            #region IncomeCircle

            decimal percentageIncome = totalIncome != 0 ? Decimal.Round((Convert.ToDecimal(sumExpenses) / totalIncome) * 100) : 0;


            #region IncomeCircleTurnAnimationCalculations
            int degreesIncome = (int)(3.6 * (int)percentageIncome);

            var degreesStartRightIncome = 0;
            var degreesRightIncome = 0;
            var degreesLeftIncome = 0;

            if (degreesIncome > 0 && degreesIncome <= 180)
            {
                degreesStartRightIncome = 0;
                degreesRightIncome = 0;
                degreesLeftIncome = degreesIncome;
            }
            else
            {
                degreesStartRightIncome = 180;
                degreesRightIncome = (degreesIncome - 180) > 180 ? 180 : ((degreesIncome - 180) == -180 ? 180 : (degreesIncome - 180));
                degreesLeftIncome = 180;
            }

            #endregion

            #region IncomeCircleCreation
            StatCircle incomeCircle = new StatCircle()
            {
                PercentageLeft = degreesLeftIncome + "deg",
                PercentageStartRight = degreesStartRightIncome + "deg",
                PercentageRight = degreesRightIncome + "deg",

                TotalIncome = (int)totalIncome,
                IncomePercentage = percentageIncome,
                SumLast30Days = sumExpenses
            };
            #endregion
            #endregion

            #region CategoryCircle
            var topCate = topCategory != null ? expensesFromThisPeriod.Where(e => e.Category == topCategory.CategoryName).Sum(e => e.Amount) : 0;
            decimal percentageCategory = sumExpenses != 0 ? Decimal.Round((Convert.ToDecimal(topCate) / sumExpenses) * 100) : 0;

            #region CategoryCircleTurnAnimationCalculations
            int degreesCategory = (int)(3.6 * (int)percentageCategory);

            var degreesStartRightCategory = 0;
            var degreesRightCategory = 0;
            var degreesLeftCategory = 0;

            if (degreesCategory > 0 && degreesCategory <= 180)
            {
                degreesStartRightCategory = 0;
                degreesRightCategory = 0;
                degreesLeftCategory = degreesCategory;
            }
            else
            {
                degreesStartRightCategory = 180;
                degreesRightCategory = (degreesCategory - 180) > 180 ? 180 : ((degreesCategory - 180) == -180 ? 180 : (degreesCategory - 180));
                degreesLeftCategory = 180;
            }
            #endregion

            #region CategoryCircleCreation
            StatCircle cateCircle = new StatCircle()
            {
                PercentageLeft = degreesLeftCategory + "deg",
                PercentageStartRight = degreesStartRightCategory + "deg",
                PercentageRight = degreesRightCategory + "deg",

                TopCategoryPercentage = percentageCategory,
                TopCategoryName = topCategory != null ? topCategory.CategoryName : null,
                TopCategoryAmount = Convert.ToInt32(topCate),
                SumLast30Days = sumExpenses
            };
            #endregion

            #endregion

            #region AnimationSpeed
            NumberFormatInfo dotFormat = new NumberFormatInfo();
            dotFormat.NumberDecimalSeparator = ".";

            double sum0 = degreesLeft + degreesRight;
            double animationProportionUnder = sum0 != 0 ? degreesLeft / sum0 : 0.5;
            underCircle.LeftSpeed = (animationProportionUnder * 0.5).ToString(dotFormat) + "s";
            underCircle.RightSpeed = (0.5 - (animationProportionUnder * 0.5)).ToString(dotFormat) + "s";

            double sum1 = degreesLeftCategory + degreesRightCategory;
            double animationProportionCategory = sum1 != 0 ? degreesLeftCategory / sum1 : 0.5;
            cateCircle.LeftSpeed = (animationProportionCategory * 0.5).ToString(dotFormat) + "s";
            cateCircle.RightSpeed = (0.5 - (animationProportionCategory * 0.5)).ToString(dotFormat) + "s";

            double sum2 = degreesLeftIncome + degreesRightIncome;
            double animationProportionIncome = sum2 != 0 ? degreesLeftIncome / sum2 : 0.5;
            incomeCircle.LeftSpeed = (animationProportionIncome * 0.5).ToString(dotFormat) + "s";
            incomeCircle.RightSpeed = (0.5 - (animationProportionIncome * 0.5)).ToString(dotFormat) + "s";
            #endregion

            List<StatCircle> circleStats = new List<StatCircle>();
            circleStats.Add(underCircle);
            circleStats.Add(cateCircle);
            circleStats.Add(incomeCircle);
            #endregion

            #region StatPills
            List<StatPill> pillsStats = new List<StatPill>();
            #region FirstPill
            var highestExpense = _expenseRepository.GetAllExpenses().Where(m => m.Date >= DateTime.Now.AddDays(-period)).OrderByDescending(d => d.Amount).FirstOrDefault();

            StatPill firstPill = new StatPill();
            if (highestExpense != null)
            {
                firstPill.AmountInt = (int)highestExpense.Amount;
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

            StatPill secondPill = new StatPill();
            if (lastExpense != null)
            {
                secondPill.AmountInt = (int)lastExpense.Amount;
                secondPill.Category = lastExpense.Category;
                secondPill.DatetimeString = lastExpense.Date.Value.DayOfWeek + ", " + lastExpense.Date.Value.Day + " " + lastExpense.Date.Value.ToString("MMMM", CultureInfo.InvariantCulture);
                secondPill.Percentage = Decimal.Round((aboveOrBelow - 1) * 100, 1);
            }
            #endregion

            #region ThirdPill
            var lowestExpense = _expenseRepository.GetAllExpenses().Where(m => m.Date >= DateTime.Now.AddDays(-period)).OrderBy(d => d.Amount).FirstOrDefault();
            StatPill thirdPill = new StatPill();
            if (lowestExpense != null)
            {
                thirdPill.AmountInt = (int)lowestExpense.Amount;
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
            List<Expense> notificationList = new List<Expense>();
            List<ExpenseRecurring> firstFiveIncomingExpenses = allRecurringExpenses.OrderBy(e => e.Date).Take(3).ToList();
            foreach (var item in firstFiveIncomingExpenses)
            {
                if (k == 4) break;

                Expense newExpense = Helper.CreateExpenseFromRecurring(item, item.Date.Value);
                notificationList.Add(newExpense);
                k++;
            }
            DashboardViewModel dashboardViewModel = new DashboardViewModel()
            {
                PillsStats = pillsStats,
                Expenses = expensesFromThisPeriod,
                CirclesStats = circleStats,
                Period = period,
                NotificationList = notificationList
            }; 
            #endregion


            if (categoriesWithAbove0Ratio != null)
            {
                dashboardViewModel.Categories = categoriesWithAbove0Ratio;
            }


            return View(dashboardViewModel);
        }


        public IActionResult Sort(string sortOrder, int id, string categoryName = null, int period = 30)
        {
            var model = _expenseRepository.GetAllExpenses().Where(m => m.Date >= DateTime.Now.AddDays(-period));

            if (id > 0 && sortOrder != "default")
            {
                var expense = _expenseRepository.GetExpense(id);
                model = _expenseRepository.GetAllExpenses().Where(e => e.Date.Value.DayOfYear == expense.Date.Value.DayOfYear && e.Date.Value.Year == expense.Date.Value.Year);
            }
            else if (categoryName != null)
            {
                model = _expenseRepository.GetAllExpenses().Where(e => e.Category == categoryName.Split(" ")[0] && e.Date >= DateTime.Today.AddDays(-period));
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

        [HttpPost, ActionName("DeleteIncome")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteIncome(int id)
        {
            var userLogin = User.FindFirstValue(ClaimTypes.Name);
            var inc = _expenseRepository.GetIncome(userLogin).FirstOrDefault();
            _expenseRepository.DeleteIncome(id);

            return Json(new { html = Helper.RenderRazorViewToString(this, "IncomeList", _expenseRepository.GetIncome(userLogin)) });
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
        public IActionResult IncomeList()
        {
            var userLogin = User.FindFirstValue(ClaimTypes.Name);
            var incomeModel = _expenseRepository.GetIncome(userLogin);
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
            ViewBag.CategoryList = _expenseRepository.GetCategoryList().OrderBy(c => c.Name);
            string[] weekdays = new string[7] { "Sn", "M", "T", "W", "Th", "F", "S" };
            ViewBag.Weekdays = weekdays;

            ExpenseRecurringViewModel model = new ExpenseRecurringViewModel();
            model.Status = preStatus;

            return View(model);

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
            ViewBag.CategoryList = _expenseRepository.GetCategoryList().OrderBy(c => c.Name);
            ExpenseRecurring expenseRecurring = new ExpenseRecurring()
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
                _expenseRepository.AddRecurring(expenseRecurring);
                var lastAddedRecurring = _expenseRepository.UpdateRecurring(expenseRecurring);
                _expenseRepository.UpdateRecurring(expenseRecurring);

                if (expenseRecurring.Date.Value <= DateTime.Today)
                {
                    List<Expense> list = new List<Expense>();
                    var expDate = expenseRecurring.Date;
                    var recurringId = lastAddedRecurring.Id;
                    switch (expenseRecurring.Status) //Adding expenses from recurring expenses if their date is before today's date, for first 3 it simply adds days depeing on status (weekly, monthly, yearly)
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
                            _expenseRepository.UpdateRecurring(expenseRecurring);
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
                            _expenseRepository.UpdateRecurring(expenseRecurring);
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
                            _expenseRepository.UpdateRecurring(expenseRecurring);
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
                            _expenseRepository.UpdateRecurring(expenseRecurring);
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
                            _expenseRepository.UpdateRecurring(expenseRecurring);
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

        public IActionResult TooltipSort(int id, string categoryName, int period) //Sorting list after tooltip action in charts
        {
            if (id >= 0)
            {
                var expense = _expenseRepository.GetExpense(id);
                var model = _expenseRepository.GetAllExpenses().Where(e => (e.Date.Value.Day == expense.Date.Value.Day
                           && e.Date.Value.Month == expense.Date.Value.Month));

                return Json(new { html = Helper.RenderRazorViewToString(this, "_ViewAll", model) });
            }
            else if (categoryName == null)
            {
                return Json(new { html = Helper.RenderRazorViewToString(this, "_ViewAll", _expenseRepository.GetAllExpenses().Where(m => m.Date >= DateTime.Now.AddDays(-period)).OrderByDescending(d => d.Date)) });
            }
            else
            {
                var withPercentage = categoryName.Split(' ');
                withPercentage = withPercentage.Take(withPercentage.Count() - 1).ToArray();
                string str = String.Join(' ', withPercentage).Trim();
                return Json(new { html = Helper.RenderRazorViewToString(this, "_ViewAll", _expenseRepository.GetAllExpenses().Where(m => m.Date >= DateTime.Now.AddDays(-period) && m.Category == str).OrderByDescending(d => d.Date)) });
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
        public IActionResult CreateOrEditIncome(int id, [Bind("Id,Amount,Description,Date")] Income income) //Creation of income, currently without editon possibility
        {
            if (ModelState.IsValid)
            {
                var userLogin = User.FindFirstValue(ClaimTypes.Name);
                income.ExpenseManagerLogin = userLogin;
                income.FirstPaycheckDate = income.Date;
                int howManyMonthsToAdd = DateTime.Now.Year >= income.Date.Value.Year ? DateTime.Now.Month + 1 - income.Date.Value.Month : 0;
                income.Date = income.Date.Value.AddMonths(howManyMonthsToAdd);
                _expenseRepository.AddIncome(income);

                return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAll", _expenseRepository.GetAllExpenses().OrderByDescending(e => e.Date)) });
            }
            return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, "CreateOrEditIncome", income) });
        }
        public JsonResult GetData(int period = 30)
        {
            List<string> newList = new List<string>();
            var ctg = _expenseRepository.GetCategoryRatios(period);
            foreach (var item in ctg)
            {
                newList.Add(item.CategoryPercentage.ToString());
            }

            DashboardDataModel data = new DashboardDataModel()
            {
                ListLast30 = _expenseRepository.GetSumDayExpense30(period),
                CategoryRatios = _expenseRepository.GetCategoryRatios(period),
                CategoryCount = _expenseRepository.GetAllCategories().Count(),
                TooltipList = _expenseRepository.GetTooltipList(period),
                ExpensesList = _expenseRepository.GetAllExpenses().Where(m => m.Date >= DateTime.Now.AddDays(-period)).OrderBy(e => e.Date).AsEnumerable()
            };

            return Json(data);
        }
    }
}
