using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TestBasketWebAPI.DataAccess;
using ShoppingBasket.Responses;
using ShoppingBasket.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace TestBasketWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ValidateController
    {        
        public BasketController(BasketDbContext context) 
            : base(context)
        {
        }        

        /// <summary>
        /// Returns customer basket in the form of an item dictionary, the client's itemId is the key and the value is the quantity
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<GetBasketResponse> GetBasket()
        {
            if (!ValidateRequest())
                return BadRequest(new GetBasketResponse() { Failure = new RequestFailure(Error) });

            GetBasketResponse response = new GetBasketResponse()
            {
                Items = this.Basket.GetBasketItems(),
                TotalPrice = Basket.Products.Sum(x => x.Price)
            };

            return Ok(response);
        }

        /// <summary>
        /// Adds an item to basket with specified quantity.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPut]
        public ActionResult<AddItemResponse> AddItemToBasket([FromBody] AddItemRequest value)
        {
            if (!ValidateRequest())
                return BadRequest(new AddItemResponse() { OriginalRequest = value, Failure = new RequestFailure(Error) });

            try
            {
                AddItemRequest request = value;
                bool resetQuantity = request.ResetQuantity;

                var item = this.Basket.Products.Where(x => x.ExternalId == request.ItemId).FirstOrDefault();
                if (item != null)
                {
                    if (resetQuantity)
                        item.Quantity = request.Quantity;
                    else
                        item.Quantity += request.Quantity;
                }
                else if(request.Quantity > 0)
                    this.Basket.Products.Add(new Product() { Price = request.Price, ExternalId = request.ItemId, Quantity = request.Quantity });

                this.DbContext.Save();

                AddItemResponse response = new AddItemResponse()
                {
                    CustomerId = Customer.CustomerId,
                    SessionId = Customer.SessionId,
                    OriginalRequest = request,
                    Items = this.Basket.GetBasketItems(),
                    TotalPrice = Basket.Products.Sum(x => x.Price)

                };

                return Ok(response);
            }
            catch(Exception ex)
            {
                return BadRequest(new AddItemResponse() { OriginalRequest = value, Failure = new RequestFailure(ex.Message) });
            }
        }

        /// <summary>
        /// Removes an item from the basket, regardless of quantity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public ActionResult<RemoveItemResponse> RemoveFromBasket(int id)
        {
            if (!ValidateRequest())
                return BadRequest(new RemoveItemResponse() { Failure = new RequestFailure(Error) });

            try
            {
                var item = this.Basket.Products.Where(x => x.ExternalId == id).FirstOrDefault();
                if (item == null)
                    throw new Exception("Cannot find item in basket.");

                this.Basket.Products.Remove(item);
                this.DbContext.Save();

                var response = new RemoveItemResponse()
                {
                    Items = this.Basket.GetBasketItems(),
                    TotalPrice = Basket.Products.Sum(x => x.Price)
                };

                return Ok(response);
            }
            catch(Exception ex)
            {
                return BadRequest(new RemoveItemResponse() { Failure = new RequestFailure(ex.Message) } );
            }
        }

        /// <summary>
        /// Completely clears all items out of the basket
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public ActionResult<RemoveItemResponse> ClearBasket()
        {
            if (!ValidateRequest())
                return BadRequest(new RemoveItemResponse() { Failure = new RequestFailure(Error) });

            try
            {
                this.Basket.Products.Clear();
                this.DbContext.Save();

                var response = new RemoveItemResponse()
                {
                    Items = this.Basket.GetBasketItems(),
                    TotalPrice = Basket.Products.Sum(x => x.Price)
                };

                return Ok(response);
            }
            catch(Exception ex)
            {
                return BadRequest(new RemoveItemResponse() { Failure = new RequestFailure(ex.Message) });
            }
        }
    }
}
