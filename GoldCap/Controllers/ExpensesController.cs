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
        private readonly IUnitOfWork _unitOfWork;

        private UserManager<ApplicationUser> userManager;
        private readonly string userLogin;

        public ExpensesController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            this.userManager = userManager;
            _unitOfWork = unitOfWork;
            this.userLogin = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
        }


        public IActionResult Index()
        {
            ViewBag.CategoryList = _unitOfWork.CategoryRepository.GetAll().ToList().OrderBy(c => c.Name);

            var expenses = _unitOfWork.ExpenseRepository.GetAll();
            ExpensesListViewModel viewModel = new()
            {
                Expenses = expenses.Where(e => e.ExpenseManagerLogin == userLogin).OrderByDescending(e => e.Date),
                CategoriesList = _unitOfWork.CategoryRepository.GetAll().ToList().OrderBy(c => c.Name).ToList(),
                SortMenu = null,
                NotificationList = Helper.GetNotificationList(_unitOfWork.RecurringRepository)
            };

            return View(viewModel);
        }

        // GET: Expenses/CreateOrEdit
        // GET: Expenses/CreateOrEdit/id
        [HttpGet]
        [NoDirectAccess]
        public async Task<IActionResult> CreateOrEdit(int id = 0)
        {
            ViewBag.CategoryList = _unitOfWork.CategoryRepository.GetAll().ToList().OrderBy(c => c.Name);

            if (id == 0)
                return View(new Expense()); //After using create button, pass new model so the form will be empty
            else
            {
                var expenseModel = await _unitOfWork.ExpenseRepository.GetAsync(id); 
                if (expenseModel == null) 
                {
                    return NotFound(); 
                }

                return View(expenseModel); //Return form with correct object
            }
        }

        // POST: Expenses/CreateOrEdit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrEdit(int id, [Bind("Id,Amount,Category,Description,Date,ExpenseManagerLogin")] Expense expense, string sortOrder, bool filtered = false)
        {
            ViewBag.CategoryList = _unitOfWork.CategoryRepository.GetAll().ToList().OrderBy(c => c.Name);
            expense.ExpenseManagerLogin = userLogin;

            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    await _unitOfWork.ExpenseRepository.AddAsync(expense);
                }
                else
                {
                    _unitOfWork.ExpenseRepository.Update(expense);
                }

                await _unitOfWork.CompleteAsync();

                var expenses = _unitOfWork.ExpenseRepository.GetAll();
                return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAll", expenses.Where(e => e.ExpenseManagerLogin == userLogin)) }); //If form is valid, close modal and show list
            }
            else return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, "CreateOrEdit", expense) }); //If form is invalid return the same object in form
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string sortOrder, ExpensesListViewModel viewModel, bool filtered = false)
        {
            await _unitOfWork.ExpenseRepository.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();
            var expenses = _unitOfWork.ExpenseRepository.GetAll();
            ExpensesListViewModel newViewModel = new()
            {
                Expenses = expenses.OrderByDescending(e => e.Date),
                CategoriesList = _unitOfWork.CategoryRepository.GetAll().ToList().OrderBy(c => c.Name).ToList(),
                SortMenu = viewModel.SortMenu
            };

            return Sort(sortOrder, newViewModel, filtered, false); //After deleting, sort list like it was before
        }

        /// <summary>
        /// Sorting by both: headers and sort menu
        /// </summary>
        /// <param name="sortOrder">Sort order from column headers</param>
        /// <param name="viewModel"></param>
        /// <param name="filtered">Parameter to check whether sort menu was used or not</param>
        /// <param name="refresh">If refresh button was used</param>
        /// <returns></returns>
        public JsonResult Sort(string sortOrder, ExpensesListViewModel viewModel, bool filtered = false,  bool refresh = false) 
        {
            ViewBag.CategoryList = _unitOfWork.CategoryRepository.GetAll().ToList().OrderBy(c => c.Name);
            bool isSortEmpty = true;
            var model = _unitOfWork.ExpenseRepository.GetAll();
            if (viewModel.Expenses != null)
                model = viewModel.Expenses;

            if(filtered)
            {
                if (viewModel.SortMenu != null && !refresh) //Sort by every parameter that was selected in sort menu
                {
                    if (viewModel.SortMenu.DateFrom != null)
                    {
                        model = model.Where(e => e.Date >= viewModel.SortMenu.DateFrom);
                        isSortEmpty = false;
                    }
                        
                    if (viewModel.SortMenu.DateTo != null)
                    {
                        model = model.Where(e => e.Date <= viewModel.SortMenu.DateTo);
                        isSortEmpty = false;
                    }
                        
                    if (viewModel.SortMenu.PriceTo != 0)
                    {
                        model = model.Where(e => e.Amount <= viewModel.SortMenu.PriceTo);
                        isSortEmpty = false;
                    }
                        
                    if (viewModel.SortMenu.PriceFrom != 0)
                    {
                        model = model.Where(e => e.Amount >= viewModel.SortMenu.PriceFrom);
                        isSortEmpty = false;
                    }
                      
                    if (viewModel.SortMenu.DescriptionSearch != null)
                    {
                        model = model.Where(e => e.Description != null).Where(e => e.Description.Contains(viewModel.SortMenu.DescriptionSearch) == true);
                        isSortEmpty = false;
                    }
                        

                    if (viewModel.SortMenu.ChosenCategories.Contains(true)) //Categories list with ticked checkboxes
                    {
                        var cateList = _unitOfWork.CategoryRepository.GetAll().ToList().Select(e => e.Name).OrderBy(c => c).ToList();
                        List<string> categoriesList = new();
                        var chosenCategoriesList = viewModel.SortMenu.ChosenCategories.ToList();
                        var cateLength = chosenCategoriesList.Count();
                        for (int i = 0; i < cateLength; i++)
                        {
                            if (chosenCategoriesList[i])
                            {
                                categoriesList.Add(cateList[i]);
                            }
                        }
                        model = model.Where(e => categoriesList.Contains(e.Category)); //Select expenses where categories match the categoriesList content
                        isSortEmpty = false;
                    }
                }
            }
            

            ViewBag.AmountSortParm = sortOrder == "Amount" ? "amount_desc" : "Amount";
            ViewBag.CategorySortParm = sortOrder == "Category" ? "category_desc" : "Category";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            switch (sortOrder) //Sort based on sortOrder from column headers
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
            

            ExpensesListViewModel newViewModel = viewModel;
            newViewModel.CategoriesList = _unitOfWork.CategoryRepository.GetAll().ToList().OrderBy(c => c.Name).ToList();
            newViewModel.Expenses = model;

            if (isSortEmpty) newViewModel.SortMenu = null;

            return Json(new { html = Helper.RenderRazorViewToString(this, "_ViewAll", newViewModel) });
        }
    }
}
