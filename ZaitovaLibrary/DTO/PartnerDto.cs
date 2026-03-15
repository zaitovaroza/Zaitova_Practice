namespace ZaitovaLibrary.DTO
{
    public class PartnerDto
    {
        public int Id { get; set; }
        public int TypeId { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string? LegalAddress { get; set; }
        public string? Inn { get; set; }
        public string? DirectorFullname { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int Rating { get; set; }
        public decimal TotalSalesAmount { get; set; }
        public int DiscountPercentage { get; set; }
        public string DiscountDisplay => $"{DiscountPercentage}%";

        // Для подсветки выбранного партнера
        public bool IsSelected { get; set; }

        public Models.Partner Partner
        {
            get => default;
            set
            {
            }
        }
    }
}