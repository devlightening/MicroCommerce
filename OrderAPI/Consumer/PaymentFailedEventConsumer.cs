using MassTransit;
using OrderAPI.Models.Entites;
using OrderAPI.Models;
using Shared.Events;
using Microsoft.EntityFrameworkCore;

namespace OrderAPI.Consumer
{
    public class PaymentFailedEventConsumer : IConsumer<PaymentFailedEvent>
    {
        readonly OrderAPIDbContext _orderAPIDbContext;

        public PaymentFailedEventConsumer(OrderAPIDbContext orderAPIDbContext)
        {
            _orderAPIDbContext = orderAPIDbContext;
        }

        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            Order order = await _orderAPIDbContext.Orders.FirstOrDefaultAsync(x => x.OrderId == context.Message.OrderId);
            order.OrderStatu = Models.Enums.OrderStatus.Failed;
            await _orderAPIDbContext.SaveChangesAsync();
        }
    }
}
