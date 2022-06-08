using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcSuperShop.Data;
using MvcSuperShop.Infrastructure.Context;
using MvcSuperShop.Services;

namespace SuperShopTestProject.Services
{
    [TestClass]
    public class PricingServiceTests
    {
        private PricingService _sut;

        [TestInitialize]
        public void Initialize()
        {
            _sut = new PricingService();
        }

        [TestMethod]
        public void When_no_agreement_exists_product_basePrice_is_used()
        {
            //ARRANGE
            var productList = new List<ProductServiceModel>
            {
                new ProductServiceModel{BasePrice = 23500}
            };
            var customerContext = new CurrentCustomerContext
            {
                Agreements = new List<Agreement>()
            };
            //ACT
            var products = _sut.CalculatePrices(productList, customerContext);
            //ASSERT
            Assert.AreEqual(23500, products.First().Price);
        }

        [TestMethod]
        public void When_multiple_agreements_exists_the_lowest_price_should_be_used()
        {
            //ARRANGE
            var productList = new List<ProductServiceModel>
            {
                new ProductServiceModel{
                    BasePrice = 100,
                    Name = "XTS Hybrid",
                    CategoryName = "van",
                    ManufacturerName = "Bugatti",
                }
            };
            var customerContext = new CurrentCustomerContext
            {
                Agreements = new List<Agreement>
                {
                    new Agreement
                    {
                        Id = 1,
                        AgreementRows = new List<AgreementRow>
                        {
                            new AgreementRow
                            {
                                Id = 1,
                                CategoryMatch = "van",
                                ManufacturerMatch = "Bugatti",
                                PercentageDiscount = 6.0m
                            }
                        }
                    },
                    new Agreement
                    {
                        Id = 1,
                        AgreementRows = new List<AgreementRow>
                        {
                            new AgreementRow
                            {
                                Id = 2,
                                ProductMatch = "hybrid",
                                ManufacturerMatch = "Bugatti",
                                PercentageDiscount = 5.0m
                            }
                        }
                    }
                }
            };
            //ACT
            var products = _sut.CalculatePrices(productList, customerContext);
            //ASSERT
            Assert.AreEqual(94, products.First().Price);
        }
    }
}
