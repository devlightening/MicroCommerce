using OrderAPI.Models.Entites;
using OrderAPI.Models.Enums;

namespace OrderAPI.ViewModels
{
    public class CreateOrderViewModel
    {
        public Guid BuyerId { get; set; }
        public List<CreateOrderItemViewModel> OrderItems { get; set; }


    }
}
