using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using RetailEnterprise.DAL;
using RetailEnterprise.Models;
using Newtonsoft.Json.Linq;
using Microsoft.AspNet.Identity;
using RetaiEnterprise.Controllers.API;

namespace RetailEnterprise.Controllers.API
{
    public class StocksController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        ///Get current stock level of all the products.
        ///TODO : T-management 
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public IQueryable<dynamic> GetStocks()
        {
            var productStocks = (
                from stocks in db.Products
                select new
                {
                    ProductId = stocks.ProductId,
                    ProductName = stocks.ProductName,
                    StocksQuantity = stocks.StocksQuantity
                }
                );
            return productStocks;
        }

        
        /// <summary>
        /// Get products in a particular stock
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public IQueryable<ProductInProductStocks> GetProductsInStocks(int id)
        {
                return db.ProductsInProductStocks.Where(p => p.ProductStocksId == id);   
        }


        /// <summary>
        /// Stock in this context is basically another name for GRN
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public IHttpActionResult GetProductStocks(int id)
        {
            ProductStocks productStocks = db.Stocks.Find(id);
            if (productStocks == null)
            {
                return NotFound();
            }

            return Ok(productStocks);
        }


        
        /// <summary>
        /// @purpose - Add Stocks recieved to the database
        // Products, ProductsInStock and ProductStocks tables are manipulated by this function.
        // Transaction management isn't done in this case. Because local operations must stay intact from
        // other systems.
        // This function employs NewtonJsoft library to process json data.
        // @parameters
        // -jsonBody    - json Object
        // 
        /// @returns - 
        // Http status 201 - if successfully added
        // Http status 304 - if not successful 
        /// </summary>
        /// <param name="jsonBody"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public HttpResponseMessage AddStocks(JObject jsonBody)
        {
            JObject integrationStock = jsonBody;

            JObject products = (JObject)jsonBody["ProductsInStocks"]; // this variable must be present in the javascript
            JObject integrationProducts = products; // to send to the integration system

            jsonBody.Remove("ProductsInStocks");

            ProductStocks productStocks = jsonBody.ToObject<ProductStocks>(); // the job card object\

            productStocks.ApplicationUserId = User.Identity.GetUserId();

            db.Stocks.Add(productStocks);

            db.SaveChanges(); // save it to db, to get the new stock id for further processing

            int productStocksId = productStocks.ProductStocksId; // the foregin key to be used for the -> proudcts

            JEnumerable<JToken> tokens = (JEnumerable<JToken>)products.Children<JToken>();

            foreach (JToken token in tokens)
            {
                JToken productJson = token.Children().First();
                ProductInProductStocks productInstance = productJson.ToObject<ProductInProductStocks>();
                productInstance.ProductStocksId = productStocksId;
                // add a products in stock entry.
                db.ProductsInProductStocks.Add(productInstance);

                // increase quantity in the products table
                Product product=db.Products.Find(productInstance.ProductId);
                product.StocksQuantity = product.StocksQuantity + productInstance.QuantityRecieved;

                db.Entry(product).State = EntityState.Modified;
            }
            
            db.SaveChanges();

            Integrator integrator = new Integrator();

            try{
            Setting setting = db.Settings.Find(1);

            // process the original data to send to IS
            integrationStock.Add("EnterpriseId", setting.SystemIdNumber);

            integrationStock.Add("ProductsInStocks", products);
            HttpWebResponse response = integrator.sendJsonObject(integrationStock, "/api/Services/AddStocks");

            if (response != null && response.StatusCode != HttpStatusCode.Conflict)
            {
                return this.Request.CreateResponse(HttpStatusCode.Created, "");
            }
            else
            {
                return this.Request.CreateResponse(HttpStatusCode.Continue, "");
            }
            }catch(WebException ex){
                System.Diagnostics.Trace.Write(ex);
                return this.Request.CreateResponse(HttpStatusCode.BadGateway,"Cannot contact IS");
            }

        }

        /// <summary>
        ///@purpose - Deduct the level of stocks
          ////Transaction management isn't done in this case. Because local operations must stay intact from
          ////other systems.
          ////Products, ProductsInStockWasteds and ProductStockWasteds tables are manipulated by this function.
          ////This function employs NewtonJsoft library to process json data.
          ////@parameters
          ////-jsonBody    - json Object
          ////@returns - 
          ////Http status 201 - if successfully added
          ////Http status 406 - if not successful
        /// </summary>
        /// <param name="jsonBody"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public HttpResponseMessage DeductStock(JObject jsonBody)
        {

            JObject products = (JObject)jsonBody["ProductInProductStockWasted"]; // this variable must be present in the javascript

            jsonBody.Remove("ProductInProductStockWasted");

            ProductStockWasted stockWasted = jsonBody.ToObject<ProductStockWasted>(); // the job card object\

            stockWasted.ApplicationUserId = User.Identity.GetUserId();

            db.ProductStockWasteds.Add(stockWasted);

            db.SaveChanges(); // save it to db, to get the new stock id for further processing

            int productStockWastedId = stockWasted.ProductStockWastedId; // the foregin key to be used for the -> proudcts

            JEnumerable<JToken> tokens = (JEnumerable<JToken>)products.Children<JToken>();

            foreach (JToken token in tokens)
            {
                JToken productJson = token.Children().First();
                ProductInProductStockWasted productInstance = productJson.ToObject<ProductInProductStockWasted>();
                productInstance.ProductStockWastedId = productStockWastedId;
                // add a product in wasted stock to the db
                db.ProductInProductStockWasteds.Add(productInstance);

                // decrease quantity in the products table

                Product product = db.Products.Find(productInstance.ProductId);
                if(product.StocksQuantity - productInstance.Quantity < 0){
                    // delete the already added stock wasted infor..
                    // user might want to remove the product from a service..
                    // so let him continue, but don't update the local system
                    db.ProductStockWasteds.Remove(stockWasted);
                    db.SaveChanges();
                    break; // break the loop so we can go and talk to the IS
                }
                else
                {
                    // reduce the stock from products table
                    product.StocksQuantity = product.StocksQuantity - productInstance.Quantity;
                    db.Entry(product).State = EntityState.Modified;
                }
            }

            // update all services through IS
            try
            {
            Integrator integrator = new Integrator();

            jsonBody.Add("ProductInProductStockWasted", products);
            HttpWebResponse response = integrator.sendJsonObject(jsonBody, "/api/Services/DeductStock");
            }
            catch (WebException ex)
            {
                System.Diagnostics.Trace.Write(ex);
                return this.Request.CreateResponse(HttpStatusCode.BadGateway, "Cannot contact IS");
            }

            db.SaveChanges();
            return this.Request.CreateResponse(HttpStatusCode.Created, "Stock added to the ES");
        }



        // housekeeping method
        protected override void Dispose(bool disposeNow)
        {
            if (disposeNow)
            {
                db.Dispose();
            }
            base.Dispose(disposeNow);
        }

    }
}