using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order.Api.Models;
using Shared.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        public readonly AppDbContext _context;

        public readonly IPublishEndpoint _publishEndpoint;

        public OrderController(AppDbContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderCreatedDtos orderCreate)
        {
            var neworder = new Models.Order
            {
                BuyerId=orderCreate.BuyerId,
                OrderStatus=OrderStatus.Suspend,
                Address=new Address { Line=orderCreate.Address.Line,Province=orderCreate.Address.Province,Dist=orderCreate.Address.Dist},
                CreatedDate=DateTime.Now,
                
            };
            orderCreate.orderItems.ForEach(items=>
            {
                neworder.Items.Add(new OrderItem()
                {

                    Price = items.Price,
                    ProductId = items.ProductId,
                    Count = items.Count

                });
            });

            await _context.AddAsync(neworder);
            await _context.SaveChangesAsync();

            var orderCreatedEvent = new OrderCreatedEvent()
            {
                BuyerId=orderCreate.BuyerId,
                OrderId=neworder.Id,
                Payment=new PaymentMessage
                {
                    CardName=orderCreate.Payment.CardName,
                    CardNumber=orderCreate.Payment.CardNumber,
                    CVV=orderCreate.Payment.CVV,
                    Expiration=orderCreate.Payment.Expiration,
                    TotalPrice=orderCreate.orderItems.Sum(x=>x.Price*x.Count)
                }
            };

            orderCreate.orderItems.ForEach(items =>
            {
                orderCreatedEvent.orderItems.Add(new OrderItemMessage()
                {

                    ProductId = items.ProductId,
                    Count = items.Count

                });
            });

            await _publishEndpoint.Publish(orderCreatedEvent);
            return Ok();
        }

        public class OrderCreatedDtos
        {
                public string BuyerId { get; set; }

                public PaymentDto Payment { get; set; }

                public List<OrderItemDto> orderItems { get; set; } = new List<OrderItemDto>();

                public AddressDto Address { get; set; }
        } 
        public class OrderItemDto
        {
            public int ProductId { get; set; }

            public int Count { get; set; }

            public decimal Price { get; set; }
        }

        public class PaymentDto
        {
            public string CardName { get; set; }

            public string CardNumber { get; set; }

            public string Expiration { get; set; }

            public string CVV { get; set; }

            public string TotalPrice { get; set; }
        }

        public class AddressDto
        {
            public string Line { get; set; }

            public string Province { get; set; }

            public string Dist { get; set; }
        }
    }
}
