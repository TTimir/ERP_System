using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DatabaseAccess;

namespace CloudERP_Model.Controllers
{
    public class tblUserTypesController : Controller
    {
        private CloudErpVEntities db = new CloudErpVEntities();

        // GET: tblUserTypes
        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            return View(db.tblUserTypes.ToList());
        }

        // GET: tblUserTypes/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                if (id == null)
                {
                    return RedirectToAction("EP500", "EP");
                }
                tblUserType tblUserType = db.tblUserTypes.Find(id);
                if (tblUserType == null)
                {
                    return RedirectToAction("EP404", "EP");
                }
                return View(tblUserType);
            }
            catch
            {
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: tblUserTypes/Create
        public ActionResult Create()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            return View();
        }

        // POST: tblUserTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblUserType tblUserType)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            if (ModelState.IsValid)
            {
                db.tblUserTypes.Add(tblUserType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblUserType);
        }

        // GET: tblUserTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                if (id == null)
                {
                    return RedirectToAction("EP500", "EP");
                }
                tblUserType tblUserType = db.tblUserTypes.Find(id);
                if (tblUserType == null)
                {
                    return RedirectToAction("EP404", "EP");
                }
                return View(tblUserType);
            }
            catch
            {
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: tblUserTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblUserType tblUserType)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            if (ModelState.IsValid)
            {
                db.Entry(tblUserType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblUserType);
        }

        // GET: tblUserTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                if (id == null)
                {
                    return RedirectToAction("EP500", "EP");
                }
                tblUserType tblUserType = db.tblUserTypes.Find(id);
                if (tblUserType == null)
                {
                    return RedirectToAction("EP404", "EP");
                }
                return View(tblUserType);
            }
            catch
            {
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: tblUserTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            tblUserType tblUserType = db.tblUserTypes.Find(id);
            db.tblUserTypes.Remove(tblUserType);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
