using System.Web.Mvc;

namespace WholesaleEnterprise.Areas.Accounts
{
    public class AccountsAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Accounts";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Accounts_default",
                "Accounts/{controller}/{action}/{id}",
                defaults: new { controller = "MyAccount", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}