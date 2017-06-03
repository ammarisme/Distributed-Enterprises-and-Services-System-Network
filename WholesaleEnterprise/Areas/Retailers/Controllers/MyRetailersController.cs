using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WholesaleEnterprise.Areas.Retailers.Models;
using WholesaleEnterprise.DAL;

namespace WholesaleEnterprise.Areas.Retailers.Controllers
{
    public class MyRetailersController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult AllRetailers()
        {
            AllRetailersViewModels model = new AllRetailersViewModels();
            model.retailers = db.Retailers;

            return View();
        }
    }
}