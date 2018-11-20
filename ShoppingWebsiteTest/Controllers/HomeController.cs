using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShoppingWebsiteTest.Models;
using ShoppingBasket;

namespace ShoppingWebsiteTest.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View(new BasketModel() { ItemDictionary = new Dictionary<int, int>() });
        }
        
        [HttpPost]
        [Route("/Home/")]
        public IActionResult Button(string someValue)
        {
            string buttonClicked = "";
            ShoppingBasketActions basket = new ShoppingBasketActions("http://localhost:61739/", -1);
            
            if (HttpContext.Request.Form.ContainsKey("minus"))
            {
                buttonClicked = "minus";
                int itemId = 0;
                int.TryParse(HttpContext.Request.Form[buttonClicked], out itemId);

                basket.AddOrUpdateItem(itemId, 0M, -1, false);
            }
            else if (HttpContext.Request.Form.ContainsKey("plus"))
            {
                buttonClicked = "plus";
                int itemId = 0;
                int.TryParse(HttpContext.Request.Form[buttonClicked], out itemId);

                basket.AddOrUpdateItem(itemId, 0M, 1, false);
            }
            else if (HttpContext.Request.Form.ContainsKey("clear"))
            {
                basket.ClearBasket();
            }

            BasketModel model = new BasketModel() { ItemDictionary = basket.ItemDictionary };

            return View("Index", model);
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
