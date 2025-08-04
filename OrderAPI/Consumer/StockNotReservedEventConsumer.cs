using MassTransit;
using OrderAPI.Models.Entites;
using OrderAPI.Models;
using Shared.Events;
using Microsoft.EntityFrameworkCore;

namespace OrderAPI.Consumer
{
    public class StockNotReservedEventConsumer : IConsumer<StockNotReservedEvent>
    {
        readonly OrderAPIDbContext _orderAPIDbContext;

        public StockNotReservedEventConsumer(OrderAPIDbContext orderAPIDbContext)
        {
            _orderAPIDbContext = orderAPIDbContext;
        }

        public async Task Consume(ConsumeContext<StockNotReservedEvent> context)
        {
            Order order = await _orderAPIDbContext.Orders.FirstOrDefaultAsync(x => x.OrderId == context.Message.OrderId);
            order.OrderStatu = Models.Enums.OrderStatus.Failed;
            await _orderAPIDbContext.SaveChangesAsync();
        }
    }
}
