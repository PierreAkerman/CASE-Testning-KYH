using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
        public CurrentCustomerContext CreateCustomer()
        {
            var customer = new CurrentCustomerContext();
            return customer;
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
        public void When_agreement_is_valid_discount_should_be_used_for_lower_price()
        {
            //ARRANGE
            var productList = new List<ProductServiceModel>
            {
                new ProductServiceModel
                {
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
                        ValidFrom = DateTime.Now.Date.AddDays(-5),
                        ValidTo = DateTime.Now.Date.AddDays(5),
                        AgreementRows = new List<AgreementRow>
                        {
                            new AgreementRow
                            {
                                Id = 1,
                                CategoryMatch = "van",
                                PercentageDiscount = 6.0m
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
        [TestMethod]
        public void When_agreement_is_not_valid_discount_should_not_be_used_for_lower_price()
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
                        ValidFrom = DateTime.Now.Date.AddDays(-5),
                        ValidTo = DateTime.Now.Date.AddDays(-2),
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
                    }
                }
            };
            //ACT
            var products = _sut.CalculatePrices(productList, customerContext);
            //ASSERT
            Assert.AreEqual(100, products.First().Price);
        }
        [TestMethod]
        public void When_multiple_agreements_rows_the_lowest_price_should_be_used()
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
                        ValidFrom = DateTime.Now.Date.AddDays(-5),
                        ValidTo = DateTime.Now.Date.AddDays(5),
                        AgreementRows = new List<AgreementRow>
                        {
                            new AgreementRow
                            {
                                Id = 1,
                                CategoryMatch = "van",
                                ManufacturerMatch = "Bugatti",
                                PercentageDiscount = 6.0m
                            },
                            new AgreementRow
                            {
                                Id = 2,
                                ProductMatch = "hybrid",
                                ManufacturerMatch = "Bugatti",
                                PercentageDiscount = 5.0m
                            }
                        }
                    },
                }
            };
            //ACT
            var products = _sut.CalculatePrices(productList, customerContext);
            //ASSERT
            Assert.AreEqual(94, products.First().Price);
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
                        ValidFrom = DateTime.Now.Date.AddDays(-5),
                        ValidTo = DateTime.Now.Date.AddDays(5),
                        AgreementRows = new List<AgreementRow>
                        {
                            new AgreementRow
                            {
                                Id = 1,
                                CategoryMatch = "van",
                                PercentageDiscount = 6.0m
                            },
                            new AgreementRow
                            {
                                Id = 2,
                                ProductMatch = "hybrid",
                                PercentageDiscount = 5.0m
                            }
                        }
                    },
                    new Agreement
                    {
                        Id = 2,
                        ValidFrom = DateTime.Now.Date.AddDays(-5),
                        ValidTo = DateTime.Now.Date.AddDays(5),
                        AgreementRows = new List<AgreementRow>
                        {
                            new AgreementRow
                            {
                                Id = 3,
                                CategoryMatch = "volvo",
                                PercentageDiscount = 10.0m
                            },
                            new AgreementRow
                            {
                                Id = 4,
                                ProductMatch = "hybrid",
                                PercentageDiscount = 4.0m
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

        [TestMethod]
        public void When_agreement_is_not_valid_should_not_be_processed()
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
                        ValidFrom = DateTime.Now.Date.AddDays(-5),
                        ValidTo = DateTime.Now.Date.AddDays(-3),
                        AgreementRows = new List<AgreementRow>
                        {
                            new AgreementRow
                            {
                                Id = 1,
                                CategoryMatch = "van",
                                PercentageDiscount = 6.0m
                            }
                        }
                    },
                    new Agreement
                    {
                        ValidFrom = DateTime.Now.Date.AddDays(-5),
                        ValidTo = DateTime.Now.Date.AddDays(5),
                        AgreementRows = new List<AgreementRow>
                        {
                            new AgreementRow
                            {
                                Id = 2,
                                ProductMatch = "hybrid",
                                PercentageDiscount = 5.0m
                            }
                        }
                    }
                }
            };
            var products = _sut.CalculatePrices(productList, customerContext);
            //ASSERT
            Assert.AreEqual(95, products.First().Price);
        }

        [TestMethod]
        public void When_agreement_is_valid_should_return_true()
        {
            var customer = CreateCustomer();

            customer.Agreements = new List<Agreement>
            {
                new Agreement
                {
                    ValidFrom = DateTime.Now.Date.AddDays(-5),
                    ValidTo = DateTime.Now.Date.AddDays(5),
                }
            };

            var result = _sut.AgreementIsValid(customer.Agreements.First());
            Assert.IsTrue(result);
        }
        [TestMethod]
        public void When_agreement_is_invalid_should_return_false()
        {
            var customer = CreateCustomer();

            customer.Agreements = new List<Agreement>
            {
                new Agreement
                {
                    ValidFrom = DateTime.Now.Date.AddDays(-15),
                    ValidTo = DateTime.Now.Date.AddDays(-5),
                }
            };

            var result = _sut.AgreementIsValid(customer.Agreements.First());
            Assert.IsFalse(result);
        }
    }
}
