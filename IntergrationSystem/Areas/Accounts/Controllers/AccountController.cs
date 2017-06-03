using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using IntegrationSystem.Models;
using IntegrationSystem.Areas.Accounts.Models;
using IntegrationSystem.DAL;
using IntegrationSystem.Controllers.API;


namespace IntegrationSystem.Areas.Accounts.Controllers
{

    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        private UserManager<ApplicationUser> UManager;
        private UserStore<ApplicationUser> UStore;

        private RoleManager<IdentityRole> RManager;
        private RoleStore<IdentityRole> RStore;

        private ApplicationDbContext UserDbContext;

        private ApplicationSignInManager SInManager;


        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UManager = userManager;
            SInManager = signInManager;
        }

        

        public AccountController()
        {
            

            UStore = new UserStore<ApplicationUser>(db);
            UManager = new UserManager<ApplicationUser>(UStore);

            RStore = new RoleStore<IdentityRole>(db);
            RManager = new RoleManager<IdentityRole>(RStore);

            //SInManager = new SignInManager<ApplicationUser>;
        }

        // GET controllers
        [AllowAnonymous]
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Dashboard");
            }
            else
            {
                return RedirectToAction("Login", "Account", new { area = "Accounts" });
            }
        }

        /*
         *@purpose - Show the Account information of the logged in user.
         *User can also edit the profile and save.
         */

        public ActionResult ManageMyAccount()
        {
            string userId = User.Identity.GetUserId();
            ApplicationUser user = db.Users.Find(userId);
            userId = user.Id;
            Account account = db.Accounts.Find(userId);
            ManageAccountViewModel model = new ManageAccountViewModel { Id = account.Id, Email = account.Email, FirstName = account.FirstName, LastName = account.LastName, PhoneNumber2 = account.PhoneNumber2, Designation = account.Designation, Address = account.Address, Status = account.Status };

            ViewBag.id = userId;
            return View(model);
        }

        /*
         * @purpose - A logged in user can change his own password
         */
        [Authorize]
        public ActionResult ChangePassword()
        {
            ViewBag.Id = User.Identity.GetUserId();
            return View();
        }
        /*
         * @purpose - SHow view to Create a new account with staff priviledges
         */
        [Authorize]
        public ActionResult AddAccount() {
            AddAccountViewModel model = new AddAccountViewModel();
            model.Id = db.Roles;
            return View(model);
        }
        /*
         * @purpose -  View all account information
         */
        public ActionResult AllAccounts() {
            return View();
        }

        // GET : a view to enter reg information
        [AllowAnonymous]
        public ActionResult Register()
        {
            AddAccountViewModel model = new AddAccountViewModel();
            
            // all roles except administrator
            IEnumerable<IdentityRole> roles = db.Roles.Where(r => r.Name != "Administrator");

            model.Id = roles;
            return View(model);
        }

        /*
         *@purpose - User can view all other staff accounts and edit them 
         */
        public ActionResult ManageAccounts() {
            return View();
        }

        // show the login view
        [AllowAnonymous]
        public ActionResult LogIn(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Dashboard");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }


        [Authorize]
        public ActionResult Dashboard()
        {
            // go through all the services and send get request to them
            // if you get a response.. set status to up
            // if not response set status to down
            // these data will be going into the  view in a ienumerable format and it will show a graph based on that


            DashboardViewModel model = new DashboardViewModel();

            if (User.IsInRole("Administrator"))
            {
                model.ServiceStatuses = GetAllServicesWithStatus();
                model.EnterpriseStatuses = GetAllEnterpriseStatuses();
            }
            else if (User.IsInRole("EnterpriseUser"))
            {
                model.ServiceStatuses = GetMyServicesWithStatus();
                model.EnterpriseStatuses = GetMyEnterprisesWithStatus();
            }
            else if (User.IsInRole("Customer"))
            {
                model.ServiceStatuses = GetMyServicesWithStatus();
            }

            return View(model);
        }

        public List<Service> GetAllServicesWithStatus()
        {
            List<Service> ServiceStatuses = new List<Service>();
            
            SystemRouter integrator = new SystemRouter();

            // get statuses from all the services
            var services = db.Services;
            foreach (var e_service in services)
            {
                string Uri = e_service.Uri + "/api/System/GetStatus";
                var response = integrator.getResponse(Uri);
                if (response == null)
                {
                    // system is up
                    e_service.Status = false;
                    ServiceStatuses.Add(e_service);
                }
                else
                {
                    // system is down
                    e_service.Status = true;
                    ServiceStatuses.Add(e_service);
                }
            }

            return ServiceStatuses;
        }

        public List<Enterprise> GetAllEnterpriseStatuses()
        {
            List<Enterprise> EnterpriseStatuses = new List<Enterprise>();

            SystemRouter integrator = new SystemRouter();

            // get statuses from all the enterprises
            var enterprises = db.Enterprises;

            foreach (var enterprise in enterprises)
            {
                var response = integrator.getResponse(enterprise.Uri + "/api/System/GetStatus");
                if (response == null)
                {
                    // system is down
                    enterprise.Status = false;
                    EnterpriseStatuses.Add(enterprise);
                }
                else
                {
                    // system is up
                    enterprise.Status = true;
                    EnterpriseStatuses.Add(enterprise);
                }
            }

            return EnterpriseStatuses;
        }

        public dynamic GetMyServicesWithStatus()
        {
            string accountId = User.Identity.GetUserId();
            
            SystemRouter integrator = new SystemRouter();

            // get all of my services
            var serviceQuery = (
                from acc_service in db.AccountServices.Where(s => s.Id == accountId)
                join service in db.Services on acc_service.ServiceId equals service.ServiceId
                where acc_service.ServiceId == service.ServiceId
                select service
                );

            List<Service> myServices = serviceQuery.ToList<Service>();
            List<Service> modifiedServices = new List<Service>();
            // go and test all the services.. 
            foreach (var e_service in myServices)
            {
                string Uri = e_service.Uri + "/api/System/GetStatus";
                var response = integrator.getResponse(Uri);
                if (response == null)
                {
                    // system is up
                    e_service.Status = false;
                    modifiedServices.Add(e_service);
                }
                else
                {
                    // system is down
                    e_service.Status = true;
                    modifiedServices.Add(e_service);
                }
            }

            return modifiedServices;
        }

        public dynamic GetMyEnterprisesWithStatus()
        {
            string accountId = User.Identity.GetUserId();

            SystemRouter integrator = new SystemRouter();

            // get all of my services
            var enterpriseQuery = (
                from acc_enterprise in db.EnterpriseAccounts.Where(e => e.Id == accountId)
                join enterprise in db.Enterprises on acc_enterprise.EnterpriseId equals enterprise.EnterpriseId
                where acc_enterprise.EnterpriseId == enterprise.EnterpriseId
                select enterprise
                );

            List<Enterprise> myEnterprises = enterpriseQuery.ToList<Enterprise>();
            List<Enterprise> modifiedEnterprise = new List<Enterprise>();

            // go and test all the services.. 
            foreach (var a_enterprise in myEnterprises)
            {
                string Uri = a_enterprise.Uri + "/api/System/GetStatus";
                var response = integrator.getResponse(Uri);
                if (response == null)
                {
                    // system is up
                    a_enterprise.Status = false;
                    modifiedEnterprise.Add(a_enterprise);
                }
                else
                {
                    // system is down
                    a_enterprise.Status = true;
                    modifiedEnterprise.Add(a_enterprise);
                }
            }

            return modifiedEnterprise;
        }

        public ActionResult ManageAccountServices()
        {
            return View();
        }

        public ActionResult ManageAccountEnterprises()
        {
            return View();
        }
        // POST FUNCTIONS

        // this method can be used by any user to register to the system. 
        // retailer and wholesaler user roles needs to be confirmed by an Administrator
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Register(RegisterAccountDataModel model)
        {

                // create a Account
                var user = new Account { UserName = model.Email, Email = model.Email, Status = "registered", PhoneNumber2 = model.PhoneNumber2, Designation = model.Designation, Address = model.Address, FirstName = model.FirstName };
                var result = await UManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // add the user to the role in the model

                    IdentityRole UserRole = db.Roles.Find(model.Id);

                    IdentityResult addedToRole = await UManager.AddToRoleAsync(user.Id, UserRole.Name);

                    // create an instance of the API controller
                    IntegrationSystem.Controllers.API.AccountsController accController = new IntegrationSystem.Controllers.API.AccountsController();
                    if (addedToRole.Succeeded)
                    {
                        List<AccountService> accServices = new List<AccountService>();

                        switch (UserRole.Name)
                        {
                            // if the user is in customer role, add RetailTrading Portal  Account/Service
                            case "Customer ":
                                accServices.Add(new AccountService { ServiceId = 1, Id = user.Id });
                                break;
                            // if the user is in Enterprise user, add Wholesale Trading Portal and Retail Trading Portal too Account/Service
                            case "EnterpriseUser":
                                accServices.Add(new AccountService { ServiceId = 1, Id = user.Id });
                                accServices.Add(new AccountService { ServiceId = 2, Id = user.Id });
                                break;
                            default:
                                break;
                        }
                        // add all the services to the account
                        foreach (AccountService acc_service in accServices)
                        {
                            db.AccountServices.Add(acc_service);
                        }

                        db.SaveChanges();

                        return RedirectToAction("Login");
                    }


                }
            return RedirectToAction("Register");
        }

        

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
        if (ModelState.IsValid)
            {
                var user = await UManager.FindAsync(model.Email, model.Password);
                if (user != null)
                {
                        await SignInAsync(user, model.RememberMe);
                        return RedirectToAction("Dashboard");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid Account.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            // Add more custom claims here if you want. Eg HomeTown can be a claim for the User
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        // Process edit request for profile
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Edit(ManageAccountViewModel model)
        {

            if (ModelState.IsValid)
            {
                Account account = db.Accounts.Find(User.Identity.GetUserId());

                if (account!= null)
                {
                  // set retailer object values
                    // create a new retailer object
                    db.Entry(account).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                }
            }
            return View("EditProfile",model);
        }

        
        //---------------- Whatever below is still not tested and integrated---------------



        //public ApplicationSignInManager SignInManager
        //{
        //    get
        //    {
        //        return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
        //    }
        //    private set
        //    {
        //        _signInManager = value;
        //    }
        //}

        //public ApplicationUserManager UserManager
        //{
        //    get
        //    {
        //        return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
        //    }
        //    private set
        //    {
        //        _userManager = value;
        //    }
        //}
        //-------------------------------------------------------------------------

        ////
        //// GET: /Account/VerifyCode
        //[AllowAnonymous]
        //public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        //{
        //    // Require that the user has already logged in via username/password or external login
        //    if (!await SInManager.HasBeenVerifiedAsync())
        //    {
        //        return View("Error");
        //    }
        //    return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        //}

        ////
        //// POST: /Account/VerifyCode
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    // The following code protects for brute force attacks against the two factor codes. 
        //    // If a user enters incorrect codes for a specified amount of time then the user account 
        //    // will be locked out for a specified amount of time. 
        //    // You can configure the account lockout settings in IdentityConfig
        //    var result = await SInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
        //    switch (result)
        //    {
        //        case SignInStatus.Success:
        //            return RedirectToLocal(model.ReturnUrl);
        //        case SignInStatus.LockedOut:
        //            return View("Lockout");
        //        case SignInStatus.Failure:
        //        default:
        //            ModelState.AddModelError("", "Invalid code.");
        //            return View(model);
        //    }
        //}

        ////
        //// GET: /Account/ConfirmEmail
        //[AllowAnonymous]
        //public async Task<ActionResult> ConfirmEmail(string userId, string code)
        //{
        //    if (userId == null || code == null)
        //    {
        //        return View("Error");
        //    }
        //    var result = await UManager.ConfirmEmailAsync(userId, code);
        //    return View(result.Succeeded ? "ConfirmEmail" : "Error");
        //}

        ////
        //// GET: /Account/ForgotPassword
        //[AllowAnonymous]
        //public ActionResult ForgotPassword()
        //{
        //    return View();
        //}

        ////
        //// POST: /Account/ForgotPassword
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await UManager.FindByNameAsync(model.Email);
        //        if (user == null || !(await UManager.IsEmailConfirmedAsync(user.Id)))
        //        {
        //            // Don't reveal that the user does not exist or is not confirmed
        //            return View("ForgotPasswordConfirmation");
        //        }

        //        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
        //        // Send an email with this link
        //        // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
        //        // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
        //        // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
        //        // return RedirectToAction("ForgotPasswordConfirmation", "Account");
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        ////
        //// POST: /Account/ResetPassword
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    var user = await UManager.FindByNameAsync(model.Email);
        //    if (user == null)
        //    {
        //        // Don't reveal that the user does not exist
        //        return RedirectToAction("ResetPasswordConfirmation", "Account");
        //    }
        //    var result = await UManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
        //    if (result.Succeeded)
        //    {
        //        return RedirectToAction("ResetPasswordConfirmation", "Account");
        //    }
        //    AddErrors(result);
        //    return View();
        //}

        ////
        //// GET: /Account/ResetPasswordConfirmation
        //[AllowAnonymous]
        //public ActionResult ResetPasswordConfirmation()
        //{
        //    return View();
        //}

        ////
        //// POST: /Account/ExternalLogin
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult ExternalLogin(string provider, string returnUrl)
        //{
        //    // Request a redirect to the external login provider
        //    return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        //}

        ////
        //// GET: /Account/SendCode
        //[AllowAnonymous]
        //public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        //{
        //    var userId = await SInManager.GetVerifiedUserIdAsync();
        //    if (userId == null)
        //    {
        //        return View("Error");
        //    }
        //    var userFactors = await UManager.GetValidTwoFactorProvidersAsync(userId);
        //    var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
        //    return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        //}

        //////
        ////// POST: /Account/SendCode
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> SendCode(SendCodeViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View();
        //    }

        //    // Generate the token and send it
        //    if (!await SInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
        //    {
        //        return View("Error");
        //    }
        //    return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        //}

        //
        //// GET: /Account/ExternalLoginCallback
        //[AllowAnonymous]
        //public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        //{
        //    var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
        //    if (loginInfo == null)
        //    {
        //        return RedirectToAction("Login");
        //    }

        //    // Sign in the user with this external login provider if the user already has a login
        //    var result = await SInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
        //    switch (result)
        //    {
        //        case SignInStatus.Success:
        //            return RedirectToLocal(returnUrl);
        //        case SignInStatus.LockedOut:
        //            return View("Lockout");
        //        case SignInStatus.RequiresVerification:
        //            return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
        //        case SignInStatus.Failure:
        //        default:
        //            // If the user does not have an account, then prompt the user to create an account
        //            ViewBag.ReturnUrl = returnUrl;
        //            ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
        //            return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
        //    }
        //}

        //
        // POST: /Account/ExternalLoginConfirmation
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        return RedirectToAction("Index", "Manage");
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        // Get the information about the user from the external login provider
        //        var info = await AuthenticationManager.GetExternalLoginInfoAsync();
        //        if (info == null)
        //        {
        //            return View("ExternalLoginFailure");
        //        }
        //        var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
        //        var result = await UManager.CreateAsync(user);
        //        if (result.Succeeded)
        //        {
        //            result = await UManager.AddLoginAsync(user.Id, info.Login);
        //            if (result.Succeeded)
        //            {
        //                await SInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //                return RedirectToLocal(returnUrl);
        //            }
        //        }
        //        AddErrors(result);
        //    }

        //    ViewBag.ReturnUrl = returnUrl;
        //    return View(model);
        //}

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account", new { area = "Accounts" });
//            return RedirectToAction("Login", "Account");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (UManager != null)
                {
                    UManager.Dispose();
                    UManager = null;
                }

                if (SInManager != null)
                {
                    SInManager.Dispose();
                    SInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Login", "Account", new { area = "Accounts" });
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}