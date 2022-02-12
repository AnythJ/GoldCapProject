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

        public IActionResult Index()
        {
            var categoriesList = _expenseRepository.GetAllCategories().Where(c => c.ExpenseManagerLogin == userLogin);
            
            int k = 0;
            List<Expense> notificationList = new List<Expense>();
            List<ExpenseRecurring> firstFiveIncomingExpenses = _expenseRepository.GetAllRecurring().ToList().OrderBy(e => e.Date).Take(3).ToList();
            foreach (var item in firstFiveIncomingExpenses)
            {
                if (k == 4) break;

                Expense newExpense = CreateExpenseFromRecurring(item, item.Date.Value);
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
        public IActionResult Create(Category category)
        {
            category.ExpenseManagerLogin = userLogin;
            if (ModelState.IsValid)
            {
                _expenseRepository.AddCategory(category);

                return RedirectToAction("index");
            }
            else
            {
                CategoryListViewModel invalidModel = new CategoryListViewModel() //If category name is incorrect, there has to be new viewModel created, since viewmodel is passed to view
                {
                    Category = category,
                    Categories = _expenseRepository.GetAllCategories()
                };
                return View("index", invalidModel);
            }

        }


        public IActionResult Delete(int id)
        {
            _expenseRepository.DeleteCategory(id);

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
