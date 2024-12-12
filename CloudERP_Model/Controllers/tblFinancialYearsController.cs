using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using DatabaseAccess;

namespace CloudERP_Model.Controllers
{
    public class tblFinancialYearsController : Controller
    {
        private CloudErpVEntities db = new CloudErpVEntities();

        // GET: tblFinancialYears
        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int userid = 0;
            userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            var tblFinancialYears = db.tblFinancialYears.Include(t => t.tblUser);
            return View(tblFinancialYears.ToList());
        }

        // GET: tblFinancialYears/Create
        public ActionResult Create()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int userid = 0;
            userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            return View();
        }

        // POST: tblFinancialYears/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblFinancialYear tblFinancialYear)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int userid = 0;
            userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            tblFinancialYear.UserID = userid;
            if (ModelState.IsValid)
            {
                var findyear = db.tblFinancialYears.Where(f => f.FinancialYear == tblFinancialYear.FinancialYear).FirstOrDefault();
                if (findyear == null)
                {
                    db.tblFinancialYears.Add(tblFinancialYear);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "Already Exist! Please Check.";
                }
            }
            return View(tblFinancialYear);
        }

        // GET: tblFinancialYears/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction("EP500", "EP");
                }
                tblFinancialYear tblFinancialYear = db.tblFinancialYears.Find(id);
                if (tblFinancialYear == null)
                {
                    return RedirectToAction("EP404", "EP");
                }
                return View(tblFinancialYear);
            }
            catch
            {
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: tblFinancialYears/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblFinancialYear tblFinancialYear)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int userid = 0;
            userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            tblFinancialYear.UserID = userid;
            if (ModelState.IsValid)
            {
                var findyear = db.tblFinancialYears.Where(f => f.FinancialYear == tblFinancialYear.FinancialYear && f.FinancialYearID != tblFinancialYear.FinancialYearID).FirstOrDefault();
                if (findyear == null)
                {
                    db.Entry(tblFinancialYear).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "Already Exist! Please Check.";
                }
            }
            return View(tblFinancialYear);
        }

        // GET: tblFinancialYears/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction("EP500", "EP");
                }
                tblFinancialYear tblFinancialYear = db.tblFinancialYears.Find(id);
                if (tblFinancialYear == null)
                {
                    return RedirectToAction("EP404", "EP");
                }
                return View(tblFinancialYear);
            }
            catch
            {
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: tblFinancialYears/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblFinancialYear tblFinancialYear = db.tblFinancialYears.Find(id);
            db.tblFinancialYears.Remove(tblFinancialYear);
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
