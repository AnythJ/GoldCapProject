using GoldCap.Models;
using GoldCap.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GoldCap.Controllers
{
    public class CategoriesController : Controller
    {
        private IExpenseRepository _expenseRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly string userLogin;

        public CategoriesController(IExpenseRepository expenseRepository, IHttpContextAccessor httpContextAccessor)
        {
            _expenseRepository = expenseRepository;
            this.userLogin = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
        }


        public IActionResult Index()
        {
            var categoriesList = _expenseRepository.GetAllCategories().Where(c => c.ExpenseManagerLogin == userLogin);
            
            int k = 0;
            List<Expense> notificationList = new List<Expense>();
            List<ExpenseRecurring> firstFiveIncomingExpenses = _expenseRepository.GetAllRecurring().ToList().OrderBy(e => e.Date).Take(3).ToList();
            foreach (var item in firstFiveIncomingExpenses)
            {
                if (k == 4) break;

                Expense newExpense = Helper.CreateExpenseFromRecurring(item, item.Date.Value);
                notificationList.Add(newExpense);
                k++;
            }
            

            CategoryListViewModel viewModel = new CategoryListViewModel()
            {
                Category = new Category(),
                Categories = categoriesList,
                NotificationList = notificationList
            };

            return View("index", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            category.ExpenseManagerLogin = userLogin;
            if (ModelState.IsValid)
            {
                await _expenseRepository.AddCategoryAsync(category);

                return RedirectToAction("index");
            }
            else
            {
                int k = 0;
                List<Expense> notificationList = new List<Expense>();
                List<ExpenseRecurring> firstFiveIncomingExpenses = _expenseRepository.GetAllRecurring().ToList().OrderBy(e => e.Date).Take(3).ToList();
                foreach (var item in firstFiveIncomingExpenses)
                {
                    if (k == 4) break;

                    Expense newExpense = Helper.CreateExpenseFromRecurring(item, item.Date.Value);
                    notificationList.Add(newExpense);
                    k++;
                }

                CategoryListViewModel invalidModel = new CategoryListViewModel() //If category name is incorrect, there has to be new viewModel created, since viewmodel is passed to view
                {
                    Category = category,
                    Categories = _expenseRepository.GetAllCategories(),
                    NotificationList = notificationList
                };
                return View("index", invalidModel);
            }

        }


        public async Task<IActionResult> Delete(int id)
        {
            await _expenseRepository.DeleteCategoryAsync(id);

            return RedirectToAction("index");
        }

        public JsonResult Sort(string sortOrder)
        {
            var model = _expenseRepository.GetAllCategories().Where(c => c.ExpenseManagerLogin == userLogin);

            ViewBag.CategorySortParm = sortOrder == "Category" ? "category_desc" : "Category";

            switch (sortOrder)
            {
                case "Category":
                    model = model.OrderBy(d => d.Name);
                    break;
                case "category_desc":
                    model = model.OrderByDescending(d => d.Name);
                    break;
                default:
                    break;
            }

            CategoryListViewModel viewModel = new CategoryListViewModel()
            {
                Category = new Category(),
                Categories = model
            };
            return Json(new { html = Helper.RenderRazorViewToString(this, "_ViewAll", viewModel) }); //Return view as partial with json
        }
    }
}
