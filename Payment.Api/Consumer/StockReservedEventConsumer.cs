using MassTransit;
using Shared.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Payment.Api.Consumer
{
    public class StockReservedEventConsumer : IConsumer<StockReservedEvent>
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public StockReservedEventConsumer(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<StockReservedEvent> context)
        {
            await _publishEndpoint.Publish(new PaymentSuccessEvent {
              
              BuyerId=context.Message.BuyerId,
              OrderId=context.Message.OrderId
            
            });

        }
    }
}
