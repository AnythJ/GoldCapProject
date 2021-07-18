using GoldCap.Models;
using GoldCap.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoldCap.Controllers
{
    public class CategoriesController : Controller
    {
        private IExpenseRepository _expenseRepository;

        public CategoriesController(IExpenseRepository expenseRepository)
        {
            _expenseRepository = expenseRepository;
        }
        public IActionResult Index()
        {
            var categoriesList = _expenseRepository.GetAllCategories();
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
