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
    public class tblCategoriesController : Controller
    {
        private CloudErpVEntities db = new CloudErpVEntities();

        // GET: tblCategories
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
            var tblCategories = db.tblCategories.Include(t => t.tblBranch).Include(t => t.tblCompany).Include(t => t.tblUser).Where(c => c.CompanyID == companyid && c.BranchID == branchid);
            return View(tblCategories.ToList());
        }

        // GET: tblCategories/Create
        public ActionResult Create()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }

        // POST: tblCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblCategory tblCategory)
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
            tblCategory.BranchID = branchid;
            tblCategory.CompanyID = companyid;
            tblCategory.UserID = userid;
            if (ModelState.IsValid)
            {
                var findcategory = db.tblCategories.Where(c => c.CompanyID == companyid && c.BranchID == branchid && c.categoryName == tblCategory.categoryName).FirstOrDefault();
                if (findcategory == null)
                {
                    db.tblCategories.Add(tblCategory);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "Already Exist! Please Check.";
                }
            }

            return View(tblCategory);
        }

        // GET: tblCategories/Edit/5
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
                tblCategory tblCategory = db.tblCategories.Find(id);
                if (tblCategory == null)
                {
                    return RedirectToAction("EP404", "EP");
                }
                return View(tblCategory);
            }
            catch
            {
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: tblCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblCategory tblCategory)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int userid = 0;
            userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            tblCategory.UserID = userid;
            if (ModelState.IsValid)
            {
                var findcategory = db.tblCategories.Where(c => c.CompanyID == tblCategory.CompanyID && c.BranchID == tblCategory.BranchID && c.categoryName == tblCategory.categoryName && c.CategoryID != tblCategory.CategoryID).FirstOrDefault();
                if (findcategory == null)
                {
                    db.Entry(tblCategory).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "Already Exist! Please Check.";
                }
            }
            return View(tblCategory);
        }

        // GET: tblCategories/Details/5
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
                tblCategory tblCategory = db.tblCategories.Find(id);
                if (tblCategory == null)
                {
                    return RedirectToAction("EP404", "EP");
                }
                return View(tblCategory);
            }
            catch
            {
                return RedirectToAction("EP500", "EP");
            }
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
