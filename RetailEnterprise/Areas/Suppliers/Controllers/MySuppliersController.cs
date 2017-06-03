using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RetailEnterprise.Areas.Suppliers.Models;
using RetailEnterprise.DAL;

namespace RetailEnterprise.Areas.Suppliers.Controllers
{
    public class MySuppliersController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult AllSuppliers()
        {
            AllSuppliersViewModels model = new AllSuppliersViewModels();
            model.suppliers = db.Suppliers;

            return View(model);
        }
    }
}