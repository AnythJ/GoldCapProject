using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GoldCap.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Key][Column(Order = 1)]
        public int ExpenseManagerId { get; set; }

        public byte[] ProfilePicture { get; set; }
    }
}
