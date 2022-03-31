using MassTransit;
using Order.Api.Models;
using Shared.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order.Api.Consumers
{
    public class StockNotReservedConsumer : IConsumer<StockNotReservedEvent>
    {
        public readonly AppDbContext _appDbContext;

        public StockNotReservedConsumer(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task Consume(ConsumeContext<StockNotReservedEvent> context)
        {
            var order = await _appDbContext.Orders.FindAsync(context.Message.OrderId);
            if (order != null)
            {
                order.OrderStatus = OrderStatus.Fail;
                order.FailMessage = context.Message.Message;
                await _appDbContext.SaveChangesAsync();
            }
        }
    }
}
