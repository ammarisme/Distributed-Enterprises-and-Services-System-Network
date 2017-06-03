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
using WholesaleEnterprise.DAL;
using WholesaleEnterprise.Models;
using Newtonsoft.Json.Linq;
using Microsoft.AspNet.Identity;

namespace WholesaleEnterprise.Controllers.API
{
    public class StocksController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Stocks
        public IQueryable<Product> GetStocks()
        {
            return db.Products ;
        }

        public IQueryable<ProductInProductStocks> GetProductsInStocks(int id)
        {
            return db.ProductsInProductStocks.Where(p => p.ProductStocksId == id);
        }



        // GET: api/Stocks/5
        [ResponseType(typeof(ProductStocks))]
        public IHttpActionResult GetProductStocks(int id)
        {
            ProductStocks productStocks = db.Stocks.Find(id);
            if (productStocks == null)
            {
                return NotFound();
            }

            return Ok(productStocks);
        }


        /*
         * @purpose - Add Stocks recieved to the database
         * Products, ProductsInStock and ProductStocks tables are manipulated by this function.
         * This function employs NewtonJsoft library to process json data.
         * @parameters
         * -jsonBody    - json Object
         * 
         * @returns - 
         * Http status 201 - if successfully added
         * Http status 304 - if not successful
         */
        [HttpPost]
        [Authorize]
        public IHttpActionResult AddStocks(JObject jsonBody)
        {

            JObject products = (JObject)jsonBody["ProductsInStocks"]; // this variable must be present in the javascript

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
            return StatusCode(HttpStatusCode.Created);
        }

        /*
         * @purpose - Deduct the level of stocks
         * Products, ProductsInStockWasteds and ProductStockWasteds tables are manipulated by this function.
         * This function employs NewtonJsoft library to process json data.
         * @parameters
         * -jsonBody    - json Object
         * 
         * @returns - 
         * Http status 201 - if successfully added
         * Http status 406 - if not successful
         */
        [HttpPost]
        [Authorize]
        public IHttpActionResult DeductStock(JObject jsonBody)
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
                    // delete the already added stock wasted infor and return, user is trying to mess with us.
                    db.ProductStockWasteds.Remove(stockWasted);
                    db.SaveChanges();
                    return StatusCode(HttpStatusCode.NotAcceptable);
                }
                else
                {
                    // reduce the stock from products table
                    product.StocksQuantity = product.StocksQuantity - productInstance.Quantity;
                    db.Entry(product).State = EntityState.Modified;
                }
            }

            db.SaveChanges();
            return StatusCode(HttpStatusCode.Created);
        }


        // PUT: api/Stocks/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutProductStocks(int id, ProductStocks productStocks)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != productStocks.ProductStocksId)
            {
                return BadRequest();
            }

            db.Entry(productStocks).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductStocksExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Stocks
        [ResponseType(typeof(ProductStocks))]
        public IHttpActionResult PostProductStocks(ProductStocks productStocks)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Stocks.Add(productStocks);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = productStocks.ProductStocksId }, productStocks);
        }

        // DELETE: api/Stocks/5
        [ResponseType(typeof(ProductStocks))]
        public IHttpActionResult DeleteProductStocks(int id)
        {
            ProductStocks productStocks = db.Stocks.Find(id);
            if (productStocks == null)
            {
                return NotFound();
            }

            db.Stocks.Remove(productStocks);
            db.SaveChanges();

            return Ok(productStocks);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductStocksExists(int id)
        {
            return db.Stocks.Count(e => e.ProductStocksId == id) > 0;
        }
    }
}