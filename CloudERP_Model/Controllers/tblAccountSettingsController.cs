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
using CloudERP_Model.Models;
using DatabaseAccess;

namespace CloudERP_Model.Controllers
{
    public class tblAccountSettingsController : Controller
    {
        private CloudErpVEntities db = new CloudErpVEntities();

        // GET: tblAccountSettings
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
            var tblAccountSettings = db.tblAccountSettings.
                Include(t => t.tblAccountActivity).Include(t => t.tblAccountControl).
                //Include(t => t.tblAccountHead).Include(t => t.tblAccountSubControl).
                Include(t => t.tblBranch).Include(t => t.tblCompany).
                Where(s => s.CompanyID == companyid && s.BranchID == branchid);
            return View(tblAccountSettings.ToList());
        }

        // GET: tblAccountSettings/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction("EP500", "EP");
                }
                tblAccountSetting tblAccountSetting = db.tblAccountSettings.Find(id);
                if (tblAccountSetting == null)
                {
                    return RedirectToAction("EP404", "EP");
                }
                return View(tblAccountSetting);
            }
            catch
            {
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: tblAccountSettings/Create
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

            ViewBag.AccountActivityID = new SelectList(db.tblAccountActivities, "AccountActivityID", "Name", 0);
            ViewBag.AccountControlID = new SelectList(db.tblAccountControls.Where(c => c.BranchID == branchid && c.CompanyID == companyid), "AccountControlID", "AccountControlName", 0);
            ViewBag.AccountHeadID = new SelectList(db.tblAccountHeads, "AccountHeadID", "AccountHeadName", 0);
            ViewBag.AccountSubControlID = new SelectList(db.tblAccountSubControls.Where(c => c.BranchID == branchid && c.CompanyID == companyid), "AccountSubControlID", "AccountSubControlName", 0);
            ViewBag.BranchID = new SelectList(db.tblBranches, "BranchID", "BranchName", 0);
            ViewBag.CompanyID = new SelectList(db.tblCompanies, "CompanyID", "Name", 0);
            return View();
        }

        // POST: tblAccountSettings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblAccountSetting tblAccountSetting)
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
            tblAccountSetting.CompanyID = companyid;
            tblAccountSetting.BranchID = branchid;
            if (ModelState.IsValid)
            {
                var findaccountsetting = db.tblAccountSettings.Where(c => c.CompanyID == tblAccountSetting.CompanyID && c.BranchID == tblAccountSetting.BranchID && c.AccountActivityID == tblAccountSetting.AccountActivityID).FirstOrDefault();
                if (findaccountsetting == null)
                {
                    db.tblAccountSettings.Add(tblAccountSetting);
                    db.SaveChanges();
                    ViewBag.Message = "Save Successfully!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "Already Exist! Please Check!";
                }
            }

            ViewBag.AccountActivityID = new SelectList(db.tblAccountActivities, "AccountActivityID", "Name", tblAccountSetting.AccountActivityID);
            ViewBag.AccountControlID = new SelectList(db.tblAccountControls.Where(c => c.BranchID == branchid && c.CompanyID == companyid), "AccountControlID", "AccountControlName", tblAccountSetting.AccountControlID);
            ViewBag.AccountHeadID = new SelectList(db.tblAccountHeads, "AccountHeadID", "AccountHeadName", tblAccountSetting.AccountHeadID);
            ViewBag.AccountSubControlID = new SelectList(db.tblAccountSubControls.Where(c => c.BranchID == branchid && c.CompanyID == companyid), "AccountSubControlID", "AccountSubControlName", tblAccountSetting.AccountSubControlID);
            return View(tblAccountSetting);
        }

        // GET: tblAccountSettings/Edit/5
        public ActionResult Edit(int? id)
        {

            int companyid = 0;
            int branchid = 0;
            int userid = 0;
            branchid = Convert.ToInt32(Session["BranchID"].ToString());
            companyid = Convert.ToInt32(Session["CompanyID"].ToString());
            userid = Convert.ToInt32(Session["UserID"].ToString());
            if (id == null)
            {
                return RedirectToAction("EP500", "EP");
            }
            tblAccountSetting tblAccountSetting = db.tblAccountSettings.Find(id);
            if (tblAccountSetting == null)
            {
                return RedirectToAction("EP404", "EP");
            }
            ViewBag.AccountActivityID = new SelectList(db.tblAccountActivities, "AccountActivityID", "Name", tblAccountSetting.AccountActivityID);
            ViewBag.AccountControlID = new SelectList(db.tblAccountControls, "AccountControlID", "AccountControlName", tblAccountSetting.AccountControlID);
            ViewBag.AccountHeadID = new SelectList(db.tblAccountHeads, "AccountHeadID", "AccountHeadName", tblAccountSetting.AccountHeadID);
            ViewBag.AccountSubControlID = new SelectList(db.tblAccountSubControls, "AccountSubControlID", "AccountSubControlName", tblAccountSetting.AccountSubControlID);
            ViewBag.BranchID = new SelectList(db.tblBranches, "BranchID", "BranchName", tblAccountSetting.BranchID);
            ViewBag.CompanyID = new SelectList(db.tblCompanies, "CompanyID", "Name", tblAccountSetting.CompanyID);
            return View(tblAccountSetting);
        }

        // POST: tblAccountSettings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblAccountSetting tblAccountSetting)
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

            var findaccountsetting = db.tblAccountSettings.Where(c => c.CompanyID == tblAccountSetting.CompanyID && c.BranchID == tblAccountSetting.BranchID && c.AccountActivityID == tblAccountSetting.AccountActivityID && c.AccountSettingID != tblAccountSetting.AccountSettingID).FirstOrDefault();
            if (findaccountsetting == null)
            {
                db.Entry(tblAccountSetting).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.Message = "Updated Successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Message = "Already Exist! Please Check!";
            }
            ViewBag.AccountActivityID = new SelectList(db.tblAccountActivities, "AccountActivityID", "Name", tblAccountSetting.AccountActivityID);
            ViewBag.AccountControlID = new SelectList(db.tblAccountControls.Where(c => c.BranchID == branchid && c.CompanyID == companyid), "AccountControlID", "AccountControlName", tblAccountSetting.AccountControlID);
            ViewBag.AccountHeadID = new SelectList(db.tblAccountHeads, "AccountHeadID", "AccountHeadName", tblAccountSetting.AccountHeadID);
            ViewBag.AccountSubControlID = new SelectList(db.tblAccountSubControls.Where(c => c.BranchID == branchid && c.CompanyID == companyid), "AccountSubControlID", "AccountSubControlName", tblAccountSetting.AccountSubControlID);
            return View(tblAccountSetting);
        }

        // GET: tblAccountSettings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblAccountSetting tblAccountSetting = db.tblAccountSettings.Find(id);
            if (tblAccountSetting == null)
            {
                return HttpNotFound();
            }
            return View(tblAccountSetting);
        }

        // POST: tblAccountSettings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblAccountSetting tblAccountSetting = db.tblAccountSettings.Find(id);
            db.tblAccountSettings.Remove(tblAccountSetting);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult GetAccountControls(int? id)
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
            List<AccountControlsMV> controls = new List<AccountControlsMV>();
            var list = db.tblAccountControls.Where(p => p.BranchID == branchid && p.CompanyID == companyid && p.AccountHeadID == id).ToList();
            foreach (var item in list)
            {
                controls.Add(new AccountControlsMV { AccountControlName = item.AccountControlName, AccountControlID = item.AccountControlID });
            }

            return Json(new { data = controls }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetSubControls(int? id)
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
            List<AccountSubControlsMV> subControls = new List<AccountSubControlsMV>();
            var list = db.tblAccountSubControls.Where(p => p.BranchID == branchid && p.CompanyID == companyid && p.AccountControlID == id).ToList();
            foreach (var item in list)
            {
                subControls.Add(new AccountSubControlsMV { AccountSubControlName = item.AccountSubControlName, AccountSubControlID = item.AccountSubControlID });
            }

            return Json(new { data = subControls }, JsonRequestBehavior.AllowGet);
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
