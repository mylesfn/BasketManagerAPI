using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestBasketWebAPI.DataAccess;
using ShoppingBasket.Requests;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace TestBasketWebAPI.Controllers
{
    public class ValidateController : ControllerBase
    {
        protected readonly BasketDbContext DbContext;
        protected Customer Customer;
        protected Basket Basket;
        protected ICollection<Product> BasketItems;
        protected IRequest BasketRequest;
        protected string SessionId;
        protected string Error;

        public ValidateController(BasketDbContext context)
        {
            this.DbContext = context;
            this.DbContext.FinishInitialising();
        }

        /// <summary>
        /// Validates the request. Checks the request header to determine if the customer is a returning customer or a temporary customer.
        /// </summary>
        /// <returns>true if the request is valid</returns>
        public bool ValidateRequest()
        {
            if (Request == null)
                return false;

            if (this.DbContext.TrustedDomains.Where(x => x.AcceptRequests && Request.Host.Host.StartsWith(x.Domain)).FirstOrDefault() == null)
                return false;

            try
            {
                int customerId = 0;
                string sessionId = "";
                GetCredsFromHeader(out customerId, out sessionId);
                this.BasketRequest = new Request()
                {
                    CustomerId = customerId,
                    SessionId = sessionId
                };

                if (BasketRequest.CustomerId == 0)
                {
                    if (!string.IsNullOrEmpty(BasketRequest.SessionId))
                    {
                        this.SessionId = BasketRequest.SessionId;
                        var customer = this.DbContext.GetCustomerBySessionId(this.SessionId);
                        if (customer != null)
                        {
                            if (customer.SessionExpiry <= DateTime.UtcNow)
                            {
                                this.DbContext.Customers.Remove(customer);
                                this.DbContext.Save();
                            }
                            else
                            {
                                this.Customer = customer;
                                this.Basket = customer.Basket;
                                return true;
                            }
                        }

                        CreateTempCustomer(this.SessionId);

                        var tempCustomer = this.DbContext.GetCustomerBySessionId(this.SessionId);
                        if (tempCustomer != null)
                        {
                            this.Customer = tempCustomer;
                            this.Basket = tempCustomer.Basket;
                            return true;
                        }
                        else
                        {
                            Error = "Unable to create temporary customer.";
                            return false;
                        }
                    }
                    else
                    {
                        Error = "Request did not contain a customer ID or session ID.";
                        return false;
                    }
                }
                else
                {
                    var customer = this.DbContext.GetCustomerByCustomerId(BasketRequest.CustomerId);
                    if (customer == null)
                    {
                        Error = "Cannot find a customer with ID = " + BasketRequest.CustomerId;
                        return false;
                    }

                    this.Customer = customer;
                    this.Basket = customer.Basket;
                    return true;
                }
            }
            catch(Exception ex)
            {
                Error = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// The customer will try to be determined from the request header
        /// </summary>
        /// <param name="customerId">The ID number that is stored in the API database against the customer</param>
        /// <param name="sessionId">A temporary session number (GUID)</param>
        private void GetCredsFromHeader(out int customerId, out string sessionId)
        {
            customerId = 0;
            sessionId = "";

            var authHeader = Request.Headers["BasketAuth"];
            if (StringValues.IsNullOrEmpty(authHeader))
                throw new Exception("Request is missing authentication header.");

            byte[] authBytes = Convert.FromBase64String(authHeader);
            var encoder = Encoding.ASCII;
            string creds = encoder.GetString(authBytes);

            string[] custAndSessionId = creds.Split(":");

            int.TryParse(custAndSessionId[0], out customerId);
            sessionId = custAndSessionId[1];
        }

        /// <summary>
        /// Temporary users are created with a lifetime of 30 minutes, after which their session will expire and their basket cleared
        /// </summary>
        /// <param name="sessionId">Temporary session number (GUID)</param>
        public void CreateTempCustomer(string sessionId)
        {
            DateTime expiryTime = DateTime.Now.AddMinutes(30);
            var customer = new Customer() { SessionId = sessionId, Basket = new Basket(), SessionExpiry = expiryTime };
            this.DbContext.Customers.Add(customer);
            this.DbContext.Save();
        }       
    }
}
