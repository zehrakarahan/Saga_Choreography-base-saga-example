using Shared.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order.Api.Models.Dtos
{
    public class OrderCreatedDto
    {
        public string BuyerId { get; set; }

        public PaymentMessage Payment { get; set; }

        public List<OrderItemMessage> orderItems { get; set; } = new List<OrderItemMessage>();
    }
}
