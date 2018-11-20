using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingBasket.Requests
{
    public class ProvisionCustomerRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
