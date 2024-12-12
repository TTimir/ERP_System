using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CloudERP_Model.HelperCls;
using DatabaseAccess;

namespace CloudERP_Model.Controllers
{
    public class tblAccountSubControlsController : Controller
    {
        private CloudErpVEntities db = new CloudErpVEntities();

        // GET: tblAccountSubControls
        public ActionResult Index()
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
            var tblAccountSubControls = db.tblAccountSubControls.Include(t => t.tblAccountControl).Include(t => t.tblAccountHead).Include(t => t.tblBranch).Include(t => t.tblUser).Where(a => a.CompanyID == companyid && a.BranchID == branchid);
            return View(tblAccountSubControls.ToList());
        }

        // GET: tblAccountSubControls/Create
        public ActionResult Create()
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
            ViewBag.AccountControlID = new SelectList(db.tblAccountControls.Where(a => a.BranchID == branchid && a.CompanyID == companyid), "AccountControlID", "AccountControlName", 0);
            return View();
        }

        // POST: tblAccountSubControls/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblAccountSubControl tblAccountSubControl)
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
            tblAccountSubControl.CompanyID = companyid;
            tblAccountSubControl.BranchID = branchid;
            tblAccountSubControl.UserID = userid;
            tblAccountSubControl.AccountHeadID = db.tblAccountControls.Find(tblAccountSubControl.AccountControlID).AccountHeadID;
            if (ModelState.IsValid)
            {
                var findsubcontrol = db.tblAccountSubControls.Where(s => s.CompanyID == tblAccountSubControl.CompanyID && s.BranchID == tblAccountSubControl.BranchID && s.AccountSubControlName == tblAccountSubControl.AccountSubControlName).FirstOrDefault();
                if (findsubcontrol == null)
                {
                    db.tblAccountSubControls.Add(tblAccountSubControl);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "Already Exist! Please Check.";
                }
            }

            ViewBag.AccountControlID = new SelectList(db.tblAccountControls.Where(a => a.BranchID == branchid && a.CompanyID == companyid), "AccountControlID", "AccountControlName", tblAccountSubControl.AccountControlID);
            return View(tblAccountSubControl);
        }

        // GET: tblAccountSubControls/Edit/5
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
                tblAccountSubControl tblAccountSubControl = db.tblAccountSubControls.Find(id);
                if (tblAccountSubControl == null)
                {
                    return RedirectToAction("EP404", "EP");
                }
                int companyid = 0;
                int branchid = 0;
                int userid = 0;
                branchid = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
                companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
                userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));
                ViewBag.AccountControlID = new SelectList(db.tblAccountControls.Where(a => a.BranchID == branchid && a.CompanyID == companyid), "AccountControlID", "AccountControlName", tblAccountSubControl.AccountControlID);
                return View(tblAccountSubControl);
            }
            catch
            {
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: tblAccountSubControls/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblAccountSubControl tblAccountSubControl)
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
            tblAccountSubControl.UserID = userid;
            tblAccountSubControl.AccountHeadID = db.tblAccountControls.Find(tblAccountSubControl.AccountControlID).AccountHeadID;
            if (ModelState.IsValid)
            {
                var findsubcontrol = db.tblAccountSubControls.Where(s => s.CompanyID == tblAccountSubControl.CompanyID && s.BranchID == tblAccountSubControl.BranchID && s.AccountSubControlName == tblAccountSubControl.AccountSubControlName && s.AccountSubControlID != tblAccountSubControl.AccountSubControlID).FirstOrDefault();
                if (findsubcontrol == null)
                {
                    db.Entry(tblAccountSubControl).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "Already Exist! Please Check.";
                }
            }
            ViewBag.AccountControlID = new SelectList(db.tblAccountControls.Where(a => a.BranchID == branchid && a.CompanyID == companyid), "AccountControlID", "AccountControlName", tblAccountSubControl.AccountControlID);
            return View(tblAccountSubControl);
        }

        // GET: tblAccountSubControls/Delete/5
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
                tblAccountSubControl tblAccountSubControl = db.tblAccountSubControls.Find(id);
                if (tblAccountSubControl == null)
                {
                    return RedirectToAction("EP404", "EP");
                }
                return View(tblAccountSubControl);
            }
            catch
            {
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: tblAccountSubControls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            tblAccountSubControl tblAccountSubControl = db.tblAccountSubControls.Find(id);
            db.tblAccountSubControls.Remove(tblAccountSubControl);
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
