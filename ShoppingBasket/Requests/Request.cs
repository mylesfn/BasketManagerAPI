using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingBasket.Requests
{
    public class Request : IRequest
    {
        public string SessionId { get; set; }
        public int CustomerId { get; set; }
    }
}
