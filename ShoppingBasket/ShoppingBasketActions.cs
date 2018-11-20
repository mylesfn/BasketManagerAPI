using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Web;
using ShoppingBasket.Requests;
using ShoppingBasket.Responses;
using System.Text;
using System.Collections.Generic;

namespace ShoppingBasket
{
    public class ShoppingBasketActions
    {
        private string BaseAddress { get; set; }
        public string SessionId { get; set; }
        private IRequest RequestContent { get; set; }
        public int CustomerId { get; set; }
        private string AuthorizationString { get { return this.CustomerId.ToString() + ":" + this.SessionId; } }
        public Dictionary<int, int> ItemDictionary { get; private set; }
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// Creates a new customer identifier
        /// </summary>
        /// <param name="apiAddress">URL of Basket API</param>
        public ShoppingBasketActions(string apiAddress)
            :this(apiAddress, Guid.NewGuid().ToString())
        {            
        }

        /// <summary>
        /// When the customerId is known.
        /// </summary>
        /// <param name="apiAddress">URL of Basket API</param>
        /// <param name="customerId">customerId corresponding to a customer in API DB</param>
        public ShoppingBasketActions(string apiAddress, int customerId)
        {
            if (string.IsNullOrEmpty(apiAddress))
                throw new Exception("API Address cannot be null");
            if (customerId == 0)
                throw new Exception("Customer ID cannot be null");

            this.BaseAddress = apiAddress;
            this.CustomerId = customerId;
        }

        /// <summary>
        /// Talks to API using passed customer GUID
        /// </summary>
        /// <param name="apiAddress">URL of Basket API</param>
        /// <param name="customerGuid">Temporary session number</param>
        public ShoppingBasketActions(string apiAddress, string sessionId)
        {
            if (string.IsNullOrEmpty(apiAddress))
                throw new Exception("API Address cannot be null");
            if (string.IsNullOrEmpty(sessionId))
                throw new Exception("Session ID cannot be null");

            this.BaseAddress = apiAddress;
            this.SessionId = sessionId;            
        }

        private HttpClient GetHttpClient()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(this.BaseAddress);
            client.DefaultRequestHeaders.Add("BasketAuth", Convert.ToBase64String(Encoding.UTF8.GetBytes(this.AuthorizationString)));
            client.Timeout = TimeSpan.FromMinutes(10);

            return client;
        }

        /// <summary>
        /// Returns the basket of the current user. The basket will be in the form of a Dictionary int,int where the client product id number is the key and the 
        /// item quantity is the value. 
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, int> GetBasket()
        {
            using (HttpClient client = GetHttpClient())
            {
                var result = client.GetAsync(ApiUrls.Basket).Result;

                GetBasketResponse response = JsonConvert.DeserializeObject<GetBasketResponse>(result.Content.ReadAsStringAsync().Result);
                this.ItemDictionary = response.Items;
                this.TotalPrice = response.TotalPrice;

                if (!string.IsNullOrEmpty(response.Failure?.ReasonForFailure))
                    throw new Exception(response.Failure.ReasonForFailure);

                return response.Items;
            }
        }

        /// <summary>
        /// Add an item to the basket or change the quantity
        /// </summary>
        /// <param name="itemId">Client item identification</param>
        /// <param name="itemPrice">Price of item</param>
        /// <param name="quantity">Quantity of item</param>
        /// <param name="resetQuantity">Reset quantity with passed value</param>
        public void AddOrUpdateItem(int itemId, decimal itemPrice, int quantity, bool resetQuantity)
        {
            using (HttpClient client = GetHttpClient())
            { 
                AddItemRequest request = new AddItemRequest()
                {
                    CustomerId = this.CustomerId,
                    SessionId = this.SessionId,
                    ItemId = itemId,
                    Price = itemPrice,
                    Quantity = quantity,
                    ResetQuantity = resetQuantity
                };

                HttpContent content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var result = client.PutAsync(ApiUrls.Basket, content).Result;

                var response = JsonConvert.DeserializeObject<AddItemResponse>(result.Content.ReadAsStringAsync().Result);
                this.ItemDictionary = response.Items;
                this.TotalPrice = response.TotalPrice;

                if (!string.IsNullOrEmpty(response.Failure?.ReasonForFailure))
                    throw new Exception(response.Failure.ReasonForFailure);
            }
        }

        /// <summary>
        /// Remove all items from the basket that correspond with the itemId 
        /// </summary>
        /// <param name="itemId">Client item identification</param>
        public void RemoveItem(int itemId)
        {
            using (HttpClient client = GetHttpClient())
            {
                var result = client.DeleteAsync(ApiUrls.Basket + "/" + itemId).Result;

                var response = JsonConvert.DeserializeObject<RemoveItemResponse>(result.Content.ReadAsStringAsync().Result);
                this.ItemDictionary = response.Items;
                this.TotalPrice = response.TotalPrice;

                if (!string.IsNullOrEmpty(response.Failure?.ReasonForFailure))
                    throw new Exception(response.Failure.ReasonForFailure);
            }
        }

        /// <summary>
        /// Completely remove all items from basket
        /// </summary>
        public void ClearBasket()
        {
            using (HttpClient client = GetHttpClient())
            {
                var result = client.DeleteAsync(ApiUrls.Basket).Result;
                var response = JsonConvert.DeserializeObject<RemoveItemResponse>(result.Content.ReadAsStringAsync().Result);
                this.ItemDictionary = response.Items;
                this.TotalPrice = response.TotalPrice;

                if (!string.IsNullOrEmpty(response?.Failure?.ReasonForFailure))
                    throw new Exception(response.Failure.ReasonForFailure);
            }
        }

        /// <summary>
        /// Create customer in the API repository
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="email"></param>
        public void ProvisionCustomer(string firstName, string lastName, string email)
        {
            using (HttpClient client = GetHttpClient())
            {
                ProvisionCustomerRequest request = new ProvisionCustomerRequest()
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email
                };

                HttpContent content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var result = client.PutAsync(ApiUrls.CustomerProvision, content).Result;
                result.EnsureSuccessStatusCode();

                var response = JsonConvert.DeserializeObject<ProvisionCustomerResponse>(result.Content.ReadAsStringAsync().Result);
                this.CustomerId = response.CustomerId;

                if (!string.IsNullOrEmpty(response.Failure?.ReasonForFailure))
                    throw new Exception(response.Failure.ReasonForFailure);
            }
        }
    }
}
