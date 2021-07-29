﻿using GoldCap.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoldCap.Controllers
{
    public class DashboardController : Controller
    {
        private IExpenseRepository _expenseRepository;

        public DashboardController(IExpenseRepository expenseRepository)
        {
            _expenseRepository = expenseRepository;
        }
        public IActionResult Index()
        {
            ViewBag.Expenses = _expenseRepository.GetAllExpenses().Where(m => m.Date >= DateTime.Now.AddDays(-30)).OrderByDescending(d => d.Date);
            var model = _expenseRepository.GetAllExpenses().Where(m => m.Date >= DateTime.Now.AddDays(-30));

            return View(model);
        }

        
        
        public JsonResult GetData()
        {
            ViewBag.Expenses = _expenseRepository.GetAllExpenses().Where(m => m.Date >= DateTime.Now.AddDays(-30));
            //ViewBag.Categories = _expenseRepository.GetCategoryRatios().Where(c => c.CategoryPercentage >= _expenseRepository.GetCategoryRatios()) // HERE 29.07
            List<string> newList = new List<string>();
            var xdList = _expenseRepository.GetCategoryRatios();
            foreach(var item in xdList)
            {
                newList.Add(item.CategoryPercentage.ToString());
            }


            DashboardDataModel data = new DashboardDataModel()
            {
                ListLast30 = _expenseRepository.GetSumDayExpense30(),
                CategoryRatios = _expenseRepository.GetCategoryRatios(),
                CategoryCount = _expenseRepository.GetAllCategories().Count(),
                TestArray = newList.ToArray() // Test
            };
            
            return Json(data);
        }
    }
}
