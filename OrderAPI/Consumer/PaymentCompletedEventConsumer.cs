using MassTransit;
using OrderAPI.Models;
using OrderAPI.Models.Entites;
using Shared.Events;
using Shared.Events.Cammon;
using System.Data.Entity;

namespace OrderAPI.Consumer
{
    public class PaymentCompletedEventConsumer : IConsumer<PaymentCompletedEvent>
    {
        readonly OrderAPIDbContext _orderAPIDbContext;

        public PaymentCompletedEventConsumer(OrderAPIDbContext orderAPIDbContext)
        {
            _orderAPIDbContext = orderAPIDbContext;
        }

        public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
        {
            Order order = await _orderAPIDbContext.Orders.FirstOrDefaultAsync(x => x.OrderId == context.Message.OrderId);
            order.OrderStatu = Models.Enums.OrderStatus.Completed;
            await _orderAPIDbContext.SaveChangesAsync();
        }
    }
}
