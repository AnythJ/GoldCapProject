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
            //modelBuilder.Entity<Expense>().HasData(
            //    new Expense
            //    {
            //        Id = 1,
            //        Amount = 27,
            //        Category = Ctg.Daily,
            //        Description = "First decscription",
            //        Date = DateTime.Now
            //    },
            //    new Expense
            //    {
            //        Id = 2,
            //        Amount = 14,
            //        Category = Ctg.Material,
            //        Description = "Second decscription",
            //        Date = DateTime.Now
            //    },
            //    new Expense
            //    {
            //        Id = 3,
            //        Amount = 35,
            //        Category = Ctg.Cleaning,
            //        Description = "Third decscription",
            //        Date = DateTime.Now
            //    },
            //    new Expense
            //    {
            //        Id = 4,
            //        Amount = 8005,
            //        Category = Ctg.Electronics,
            //        Description = "Fourth decscription",
            //        Date = DateTime.Now
            //    },
            //    new Expense
            //    {
            //        Id = 5,
            //        Amount = 425,
            //        Category = Ctg.Cloths,
            //        Description = "Fifth decscription",
            //        Date = DateTime.Now
            //    },
            //    new Expense
            //    {
            //        Id = 6,
            //        Amount = 255,
            //        Category = Ctg.Food,
            //        Description = "Sixth decscription",
            //        Date = DateTime.Now
            //    },
            //    new Expense
            //    {
            //        Id = 7,
            //        Amount = 27,
            //        Category = Ctg.Daily,
            //        Description = "First decscription",
            //        Date = DateTime.Now
            //    },
            //    new Expense
            //    {
            //        Id = 8,
            //        Amount = 14,
            //        Category = Ctg.Material,
            //        Description = "Second decscription",
            //        Date = DateTime.Now
            //    },
            //    new Expense
            //    {
            //        Id = 9,
            //        Amount = 35,
            //        Category = Ctg.Cleaning,
            //        Description = "Third decscription",
            //        Date = DateTime.Now
            //    },
            //    new Expense
            //    {
            //        Id = 10,
            //        Amount = 8005,
            //        Category = Ctg.Electronics,
            //        Description = "Very long description to check if everything is ok-Very long description to check if everything is ok-Ending of the sentence somewhere here should brk",
            //        Date = DateTime.Now
            //    },
            //    new Expense
            //    {
            //        Id = 11,
            //        Amount = 425,
            //        Category = Ctg.Cloths,
            //        Description = "Fifth decscription",
            //        Date = DateTime.Now
            //    },
            //    new Expense
            //    {
            //        Id = 12,
            //        Amount = 255,
            //        Category = Ctg.Food,
            //        Description = "Sixth decscription",
            //        Date = DateTime.Now
            //    },
            //    new Expense
            //    {
            //        Id = 13,
            //        Amount = 27,
            //        Category = Ctg.Daily,
            //        Description = "First decscription",
            //        Date = DateTime.Now
            //    },
            //    new Expense
            //    {
            //        Id = 14,
            //        Amount = 14,
            //        Category = Ctg.Material,
            //        Description = "Second decscription",
            //        Date = DateTime.Now
            //    },
            //    new Expense
            //    {
            //        Id = 15,
            //        Amount = 35,
            //        Category = Ctg.Cleaning,
            //        Description = "Third decscription",
            //        Date = DateTime.Now
            //    },
            //    new Expense
            //    {
            //        Id = 16,
            //        Amount = 8005,
            //        Category = Ctg.Electronics,
            //        Description = "Fourth decscription",
            //        Date = DateTime.Now
            //    },
            //    new Expense
            //    {
            //        Id = 17,
            //        Amount = 425,
            //        Category = Ctg.Cloths,
            //        Description = "Fifth decscription",
            //        Date = DateTime.Now
            //    },
            //    new Expense
            //    {
            //        Id = 18,
            //        Amount = 255,
            //        Category = Ctg.Food,
            //        Description = "Sixth decscription",
            //        Date = DateTime.Now
            //    }
            //    );
        }
    }
}