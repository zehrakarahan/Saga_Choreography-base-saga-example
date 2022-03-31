using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Event;
using Stock.Api.Models.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stock.Api.Consumer
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {

        public readonly AppDbContext _appDbContext;

        public readonly ISendEndpointProvider _sendEndpointProvider;

        public readonly IPublishEndpoint _publishEndpoint;

        public OrderCreatedEventConsumer(AppDbContext appDbContext, ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint)
        {
            _appDbContext = appDbContext;
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            var stockrresult = new List<bool>();
            foreach (var item in context.Message.orderItems)
            {
                stockrresult.Add(await _appDbContext.Stocks.AnyAsync(x=>x.ProductId==item.ProductId && x.Count>item.Count));
            }

            if (stockrresult.All(x=>x.Equals(true)))
            {
                foreach (var item in context.Message.orderItems)
                {
                    var stock = await _appDbContext.Stocks.FirstOrDefaultAsync(x=>x.ProductId==item.ProductId);
                    if (stock!=null)
                    {
                        stock.Count -= item.Count;
                    }
                    await _appDbContext.SaveChangesAsync();
                    var sendendpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"quene:{RabbitMQSettings.StockReservedEventQueueName}"));

                    StockReservedEvent stockReservedEvent = new StockReservedEvent()
                    {
                        Payment=context.Message.Payment,
                        BuyerId=context.Message.BuyerId,
                        OrderId=context.Message.OrderId,
                        orderItems=context.Message.orderItems
                        
                    };
                   await sendendpoint.Send(stockReservedEvent);

                }

            }
            else
            {
                await _publishEndpoint.Publish(new StockNotReservedEvent {
                
                  BuyerId=context.Message.BuyerId,
                  OrderId=context.Message.OrderId,
                  Message="suan yeterli stock bulunmamaktadir"
                
                });
            }
        }
    }
}
