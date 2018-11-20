using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TestBasketWebAPI.DataAccess
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public Basket Basket { get; set; }
        public string SessionId { get; set; }
        public DateTime SessionExpiry { get; set; }
    }
}
