using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TestBasketWebAPI.DataAccess
{
    public class Basket
    {
        public Basket()
        {
            this.Products = new List<Product>();
        }

        [Key]
        public int BasketId { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
        public decimal TotalPrice { get; set; }        

        public Dictionary<int,int> GetBasketItems()
        {
            var items = new Dictionary<int, int>();
            foreach (var item in this.Products)
            {
                items.Add(item.ExternalId, item.Quantity);
            }

            return items;
        }
    }
}
