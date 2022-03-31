﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Event
{
    public class PaymentFailedEvent
    {
        public int OrderId { get; set; }

        public string BuyerId { get; set; }

        public string Message { get; set; }

        public List<OrderItemMessage> orderItems{ get; set; }
    }
}
