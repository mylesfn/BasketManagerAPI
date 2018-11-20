using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingBasket.Responses
{
    public class RequestFailure
    {
        public string ReasonForFailure { get; set; }
        
        public RequestFailure(string reason)
        {
            this.ReasonForFailure = reason;
        }
    }
}
