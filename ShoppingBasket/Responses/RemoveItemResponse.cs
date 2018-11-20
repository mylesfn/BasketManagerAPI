using System;
using System.Collections.Generic;
using System.Text;
using ShoppingBasket.Requests;

namespace ShoppingBasket.Responses
{
    public class RemoveItemResponse
    {
        public Dictionary<int, int> Items { get; set; }
        public decimal TotalPrice { get; set; }
        public RequestFailure Failure { get; set; }
    }
}
