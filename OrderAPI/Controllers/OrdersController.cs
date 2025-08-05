using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderAPI.DTO;
using OrderAPI.Models;
using OrderAPI.Models.Entites;
using OrderAPI.ViewModels;
using Shared.Events;
using Shared.Messages;

namespace OrderAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderAPIDbContext _context;
        private readonly IPublishEndpoint _publishEndPoint;

        public OrdersController(OrderAPIDbContext context, IPublishEndpoint publishEndPoint)
        {
            _context = context;
            _publishEndPoint = publishEndPoint;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderViewModel createOrderViewModel)
        {
            Order order = new Order
            {
                OrderId = Guid.NewGuid(),
                BuyerId = createOrderViewModel.BuyerId,
                CreatedDate = DateTime.Now,
                OrderStatu = Models.Enums.OrderStatus.Suspend
            };

            order.OrderItems = createOrderViewModel.OrderItems.Select(oi => new OrderItem
            {
                Count = oi.Count,
                Price = oi.Price,
                ProductId = oi.ProductId,
            }).ToList();

            order.TotalPrice = createOrderViewModel.OrderItems.Sum(oi => oi.Count * oi.Price);

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            OrderCreatedEvent orderCreatedEvent = new()
            {
                BuyerId = order.BuyerId,
                OrderId = order.OrderId,
                OrderItems = order.OrderItems.Select(oi => new OrderItemMessage
                {
                    Count = oi.Count,
                    ProductId = oi.ProductId
                }).ToList(),
                TotalPrice = order.TotalPrice,
            };

            await _publishEndPoint.Publish(orderCreatedEvent);
            return Ok();
        }



        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _context.Orders
                                       .Include(o => o.OrderItems)
                                       .ToListAsync();

            if (orders == null || !orders.Any())
            {
                return NotFound("Hiç sipariş bulunamadı.");
            }

            // DTO'lara dönüştürme işlemini burada yapıyoruz
            // *** DİKKAT: Artık 'Order' ve 'OrderItem' yerine 'OrderDto' ve 'OrderItemDto' kullanılıyor. ***
            var orderDtos = orders.Select(o => new OrderDto
            {
                OrderId = o.OrderId,
                BuyerId = o.BuyerId,
                CreatedDate = o.CreatedDate,
                OrderStatu = o.OrderStatu,
                TotalPrice = o.TotalPrice,
                OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    Count = oi.Count,
                    Price = oi.Price
                }).ToList()
            }).ToList();

            return Ok(orderDtos);
        }
    }
}