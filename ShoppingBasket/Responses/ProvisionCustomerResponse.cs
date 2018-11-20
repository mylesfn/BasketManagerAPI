using System;
using System.Collections.Generic;
using System.Text;
using ShoppingBasket.Requests;

namespace ShoppingBasket.Responses
{
    public class ProvisionCustomerResponse
    {
        public int CustomerId { get; set; }
        public ProvisionCustomerRequest OriginalRequest { get; set; }
        public RequestFailure Failure { get; set; }
    }
}
