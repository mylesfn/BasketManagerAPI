using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingBasket.Requests
{
    public class AddItemRequest : IRequest
    {
        public string SessionId { get; set; }
        public int CustomerId { get; set; }
        public int ItemId { get; set; }
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public bool ResetQuantity { get; set; }
    }
}
