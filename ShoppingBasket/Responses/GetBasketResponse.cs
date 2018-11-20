using System;
using System.Collections.Generic;
using System.Text;
using ShoppingBasket.Requests;

namespace ShoppingBasket.Responses
{
    public class GetBasketResponse : IResponse
    {
        public decimal TotalPrice { get; set; }
        public Dictionary<int, int> Items { get; set; }
        public string SessionId { get; set; }
        public int CustomerId { get; set; }
        public RequestFailure Failure { get; set; }
    }
}
