using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TestBasketWebAPI.DataAccess;
using ShoppingBasket.Requests;
using ShoppingBasket.Responses;

namespace TestBasketWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ValidateController
    {
        public CustomersController(BasketDbContext context)
            :base(context)
        {
        }

        /// <summary>
        /// A request that will provision a customer in the API database for prolonged sessions.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        public ActionResult<ProvisionCustomerResponse> ProvisionCustomer([FromBody] ProvisionCustomerRequest request)
        {
            try
            {
                var customer = this.DbContext.Customers.Add(new Customer() { FirstName = request.FirstName, LastName = request.LastName, Email = request.Email, Basket = new Basket() }).Entity;
                this.DbContext.Save();

                ProvisionCustomerResponse response = new ProvisionCustomerResponse()
                {
                    CustomerId = customer.CustomerId,
                    OriginalRequest = request
                };

                return Ok(response);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}