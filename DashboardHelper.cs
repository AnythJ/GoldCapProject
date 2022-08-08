using GoldCap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GoldCap
{
    public static class DashboardHelper
    {
        public static async void UpdateRecurringExpensesOnIndex(List<ExpenseRecurring> allRecurringExpenses, IExpenseRepository _expenseRepository, string expenseManageLogin)
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
                            await _expenseRepository.UpdateRecurringAsync(item);
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
                            await _expenseRepository.UpdateRecurringAsync(item);
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
                            await _expenseRepository.UpdateRecurringAsync(item);
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
                            await _expenseRepository.UpdateRecurringAsync(item);
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
                            await _expenseRepository.UpdateRecurringAsync(item);
                            break;
                        default:
                            break;
                    }

                    await _expenseRepository.AddExpensesAsync(list.AsEnumerable()); //Adds created expenses in period from start date to actual date as a list
                }
            }
        }
    
        public static async Task<decimal> GetAndUpdateIncomes(List<Income> userIncomes, IExpenseRepository _expenseRepository, string expenseManageLogin)
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
                    await _expenseRepository.UpdateIncomeAsync(item);
                }

            }

            return totalIncome;
        }
    }
}
