using GoldCap.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GoldCap
{
    public static class DashboardHelper
    {
        public static async void UpdateRecurringExpensesOnIndex(List<ExpenseRecurring> allRecurringExpenses, IExpenseRepository _expenseRepository, IRecurringRepository _recurringRepository, IIncomeRepository _incomeRepository, string expenseManageLogin)
        {
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
                                expense.ExpenseManagerLogin = expenseManageLogin;
                                list.Add(expense);
                                nextDate0 = i.AddDays(1);
                            }
                            item.Date = nextDate0;
                            await _recurringRepository.UpdateAsync(item);
                            break;
                        case 1:
                            DateTime nextDate1 = item.Date.Value;
                            for (var i = item.Date.Value; i <= DateTime.Now; i = i.AddDays(7))
                            {
                                Expense expense = Helper.CreateExpenseFromRecurring(item, i);
                                expense.ExpenseManagerLogin = expenseManageLogin;
                                list.Add(expense);
                                nextDate1 = i.AddDays(7);
                            }
                            item.Date = nextDate1;
                            await _recurringRepository.UpdateAsync(item);
                            break;
                        case 2:
                            DateTime nextDate2 = item.Date.Value;
                            for (var i = item.Date.Value; i <= DateTime.Now; i = i.AddMonths(1))
                            {
                                Expense expense = Helper.CreateExpenseFromRecurring(item, i);
                                expense.ExpenseManagerLogin = expenseManageLogin;
                                list.Add(expense);
                                nextDate2 = i.AddMonths(1);
                            }
                            item.Date = nextDate2;
                            await _recurringRepository.UpdateAsync(item);
                            break;
                        case 3:
                            DateTime nextDate3 = item.Date.Value;
                            for (var i = item.Date.Value; i <= DateTime.Now; i = i.AddYears(1))
                            {
                                Expense expense = Helper.CreateExpenseFromRecurring(item, i);
                                expense.ExpenseManagerLogin = expenseManageLogin;
                                list.Add(expense);
                                nextDate3 = i.AddYears(1);
                            }
                            item.Date = nextDate3;
                            await _recurringRepository.UpdateAsync(item);
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
                                        expense.ExpenseManagerLogin = expenseManageLogin;
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
                            await _recurringRepository.UpdateAsync(item);
                            break;
                        default:
                            break;
                    }

                    await _expenseRepository.AddRangeAsync(list.AsEnumerable()); //Adds created expenses in period from start date to actual date as a list
                }
            }
        }
    
        public static async Task<decimal> GetAndUpdateIncomes(List<Income> userIncomes, IExpenseRepository _expenseRepository, IIncomeRepository _incomeRepository, string expenseManageLogin)
        {
            decimal totalIncome = 0;
            foreach (var item in userIncomes)
            {
                if (item.FirstPaycheckDate.Value <= DateTime.Now)
                {
                    totalIncome += item.Amount.Value;
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
                    await _incomeRepository.UpdateAsync(item);
                }

            }

            return totalIncome;
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

        public static StatCircle CreateLastPeriodStatCircle(int sumExpenses, int sumExpensesLastPeriod, int period)
        {
            NumberFormatInfo dotFormat = new();
            dotFormat.NumberDecimalSeparator = ".";

            var underPeriod = sumExpensesLastPeriod - sumExpenses;
            decimal percentage = 0;
            if (sumExpensesLastPeriod != 0) percentage = (Convert.ToDecimal(sumExpenses) / sumExpensesLastPeriod) * 100;

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


            StatCircle lastPeriodCircle = new()
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
                    lastPeriodCircle.PeriodName = "week";
                    break;
                case 30:
                    lastPeriodCircle.PeriodName = "month";
                    break;
                case 365:
                    lastPeriodCircle.PeriodName = "year";
                    break;
            }

            double sum0 = degreesLeft + degreesRight;
            double animationProportionUnder = sum0 != 0 ? degreesLeft / sum0 : 0.5;
            lastPeriodCircle.LeftSpeed = (animationProportionUnder * 0.5).ToString(dotFormat) + "s";
            lastPeriodCircle.RightSpeed = (0.5 - (animationProportionUnder * 0.5)).ToString(dotFormat) + "s";

            return lastPeriodCircle;
        }
   
        public static StatCircle CreateIncomeStatCircle(decimal totalIncome, int sumExpenses)
        {
            NumberFormatInfo dotFormat = new();
            dotFormat.NumberDecimalSeparator = ".";

            decimal percentageIncome = totalIncome != 0 ? Decimal.Round((Convert.ToDecimal(sumExpenses) / totalIncome) * 100) : 0;

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



            StatCircle incomeCircle = new()
            {
                PercentageLeft = degreesLeftIncome + "deg",
                PercentageStartRight = degreesStartRightIncome + "deg",
                PercentageRight = degreesRightIncome + "deg",

                TotalIncome = (int)totalIncome,
                IncomePercentage = percentageIncome,
                SumLast30Days = sumExpenses
            };

            double sum2 = degreesLeftIncome + degreesRightIncome;
            double animationProportionIncome = sum2 != 0 ? degreesLeftIncome / sum2 : 0.5;
            incomeCircle.LeftSpeed = (animationProportionIncome * 0.5).ToString(dotFormat) + "s";
            incomeCircle.RightSpeed = (0.5 - (animationProportionIncome * 0.5)).ToString(dotFormat) + "s";

            return incomeCircle;
        }

        public static StatCircle CreateCategoryStatCircle(int sumExpenses, decimal? topCate, CategoryChart topCategory)
        {
            NumberFormatInfo dotFormat = new();
            dotFormat.NumberDecimalSeparator = ".";

            decimal percentageCategory = sumExpenses != 0 ? Decimal.Round((Convert.ToDecimal(topCate) / sumExpenses) * 100) : 0;
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



            StatCircle cateCircle = new()
            {
                PercentageLeft = degreesLeftCategory + "deg",
                PercentageStartRight = degreesStartRightCategory + "deg",
                PercentageRight = degreesRightCategory + "deg",

                TopCategoryPercentage = percentageCategory,
                TopCategoryName = topCategory != null ? topCategory.CategoryName : null,
                TopCategoryAmount = Convert.ToInt32(topCate),
                SumLast30Days = sumExpenses
            };

            double sum1 = degreesLeftCategory + degreesRightCategory;
            double animationProportionCategory = sum1 != 0 ? degreesLeftCategory / sum1 : 0.5;
            cateCircle.LeftSpeed = (animationProportionCategory * 0.5).ToString(dotFormat) + "s";
            cateCircle.RightSpeed = (0.5 - (animationProportionCategory * 0.5)).ToString(dotFormat) + "s";

            return cateCircle;
        }
    }
}
