using GoldCap.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoldCap.Controllers
{
    public class AboutController : Controller
    {
        private IExpenseRepository _expenseRepository;

        public AboutController(IExpenseRepository expenseRepository)
        {
            _expenseRepository = expenseRepository;
        }
        public IActionResult Index()
        {
            var model = _expenseRepository.GetAllExpenses();
            return View(model);
        }
    }
}
