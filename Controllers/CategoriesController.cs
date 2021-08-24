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
            this.userLogin = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name) == "guestTest@gm.com" ? null : httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
        }
        public IActionResult Index()
        {
            var categoriesList = _expenseRepository.GetAllCategories().Where(c => c.ExpenseManagerLogin == userLogin);
            CategoryListViewModel viewModel = new CategoryListViewModel()
            {
                Category = new Category(),
                Categories = categoriesList
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
                CategoryListViewModel invalidModel = new CategoryListViewModel()
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
    }
}
