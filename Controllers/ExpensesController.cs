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
            var expenses = _expenseRepository.GetAllExpenses().Where(e => e.ExpenseManagerLogin == userLogin).OrderByDescending(e => e.Date);

            return View(expenses);
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


                return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAll", _expenseRepository.GetAllExpenses().Where(e => e.ExpenseManagerLogin == userLogin).OrderByDescending(e => e.Date)) });
            }
            return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, "CreateOrEdit", expense) });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _expenseRepository.Delete(id);

            return Json(new { html = Helper.RenderRazorViewToString(this, "_ViewAll", _expenseRepository.GetAllExpenses().Where(e => e.ExpenseManagerLogin == userLogin).OrderByDescending(e => e.Date)) });
        }

        public IActionResult DeleteAll()
        {
            _expenseRepository.DeleteAllExpenses();

            return RedirectToAction("index", "expenses");
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
    }
}
