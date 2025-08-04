namespace OrderAPI.DTO
{
    public class OrderDto
    {
        public Guid OrderId { get; set; }
        public Guid BuyerId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Models.Enums.OrderStatus OrderStatu { get; set; }
        public decimal TotalPrice { get; set; }
        public List<OrderItemDto> OrderItems { get; set; }
    }
}
