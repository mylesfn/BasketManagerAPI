using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestBasketWebAPI.DataAccess
{
    public class EntityFrameworkSettings
    {
        public string ConnectionString { get; set; }
        public bool UseInMemoryDb { get; set; }
    }
}
