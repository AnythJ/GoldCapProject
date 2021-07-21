﻿using GoldCap.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoldCap.Controllers
{
    public class HomeController : Controller
    {
        private IExpenseRepository _expenseRepository;

        public HomeController(IExpenseRepository expenseRepository)
        {
            _expenseRepository = expenseRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}