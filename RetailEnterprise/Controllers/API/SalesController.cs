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
using System.Web;

namespace RetailEnterprise.Controllers.API
{
    public class SalesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /*
         * Get all sales information from the database
         * TODO : This transaction requires locking
         */
        public IHttpActionResult GetSales()
        {
            var sales = (
                from sale in db.Sales
                join customers in db.Customers on sale.CustomerId equals customers.CustomerId
                where customers.CustomerId == sale.CustomerId
                select new
                {
                    OrderId = sale.OrderId,
                    OrderDate = sale.OrderDate,
                    OrderDueDate = sale.OrderDueDate,
                    OrderStatus = sale.OrderStatus,
                    CustomerFullName = customers.FirstName + " " + customers.LastName,
                    DeliveredDate = sale.DeliveredDate,
                    DeliveryStatus = sale.DeliveryStatus,
                    DeliveryMode = sale.DeliveryMode,
                    PaymentMethod = sale.PaymentMethod,
                    PaymentDuration = sale.PaymentDuration,
                    Remark = sale.Remark
                }

                );
            return Ok(sales);
        }

        // temporaty object used to change sales status
        public class SaleStatus
        {
            public int OrderId { get; set; }

            public string Status { get; set; }
        }
        /*
         * Changing a sales status in the database and the service which sent the sale
         * transaction rolebacks on http 409 status
         */
        [AllowAnonymous]
        public HttpResponseMessage ChangeSaleStatus(JObject jsonBody)
        {
            using (var dbTransaction = db.Database.BeginTransaction())
            {
                HttpWebResponse response = null;
                try
                {
                    SaleStatus sale = jsonBody.ToObject<SaleStatus>();
                    // update the sale if it existed
                    if (db.Sales.Count(s => s.OrderId == sale.OrderId) > 0)
                    {
                        Sale originalSale = db.Sales.Find(sale.OrderId);
                        originalSale.OrderStatus = sale.Status;
                        db.Entry(originalSale).State = EntityState.Modified;
                        db.SaveChanges();

                        // if this sale came from a service.. send this update to the service
                        if (originalSale.ServiceId != null)
                        {
                            // send this sale update to the IS
                            Integrator integrator = new Integrator();

                            Setting setting = db.Settings.Find(1);

                            // add routing information
                            jsonBody.Add("ServiceId", originalSale.ServiceId);
                            jsonBody.Add("EnterpriseId", setting.SystemIdNumber);

                            // adding payload information used for model conversion
                            jsonBody["OrderId"] = originalSale.ServiceOrderId;

                            response = integrator.sendJsonObject(jsonBody, "/api/Services/ChangeSaleStatus");
                            if (response == null || response.StatusCode == HttpStatusCode.Conflict)
                            {
                                dbTransaction.Rollback();
                                return this.Request.CreateResponse(HttpStatusCode.OK, "Status changed");
                            }
                            else
                            {
                                dbTransaction.Commit();
                                return this.Request.CreateResponse(HttpStatusCode.Conflict, "Transaction rollbacked because of response from IS");
                            }
                        }
                    }
                    else
                    {
                        return this.Request.CreateResponse(HttpStatusCode.NoContent, "Sale not found");
                    }
                    // send the update to the IS. 
                    // add service routing information to send the order to the relevant service
                }
                catch (Exception ex)
                {
                    dbTransaction.Rollback();
                    System.Diagnostics.Trace.WriteLine(ex);
                    return this.Request.CreateResponse(HttpStatusCode.Conflict, "RE system controller error");
                }
            }
                return this.Request.CreateResponse(HttpStatusCode.BadRequest, "using statement didn't execute");
        }

        /*
         * public method which can be accessed by the IS to add a sale (Order) in the ES
         * 
         */
        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage AddOrder(JObject jsonBody)
        {
            using (var dbTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    JObject products = (JObject)jsonBody["ProductsInRetailOrder"]; // we have products information
                    JObject account = (JObject)jsonBody["Account"]; // we have account information
                    JValue AccountId = (JValue)jsonBody["AccountId"]; // we have account Id

                    Sale sale = new Sale();
                    sale.Remark = jsonBody["Remark"].ToString();
                    sale.PaymentMethod = jsonBody["PaymentMethod"].ToString();
                    sale.DeliveryStatus = "Pending";
                    sale.OrderStatus = "Unconfirmed";
                    sale.OrderDate = DateTime.Today;
                    sale.DeliveryMode = jsonBody["DeliveryModeName"].ToString();
                    sale.ServiceOrderId = Int32.Parse(jsonBody["ServiceOrderId"].ToString());
                    sale.ServiceId = Int32.Parse(jsonBody["ServiceId"].ToString());
                    string accountId = AccountId.ToString();
                    int customerId;

                    // lambda expression in the if condition always evaluates to false
                    int i = db.Customers.Where(c => c.UniversalId == accountId).Count();

                    // check if a customer with the account id exist, if not create it.
                    if (i <= 0)
                    {
                        // create the customer
                        Customer customer = jsonBody["Account"].ToObject<Customer>();
                        // some model information are not mapping here .. so map it manually .
                        // with the next version model synchronization must be a priority
                        customer.PhoneNumber = jsonBody["Account"]["PhoneNumber2"].ToString();
                        customer.UniversalId = jsonBody["Account"]["AccountId"].ToString();
                        customer.BillingAddress = jsonBody["Account"]["Address"].ToString();
                        customer.UniversalId = jsonBody["Account"]["AccountId"].ToString();
                        db.Customers.Add(customer);
                        customerId = customer.CustomerId;
                    }
                    else
                    {
                        Customer temp = db.Customers.Where(c => c.UniversalId == accountId).First();
                        // get the customer id where uni id is accountId
                        customerId = temp.CustomerId;
                    }

                    sale.CustomerId = customerId;
                    db.Sales.Add(sale);

                    db.SaveChanges();

                    int OrderId = sale.OrderId; // the foregin key to be used for the -> proudcts

                    JEnumerable<JToken> tokens = (JEnumerable<JToken>)products.Children<JToken>();

                    foreach (JToken token in tokens)
                    {
                        JToken productJson = token.Children().First();
                        ProductInSale productInstance = productJson.ToObject<ProductInSale>();
                        productInstance.SaleId = OrderId;
                        db.ProductsInSales.Add(productInstance);

                        // update the stock
                        Product dbProduct = db.Products.Find(productInstance.ProductId);
                        float newQuantity = (float)dbProduct.StocksQuantity;
                        newQuantity = newQuantity - productInstance.Quantity;
                        if (newQuantity < 0)
                        {
                            dbTransaction.Rollback();
                            return this.Request.CreateResponse(HttpStatusCode.Conflict, "Stock not sufficient.");
                        }
                        else
                        {
                            dbProduct.StocksQuantity = newQuantity;
                            db.Entry(dbProduct).State = EntityState.Modified;
                        }
                    }

                    db.SaveChanges();

                    dbTransaction.Commit();
                    return this.Request.CreateResponse(HttpStatusCode.Created, "Information created in Enterprise System");
                }
                catch (Exception ex)
                {
                    dbTransaction.Rollback();
                    System.Diagnostics.Trace.Write(ex);
                    return this.Request.CreateResponse(HttpStatusCode.Conflict, "Db operation failed in Enterprise System");
                }
            }
        }

        /*
         * Get individual order from db
         */
        [AllowAnonymous]
        public IHttpActionResult GetRetailOrder(int id)
        {
            Sale retailOrder = db.Sales.Find(id);
            if (retailOrder == null)
            {
                return NotFound();
            }

            return Ok(retailOrder);
        }

        /*
         * Get all the products in an order
         */
        [Authorize]
        public IHttpActionResult GetProductsInRetailOrder(int id)
        {
            if (SaleExists(id))
            {
            return  Ok(db.ProductsInSales.Where(p => p.SaleId == id));
            }
            return NotFound();
        }

        // check if a oreder exist or not
        private bool SaleExists(int id)
        {
            return db.Sales.Count(e => e.OrderId == id) > 0;
        }
        
        // the housekeeper
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