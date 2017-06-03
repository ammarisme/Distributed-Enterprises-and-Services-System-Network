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
using IntegrationSystem.DAL;
using IntegrationSystem.Models;
using IntegrationSystem.Areas.Enterprises.Models;
using System.Data.Entity.Validation;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.Text;
using System.IO;

namespace IntegrationSystem.Controllers.API
{
    [Authorize]
    public class EnterprisesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private SystemRouter router = new SystemRouter();

        // Management Controllers : Require authorization
        public IHttpActionResult GetServicesOfEnterprise(int id)
        {
            var result = (
                from es in db.EnterpriseServices.Where(s => s.EnterpriseId == id)
                join services in db.Services on es.ServiceId equals services.ServiceId
                where es.ServiceId == services.ServiceId
                select new
                {
                    ServiceId = services.ServiceId,
                    Type = services.Type,
                    Status = services.Status
                }
                );

            return Ok(result);
        }
        public IQueryable<Enterprise> GetEnterprises()
        {
            return db.Enterprises;
        }
        public IHttpActionResult GetEnterpriseAccounts(int id)
        {
            IEnumerable<EnterpriseAccount> ea = db.EnterpriseAccounts.Where(es => es.EnterpriseId == id);
            var result = (
                from e_accounts in ea
                join accounts in db.Accounts on e_accounts.Id equals accounts.Id
                where e_accounts.Id == accounts.Id
                select new
                {
                    Id = accounts.Id,
                    Name = accounts.FirstName + " " + accounts.LastName,
                    Address = accounts.Address,
                    PhoneNumber = accounts.PhoneNumber2,
                    Status = accounts.Status,
                    Designation = accounts.Designation
                }
                );

            return Ok(result);

        }
        public HttpResponseMessage RemoveEnterpriseService(EnterpriseService enterpriseService)
        {
            if (!EnterpriseExistsById(enterpriseService.EnterpriseId) || (db.Services.Where(s => enterpriseService.ServiceId == s.ServiceId).Count() < 0)
                || (db.EnterpriseServices.Where(es => es.EnterpriseId == enterpriseService.EnterpriseId && es.ServiceId == enterpriseService.ServiceId).Count() < 0))
            {
                return this.Request.CreateResponse(HttpStatusCode.Conflict, "");
            }
            else
            {
                EnterpriseService toBeDeleted = db.EnterpriseServices.Where(s => s.ServiceId == enterpriseService.ServiceId && s.EnterpriseId == enterpriseService.EnterpriseId).Single<EnterpriseService>();
                db.Entry(toBeDeleted).State = EntityState.Deleted;
                db.SaveChanges();
                return this.Request.CreateResponse(HttpStatusCode.OK, "");
            }
        }
        public HttpResponseMessage AddEnterpriseService(EnterpriseService enterpriseService)
        {
            if (!EnterpriseExistsById(enterpriseService.EnterpriseId) || (db.Services.Count(s => s.ServiceId == enterpriseService.ServiceId) < 0)
                || (db.EnterpriseServices.Where(es => es.EnterpriseId == enterpriseService.EnterpriseId && es.ServiceId == enterpriseService.ServiceId).Count() > 0)
                )
            {
                return this.Request.CreateResponse(HttpStatusCode.Conflict, "");
            }
            else
            {

                db.EnterpriseServices.Add(enterpriseService);
                db.SaveChanges();
                return this.Request.CreateResponse(HttpStatusCode.OK, "");
            }
        }
        public IHttpActionResult GetEnterprise(int id)
        {
            Enterprise enterprise = db.Enterprises.Find(id);
            if (enterprise == null)
            {
                return NotFound();
            }

            return Ok(enterprise);
        }
        public IHttpActionResult AddEnterprise(Enterprise enterprise)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Enterprises.Add(enterprise);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (EnterpriseExists(enterprise.EnterpriseName))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                    }
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = enterprise.EnterpriseId }, enterprise);
        }
        public IHttpActionResult DeleteEnterprise(int id)
        {
            Enterprise enterprise = db.Enterprises.Find(id);
            if (enterprise == null)
            {
                return NotFound();
            }

            db.Enterprises.Remove(enterprise);
            db.SaveChanges();

            return Ok(enterprise);
        }
        [HttpPost]
        public IHttpActionResult UpdateEnterprise(Enterprise enterprise)
        {
            Enterprise enterpriseOriginal = db.Enterprises.Find(enterprise.EnterpriseId);
            enterpriseOriginal.EnterpriseName = enterprise.EnterpriseName;
            enterpriseOriginal.EntepriseAddress = enterprise.EntepriseAddress;
            enterpriseOriginal.BusinessPhoneNumber = enterprise.BusinessPhoneNumber;
            enterpriseOriginal.Status = enterprise.Status;
            enterpriseOriginal.BRCNumber = enterprise.BRCNumber;
            enterpriseOriginal.Category = enterprise.Category;
            enterpriseOriginal.Currency = enterprise.Currency;
            enterpriseOriginal.Country = enterprise.Country;
            enterpriseOriginal.Region = enterprise.Region;
            enterpriseOriginal.Uri = enterprise.Uri;

            db.Entry(enterpriseOriginal).State = EntityState.Modified;

            db.SaveChanges();
            return StatusCode(HttpStatusCode.Created);
        }
        public HttpResponseMessage AddEnterpriseAccount(EnterpriseAccount enterpriseAccount)
        {
            if (!EnterpriseExistsById(enterpriseAccount.EnterpriseId) || (db.Enterprises.Count(s => s.EnterpriseId == enterpriseAccount.EnterpriseId) < 0)
                || (db.EnterpriseAccounts.Where(es => es.EnterpriseId == enterpriseAccount.EnterpriseId && es.Id == enterpriseAccount.Id).Count() > 0)
                )
            {
                return this.Request.CreateResponse(HttpStatusCode.Conflict, "");
            }
            else
            {

                db.EnterpriseAccounts.Add(enterpriseAccount);
                db.SaveChanges();
                return this.Request.CreateResponse(HttpStatusCode.OK, "");
            }
        }
        public IHttpActionResult RemoveEnterpriseAccount(EnterpriseAccount enterpriseAccount)
        {
            if (!EnterpriseExistsById(enterpriseAccount.EnterpriseId) || (db.Enterprises.Count(s => s.EnterpriseId == enterpriseAccount.EnterpriseId) < 0)
                || (db.EnterpriseAccounts.Where(es => es.EnterpriseId == enterpriseAccount.EnterpriseId && es.Id == enterpriseAccount.Id).Count() > 0)
                )
            {
                EnterpriseAccount removeThis = db.EnterpriseAccounts.Where(es => es.EnterpriseId == enterpriseAccount.EnterpriseId && es.Id == enterpriseAccount.Id).First();
                db.EnterpriseAccounts.Remove(removeThis);
                db.SaveChanges();
                return Ok();
            }
            else
            {
                return StatusCode(HttpStatusCode.NoContent);
            }
        }
        // Management and communication, hybrid controllers 
        [AllowAnonymous]
        private bool EnterpriseExists(string name)
        {
            return db.Enterprises.Count(e => e.EnterpriseName == name) > 0;
        }
        [AllowAnonymous]
        private bool EnterpriseExistsById(int id)
        {
            return db.Enterprises.Count(e => e.EnterpriseId == id) > 0;
        }

        
        // Communication controllers : used for enterprise communications, public API
        [AllowAnonymous]
        private HttpWebResponse sendToEnterprise(JObject jsonBody, string apiRoute)
        {
            // get routing information to send the data
            int enterpriseId = Int32.Parse(jsonBody["EnterpriseId"].ToString());

            string enterpriseUrl = router.getEnterpriseRoute(enterpriseId);

            // send data to enterprise
            // add service id to the payload
            HttpWebResponse  response = router.postJsonData(jsonBody, enterpriseUrl+apiRoute);
            return response;
        }
        [AllowAnonymous]
        public HttpResponseMessage AddRetailOrder(JObject order)
        {
            HttpWebResponse response = sendToEnterprise(order, "/api/Sales/AddOrder");
            if (response == null)
            {
                return this.Request.CreateResponse(HttpStatusCode.Conflict, "Unknown Transaction failure");
            }
            else if(response.StatusCode != HttpStatusCode.Conflict)
            {
                return this.Request.CreateResponse(HttpStatusCode.OK, response);
            }
            else
            {
                return this.Request.CreateResponse(HttpStatusCode.Conflict, response);
            }

        }
        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage PlaceOrderToEnterprise(JObject jsonBody)
        {
            // get the enterprise Id
            int enterpriseId;
            if (jsonBody["RetailerId"] != null)
            {
                enterpriseId = jsonBody["RetailerId"].Value<int>();
            }
            else
            {
                enterpriseId = jsonBody["WholesalerId"].Value<int>();
            }

            //get the url of the enterprise
            string enterpriseUrl = db.Enterprises.Find(enterpriseId).Uri;
            // customize the URI to reach the api controller to add the order
            enterpriseUrl = enterpriseUrl + "/api/Sales/AddOrder";

            var http = (HttpWebRequest)WebRequest.Create(new Uri(enterpriseUrl));
            http.Accept = "application/json";
            http.ContentType = "application/json";
            http.Method = "POST";

            string parsedContent = jsonBody.ToString();
            ASCIIEncoding encoding = new ASCIIEncoding();
            Byte[] bytes = encoding.GetBytes(parsedContent);

            Stream newStream = http.GetRequestStream();
            newStream.Write(bytes, 0, bytes.Length);
            newStream.Close();

            HttpWebResponse response = (HttpWebResponse)http.GetResponse();

            return this.Request.CreateResponse(response.StatusCode, response);
        }

        // housekeeping . dispose iDisposables
        protected override void Dispose(bool disposnow)
        {
            if (disposnow)
            {
                db.Dispose();
            }
            base.Dispose(disposnow);
        }
    }
}