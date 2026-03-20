using Microsoft.VisualStudio.TestTools.UnitTesting;
using MasterPol.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MasterPol.Data.Tests
{
    [TestClass]
    public class PartnerRepositoryTests
    {
        [TestMethod]
        public void Partner_CalculateDiscount_NoSales_Returns0()
        {
            var partner = new Partner();
            partner.SaleHistories = new List<SaleHistory>();

            var result = partner.CalculateDiscount();

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void Partner_CalculateDiscount_SalesLess10000_Returns0()
        {
            var partner = new Partner();
            partner.SaleHistories = new List<SaleHistory>
            {
                new SaleHistory
                {
                    Quantity = 5,
                    Product = new Product { MinPartnerPrice = 1000 }
                }
            };

            var result = partner.CalculateDiscount();

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void Partner_CalculateDiscount_Sales10000To50000_Returns5()
        {
            var partner = new Partner();
            partner.SaleHistories = new List<SaleHistory>
            {
                new SaleHistory
                {
                    Quantity = 20,
                    Product = new Product { MinPartnerPrice = 2000 }
                }
            };

            var result = partner.CalculateDiscount();

            Assert.AreEqual(5, result);
        }

        [TestMethod]
        public void Partner_CalculateDiscount_Sales50000To300000_Returns10()
        {
            var partner = new Partner();
            partner.SaleHistories = new List<SaleHistory>
            {
                new SaleHistory
                {
                    Quantity = 100,
                    Product = new Product { MinPartnerPrice = 2000 }
                }
            };

            var result = partner.CalculateDiscount();

            Assert.AreEqual(10, result);
        }

        [TestMethod]
        public void Partner_CalculateDiscount_SalesMore300000_Returns15()
        {
            var partner = new Partner();
            partner.SaleHistories = new List<SaleHistory>
            {
                new SaleHistory
                {
                    Quantity = 200,
                    Product = new Product { MinPartnerPrice = 2000 }
                }
            };

            var result = partner.CalculateDiscount();

            Assert.AreEqual(15, result);
        }

        [TestMethod]
        public void Partner_CalculateDiscount_MultipleSales_SumsCorrectly()
        {
            var partner = new Partner();
            partner.SaleHistories = new List<SaleHistory>
            {
                new SaleHistory { Quantity = 10, Product = new Product { MinPartnerPrice = 1000 } },
                new SaleHistory { Quantity = 20, Product = new Product { MinPartnerPrice = 2000 } }
            };

            var result = partner.CalculateDiscount();

            Assert.AreEqual(10, result); // 10*1000 + 20*2000 = 50000
        }

        [TestMethod]
        public void Partner_Constructor_InitializesCollections()
        {
            var partner = new Partner();

            Assert.IsNotNull(partner.SaleHistories);
            Assert.AreEqual(0, partner.SaleHistories.Count);
        }

        [TestMethod]
        public void Product_Constructor_InitializesCollections()
        {
            var product = new Product();

            Assert.IsNotNull(product.SaleHistories);
            Assert.AreEqual(0, product.SaleHistories.Count);
        }

        [TestMethod]
        public void PartnerType_Constructor_InitializesCollections()
        {
            var partnerType = new PartnerType();

            Assert.IsNotNull(partnerType.Partners);
            Assert.AreEqual(0, partnerType.Partners.Count);
        }

        [TestMethod]
        public void SaleHistory_Constructor_SetsDates()
        {
            var sale = new SaleHistory();

            Assert.IsTrue(sale.CreatedAt > DateTime.MinValue);
            Assert.IsTrue(sale.SaleDate > DateTime.MinValue);
        }

        [TestMethod]
        public void Partner_Properties_SetAndGetCorrectly()
        {
            var partner = new Partner
            {
                Id = 1,
                Name = "Тест",
                Rating = 50,
                Phone = "123456",
                Email = "test@test.ru"
            };

            Assert.AreEqual(1, partner.Id);
            Assert.AreEqual("Тест", partner.Name);
            Assert.AreEqual(50, partner.Rating);
            Assert.AreEqual("123456", partner.Phone);
            Assert.AreEqual("test@test.ru", partner.Email);
        }
    }
}