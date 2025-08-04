using OrderAPI.Models.Enums;

namespace OrderAPI.Models.Entites
{
    public class Order
    {
        public Guid OrderId { get; set; }
        public Guid BuyerId { get; set; }
        public decimal TotalPrice { get; set; }
        public OrderStatus OrderStatu { get; set; }
        public DateTime CreatedDate { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }



    }
}
