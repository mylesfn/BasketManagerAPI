using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingBasket.Requests
{
    public interface IRequest
    {
        string SessionId { get; set; }
        int CustomerId { get; set; }
    }
}
