using ZaitovaLibrary.DTO;
using ZaitovaLibrary.Data;
using ZaitovaLibrary.Models;

namespace ZaitovaLibrary.Services
{
    public interface IPartnerService
    {
        Task<List<PartnerDto>> GetAllPartnersAsync();
        Task<PartnerCreateUpdateDto?> GetPartnerForEditAsync(int id);
        Task<Partner?> GetPartnerByIdAsync(int id);
        Task<Partner> CreatePartnerAsync(PartnerCreateUpdateDto dto);
        Task UpdatePartnerAsync(PartnerCreateUpdateDto dto);
        Task DeletePartnerAsync(int id);
        Task<List<SalesHistoryDto>> GetPartnerSalesHistoryAsync(int partnerId);
    }

    public class PartnerService : IPartnerService
    {
        private readonly IPartnerRepository _partnerRepository;
        private readonly IProductRepository _productRepository;
        private readonly IPartnerTypeRepository _partnerTypeRepository;
        private readonly IDiscountCalculator _discountCalculator;

        public PartnerService(
            IPartnerRepository partnerRepository,
            IProductRepository productRepository,
            IPartnerTypeRepository partnerTypeRepository,
            IDiscountCalculator discountCalculator)
        {
            _partnerRepository = partnerRepository;
            _productRepository = productRepository;
            _partnerTypeRepository = partnerTypeRepository;
            _discountCalculator = discountCalculator;
        }

        public IPartnerService IPartnerService
        {
            get => default;
            set
            {
            }
        }

        public async Task<List<PartnerDto>> GetAllPartnersAsync()
        {
            var partners = await _partnerRepository.GetAllAsync();
            var partnerDtos = new List<PartnerDto>();

            foreach (var partner in partners)
            {
                var totalSales = await _partnerRepository.GetTotalSalesAmountByPartnerAsync(partner.Id);
                var discount = _discountCalculator.CalculateDiscount(totalSales);

                partnerDtos.Add(new PartnerDto
                {
                    Id = partner.Id,
                    TypeId = partner.TypeId,
                    TypeName = partner.PartnerType?.Name ?? "Не указан",
                    CompanyName = partner.CompanyName,
                    LegalAddress = partner.LegalAddress,
                    Inn = partner.Inn,
                    DirectorFullname = partner.DirectorFullname,
                    Phone = partner.Phone,
                    Email = partner.Email,
                    Rating = partner.Rating,
                    TotalSalesAmount = totalSales,
                    DiscountPercentage = discount
                });
            }

            return partnerDtos;
        }

        public async Task<PartnerCreateUpdateDto?> GetPartnerForEditAsync(int id)
        {
            var partner = await _partnerRepository.GetByIdAsync(id);
            if (partner == null)
                return null;

            return new PartnerCreateUpdateDto
            {
                Id = partner.Id,
                TypeId = partner.TypeId,
                CompanyName = partner.CompanyName,
                LegalAddress = partner.LegalAddress,
                Inn = partner.Inn,
                DirectorFullname = partner.DirectorFullname,
                Phone = partner.Phone,
                Email = partner.Email,
                Rating = partner.Rating
            };
        }

        public async Task<Partner?> GetPartnerByIdAsync(int id)
        {
            return await _partnerRepository.GetByIdAsync(id);
        }

        public async Task<Partner> CreatePartnerAsync(PartnerCreateUpdateDto dto)
        {
            var partner = new Partner
            {
                TypeId = dto.TypeId,
                CompanyName = dto.CompanyName.Trim(),
                LegalAddress = dto.LegalAddress?.Trim(),
                Inn = dto.Inn?.Trim(),
                DirectorFullname = dto.DirectorFullname?.Trim(),
                Phone = dto.Phone?.Trim(),
                Email = dto.Email?.Trim()?.ToLower(),
                Rating = dto.Rating
            };

            return await _partnerRepository.AddAsync(partner);
        }

        public async Task UpdatePartnerAsync(PartnerCreateUpdateDto dto)
        {
            if (!dto.Id.HasValue)
                throw new ArgumentException("ID партнера не указан");

            var existingPartner = await _partnerRepository.GetByIdAsync(dto.Id.Value);
            if (existingPartner == null)
                throw new KeyNotFoundException($"Партнер с ID {dto.Id} не найден");

            existingPartner.TypeId = dto.TypeId;
            existingPartner.CompanyName = dto.CompanyName.Trim();
            existingPartner.LegalAddress = dto.LegalAddress?.Trim();
            existingPartner.Inn = dto.Inn?.Trim();
            existingPartner.DirectorFullname = dto.DirectorFullname?.Trim();
            existingPartner.Phone = dto.Phone?.Trim();
            existingPartner.Email = dto.Email?.Trim()?.ToLower();
            existingPartner.Rating = dto.Rating;

            await _partnerRepository.UpdateAsync(existingPartner);
        }

        public async Task DeletePartnerAsync(int id)
        {
            await _partnerRepository.DeleteAsync(id);
        }

        public async Task<List<SalesHistoryDto>> GetPartnerSalesHistoryAsync(int partnerId)
        {
            var salesHistory = await _partnerRepository.GetPartnerSalesHistoryAsync(partnerId);

            var result = salesHistory.Select(sh => new SalesHistoryDto
            {
                Id = sh.Id,
                ProductName = sh.Product?.Name ?? "Неизвестный продукт",
                ProductArticle = sh.Product?.Article ?? "N/A",
                Quantity = sh.Quantity,
                SaleDate = sh.SaleDate,
                TotalAmount = sh.TotalAmount
            }).ToList();

            return result;
        }
    }
}