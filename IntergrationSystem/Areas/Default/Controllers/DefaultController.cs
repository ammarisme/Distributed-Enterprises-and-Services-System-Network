using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IntegrationSystem.Areas.Default.Controllers
{
    public class DefaultController : Controller
    {
        // GET: Default/Default
        public ActionResult Index()
        {
            return RedirectToAction("/Accounts/Account/Login");
        }

        // GET: Default/Default/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Default/Default/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Default/Default/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Default/Default/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Default/Default/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Default/Default/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Default/Default/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
