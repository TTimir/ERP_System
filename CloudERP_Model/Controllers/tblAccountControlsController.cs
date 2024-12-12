using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CloudERP_Model.Models;
using DatabaseAccess;

namespace CloudERP_Model.Controllers
{
    public class tblAccountControlsController : Controller
    {
        private CloudErpVEntities db = new CloudErpVEntities();
        private List<AccountControlMV> accountControls = new List<AccountControlMV>();

        // GET: tblAccountControls
        public ActionResult Index()
        {
            accountControls.Clear();
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

            var tblAccountControls = db.tblAccountControls.Include(t => t.tblBranch).Include(t => t.tblCompany).Include(t => t.tblUser).Where(a => a.CompanyID == companyid && a.BranchID == branchid);

            foreach (var item in tblAccountControls)
            {
                accountControls.Add(new AccountControlMV
                {
                    AccountControlID = item.AccountControlID,
                    AccountControlName = item.AccountControlName,
                    AccountHeadID = item.AccountHeadID,
                    AccountHeadName = db.tblAccountHeads.Find(item.AccountHeadID).AccountHeadName,
                    BranchID = item.BranchID,
                    BranchName = item.tblBranch.BranchName,
                    CompanyID = item.CompanyID,
                    Name = item.tblCompany.Name,
                    UserID = item.UserID,
                    UserName = item.tblUser.UserName
                }); 
            }
            return View(accountControls);
        }

        // GET: tblAccountControls/Create
        public ActionResult Create()
        {
            ViewBag.AccountHeadID = new SelectList(db.tblAccountHeads, "AccountHeadID", "AccountHeadName");
            return View();
        }

        // POST: tblAccountControls/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblAccountControl tblAccountControl)
        {
            accountControls.Clear();
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
            tblAccountControl.CompanyID = companyid;
            tblAccountControl.BranchID = branchid;
            tblAccountControl.UserID = userid;
            if (ModelState.IsValid)
            {
                var findcontrol = db.tblAccountControls.Where(a => a.CompanyID == companyid && a.BranchID == branchid && a.AccountControlName == tblAccountControl.AccountControlName).FirstOrDefault();
                if (findcontrol == null)
                {
                    db.tblAccountControls.Add(tblAccountControl);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "Already Exist! Please Check.";
                }

            }

            ViewBag.AccountHeadID = new SelectList(db.tblAccountHeads, "AccountHeadID", "AccountHeadName", tblAccountControl.AccountHeadID);
            return View(tblAccountControl);
        }

        // GET: tblAccountControls/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction("EP500", "EP");
                }
                tblAccountControl tblAccountControl = db.tblAccountControls.Find(id);
                if (tblAccountControl == null)
                {
                    return RedirectToAction("EP404", "EP");
                }
                ViewBag.AccountHeadID = new SelectList(db.tblAccountHeads, "AccountHeadID", "AccountHeadName", tblAccountControl.AccountHeadID);
                return View(tblAccountControl);
            }
            catch
            {
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: tblAccountControls/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblAccountControl tblAccountControl)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int userid = 0;
            userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            tblAccountControl.UserID = userid;
            if (ModelState.IsValid)
            {
                var findcontrol = db.tblAccountControls.Where(a => a.CompanyID == tblAccountControl.CompanyID && a.BranchID == tblAccountControl.BranchID && a.AccountControlName == tblAccountControl.AccountControlName && a.AccountControlID != tblAccountControl.AccountControlID).FirstOrDefault();
                if (findcontrol == null)
                {
                    db.Entry(tblAccountControl).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "Already Exist! Please Check.";
                }
            }
            ViewBag.AccountHeadID = new SelectList(db.tblAccountHeads, "AccountHeadID", "AccountHeadName", tblAccountControl.AccountHeadID);
            return View(tblAccountControl);
        }

        // GET: tblAccountControls/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction("EP500", "EP");
                }
                tblAccountControl tblAccountControl = db.tblAccountControls.Find(id);
                if (tblAccountControl == null)
                {
                    return RedirectToAction("EP404", "EP");
                }
                return View(tblAccountControl);
            }
            catch
            {
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: tblAccountControls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            accountControls.Clear();
            tblAccountControl tblAccountControl = db.tblAccountControls.Find(id);
            db.tblAccountControls.Remove(tblAccountControl);
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
