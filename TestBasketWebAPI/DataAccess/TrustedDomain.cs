using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TestBasketWebAPI.DataAccess
{
    public class TrustedDomain
    {
        [Key]
        public string Domain { get; set; }
        public bool AcceptRequests { get; set; }
    }
}
