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
using System.Threading.Tasks;
using RetailEnterprise.Areas.Accounts.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace RetailEnterprise.Controllers.API
{
    [Authorize]
    public class AccountsController : ApiController
    {
        /*
         * Methods in this class can only be accessed by authorized users.
         */
        private ApplicationDbContext db = new ApplicationDbContext();

        // Following are variables used for I-Framework implementation
        private UserManager<ApplicationUser> UManager;
        private UserStore<ApplicationUser> UStore;

        private RoleManager<IdentityRole> RManager;
        private RoleStore<IdentityRole> RStore;

        public AccountsController()
        {
            UStore = new UserStore<ApplicationUser>(db);
            UManager = new UserManager<ApplicationUser>(UStore);

            RStore = new RoleStore<IdentityRole>(db);
            RManager = new RoleManager<IdentityRole>(RStore);
        }


        /*
         * Get all the accounts data from the local db
         */
        [Authorize]
        public IHttpActionResult GetAccounts()
        {
            var result = (
                from acc in db.Accounts
                select new { 
                Id = acc.Id,
                Address = acc.Address,
                Name = acc.FirstName + " " + acc.LastName,
                PhoneNumber = acc.PhoneNumber2,
                Designation = acc.Designation
                }
                );
            return Ok(result);
        }

        /*
         * Changin the password of a logged in user
         */
        [HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> ChangePassword(UserCredentialsModel usermodel)
        {
            // check if the user provided a valid old password
            ApplicationUser user = await UManager.FindAsync(usermodel.Id, usermodel.OldPassword);
            if (user == null)
            {
                return BadRequest();
            }
            user.PasswordHash = UManager.PasswordHasher.HashPassword(usermodel.Password);
            var result = await UManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return Conflict();
            }
            return Ok();
        }

        /*
         * Get individual account
         */
        [Authorize]
        public IHttpActionResult GetAccount(string id)
        {
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return NotFound();
            }

            return Ok(account);
        }

        /*
         *Updaing the account sent as a model to this method 
         */
        [Authorize]
        public IHttpActionResult EditAccount(Account account)
        {
            // if the model state is not valid, return bad request
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // update the account
            db.Entry(account).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(HttpStatusCode.ExpectationFailed);
            }

            return StatusCode(HttpStatusCode.NoContent);
        }


        /*
         * Only managers can do this
         */
        [Authorize(Roles = "Manager")]
        public async Task<IHttpActionResult> Register(AddAccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            IdentityResult result;
            using (var context = db)
            {
                var roleStore = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(roleStore);

                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                var user = new Account { UserName = model.Email, Email = model.Email, Status = "basic", PhoneNumber2 = model.PhoneNumber2, Designation = model.Designation, Address = model.Address, FirstName = model.FirstName };
                result = await userManager.CreateAsync(user, model.Password);
                await userManager.AddToRoleAsync(user.Id, "Staff");

            }

            if (!result.Succeeded)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            return Ok();
        }
        

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AccountExists(string id)
        {
            return db.Accounts.Count(e => e.Id == id) > 0;
        }
    }
}