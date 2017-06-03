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
using RetailEnterprise.Areas.Products.Models;
using RetailEnterprise.Areas.PurchaseOrders.Models;

namespace RetailEnterprise.Controllers.API
{
    [Authorize]
    public class PurchaseOrdersController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary> 
        /// Get all the POs
        /// </summary>
        /// <returns>an iqueryable list of all POs</returns>
        public IQueryable<PurchaseOrder> GetPurchaseOrders()
        {
            return db.PurchaseOrders;
        }

        /// <summary>
        /// adding a PO to the ES
        /// TODO : Universal Enterprise Id integration
        /// </summary>
        /// <param name="jsonBody"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public HttpResponseMessage AddOrder(JObject jsonBody)
        {
            JObject products = (JObject)jsonBody["ProductsInPurchaseOrders"]; // this variable must be present in the javascript

            jsonBody.Remove("ProductsInPurchaseOrders");

            PurchaseOrder wholesaleOrder = jsonBody.ToObject<PurchaseOrder>(); // the job card object

            db.PurchaseOrders.Add(wholesaleOrder);

            db.SaveChanges();

            int wholesaleOrderId = wholesaleOrder.OrderId; // the foregin key to be used for the -> proudcts

            JEnumerable<JToken> tokens = (JEnumerable<JToken>)products.Children<JToken>();

            foreach (JToken token in tokens)
            {
                JToken productJson = token.Children().First();
                ProductInPurchaseOrder productInstance = productJson.ToObject<ProductInPurchaseOrder>();
                productInstance.PurchaseOrderId = wholesaleOrderId;
                db.ProductsInPurchaseOrders.Add(productInstance);
            }

            db.SaveChanges();
            return this.Request.CreateResponse(HttpStatusCode.Created,wholesaleOrderId);
        }

        /// <summary>
        /// Update a specific product in P/O
        /// TODO : IS integration
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public IHttpActionResult UpdateProductInPurchaseOrder(ProductInPurchaseOrder product)
        {
            if (!WholesaleOrderExists(product.PurchaseOrderId))
            {
                return StatusCode(HttpStatusCode.NotModified);
            }
            // else
            db.Entry(product).State = EntityState.Modified;

            db.SaveChanges();

            return StatusCode(HttpStatusCode.Created);
        }

        public IHttpActionResult GetProductsInPurchaseOrder(int id)
        {
            // get all the products of the PO
            var products = db.ProductsInPurchaseOrders.Where(p => p.PurchaseOrderId == id); 
            List<ProductsInPurchaseOrderView> productsInPo = new List<ProductsInPurchaseOrderView>();

            var joinNative = (
                              from pop in products
                              join p in db.Products on pop.ProductId equals p.ProductId
                              where p.ProductId == pop.ProductId
                              select new { 
                                ProductInPurchaseOrderId = pop.ProductInPurchaseOrderId ,
                                ProductId = p.ProductId ,
                                ProductName = p.ProductName,
                                Quantity = pop.Quantity,
                                Cost = pop.Cost,
                                Remark = pop.Remark
                              }
                                  );

            
            return Ok(joinNative);
        }

        /// <summary>
        /// Update Order
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public IHttpActionResult UpdateOrder(PurchaseOrder order)
        {
            if (!WholesaleOrderExists(order.OrderId))
            {
                return StatusCode(HttpStatusCode.NotModified);
            }
            // else
            db.Entry(order).State = EntityState.Modified;

            db.SaveChanges();

            return StatusCode(HttpStatusCode.Created);
        }

        /// <summary>
        /// Getting P.O based on the PK 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IHttpActionResult GetPurchseOrder(int id)
        {
            PurchaseOrder wholesaleOrder = db.PurchaseOrders.Find(id);
            if (wholesaleOrder == null)
            {
                return NotFound();
            }

            return Ok(wholesaleOrder);
        }

        /// <summary>
        ///  The housekeeper
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool WholesaleOrderExists(int id)
        {
            return db.PurchaseOrders.Count(e => e.OrderId == id) > 0;
        }
    }
}