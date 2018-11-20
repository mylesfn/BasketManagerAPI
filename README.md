# BasketManagerAPI
.NET Core API to manage item baskets

The projects in this solution were all built in .NET Core 2.1, which requires the latest SDK from microsoft. 
https://www.microsoft.com/net/download

# Solution Contents
* **TestBasketWebAPI** - Basket Manager Web API
* **ShoppingBasket** - Class Library to allow client applications to easily perform basket actions
* **ShoppingWebsiteTest** - A very simple MVC Web App that makes use of ShoppingBasket to manage 3 items

To run both the API and the test website - Right click the 'TestBasketWebApi' solution header in solution explorer and go to properties. Common properties -> Startup Project and select Multiple startup projects. Move TestBasketWebAPI to the top of the list and set Action to 'Start', then also set ShoppingWebsiteTest action to 'Start'.

# TestBasketWebAPI
This API contains several request methods used to manage a basket of items sent from a client application. Database access is via an Entity Framework implementation that will connect to a SQL Server database but, for the purposes of this prototype, data is stored in-memory. In-memory can be switched off and a connection string configured in the appsettings.json file of the project. 

The API is only intended to manage items and quantity. Any front-end view and other item details will be configured by the client application. 

The following endpoints can be accessed in the API:
* **GetBasket (GET)** - Returns the current state of the basket as an item dictionary. The dictionary key is the itemId as set in the client application and the value is the current quantity in the basket
* **AddItemToBasket (PUT)** - Adds an item to basket with set quantity. The method will return the updated state of the basket. A minus quantity will take items away from the basket
* **RemoveFromBaset (DELETE{id})** - Removes all quantity of the itemId from the basket. The method will return the updated state of the basket
* **ClearBasket (DELETE)** - Removes all quantity of all items from the basket. The method will return the updated state of the basket
* **ProvisionCustomer (PUT)** - Registers a customer in the API database.

## Validation
Each request will be validated in the ValidateController. Firstly it will be checked against DbContext.TrustedDomains, which contains a list of URL domains that the API allows access to. If no domains are added then all requests will be allowed access. 

Next the request header is checked for a customerId or a session number. If a customerId is supplied then the customer is found in the API database and set to the current session. If no customer is found then the request is kicked back. In the case of a sessionId being supplied, a temporary session will be created in the API that is given a lifetime of 30 minutes. After 30 minutes the customer is removed and all basket items deleted.

# ShoppingBasket
This library allows client applications to use the basket API. A ShoppingBasketActions object can perform multiple requests to manage customer baskets. Each constructor will take the URL of the API, (in this case should be http://localhost:61739). If no customerId is supplied in the constructor then a sessionId will be used in requests. These are then kept as an object attribute to be managed by a client application. 
