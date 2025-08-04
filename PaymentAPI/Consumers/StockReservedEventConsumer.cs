using MassTransit;
using Shared.Events;

namespace PaymentAPI.Consumers
{
    public class StockReservedEventConsumer : IConsumer<StockReservedEvent>
    {
        readonly IPublishEndpoint _publishEndpoint;

        public StockReservedEventConsumer(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public Task Consume(ConsumeContext<StockReservedEvent> context)
        {

            //Ödeme işlemleri 
            //kayıtlı kredi kartları ,TotalPrice ,Kredi kartı bilgileri..
            if (true)
            {
                //Ödemenin Başarıyla Tamamlanıldığı..
                PaymentCompletedEvent paymentCompletedEvent = new()
                {
                    OrderId = context.Message.OrderId
                };

                 Console.WriteLine("Ödeme Başarılı Bir Şekilde Tamamlandı.");

            }
            else
            {
                //Ödemede sıkıntı çıktığı durumda...
                PaymentFailedEvent paymentFailedEvent = new()
                {
                    OrderId = context.Message.OrderId,
                    Reason = "Ödeme işlemi başarısız oldu."
                };
                _publishEndpoint.Publish(paymentFailedEvent);

            }




            return Task.CompletedTask;
        }
    }
}
