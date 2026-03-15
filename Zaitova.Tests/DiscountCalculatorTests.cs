using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZaitovaLibrary.Services;

namespace Zaitova.Tests
{
    [TestClass]
    public class DiscountCalculatorTests
    {
        private DiscountCalculator _calculator;

        [TestInitialize]
        public void Setup()
        {
            _calculator = new DiscountCalculator();
        }

        [TestMethod]
        public void CalculateDiscount_TotalLessThan10000_Returns0()
        {
            // Arrange
            decimal totalSales = 5000;

            // Act
            int discount = _calculator.CalculateDiscount(totalSales);

            // Assert
            Assert.AreEqual(0, discount);
        }

        [TestMethod]
        public void CalculateDiscount_TotalEqualTo10000_Returns5()
        {
            // Arrange
            decimal totalSales = 10000;

            // Act
            int discount = _calculator.CalculateDiscount(totalSales);

            // Assert
            Assert.AreEqual(5, discount);
        }

        [TestMethod]
        public void CalculateDiscount_TotalBetween10000And50000_Returns5()
        {
            // Arrange
            decimal totalSales = 25000;

            // Act
            int discount = _calculator.CalculateDiscount(totalSales);

            // Assert
            Assert.AreEqual(5, discount);
        }

        [TestMethod]
        public void CalculateDiscount_TotalEqualTo50000_Returns10()
        {
            // Arrange
            decimal totalSales = 50000;

            // Act
            int discount = _calculator.CalculateDiscount(totalSales);

            // Assert
            Assert.AreEqual(10, discount);
        }

        [TestMethod]
        public void CalculateDiscount_TotalBetween50000And300000_Returns10()
        {
            // Arrange
            decimal totalSales = 150000;

            // Act
            int discount = _calculator.CalculateDiscount(totalSales);

            // Assert
            Assert.AreEqual(10, discount);
        }

        [TestMethod]
        public void CalculateDiscount_TotalEqualTo300000_Returns15()
        {
            // Arrange
            decimal totalSales = 300000;

            // Act
            int discount = _calculator.CalculateDiscount(totalSales);

            // Assert
            Assert.AreEqual(15, discount);
        }

        [TestMethod]
        public void CalculateDiscount_TotalMoreThan300000_Returns15()
        {
            // Arrange
            decimal totalSales = 500000;

            // Act
            int discount = _calculator.CalculateDiscount(totalSales);

            // Assert
            Assert.AreEqual(15, discount);
        }

        [TestMethod]
        public void CalculateDiscount_TotalZero_Returns0()
        {
            // Arrange
            decimal totalSales = 0;

            // Act
            int discount = _calculator.CalculateDiscount(totalSales);

            // Assert
            Assert.AreEqual(0, discount);
        }

        [TestMethod]
        public void CalculateDiscount_TotalNegative_Returns0()
        {
            // Arrange
            decimal totalSales = -1000;

            // Act
            int discount = _calculator.CalculateDiscount(totalSales);

            // Assert
            Assert.AreEqual(0, discount);
        }
    }
}