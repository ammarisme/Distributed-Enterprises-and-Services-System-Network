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
using RetailEnterprise.Models;
using RetailEnterprise.DAL;
using Newtonsoft.Json.Linq;
using Microsoft.AspNet.Identity;
using System.IO;
using System.Web.Hosting;
using RetaiEnterprise.Controllers.API;
using Newtonsoft.Json;
namespace RetailEnterprise.Controllers.API
{
    /// <summary>
    /// does CRUD operations on Product data
    /// has 2 kinds of Controllers.
    /// public controllers which can be accessed by the IS
    /// public authorized controllers only accesible by authorized users
    /// </summary>
    [Authorize]
    public class ProductsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ProductsController() {

        }

        /// <summary>
        // can be done from the IS
        /// </summary>
        /// <returns>IEnumerable<Product> all the products</returns>
        [AllowAnonymous]
        public IQueryable<Product> GetProducts()
        {
            return db.Products;
        }


        /// <summary>
        /// Get all specifications of a product
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IQueryable<SpecificationInProduct> GetSpecificationsInProduct(int id)
        {
            IQueryable<SpecificationInProduct> productSpecifications = db.SpecificationInProduct.Where(m => m.ProductId== id);
            return productSpecifications;
        }

        /// <summary>
        /// Adding a  product to the ES
        /// after adding to the ES, we will send this to the IS
        /// </summary>
        /// <param name="jsonBody">
        /// A jason object,
        /// <example>
        /// {
        /// Product data, , , , 
        /// SpecificationInProduct : { {},{},{}}
        /// </example>
        /// </param>
        /// <returns></returns>
        [HttpPost]
        //[Authorize]
        public HttpResponseMessage AddProduct(JObject jsonBody)
        {
            JObject originalProduct = jsonBody; // saving this to send to IS
            // deserializing the product specifics
            JObject specifications = (JObject)jsonBody["SpecificationsInProduct"]; // this variable must be present in the javascript

            jsonBody.Remove("SpecificationsInProduct");

            // deserializing the product
            Product product= jsonBody.ToObject<Product>(); // the job card object\

            db.Products.Add(product);

            db.SaveChanges(); // save the shit

            int productId = product.ProductId; // the foregin key to be used for the -> proudcts
            try
            {

            JEnumerable<JToken> tokens = (JEnumerable<JToken>)specifications.Children<JToken>();
            
            // addin the product one by one
            foreach (JToken token in tokens)
            {
                    JToken specificationJson = token.Children().First();
                    SpecificationInProduct specificationInstance = specificationJson.ToObject<SpecificationInProduct>();
                    specificationInstance.ProductId = productId;
                    db.SpecificationInProduct.Add(specificationInstance);

            }

            db.SaveChanges();

            }catch (NullReferenceException ex){
                Console.Write(ex);
                return this.Request.CreateResponse(HttpStatusCode.BadRequest, ex.ToString());
            }
            // product images are saved in a temporay folder, rename it to the product name
            try
            {
                var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\products", HostingEnvironment.MapPath(@"\")));

                string newPath = System.IO.Path.Combine(originalDirectory.ToString(), ""+productId+"");
                string oldPath = System.IO.Path.Combine(originalDirectory.ToString(), "temporary");

             
                Directory.Move(oldPath,newPath);

            }catch(DirectoryNotFoundException ex){
                System.Diagnostics.Debug.Write(ex.Data);
            }

            try
            {
            Integrator integrator = new Integrator();

            // process the original data to send to IS
            Setting setting = db.Settings.Find(1);

            originalProduct.Add("EnterpriseId", setting.SystemIdNumber);
            originalProduct.Add("Price", product.RetailPrice);

            HttpWebResponse response =  integrator.sendJsonObject(originalProduct, "/api/Services/AddProduct");
            if (response == null || response.StatusCode != HttpStatusCode.Conflict)
            {
                return this.Request.CreateResponse(HttpStatusCode.Created, originalProduct);
            }
            }catch(WebException ex){
                return this.Request.CreateResponse(HttpStatusCode.Conflict, ex);
            }
            return this.Request.CreateResponse(HttpStatusCode.NotModified, originalProduct);
        }


        /// <summary>
        /// Update product information in ES and IS
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public HttpResponseMessage UpdateProduct(Product product)
        {
            if (!ProductExists(product.ProductId))
            {
                // Product doesn't exist
                return this.Request.CreateResponse(HttpStatusCode.BadRequest, "Product doesn't exist");
            }
            // else
            // get the product
            Product original = db.Products.Find(product.ProductId);

            original.ProductName = product.ProductName;
            original.RetailPrice = product.RetailPrice;
            original.Unit = product.Unit;
            original.ShortDescription = product.ShortDescription;

            db.Entry(original).State = EntityState.Modified;

            db.SaveChanges();

            Integrator integrator = new Integrator();

            // process the original data to send to IS
            try
            {

            Setting setting = db.Settings.Find(1);

            JObject updatedProduct = JObject.FromObject(original);
            updatedProduct.Add("EnterpriseId", setting.SystemIdNumber);
            updatedProduct.Add("Price", product.RetailPrice);


            HttpWebResponse response = integrator.sendJsonObject(updatedProduct, "/api/Services/UpdateProduct");
            }catch(WebException ex){
                System.Diagnostics.Trace.Write(ex);
                return this.Request.CreateResponse(HttpStatusCode.Conflict,"IS error");
            }

            return this.Request.CreateResponse(HttpStatusCode.OK, original);
        }
        
        /// <summary>
        ///Updating a selected specification in product
        /// </summary>
        [HttpPost]
        [Authorize]
        public IHttpActionResult UpdateProductSpecifications(SpecificationInProduct specification)
        {
            if (!SpecificationInProductExist(specification.SpecificationInProductId))
            {
                return StatusCode(HttpStatusCode.NotModified);
            }
            // else
            SpecificationInProduct original = db.SpecificationInProduct.Find(specification.SpecificationInProductId);

            original.Specification = specification.Specification;
            original.Value = specification.Value;
            db.Entry(original).State = EntityState.Modified;

            db.SaveChanges(); 

            return StatusCode(HttpStatusCode.Created);
        }
            
        /// <summary>
        /// Get a product information using it id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IHttpActionResult GetProduct(int id)
        {
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        /// <summary>
        /// To dispose iDisposable objects
        /// </summary>
        /// <param name="disposeNow"></param>
        protected override void Dispose(bool disposeNow)
        {
            if (disposeNow)
            {
                db.Dispose();
            }
            base.Dispose(disposeNow);
        }

        private bool ProductExists(int id)
        {
            return db.Products.Count(e => e.ProductId == id) > 0;
        }

        [HttpGet]
        public  IHttpActionResult ProductExistsByName(string name)
        {
            if (db.Products.Count(e => e.ProductName == name) > 0) {
                return StatusCode(HttpStatusCode.OK);
            }
            else
            {
                return StatusCode(HttpStatusCode.NotFound);
            }
                ;
        }

        [HttpGet]
        public  bool SpecificationInProductExist(int id)
        {
            return db.SpecificationInProduct.Count(e => e.SpecificationInProductId== id) > 0;
        }
    }
}