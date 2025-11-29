namespace LegacyOrderService.Models
{
    public class Order
    {
        public Guid Id;
        public string CustomerName;
        public string ProductName;
        public int Quantity;
        public double Price;
        public int IsProceed;
    }
}
