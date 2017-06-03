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
using Newtonsoft.Json.Linq;

namespace IntegrationSystem.Controllers.API
{
    public class ServicesController : ApiController
    {
        /*
         * There are 2 kinds of API controllers you can find here.
         */
        private ApplicationDbContext db = new ApplicationDbContext();

        // Management Controller: API controllers that provide data operation in the IS database
        public IQueryable<Service> GetServices()
        {
            return db.Services;
        }

        public IHttpActionResult GetEnterprisesOfService(int id)
        {
            IEnumerable<EnterpriseService> enterpriseServices = db.EnterpriseServices.Where(s => s.ServiceId == id);
            var result =
            (
            from es in enterpriseServices
            join enterprises in db.Enterprises on es.EnterpriseId equals enterprises.EnterpriseId
            where es.EnterpriseId == enterprises.EnterpriseId
            select new
            {
                EnterpriseId = enterprises.EnterpriseId,
                EnterpriseTypeId = enterprises.EnterpriseTypeId,
                EnterpriseName = enterprises.EnterpriseName,
                EntepriseAddress = enterprises.EntepriseAddress,
                Rating = enterprises.Rating,
                BusinessPhoneNumber = enterprises.Rating,
                Status = enterprises.Status,
                BRCNumber = enterprises.BRCNumber,
                Category = enterprises.Category,
                Currency = enterprises.Currency,
                Country = enterprises.Country,
                Region = enterprises.Region,
                Uri = enterprises.Uri
            }
            );
            return Ok(result);
        }

        // return services that an account is connected to
        public IHttpActionResult GetAccountServices(string id)
        {
            IEnumerable<AccountService> e_services = db.AccountServices.Where(acc_serv => acc_serv.Id == id);

            var result =
            (
            from acc_service in e_services
            join services in db.Services on acc_service.ServiceId equals services.ServiceId
            where acc_service.ServiceId == services.ServiceId
            select new
            {
                ServiceId = services.ServiceId,
                Type = services.Type,
                Uri = services.Uri,
                Status = services.Status
            }
            );

            return Ok(result);
        }

        //return accounts that are connected to a service
        public IHttpActionResult GetServiceAccounts(int id)
        {
            IEnumerable<AccountService> e_services = db.AccountServices.Where(acc_serv => acc_serv.ServiceId == id);

            var result =
            (
            from acc_service in e_services
            join accounts in db.Accounts on acc_service.Id equals accounts.Id
            where acc_service.Id == accounts.Id
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
        // GET: api/Services/5
        [ResponseType(typeof(Service))]
        public IHttpActionResult GetService(int id)
        {
            Service service = db.Services.Find(id);
            if (service == null)
            {
                return NotFound();
            }

            return Ok(service);
        }

        // POST: api/Services
        [ResponseType(typeof(Service))]
        public HttpResponseMessage AddService(Service service)
        {

            db.Services.Add(service);
            db.SaveChanges();

            return this.Request.CreateResponse(HttpStatusCode.OK, "");
        }


        public HttpResponseMessage UpdateService(Service service)
        {

            Service original = db.Services.Find(service.ServiceId);
            original.Type = service.Type;
            original.Uri = service.Uri;

            db.Entry(original).State = EntityState.Modified;

            db.SaveChanges();

            return this.Request.CreateResponse(HttpStatusCode.OK, "");
        }

        public IHttpActionResult AddServiceAccount(AccountService model)
        {

            db.AccountServices.Add(model);
            var result = db.SaveChanges();

            return StatusCode(HttpStatusCode.Created);
        }

        public IHttpActionResult RemoveServiceAccount(AccountService model)
        {
            if (db.AccountServices.Where(a_serv => a_serv.ServiceId == model.ServiceId && a_serv.Id == model.Id).Count() > 0)
            {
                AccountService acc_serv = db.AccountServices.Where(a_serv => a_serv.ServiceId == model.ServiceId && a_serv.Id == model.Id).First();
                db.AccountServices.Remove(acc_serv);
                db.SaveChanges();
                return Ok();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }


        public bool AddServiceToAccount(AccountService model)
        {

            db.AccountServices.Add(model);
            var result = db.SaveChanges();

            if (result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        // DELETE: api/Services/5
        [ResponseType(typeof(Service))]
        public IHttpActionResult DeleteService(int id)
        {
            Service service = db.Services.Find(id);
            if (service == null)
            {
                return NotFound();
            }

            db.Services.Remove(service);
            db.SaveChanges();

            return Ok(service);
        }

        private bool ServiceExistById(int id)
        {
            return (db.Services.Count(e => e.ServiceId == id) > 0);
        }

        // Communication controllers: API controllers that provide operation functions in connected Enterprise/Service systems
        private HttpWebResponse sendToAllSubscribedServices(JObject jsonBody, string apiRoute)
        {
            int enterpriseId = Int32.Parse(jsonBody["EnterpriseId"].ToString());
            // get all the service urls of the enterprise
            var services = (
                from service_enterprise in db.EnterpriseServices.Where(s => s.EnterpriseId == enterpriseId)
                join service in db.Services on service_enterprise.ServiceId equals service.ServiceId
                where service_enterprise.ServiceId == service.ServiceId
                select new
                {
                    Uri = service.Uri,
                    ID = service.ServiceId
                }
                );

            SystemRouter integrator = new SystemRouter();
            
            HttpWebResponse response = null;
            try
            {
            // update all the services
            foreach (var e_service in services)
            {
                string Uri = e_service.Uri + apiRoute;
                jsonBody.Remove("EnterpriseId");
                response = integrator.postJsonData(jsonBody, Uri);
            }
            return response;
            }catch (Exception ex){
                System.Diagnostics.Trace.WriteLine(ex);
                return null;
            }
        }
        private HttpWebResponse sendToService(JObject jsonBody, string apiRoute)
        {
            int serviceId = Int32.Parse(jsonBody["ServiceId"].ToString());
            // get all the service urls of the enterprise

            SystemRouter integrator = new SystemRouter();
            HttpWebResponse response = null;
            try
            {
                 string route = integrator.getServiceRoute(serviceId) + apiRoute;
                 response = integrator.postJsonData(jsonBody, route);
                 return response;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
                return null;
            }
        }

        
        // to be accessed publicly by enterprises and systems
        public HttpResponseMessage AddProduct(JObject jsonBody)
        {
            sendToAllSubscribedServices(jsonBody, "/api/Products/AddProduct");
            return this.Request.CreateResponse(HttpStatusCode.Created, "sent");
        }
        public HttpResponseMessage UpdateProduct(JObject jsonBody)
        {
            sendToAllSubscribedServices(jsonBody, "/api/Products/UpdateProduct");
            return this.Request.CreateResponse(HttpStatusCode.Created, "sent");
        }

        public HttpResponseMessage ChangeSaleStatus(JObject jsonBody)
        {
            sendToService(jsonBody, "/api/Orders/ChangeOrderStatus");
            return this.Request.CreateResponse(HttpStatusCode.Created, "sent");
        }
        
        public HttpResponseMessage AddStocks(JObject products)
        {
            sendToAllSubscribedServices(products, "/api/Stocks/AddStocks");
            return this.Request.CreateResponse(HttpStatusCode.Created, "sent");
        }
        public HttpResponseMessage DeductStock(JObject products)
        {
            HttpWebResponse response = sendToAllSubscribedServices(products, "/api/Stocks/DeductStock");
            if (response == null)
            {
                return this.Request.CreateResponse(HttpStatusCode.Created, "No services to update");
            }
            else
            {
                return this.Request.CreateResponse(HttpStatusCode.Created, "At least one service updated");
            }
        }

        // housekeeping, dispose all the iDisposable properties
        protected override void Dispose(bool disposenow)
        {
            if (disposenow)
            {
                db.Dispose();
            }
            base.Dispose(disposenow);
        }
    }
}