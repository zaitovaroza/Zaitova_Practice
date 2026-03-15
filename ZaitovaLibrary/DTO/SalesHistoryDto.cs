namespace ZaitovaLibrary.DTO
{
    public class SalesHistoryDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductArticle { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string FormattedSaleDate => SaleDate.ToString("dd.MM.yyyy");
        public string FormattedTotalAmount => TotalAmount.ToString("N2") + " ₽";
    }
}