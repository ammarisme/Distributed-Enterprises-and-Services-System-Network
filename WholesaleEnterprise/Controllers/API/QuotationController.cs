using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using WholesaleEnterprise.Models;
using System.Net;
using WholesaleEnterprise.DAL ;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
using System.Data.Entity;


namespace WholesaleEnterprise.Controllers.API
{
    public class QuotationsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public IHttpActionResult GetQuotation(int id)
        {
            Quotation quotation = db.Quotations.Where(q => q.QuotationId == id).Single<Quotation>();
            return Ok(quotation);
        }

        public IHttpActionResult GetProductsInQuotation(int id)
        {
            var productsInQuote = db.ProductsInQuotations.Where(p => p.QuotationId == id);
            var result = (
                from pq in productsInQuote
                join po in db.Products on pq.ProductId equals po.ProductId
                where po.ProductId == pq.ProductId
                select new { 
                    ProductName = po.ProductName,
                    Quantity = pq.Quantity,
                    UnitPrice = pq.UnitPrice
                }
                );
            return Ok(result);
        }
        

        public IHttpActionResult RequestQuotation(JObject jsonBody)
        {
            // set quotations status to request
            // add Request for Quotation


            JObject products = (JObject)jsonBody["ProductsInQuotation"]; // this variable must be present in the javascript

            jsonBody.Remove("ProductsInQuotation");

            Quotation quotation = jsonBody.ToObject<Quotation>(); // the job card object\

            quotation.Status = "Request";

            db.Quotations.Add(quotation);

            db.SaveChanges(); // save the shit

            int quotationId = quotation.QuotationId; // the foregin key to be used for the -> products

            JEnumerable<JToken> tokens = (JEnumerable<JToken>)products.Children<JToken>();

            foreach (JToken token in tokens)
            {
                JToken productJson = token.Children().First();
                ProductInQuotation productInstance = productJson.ToObject<ProductInQuotation>();
                productInstance.QuotationId = quotationId;
                db.ProductsInQuotations.Add(productInstance);

            }

            db.SaveChanges();
            return StatusCode(HttpStatusCode.Created);
        }

        public IHttpActionResult SendQuotation(Quotation quotation)
        {
            // set quotation status to sent
            // update quotation details 
            // update products in quotation

            return StatusCode(HttpStatusCode.OK);
        }

        // user must have logged in
        public IHttpActionResult GetSentQuotations()
        {

            var quotations = db.Quotations.Where(q => q.Status == "Sent");
            var result = (
                from quotes in quotations
                join suppliers in db.Retailers on quotes.SupplierId equals suppliers.RetailerId
                where quotes.SupplierId == suppliers.RetailerId
                select new
                {
                    QuotationId = quotes.QuotationId,
                    Supplier = suppliers.RetailerName,
                    Status = quotes.Status,
                    PaymentMethod = quotes.PaymentMethod,
                    PaymentDuration = quotes.PaymentDuration,
                    DeliveryMethod = quotes.DeliveryMethod
                }
                );

            return Ok(result);
        }

        // user must have logged in
        // TODO : user validation
        [Authorize]
        public IHttpActionResult GetRequestedQuotations()
        {
            //string wholesalerId = User.Identity.GetUserId();
            //IQueryable<Quotation> quotations = db.Quotations;
            //return quotations;
            var quotations = db.Quotations.Where(q => q.Status == "Requested");
            var result = (
                from quotes in quotations
                join suppliers in db.Retailers on quotes.SupplierId equals suppliers.RetailerId
                where quotes.SupplierId == suppliers.RetailerId
                select new {
                    QuotationId = quotes.QuotationId,
                    Supplier = suppliers.RetailerName,
                    Status = quotes.Status,
                    PaymentMethod = quotes.PaymentMethod,
                    PaymentDuration = quotes.PaymentDuration,
                    DeliveryMethod = quotes.DeliveryMethod
                }
                );

            return Ok(result);
        }

        [Authorize]
        public IHttpActionResult GetReceivedQuotations()
        {
            //string wholesalerId = User.Identity.GetUserId();
            //IQueryable<Quotation> quotations = db.Quotations;
            //return quotations;
            var quotations = db.Quotations.Where(q => q.Status == "Received" || q.Status == "Accepted" || q.Status == "Rejected");
            var result = (
                from quotes in quotations
                join suppliers in db.Retailers on quotes.SupplierId equals suppliers.RetailerId
                where quotes.SupplierId == suppliers.RetailerId
                select new
                {
                    QuotationId = quotes.QuotationId,
                    Supplier = suppliers.RetailerName,
                    Status = quotes.Status,
                    PaymentMethod = quotes.PaymentMethod,
                    PaymentDuration = quotes.PaymentDuration,
                    DeliveryMethod = quotes.DeliveryMethod
                }
                );

            return Ok(result);
        }

        
        /*
         * @purpose - this function status of a quotation
         * 
         */
        public class QuotationStatus
        {
            public int QuotationId { get; set; }

            public string Status { get; set; }
        }
        public IHttpActionResult ChangeQuotationStatus(JObject jsonBody)
        {
            var statusCode=HttpStatusCode.OK;

            using (DbContextTransaction scope = db.Database.BeginTransaction())
            {
                QuotationStatus quote = jsonBody.ToObject<QuotationStatus>();
                if (db.Quotations.Count(s => s.QuotationId == quote.QuotationId) > 0)
                {

                    db.Database.ExecuteSqlCommand("SELECT * FROM Quotations WITH (TABLOCKX)");
                    Quotation originalQuotation = db.Quotations.Find(quote.QuotationId);
                    originalQuotation.Status = quote.Status;
                    db.Entry(originalQuotation).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    statusCode = HttpStatusCode.NotModified;
                }    
                scope.Commit();
            }

            return StatusCode(statusCode);
            
        }
                
                    
    }

}