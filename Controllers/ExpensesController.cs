using GoldCap.Models;
using GoldCap.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static GoldCap.Helper;

namespace GoldCap.Controllers
{
    public class ExpensesController : Controller
    {
        private IExpenseRepository _expenseRepository;

        public ExpensesController(IExpenseRepository expenseRepository)
        {
            _expenseRepository = expenseRepository;
        }

        public IActionResult Index()
        {
            var expenses = _expenseRepository.GetAllExpenses();

            return View(expenses);
        }

        // GET: Expenses/CreateOrEdit
        // GET: Expenses/CreateOrEdit/id
        [HttpGet]
        [NoDirectAccess]
        public IActionResult CreateOrEdit(int id = 0)
        {
            List<SelectList> ConvertedList = new List<SelectList>();
            ViewBag.CategoryList = _expenseRepository.GetAllCategories();

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
        public IActionResult CreateOrEdit(int id, [Bind("Id,Amount,Category,Description,Date")] Expense expense)
        {
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


                return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAll", _expenseRepository.GetAllExpenses()) });
            }
            return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, "CreateOrEdit", expense) });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _expenseRepository.Delete(id);

            return Json(new { html = Helper.RenderRazorViewToString(this, "_ViewAll", _expenseRepository.GetAllExpenses()) });
        }
    }
}
