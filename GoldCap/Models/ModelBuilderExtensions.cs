using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoldCap.Models
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    Id = 1,
                    Name = "None"
                },
                new Category
                {
                    Id = 2,
                    Name = "Daily"
                },
                new Category
                {
                    Id = 3,
                    Name = "Food"
                },
                new Category
                {
                    Id = 4,
                    Name = "Electronics"
                },
                new Category
                {
                    Id = 5,
                    Name = "Drinks"
                },
                new Category
                {
                    Id = 6,
                    Name = "Cloths"
                },
                new Category
                {
                    Id = 7,
                    Name = "Insurance"
                },
                new Category
                {
                    Id = 8,
                    Name = "Cloths"
                });
            
        }
    }
}