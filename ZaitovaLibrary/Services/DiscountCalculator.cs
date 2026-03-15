namespace ZaitovaLibrary.Services
{
    public interface IDiscountCalculator
    {
        int CalculateDiscount(decimal totalSalesAmount);
    }

    public class DiscountCalculator : IDiscountCalculator
    {
        public IDiscountCalculator IDiscountCalculator
        {
            get => default;
            set
            {
            }
        }

        public int CalculateDiscount(decimal totalSalesAmount)
        {
            if (totalSalesAmount < 10000)
                return 0;
            else if (totalSalesAmount < 50000)
                return 5;
            else if (totalSalesAmount < 300000)
                return 10;
            else
                return 15;
        }
    }
}