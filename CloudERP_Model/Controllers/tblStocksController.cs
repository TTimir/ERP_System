using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DatabaseAccess;

namespace CloudERP_Model.Controllers
{
    public class tblStocksController : Controller
    {
        private CloudErpVEntities db = new CloudErpVEntities();

        // GET: tblStocks
        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyid = 0;
            int branchid = 0;
            branchid = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            var tblStocks = db.tblStocks.Include(t => t.tblBranch).Include(t => t.tblCategory).Include(t => t.tblCompany).Include(t => t.tblUser).Where(t => t.CompanyID == companyid && t.BranchID == branchid);
            return View(tblStocks.ToList());
        }

        // GET: tblStocks/Details/5
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
                tblStock tblStock = db.tblStocks.Find(id);
                if (tblStock == null)
                {
                    return RedirectToAction("EP404", "EP");
                }
                return View(tblStock);
            }
            catch
            {
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: tblStocks/Create
        public ActionResult Create()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyid = 0;
            int branchid = 0;
            branchid = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            ViewBag.CategoryID = new SelectList(db.tblCategories.Where(c => c.BranchID == branchid && c.CompanyID == companyid), "CategoryID", "categoryName", 0);
            return View();
        }

        // POST: tblStocks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblStock tblStock)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyid = 0;
            int branchid = 0;
            int userid = 0;
            branchid = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            tblStock.BranchID = branchid;
            tblStock.CompanyID = companyid;
            tblStock.UserID = userid;
            if (ModelState.IsValid)
            {
                var findproduct = db.tblStocks.Where(p => p.CompanyID == companyid && p.BranchID == branchid && p.ProductName == tblStock.ProductName).FirstOrDefault();
                if (findproduct == null)
                {
                    db.tblStocks.Add(tblStock);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "Already in Stock! Please Check.";
                }
            }
            ViewBag.CategoryID = new SelectList(db.tblCategories.Where(c => c.BranchID == branchid && c.CompanyID == companyid), "CategoryID", "categoryName", tblStock.CategoryID);
            return View(tblStock);
        }

        // GET: tblStocks/Edit/5
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
                tblStock tblStock = db.tblStocks.Find(id);
                if (tblStock == null)
                {
                    return RedirectToAction("EP404", "EP");
                }
                ViewBag.CategoryID = new SelectList(db.tblCategories.Where(c => c.BranchID == tblStock.BranchID && c.CompanyID == tblStock.CompanyID), "CategoryID", "categoryName", tblStock.CategoryID);
                return View(tblStock);
            }
            catch
            {
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: tblStocks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblStock tblStock)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int userid = 0;
            userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            tblStock.UserID = userid;

            if (ModelState.IsValid)
            {
                var findproduct = db.tblStocks.Where(p => p.CompanyID == tblStock.CompanyID && p.BranchID == tblStock.BranchID && p.ProductName == tblStock.ProductName && p.ProductID != tblStock.ProductID).FirstOrDefault();
                if (findproduct == null)
                {
                    db.Entry(tblStock).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "Already in Stock! Please Check.";
                }
            }
            ViewBag.CategoryID = new SelectList(db.tblCategories.Where(c => c.BranchID == tblStock.BranchID && c.CompanyID == tblStock.CompanyID), "CategoryID", "categoryName", tblStock.CategoryID);
            return View(tblStock);
        }

        // GET: tblStocks/Delete/5
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
                tblStock tblStock = db.tblStocks.Find(id);
                if (tblStock == null)
                {
                    return RedirectToAction("EP404", "EP");
                }
                return View(tblStock);
            }
            catch
            {
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: tblStocks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            tblStock tblStock = db.tblStocks.Find(id);
            db.tblStocks.Remove(tblStock);
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
