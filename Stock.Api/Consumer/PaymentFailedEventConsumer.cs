using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Event;
using Stock.Api.Models.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stock.Api.Consumer
{
    public class PaymentFailedEventConsumer : IConsumer<PaymentFailedEvent>
    {
        public readonly AppDbContext _appDbContext;

        public PaymentFailedEventConsumer(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            foreach (var item in context.Message.orderItems)
            {
                var stock = await _appDbContext.Stocks.FirstOrDefaultAsync(x=>x.ProductId==item.ProductId);
                if (stock!=null)
                {
                    stock.Count += item.Count;
                    await _appDbContext.SaveChangesAsync();

                }
            }
        }
    }
}
