using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TestBasketWebAPI.DataAccess
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        public int ExternalId { get; set; }
        public int BasketId { get; set; }
        public Basket Basket { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
