using GoldCap.Models;
using GoldCap.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static GoldCap.Helper;

namespace GoldCap.Controllers
{
    public class ExpensesController : Controller
    {
        private UserManager<ApplicationUser> userManager;
        private IExpenseRepository _expenseRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly string userLogin;

        public ExpensesController(IExpenseRepository expenseRepository, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            this.userManager = userManager;
            _expenseRepository = expenseRepository;
            this.userLogin = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
        }

        public IActionResult Index()
        {
            ViewBag.CategoryList = _expenseRepository.GetCategoryList().OrderBy(c => c.Name);
            ExpensesListViewModel viewModel = new ExpensesListViewModel()
            {
                Expenses = _expenseRepository.GetAllExpenses().Where(e => e.ExpenseManagerLogin == userLogin).OrderByDescending(e => e.Date),
                CategoriesList = _expenseRepository.GetCategoryList().OrderBy(c => c.Name).ToList()
            };

            return View(viewModel);
        }

        // GET: Expenses/CreateOrEdit
        // GET: Expenses/CreateOrEdit/id
        [HttpGet]
        [NoDirectAccess]
        public IActionResult CreateOrEdit(int id = 0)
        {
            ViewBag.CategoryList = _expenseRepository.GetCategoryList().OrderBy(c => c.Name);

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

        // POST: Expenses/CreateOrEdit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateOrEdit(int id, [Bind("Id,Amount,Category,Description,Date,ExpenseManagerLogin")] Expense expense)
        {
            ViewBag.CategoryList = _expenseRepository.GetCategoryList().OrderBy(c => c.Name);
            expense.ExpenseManagerLogin = userLogin;

            ExpensesListViewModel viewModel = new ExpensesListViewModel()
            {
                Expenses = _expenseRepository.GetAllExpenses().Where(e => e.ExpenseManagerLogin == userLogin).OrderByDescending(e => e.Date),
                CategoriesList = _expenseRepository.GetCategoryList().OrderBy(c => c.Name).ToList()
            };

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


                return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAll", _expenseRepository.GetAllExpenses().Where(e => e.ExpenseManagerLogin == userLogin)) });
            }
            return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, "CreateOrEdit", expense) });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(string sortOrder, int id, ExpensesListViewModel viewModel)
        {
            _expenseRepository.Delete(id);
            ExpensesListViewModel newViewModel = new ExpensesListViewModel()
            {
                Expenses = _expenseRepository.GetAllExpenses().OrderByDescending(e => e.Date),
                CategoriesList = _expenseRepository.GetCategoryList().OrderBy(c => c.Name).ToList(),
                SortMenu = viewModel.SortMenu
            };

            return Sort(sortOrder, newViewModel, true);
        }

        public IActionResult DeleteAll()
        {
            _expenseRepository.DeleteAllExpenses();

            return RedirectToAction("index", "expenses");
        }


        public JsonResult Sort(string sortOrder, ExpensesListViewModel viewModel, bool filtered = false,  bool refresh = false)
        {
            ViewBag.CategoryList = _expenseRepository.GetCategoryList().OrderBy(c => c.Name);

            var model = _expenseRepository.GetAllExpenses();
            if (viewModel.Expenses != null)
                model = viewModel.Expenses;

            if(filtered)
            {
                if (viewModel.SortMenu != null && !refresh)
                {
                    model = _expenseRepository.GetAllExpenses();
                    if (viewModel.SortMenu.DateFrom != null)
                        model = model.Where(e => e.Date >= viewModel.SortMenu.DateFrom);

                    if (viewModel.SortMenu.DateTo != null)
                        model = model.Where(e => e.Date <= viewModel.SortMenu.DateTo);

                    if (viewModel.SortMenu.PriceTo != 0)
                        model = model.Where(e => e.Amount <= viewModel.SortMenu.PriceTo);

                    if (viewModel.SortMenu.PriceFrom != 0)
                        model = model.Where(e => e.Amount >= viewModel.SortMenu.PriceFrom);

                    if (viewModel.SortMenu.DescriptionSearch != null)
                        model = model.Where(e => e.Description != null).Where(e => e.Description.Contains(viewModel.SortMenu.DescriptionSearch) == true);

                    if (viewModel.SortMenu.ChosenCategories.Contains(true))
                    {
                        var cateList = _expenseRepository.GetCategoryList().Select(e => e.Name).OrderBy(c => c).ToList();
                        List<string> categoriesList = new List<string>();
                        var chosenCategoriesList = viewModel.SortMenu.ChosenCategories.ToList();
                        var cateLength = chosenCategoriesList.Count();
                        for (int i = 0; i < cateLength; i++)
                        {
                            if (chosenCategoriesList[i])
                            {
                                categoriesList.Add(cateList[i]);
                            }
                        }
                        model = model.Where(e => categoriesList.Contains(e.Category));
                    }
                }
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
            ExpensesListViewModel expVM = new ExpensesListViewModel()
            {
                CategoriesList = _expenseRepository.GetCategoryList().OrderBy(c => c.Name).ToList(),
                Expenses = model
            };
            ExpensesListViewModel newViewModel = viewModel;
            newViewModel.CategoriesList = _expenseRepository.GetCategoryList().OrderBy(c => c.Name).ToList();
            newViewModel.Expenses = model;
            return Json(new { html = Helper.RenderRazorViewToString(this, "_ViewAll", newViewModel) });
        }
    }
}
