using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ZaitovaLibrary.Data;
using ZaitovaLibrary.DTO;
using ZaitovaLibrary.Models;
using ZaitovaLibrary.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Zaitova.Tests
{
    [TestClass]
    public class PartnerServiceTests
    {
        private Mock<IPartnerRepository> _mockPartnerRepo;
        private Mock<IProductRepository> _mockProductRepo;
        private Mock<IPartnerTypeRepository> _mockPartnerTypeRepo;
        private Mock<IDiscountCalculator> _mockDiscountCalc;
        private PartnerService _service;

        [TestInitialize]
        public void Setup()
        {
            _mockPartnerRepo = new Mock<IPartnerRepository>();
            _mockProductRepo = new Mock<IProductRepository>();
            _mockPartnerTypeRepo = new Mock<IPartnerTypeRepository>();
            _mockDiscountCalc = new Mock<IDiscountCalculator>();

            _service = new PartnerService(
                _mockPartnerRepo.Object,
                _mockProductRepo.Object,
                _mockPartnerTypeRepo.Object,
                _mockDiscountCalc.Object
            );
        }

        [TestMethod]
        public async Task GetPartnerForEditAsync_ExistingId_ReturnsDto()
        {
            // Arrange
            int partnerId = 1;
            var partner = new Partner
            {
                Id = partnerId,
                TypeId = 1,
                CompanyName = "ООО Тест",
                Rating = 50
            };

            _mockPartnerRepo.Setup(r => r.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            // Act
            var result = await _service.GetPartnerForEditAsync(partnerId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(partnerId, result.Id);
            Assert.AreEqual("ООО Тест", result.CompanyName);
            Assert.AreEqual(50, result.Rating);
        }

        [TestMethod]
        public async Task GetPartnerForEditAsync_NonExistingId_ReturnsNull()
        {
            // Arrange
            int partnerId = 999;
            _mockPartnerRepo.Setup(r => r.GetByIdAsync(partnerId))
                .ReturnsAsync((Partner?)null);

            // Act
            var result = await _service.GetPartnerForEditAsync(partnerId);

            // Assert
            Assert.IsNull(result);
        }
    }
}