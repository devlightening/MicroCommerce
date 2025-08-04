using MassTransit;
using MongoDB.Driver;
using Shared.Events;
using Shared.Messages;
using Shared.Queues;
using StockAPI.Models.Entites;
using StockAPI.Services;

namespace StockAPI.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {

        IMongoCollection<Stock> _stockCollection;
        readonly ISendEndpointProvider _sendEndpointProvider;

        public OrderCreatedEventConsumer(MongoDBService mongoDBService, ISendEndpointProvider sendEndpointProvider = null)
        {
            _stockCollection = mongoDBService.GetCollection<Stock>();
            _sendEndpointProvider = sendEndpointProvider;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            List<bool> stockResult = new();
            foreach (OrderItemMessage orderItem in context.Message.OrderItems)
            {
                stockResult.Add((await _stockCollection.FindAsync(s => s.ProductId == orderItem.ProductId && s.Count >= orderItem.Count)).Any());
            }
            if (stockResult.TrueForAll(sr => sr.Equals(true)))
            {
                foreach (OrderItemMessage orderItem in context.Message.OrderItems)
                {
                  Stock  stock =await (await _stockCollection.FindAsync(s => s.ProductId == orderItem.ProductId))
                        .FirstOrDefaultAsync();

                    stock.Count -= orderItem.Count;
                    await _stockCollection.FindOneAndReplaceAsync(s => s.ProductId == orderItem.ProductId, stock);

                }

                //Payment eventleri..
                StockReservedEvent stockReservedEvent = new()
                {
                    OrderId = context.Message.OrderId,
                    BuyerId = context.Message.BuyerId,
                    TotalPrrice =context.Message.TotalPrice
                };

                ISendEndpoint sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettings.Payment_StockReservedEventQueue}"));
                await sendEndpoint.Send(stockReservedEvent);
            }
            else
            {
                

                //Siparişin tutarsız /geçersiz olduğuna dair işlemler..

            }
        }
    }
}
