using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcSuperShop.Data;
using MvcSuperShop.Services;


namespace SuperShopTestProject.Services
{
    [TestClass]
    public class CategoryServiceTests
    {
        private CategoryService sut;
        private ApplicationDbContext context;

        [TestInitialize]
        public void Initialize()
        {
            var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            context = new ApplicationDbContext(contextOptions);
            context.Database.EnsureCreated();

            sut = new CategoryService(context);
        }

        [TestMethod]
        public void When_get_trending_categories_should_return_correct_number_of_categories()
        {
            //ARRANGE
            context.Categories.Add(new Category
            {
                Name = "Van",
                Icon = "lalala"
            });
            context.Categories.Add(new Category
            {
                Name = "Hybrid",
                Icon = "lalala"
            });
            context.Categories.Add(new Category
            {
                Name = "Suv",
                Icon = "lalala"
            });
            context.Categories.Add(new Category
            {
                Name = "Wagon",
                Icon = "lalala"
            });
            context.Categories.Add(new Category
            {
                Name = "Coupe",
                Icon = "lalala"
            });
            context.SaveChanges();
            //ACT
            var result = sut.GetTrendingCategories(3);
            //ASSERT
            Assert.AreEqual(3, result.Count());
        }
        [TestMethod]
        public void When_get_to_many_trending_categories_should_return_all_existing_categories()
        {
            //ARRANGE
            context.Categories.Add(new Category
            {
                Name = "Van",
                Icon = "lalala"
            });
            context.Categories.Add(new Category
            {
                Name = "Hybrid",
                Icon = "lalala"
            });
            context.Categories.Add(new Category
            {
                Name = "Suv",
                Icon = "lalala"
            });
            context.Categories.Add(new Category
            {
                Name = "Wagon",
                Icon = "lalala"
            });
            context.Categories.Add(new Category
            {
                Name = "Coupe",
                Icon = "lalala"
            });

            context.SaveChanges();
            //ACT
            var result = sut.GetTrendingCategories(6);
            //ASSERT
            Assert.AreEqual(5, result.Count());
        }
    }
}
