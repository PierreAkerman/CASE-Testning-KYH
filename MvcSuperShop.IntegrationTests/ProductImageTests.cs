using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcSuperShop.Data;

namespace MvcSuperShop.IntegrationTests
{
    [TestClass]
    public class ProductImageTests
    {
        private ApplicationDbContext _context;

        [TestInitialize]
        public void Initializer()
        {
            var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=localhost;Database=TestingSuperShop;Trusted_Connection=True;MultipleActiveResultSets=true")
                .Options;

            _context = new ApplicationDbContext(contextOptions);
        }

        [TestMethod]
        public void When_product_image_url_is_found_should_return_true()
        {
            //ARRANGE
            var correctUrl = false;
            var products = _context.Products;
            //ACT
            foreach (var product in products)
            {
                if (product.ImageUrl != null && product.ImageUrl.EndsWith(".png"))
                {
                    correctUrl = true;
                }
                else
                {
                    correctUrl = false;
                }
            }
            //ASSERT
            Assert.IsTrue(correctUrl);
        }
        [TestMethod]
        public void When_product_image_is_found_should_return_OK()
        {
            //ARRANGE
            var products = _context.Products;

            foreach(var product in products)
            {
                HttpClient request = new HttpClient();
                var response = request.GetAsync(product.ImageUrl).Result;
                Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
            }
        }
    }
}
