using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Order.Api.Models
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        public int  ProductId { get; set; }

        [Column(TypeName ="decimal(18,2)")]
        public decimal Price { get; set; }

        public int OrderId { get; set; }

        public Order Order { get; set; }

        public int Count { get; set; }
    }
}
