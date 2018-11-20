using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingBasket.Responses
{
    public interface IResponse 
    {
        string SessionId { get; set; }
        int CustomerId { get; set; }
    }
}
