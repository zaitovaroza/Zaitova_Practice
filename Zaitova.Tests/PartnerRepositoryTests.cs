using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using ZaitovaLibrary.Data;
using ZaitovaLibrary.Models;
using System.Threading.Tasks;
using System.Linq;

namespace Zaitova.Tests
{
    [TestClass]
    public class PartnerRepositoryTests
    {
        private AppDbContext _context;
        private PartnerRepository _repository;

        [TestInitialize]
        public void Setup()
        {
            // Создаем уникальное имя базы для каждого теста
            var databaseName = $"ZaitovaTestDb_{System.Guid.NewGuid()}";

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;

            _context = new AppDbContext(options);
            _repository = new PartnerRepository(_context);

            // Добавляем тестовые данные
            SeedData();
        }

        private void SeedData()
        {
            // Добавляем тип партнера (нужен для внешнего ключа)
            if (!_context.PartnerTypes.Any())
            {
                _context.PartnerTypes.Add(new PartnerType
                {
                    Id = 1,
                    Name = "ООО"
                });
                _context.SaveChanges();
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public async Task AddAsync_ValidPartner_ReturnsPartnerWithId()
        {
            // Arrange
            var partner = new Partner
            {
                TypeId = 1,
                CompanyName = "ООО Тест",
                DirectorFullname = "Иванов Иван Иванович",
                Phone = "+7(123)456-78-90",
                Email = "test@test.ru",
                Rating = 50,
                CreatedAt = System.DateTime.Now,
                UpdatedAt = System.DateTime.Now
            };

            // Act
            var result = await _repository.AddAsync(partner);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Id > 0);
            Assert.AreEqual("ООО Тест", result.CompanyName);
        }

        [TestMethod]
        public async Task GetByIdAsync_ExistingId_ReturnsPartner()
        {
            // Arrange
            var partner = new Partner
            {
                TypeId = 1,
                CompanyName = "ООО Тест",
                DirectorFullname = "Иванов Иван Иванович",
                Phone = "+7(123)456-78-90",
                Email = "test@test.ru",
                Rating = 50,
                CreatedAt = System.DateTime.Now,
                UpdatedAt = System.DateTime.Now
            };

            var added = await _repository.AddAsync(partner);

            // Act
            var result = await _repository.GetByIdAsync(added.Id);

            // Assert
            Assert.IsNotNull(result, "Партнер не найден по Id");
            Assert.AreEqual(added.Id, result.Id);
            Assert.AreEqual("ООО Тест", result.CompanyName);
        }

        [TestMethod]
        public async Task GetByIdAsync_NonExistingId_ReturnsNull()
        {
            // Arrange
            int nonExistingId = 999;

            // Act
            var result = await _repository.GetByIdAsync(nonExistingId);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetAllAsync_ReturnsAllPartners()
        {
            // Arrange
            await _repository.AddAsync(new Partner
            {
                TypeId = 1,
                CompanyName = "ООО Тест 1",
                Rating = 50,
                CreatedAt = System.DateTime.Now,
                UpdatedAt = System.DateTime.Now
            });

            await _repository.AddAsync(new Partner
            {
                TypeId = 1,
                CompanyName = "ООО Тест 2",
                Rating = 60,
                CreatedAt = System.DateTime.Now,
                UpdatedAt = System.DateTime.Now
            });

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public async Task UpdateAsync_ExistingPartner_UpdatesProperties()
        {
            // Arrange
            var partner = new Partner
            {
                TypeId = 1,
                CompanyName = "Старое название",
                Rating = 50,
                CreatedAt = System.DateTime.Now,
                UpdatedAt = System.DateTime.Now
            };

            var added = await _repository.AddAsync(partner);

            // Act
            added.CompanyName = "Новое название";
            added.Rating = 75;
            await _repository.UpdateAsync(added);

            // Assert
            var updated = await _repository.GetByIdAsync(added.Id);
            Assert.IsNotNull(updated);
            Assert.AreEqual("Новое название", updated.CompanyName);
            Assert.AreEqual(75, updated.Rating);
        }

        [TestMethod]
        public async Task DeleteAsync_ExistingId_RemovesPartner()
        {
            // Arrange
            var partner = new Partner
            {
                TypeId = 1,
                CompanyName = "ООО Для удаления",
                Rating = 50,
                CreatedAt = System.DateTime.Now,
                UpdatedAt = System.DateTime.Now
            };

            var added = await _repository.AddAsync(partner);

            // Act
            await _repository.DeleteAsync(added.Id);
            var result = await _repository.GetByIdAsync(added.Id);

            // Assert
            Assert.IsNull(result);
        }
    }
}