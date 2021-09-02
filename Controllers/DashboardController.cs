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
            Income income = new Income();
            DateTime nextIncomeDate = item.Date.Value;
            for (var i = item.Date.Value; i <= DateTime.Today.AddDays(1); i = i.AddMonths(1))
            {
                income.Amount = item.Amount;
                income.Description = item.Description;
                income.Date = i;
                nextIncomeDate = i.AddMonths(1);
            }
            item.Date = nextIncomeDate;

            return Tuple.Create(item, income);
        }

        public static Expense CreateExpenseFromRecurring(ExpenseRecurring item, DateTime date)
        {
            Expense expense = new Expense()
            {
                Amount = item.Amount,
                Category = item.Category,
                Description = item.Description,
                Status = ((StatusName)item.Status).ToString(),
                Date = date,
                StatusId = item.Id
            };

            return expense;
        }
        public IActionResult Index(int period = 30)
        {
            var thisMonth = _expenseRepository.GetAllExpenses().Where(m => m.Date >= DateTime.Now.AddDays(-period)).OrderByDescending(d => d.Date);
            var lastMonth = _expenseRepository.GetAllExpenses().Where(m => m.Date >= DateTime.Now.AddDays(-period * 2) && m.Date <= DateTime.Now.AddDays(-period));
            var topCategories = _expenseRepository.GetCategoryRatios(period).Where(c => c.CategoryPercentage > 0);
            var topCategory = _expenseRepository.GetCategoryRatios(period) != null ? _expenseRepository.GetCategoryRatios(period).FirstOrDefault() : null;

            #region MonthlyAddition
            var userIncomes = _expenseRepository.GetIncome(User.FindFirstValue(ClaimTypes.Name)).ToList();
            int totalIncome = 0;
            foreach (var item in userIncomes)
            {
                if (item is not null && item.Date.Value <= DateTime.Now)
                {
                    Income income = CreateIncomeInPeriod(item).Item1;
                    _expenseRepository.UpdateIncome(CreateIncomeInPeriod(item).Item2);
                }

                totalIncome += (int)item.Amount;
                if (period == 365)
                    totalIncome *= 12;
            }

            var allRecurring = _expenseRepository.GetAllRecurring().ToList();
            foreach (var item in allRecurring)
            {
                if (item.Date.Value >= DateTime.Today.AddDays(-365) && item.Date.Value <= DateTime.Now)
                {
                    List<Expense> list = new List<Expense>();
                    switch (item.Status)
                    {
                        case 0:
                            DateTime nextDate0 = item.Date.Value;
                            for (var i = item.Date.Value; i <= DateTime.Today.AddDays(1); i = i.AddDays(1))
                            {
                                Expense expense = CreateExpenseFromRecurring(item, i);
                                expense.ExpenseManagerLogin = User.FindFirstValue(ClaimTypes.Name);
                                list.Add(expense);
                                nextDate0 = i.AddDays(1);
                            }
                            item.Date = nextDate0;
                            _expenseRepository.UpdateRecurring(item);
                            break;
                        case 1:
                            DateTime nextDate1 = item.Date.Value;
                            for (var i = item.Date.Value; i <= DateTime.Today.AddDays(1); i = i.AddDays(7))
                            {
                                Expense expense = CreateExpenseFromRecurring(item, i);
                                expense.ExpenseManagerLogin = User.FindFirstValue(ClaimTypes.Name);
                                list.Add(expense);
                                nextDate1 = i.AddDays(7);
                            }
                            item.Date = nextDate1;
                            _expenseRepository.UpdateRecurring(item);
                            break;
                        case 2:
                            DateTime nextDate2 = item.Date.Value;
                            for (var i = item.Date.Value; i <= DateTime.Today.AddDays(1); i = i.AddMonths(1))
                            {
                                Expense expense = CreateExpenseFromRecurring(item, i);
                                expense.ExpenseManagerLogin = User.FindFirstValue(ClaimTypes.Name);
                                list.Add(expense);
                                nextDate2 = i.AddMonths(1);
                            }
                            item.Date = nextDate2;
                            _expenseRepository.UpdateRecurring(item);
                            break;
                        case 3:
                            DateTime nextDate3 = item.Date.Value;
                            for (var i = item.Date.Value; i <= DateTime.Today.AddDays(1); i = i.AddYears(1))
                            {
                                Expense expense = CreateExpenseFromRecurring(item, i);
                                expense.ExpenseManagerLogin = User.FindFirstValue(ClaimTypes.Name);
                                list.Add(expense);
                                nextDate3 = i.AddYears(1);
                            }
                            item.Date = nextDate3;
                            _expenseRepository.UpdateRecurring(item);
                            break;
                        case 4:
                            DateTime nextDate4 = item.Date.Value;
                            List<string> wkdays = item.WeekdaysInString.Split(',').ToList();
                            for (DateTime i = item.Date.Value; i <= DateTime.Now; i = i.AddDays(7 * (int)item.HowOften))
                            {
                                int k = 0;
                                for (int j = 0; j < 7; j++)
                                {
                                    if (wkdays[j] == "True" && k == 0)
                                    {
                                        Expense expense = CreateExpenseFromRecurring(item, i);
                                        expense.ExpenseManagerLogin = User.FindFirstValue(ClaimTypes.Name);
                                        for (DateTime time = item.Date.Value; time <= DateTime.Now; time = time.AddDays(7 * (int)item.HowOften))
                                        {
                                            nextDate4 = nextDate4.AddDays(7 * (int)item.HowOften);
                                        }
                                        list.Add(expense);
                                        k = 1;
                                    }
                                    else if (wkdays[j] == "True")
                                    {
                                        Expense expense = CreateExpenseFromRecurring(item, i);
                                        expense.ExpenseManagerLogin = User.FindFirstValue(ClaimTypes.Name);
                                        expense.Date = i.AddDays(j);
                                        list.Add(expense);
                                    }
                                }
                            }
                            item.Date = nextDate4;
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
            int sumExpensesLastPeriod = 0;
            foreach (var item in lastMonth)
            {
                sumExpensesLastPeriod += (int)item.Amount.Value;
            }
            var underPeriod = sumExpensesLastPeriod - sumExpenses;
            decimal percentage = 0;
            if (sumExpensesLastPeriod != 0)
                percentage = (Convert.ToDecimal(sumExpenses) / sumExpensesLastPeriod) * 100;

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

            StatCircle underCircle = new StatCircle();
            underCircle.PercentageLeft = degreesLeft + "deg";
            underCircle.PercentageStartRight = degreesStartRight + "deg";
            underCircle.PercentageRight = degreesRight + "deg";

            underCircle.SumLast30Days = sumExpenses;
            underCircle.SumBeforeLast30Days = sumExpensesLastPeriod;
            underCircle.UnderMonthAmount = Math.Abs(underPeriod);
            underCircle.TooltipPercentage = Decimal.Round(percentage);
            underCircle.AdditionalString = underPeriod >= 0 ? "under" : "above";
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
            }
            #endregion

            #region IncomeCircle
            StatCircle incomeCircle = new StatCircle();
            decimal percentageIncome = totalIncome != 0 ? Decimal.Round((Convert.ToDecimal(sumExpenses) / totalIncome) * 100) : 0;

            int degreesIncome = (int)(3.6 * (int)percentageIncome);

            var degreesStartRightIncome = 0;
            var degreesRightIncome= 0;
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

            incomeCircle.PercentageLeft = degreesLeftIncome + "deg";
            incomeCircle.PercentageStartRight = degreesStartRightIncome + "deg";
            incomeCircle.PercentageRight = degreesRightIncome + "deg";

            incomeCircle.TotalIncome = (int)totalIncome;
            incomeCircle.IncomePercentage = percentageIncome;
            incomeCircle.SumLast30Days = sumExpenses;
            #endregion

            #region CategoryCircle
            var topCate = topCategory != null ? thisMonth.Where(e => e.Category == topCategory.CategoryName).Sum(e => e.Amount) : 0;
            decimal percentageCategory = sumExpenses != 0 ? Decimal.Round((Convert.ToDecimal(topCate) / sumExpenses) * 100) : 0;
            StatCircle cateCircle = new StatCircle();

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
                degreesRightCategory = (degreesCategory - 180) == -180 ? 180 : degreesCategory - 100;
                degreesLeftCategory = 180;
            }


            cateCircle.PercentageLeft = degreesLeftCategory + "deg";
            cateCircle.PercentageStartRight = degreesStartRightCategory + "deg";
            cateCircle.PercentageRight = degreesRightCategory + "deg";

            cateCircle.TopCategoryPercentage = percentageCategory;
            cateCircle.TopCategoryName = topCategory != null ? topCategory.CategoryName : null;
            cateCircle.TopCategoryAmount = Convert.ToInt32(topCate);
            cateCircle.SumLast30Days = sumExpenses;

            #endregion

            #region AnimationSpeed
            NumberFormatInfo dotFormat = new NumberFormatInfo();
            dotFormat.NumberDecimalSeparator = ".";

            double sum0 = degreesLeft + degreesRight;
            double animationProportionUnder = sum0 != 0 ? degreesLeft / sum0 : 0.5;
            underCircle.LeftSpeed = (animationProportionUnder * 0.5).ToString(dotFormat) + "s";
            underCircle.RightSpeed = (0.5 - (animationProportionUnder * 0.5)).ToString(dotFormat) + "s";

            double sum1 = degreesLeftCategory + degreesRightCategory;
            double animationProportionCategory = sum0 != 0 ? degreesLeftCategory / sum0 : 0.5;
            cateCircle.LeftSpeed = (animationProportionCategory * 0.5).ToString(dotFormat) + "s";
            cateCircle.RightSpeed = (0.5 - (animationProportionCategory * 0.5)).ToString(dotFormat) + "s";

            double sum2 = degreesLeftIncome + degreesRightIncome;
            double animationProportionIncome = sum2 != 0 ? degreesLeftIncome / sum2 : 0.5;
            incomeCircle.LeftSpeed = (animationProportionIncome * 0.5).ToString(dotFormat) + "s";
            incomeCircle.RightSpeed = (0.5 - (animationProportionIncome * 0.5)).ToString(dotFormat) + "s";
            #endregion

            List<StatCircle> circleStats = new List<StatCircle>();
            circleStats.Add(incomeCircle);
            circleStats.Add(cateCircle);
            circleStats.Add(incomeCircle);
            #endregion

            #region StatPills
            List<StatPill> pillsStats = new List<StatPill>();
            var topExpense = _expenseRepository.GetAllExpenses().Where(m => m.Date >= DateTime.Now.AddDays(-period)).OrderByDescending(d => d.Amount).FirstOrDefault();
            var lastExpense = thisMonth.FirstOrDefault();
            StatPill firstPill = new StatPill();
            if (topExpense != null)
            {
                firstPill.AmountInt = (int)topExpense.Amount;
                firstPill.Category = topExpense.Category;
                firstPill.Date = topExpense.Date;
                firstPill.Percentage = (sumExpensesLastPeriod != 0) ? Decimal.Round((Convert.ToDecimal(topExpense.Amount) / sumExpensesLastPeriod) * 100, 1) : 0;
                firstPill.DatetimeString = topExpense.Date.Value.ToString("dd/M/yyyy hh:mm");
            }

            decimal averageAmount = thisMonth.Count() != 0 ? sumExpenses / thisMonth.Count() : 0;
            decimal aboveOrBelow = averageAmount != 0 ? (decimal)lastExpense.Amount / averageAmount : 0;
            StatPill secondPill = new StatPill();
            if (lastExpense != null)
            {
                secondPill.AmountInt = (int)lastExpense.Amount;
                secondPill.Category = lastExpense.Category;
                secondPill.DatetimeString = lastExpense.Date.Value.DayOfWeek + ", " + lastExpense.Date.Value.Day + " " + lastExpense.Date.Value.ToString("MMMM", CultureInfo.InvariantCulture);
                secondPill.Percentage = Decimal.Round((aboveOrBelow - 1) * 100, 1);
            }
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


            pillsStats.Add(firstPill);
            pillsStats.Add(secondPill);
            pillsStats.Add(thirdPill);
            #endregion

            DashboardViewModel dashboardViewModel = new DashboardViewModel();

            dashboardViewModel.PillsStats = pillsStats;
            dashboardViewModel.Expenses = thisMonth;
            dashboardViewModel.CirclesStats = circleStats;

            if (topCategories != null)
            {
                dashboardViewModel.Categories = topCategories;
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
            ViewBag.CategoryList = _expenseRepository.GetCategoryList();
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
            ExpenseRecurring expense = new ExpenseRecurring()
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
            ViewBag.CategoryList = _expenseRepository.GetCategoryList();
            if (ModelState.IsValid)
            {
                _expenseRepository.AddRecurring(expense);
                var lastAddedRecurring = _expenseRepository.UpdateRecurring(expense);

                _expenseRepository.UpdateRecurring(expense);
                if (expense.Date.Value <= DateTime.Today)
                {
                    List<Expense> list = new List<Expense>();
                    var expDate = expense.Date;
                    var x = lastAddedRecurring.Id;
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
                                    StatusId = x,
                                    ExpenseManagerLogin = User.FindFirstValue(ClaimTypes.Name)
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
                                    StatusId = x,
                                    ExpenseManagerLogin = User.FindFirstValue(ClaimTypes.Name)
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
                            for (var i = expense.Date; i <= DateTime.Today; i = i.Value.AddMonths(1))
                            {
                                Expense exp = new Expense()
                                {
                                    Amount = expense.Amount,
                                    Category = expense.Category,
                                    Description = expense.Description,
                                    Status = ((StatusName)expense.Status).ToString(),
                                    Date = expDate,
                                    StatusId = x,
                                    ExpenseManagerLogin = User.FindFirstValue(ClaimTypes.Name)
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
                            for (var i = expense.Date; i <= DateTime.Today; i = i.Value.AddYears(1))
                            {
                                Expense exp = new Expense()
                                {
                                    Amount = expense.Amount,
                                    Category = expense.Category,
                                    Description = expense.Description,
                                    Status = ((StatusName)expense.Status).ToString(),
                                    Date = expDate,
                                    StatusId = x,
                                    ExpenseManagerLogin = User.FindFirstValue(ClaimTypes.Name)
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
                                        StatusId = x,
                                        ExpenseManagerLogin = User.FindFirstValue(ClaimTypes.Name)
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

        public IActionResult TooltipSort(int id, string categoryName, int period)
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
        public IActionResult CreateOrEditIncome(int id, [Bind("Id,Amount,Description,Date")] Income income)
        {
            if (ModelState.IsValid)
            {
                var userLogin = User.FindFirstValue(ClaimTypes.Name);
                income.ExpenseManagerLogin = userLogin;
                income.FirstPaycheckDate = income.Date;

                income.Date = income.Date.Value.AddMonths(1);
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
