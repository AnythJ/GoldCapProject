using GoldCap.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoldCap.ViewModels
{
    public class CreateOrEditViewModel
    {
        public Expense Expense { get; set; }
        public List<Category> Categories { get; set; }
    }
}
