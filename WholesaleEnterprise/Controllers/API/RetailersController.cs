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

namespace RetailEnterprise.Controllers.API
{
    public class RetailersController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Retailers
        public IQueryable<Retailer> GetRetailers()
        {
            return db.Retailers;
        }

        // GET: api/Retailers/5
        [ResponseType(typeof(Retailer))]
        public IHttpActionResult GetRetailer(string id)
        {
            Retailer retailer = db.Retailers.Find(id);
            if (retailer == null)
            {
                return NotFound();
            }

            return Ok(retailer);
        }

        // PUT: api/Retailers/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutRetailer(string id, Retailer retailer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != retailer.RetailerId)
            {
                return BadRequest();
            }

            db.Entry(retailer).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RetailerExists(id))
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

        // POST: api/Retailers
        [ResponseType(typeof(Retailer))]
        public IHttpActionResult PostRetailer(Retailer retailer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Retailers.Add(retailer);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (RetailerExists(retailer.RetailerId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = retailer.RetailerId }, retailer);
        }

        // DELETE: api/Retailers/5
        [ResponseType(typeof(Retailer))]
        public IHttpActionResult DeleteRetailer(string id)
        {
            Retailer retailer = db.Retailers.Find(id);
            if (retailer == null)
            {
                return NotFound();
            }

            db.Retailers.Remove(retailer);
            db.SaveChanges();

            return Ok(retailer);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RetailerExists(string id)
        {
            return db.Retailers.Count(e => e.RetailerId == id) > 0;
        }
    }
}