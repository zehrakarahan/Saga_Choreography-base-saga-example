using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order.Api.Models
{
    [Owned]
    public class Address
    {
        public string Line { get; set; }

        public string Province { get; set; }

        public string Dist { get; set; }
    }
}
