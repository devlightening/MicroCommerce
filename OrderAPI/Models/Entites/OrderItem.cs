namespace OrderAPI.Models.Entites
{
    public class OrderItem
    {
        public Guid OrderItemId { get; set; }
        public String ProductId { get; set; }
        public Guid OrderId { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
        public Order  Order { get; set; }
    }
}
