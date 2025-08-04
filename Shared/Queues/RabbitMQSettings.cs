using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Queues
{
    static public class RabbitMQSettings
    {
        public const string Stock_OrderCreatedQueue = "Stock.OrderCreatedQueue";
        public const string Payment_StockReservedEventQueue = "Payment.StockReservedQueue";
        public const string Order_PaymentCompletedEventQueue = "Order.PaymentCompletedQueue";
        public const string Order_StockNotReservedEventQueue = "Order.StockNotReservedQueue";
        public const string Order_PaymentFailedEventQueue = "Order.PaymentFailedQueue";
    }
}
