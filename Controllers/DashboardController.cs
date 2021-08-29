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

        public IActionResult Index(int period = 30)
        {
            var thisMonth = _expenseRepository.GetAllExpenses().Where(m => m.Date >= DateTime.Now.AddDays(-period)).OrderByDescending(d => d.Date);
            var lastMonth = _expenseRepository.GetAllExpenses().Where(m => m.Date >= DateTime.Now.AddDays(-period * 2) && m.Date <= DateTime.Now.AddDays(-period));
            var topCategories = _expenseRepository.GetCategoryRatios(period).Where(c => c.CategoryPercentage > 0);
            var topCategory = _expenseRepository.GetCategoryRatios(period) != null ? _expenseRepository.GetCategoryRatios(period).FirstOrDefault() : null;

            DashboardViewModel dashboardViewModel = new DashboardViewModel();

            #region MonthlyAddition
            var userIncomes = _expenseRepository.GetIncome(User.FindFirstValue(ClaimTypes.Name)).ToList();
            int totalIncome = 0;
            foreach (var item in userIncomes)
            {
                if (item != null && item.Date.Value >= DateTime.Today.AddDays(-365) && item.Date.Value <= DateTime.Now)
                {
                    Income income = new Income();
                    DateTime finalIncomeDate = item.Date.Value;
                    for (var i = item.Date.Value; i <= DateTime.Today.AddDays(1); i = i.AddMonths(1))
                    {
                        income.Amount = item.Amount;
                        income.Description = item.Description;
                        income.Date = i;
                        finalIncomeDate = i.AddMonths(1);
                    }
                    item.Date = finalIncomeDate;
                    _expenseRepository.UpdateIncome(item);
                }


                totalIncome += (int)item.Amount;
                if (period >= 31)
                    totalIncome *= 12;
            }

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
            else if (avg == 0)
            {
                rightStart = 180;
                avgRight = 180;
                avgLeft = 180;
            }
            else
            {
                rightStart = 180;
                avgRight = (avg - 180) > 180 ? 180 : avg - 180;
                avgLeft = 180;
            }

            StatCircle underCircle = new StatCircle();
            underCircle.PercentageLeft = avgLeft + "deg";
            underCircle.PercentageStartRight = rightStart + "deg";
            underCircle.PercentageRight = avgRight + "deg";

            underCircle.SumLast30Days = sumExpenses;
            underCircle.SumBeforeLast30Days = sumExpensesLastMonth;
            underCircle.UnderMonthAmount = underMonth;
            underCircle.TooltipPercentage = Decimal.Round(percentage);
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

            incomeCircle.TotalIncome = (int)totalIncome;
            incomeCircle.IncomePercentage = percentageIncome;
            incomeCircle.SumLast30Days = sumExpenses;

            int avgI = (int)(3.6 * (int)percentageIncome);

            var rightStartI = 0;
            var avgRightI = 0;
            var avgLeftI = 0;

            if (avgI > 0 && avgI <= 180)
            {
                rightStartI = 0;
                avgRightI = 0;
                avgLeftI = avgI;
            }
            else if(avgI == 0)
            {
                rightStartI = 180;
                avgRightI = 180;
                avgLeftI = 180;
            }
            else
            {
                rightStartI = 180;
                avgRightI = (avgI - 180) > 180 ? 180 : avgI - 180;
                avgLeftI = 180;
            }


            incomeCircle.PercentageLeft = avgLeftI + "deg";
            incomeCircle.PercentageStartRight = rightStartI + "deg";
            incomeCircle.PercentageRight = avgRightI + "deg";
            #endregion
            #region CategoryCircle
            var topCate = thisMonth.Where(e => e.Category == topCategory.CategoryName).Sum(e => e.Amount);
            StatCircle cateCircle = new StatCircle();
            decimal percentageCategory = sumExpenses != 0 ? Decimal.Round((Convert.ToDecimal(topCate) / sumExpenses) * 100) : 0;

            cateCircle.TopCategoryPercentage = percentageCategory;
            cateCircle.TopCategoryName = topCategory != null ? topCategory.CategoryName : null;
            cateCircle.TopCategoryAmount = (int)topCate;

            cateCircle.SumLast30Days = sumExpenses;
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
            else if (avgC == 0)
            {
                rightStartC = 180;
                avgRightC = 180;
                avgLeftC = 180;
            }
            else
            {
                rightStartC = 180;
                avgRightC = (avgC - 180) > 180 ? 180 : avgC - 180;
                avgLeftC = 180;
            }


            
            #region AnimationSpeed
            NumberFormatInfo dotFormat = new NumberFormatInfo();
            dotFormat.NumberDecimalSeparator = ".";

            double dg = avgLeftC + avgRightC;
            double animationProportionCategory = dg != 0 ? avgLeftC / dg : 0.5;
            cateCircle.LeftSpeed = (animationProportionCategory * 0.5).ToString(dotFormat) + "s";
            cateCircle.RightSpeed = (0.5 - (animationProportionCategory * 0.5)).ToString(dotFormat) + "s";

            double gd = avgLeft + avgRight;
            double animationProportionUnder = gd != 0 ? avgLeft / gd : 0.5;
            underCircle.LeftSpeed = (animationProportionUnder * 0.5).ToString(dotFormat) + "s";
            underCircle.RightSpeed = (0.5 - (animationProportionUnder * 0.5)).ToString(dotFormat) + "s";

            double ic = avgLeftI + avgRightI;
            double animationProportionIncome = ic != 0 ? avgLeftI / ic : 0.5;
            incomeCircle.LeftSpeed = (animationProportionIncome * 0.5).ToString(dotFormat) + "s";
            incomeCircle.RightSpeed = (0.5 - (animationProportionIncome * 0.5)).ToString(dotFormat) + "s";
            #endregion

            cateCircle.PercentageLeft = avgLeftC + "deg";
            cateCircle.PercentageStartRight = rightStartC + "deg";
            cateCircle.PercentageRight = avgRightC + "deg";
            #endregion


            List<StatCircle> circleStats = new List<StatCircle>();
            circleStats.Add(underCircle);
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
                firstPill.Percentage = (sumExpensesLastMonth != 0) ? Decimal.Round((Convert.ToDecimal(topExpense.Amount) / sumExpensesLastMonth) * 100, 1) : 0;
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
                thirdPill.Percentage = (sumExpensesLastMonth != 0) ? Decimal.Round((Convert.ToDecimal(lowestExpense.Amount) / sumExpensesLastMonth) * 100, 1) : 0;
                thirdPill.DatetimeString = lowestExpense.Date.Value.ToString("dd/M/yyyy hh:mm");
            }


            pillsStats.Add(firstPill);
            pillsStats.Add(secondPill);
            pillsStats.Add(thirdPill);
            #endregion

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
