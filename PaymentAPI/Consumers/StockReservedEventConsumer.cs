using MassTransit;
using Shared.Events;
using System.Threading.Tasks;

namespace PaymentAPI.Consumers
{
    public class StockReservedEventConsumer : IConsumer<StockReservedEvent>
    {
        readonly IPublishEndpoint _publishEndpoint;

        public StockReservedEventConsumer(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<StockReservedEvent> context)
        {
            // Ödeme işlemleri
            // kayıtlı kredi kartları, TotalPrice, Kredi kartı bilgileri..

            if (true) // Ödemenin başarılı olduğu senaryo
            {
                // Ödemenin Başarıyla Tamamlanıldığı..
                PaymentCompletedEvent paymentCompletedEvent = new()
                {
                    OrderId = context.Message.OrderId
                };

                // Önemli: Mesajı yayımlama işlemi burada yapılmalı.
                await _publishEndpoint.Publish(paymentCompletedEvent);

                Console.WriteLine("Ödeme Başarılı Bir Şekilde Tamamlandı.");
            }
            else // Ödemede sıkıntı çıktığı durum
            {
                PaymentFailedEvent paymentFailedEvent = new()
                {
                    OrderId = context.Message.OrderId,
                    Reason = "Ödeme işlemi başarısız oldu."
                };

                // Önemli: Bu mesaj zaten yayımlanıyordu.
                await _publishEndpoint.Publish(paymentFailedEvent);
            }

            // Consume metodu async olduğu için, Task.CompletedTask yerine return Task; kullanmak daha doğru olur.
            // Bu zaten async/await ile halledildiği için ekstra bir dönüşe gerek kalmıyor, ancak best practice olarak dönüşü düzeltelim.
        }
    }
}