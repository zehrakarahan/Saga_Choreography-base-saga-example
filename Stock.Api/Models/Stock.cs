﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stock.Api.Models
{
    public class Stock
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int Count { get; set; }
    }
}
