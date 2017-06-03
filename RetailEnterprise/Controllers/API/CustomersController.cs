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
using RetailEnterprise.Areas.Customers.Models;
using System.Data.Entity.Validation;
using System.Diagnostics;
using RetaiEnterprise.Controllers.API;

namespace RetailEnterprise.Controllers.API
{
    public class CustomersController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /*Get all customers from db*/
        [Authorize]
        public IQueryable<Customer> GetCustomers()
        {
            return db.Customers;
        }

        /*
         * get individual cx from db
         */
        [Authorize]
        public IHttpActionResult GetCustomer(string id)
        {
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }
        
       
        
        /// <summary>
        /// adding a local customer to db. this customer record is only unique to the local db.
        /// to share the customer information globally.. the UniversalId attribute must be useed
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        [Authorize]
        public IHttpActionResult AddCustomer(AddCustomerViewModel customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Customer dbCustomer = new Customer();
            dbCustomer.FirstName = customer.FirstName;
            dbCustomer.LastName = customer.LastName;
            dbCustomer.BillingAddress = customer.BillingAddress;
            dbCustomer.City = customer.City;
            dbCustomer.Status = "Active";
            dbCustomer.PhoneNumber = customer.PhoneNumber;
            dbCustomer.Remark = customer.Remark;

            if (customer.UniversalId != null)
            {
                Integrator integrator = new Integrator();
                var response = integrator.getResponse("/api/Accounts/AccountExists/"+customer.UniversalId);
                if (response == null || response.StatusCode != null)
                {
                    // universal id doesn't exist, reset the universal id
                    dbCustomer.UniversalId = "";
                }
                else
                {
                    dbCustomer.UniversalId = customer.UniversalId;
                }
            }

            db.Customers.Add(dbCustomer);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                return StatusCode(HttpStatusCode.NoContent);
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

            return CreatedAtRoute("DefaultApi", new { id = dbCustomer.CustomerId }, customer);
        }

        /// <summary>
        /// housekeeping method
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

        /// <summary>
        ///  check if a customer exist by name
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        private bool CustomerExists(int id)
        {
            return db.Customers.Count(e => e.CustomerId == id) > 0;
        }
    }
}