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
        private readonly IUnitOfWork _unitOfWork;
        private readonly string userLogin;

        public CategoriesController(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            this.userLogin = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
        }


        public IActionResult Index()
        {
            var categoriesList = _unitOfWork.CategoryRepository.GetAll().ToList().Where(c => c.ExpenseManagerLogin == userLogin);
            
            CategoryListViewModel viewModel = new()
            {
                Category = new Category(),
                Categories = categoriesList,
                NotificationList = Helper.GetNotificationList(_unitOfWork.RecurringRepository)
            };

            return View("index", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            category.ExpenseManagerLogin = userLogin;
            if (ModelState.IsValid)
            {
                await _unitOfWork.CategoryRepository.AddAsync(category);
                await _unitOfWork.CompleteAsync();

                return RedirectToAction("index");
            }
            else
            {
                CategoryListViewModel invalidModel = new() //If category name is incorrect, there has to be new viewModel created, since viewmodel is passed to view
                {
                    Category = category,
                    Categories = _unitOfWork.CategoryRepository.GetAll().ToList(),
                    NotificationList = Helper.GetNotificationList(_unitOfWork.RecurringRepository)
                };
                return View("index", invalidModel);
            }

        }


        public async Task<IActionResult> Delete(int id)
        {
            await _unitOfWork.CategoryRepository.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction("index");
        }

        public JsonResult Sort(string sortOrder)
        {
            var model = _unitOfWork.CategoryRepository.GetAll().ToList().Where(c => c.ExpenseManagerLogin == userLogin);

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

            CategoryListViewModel viewModel = new()
            {
                Category = new Category(),
                Categories = model
            };
            return Json(new { html = Helper.RenderRazorViewToString(this, "_ViewAll", viewModel) }); //Return view as partial with json
        }
    }
}
