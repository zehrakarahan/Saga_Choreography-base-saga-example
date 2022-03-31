using MassTransit;
using Order.Api.Models;
using Shared.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order.Api.Consumers
{
    public class PaymentCompletedEventConsumer : IConsumer<PaymentSuccessEvent>
    {
        private readonly AppDbContext _appDbContext;

        public PaymentCompletedEventConsumer(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task Consume(ConsumeContext<PaymentSuccessEvent> context)
        {
            var order = await _appDbContext.Orders.FindAsync(context.Message.OrderId);
            if (order!=null)
            {
                order.OrderStatus = OrderStatus.Complete;
                await _appDbContext.SaveChangesAsync();
            }
        }
    }
}
