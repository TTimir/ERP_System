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
    public class tblSuppliersController : Controller
    {
        private CloudErpVEntities db = new CloudErpVEntities();

        // GET: tblSuppliers
        public ActionResult AllSupplier()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            var tblSuppliers = db.tblSuppliers.Include(t => t.tblBranch).Include(t => t.tblCompany).Include(t => t.tblUser);
            return View(tblSuppliers.ToList());
        }

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

            var tblSuppliers = db.tblSuppliers.Include(t => t.tblBranch).Include(t => t.tblCompany).Include(t => t.tblUser).Where(c => c.BranchID == branchid && c.CompanyID == companyid);
            return View(tblSuppliers.ToList());
        }

        public ActionResult SubBranchSupplier()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            List<BranchSuppliersMV> list = new List<BranchSuppliersMV>();

            int branchid = 0;
            branchid = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            List<int> branchids = CloudERP_Model.HelperCls.Branch.GetBranchids(branchid, db);
            foreach (var item in branchids)
            {
                foreach (var supplier in db.tblSuppliers.Where(c => c.BranchID == item))
                {
                    var sus = new BranchSuppliersMV();
                    sus.BranchName = supplier.tblBranch.BranchName;
                    sus.CompanyName = supplier.tblCompany.Name;
                    sus.SupplierAddress = supplier.SupplierAddress;
                    sus.SupplierConatctNo = supplier.SupplierConatctNo;
                    sus.SupplierEmail = supplier.SupplierEmail;
                    sus.Discription = supplier.Discription;
                    sus.User = supplier.tblUser.UserName;
                    sus.SupplierName = supplier.SupplierName;
                    list.Add(sus);
                }

            }
            return View(list);
        }

        // GET: tblSuppliers/Details/5
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
                tblSupplier tblSupplier = db.tblSuppliers.Find(id);
                if (tblSupplier == null)
                {
                    return RedirectToAction("EP404", "EP");
                }
                return View(tblSupplier);
            }
            catch
            {
                return RedirectToAction("EP500", "EP");
            }
        }

        public ActionResult SupplierDetails(int? id)
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
                tblSupplier tblSupplier = db.tblSuppliers.Find(id);
                if (tblSupplier == null)
                {
                    return RedirectToAction("EP404", "EP");
                }
                return View(tblSupplier);
            }
            catch
            {
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: tblSuppliers/Create
        public ActionResult Create()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }

        // POST: tblSuppliers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblSupplier tblSupplier)
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
            tblSupplier.CompanyID = companyid;
            tblSupplier.BranchID = branchid;
            tblSupplier.UserID = userid;
            if (ModelState.IsValid)
            {
                var findsupplier = db.tblSuppliers.Where(s => s.SupplierName == tblSupplier.SupplierName && s.SupplierConatctNo == tblSupplier.SupplierConatctNo && s.BranchID == tblSupplier.BranchID).FirstOrDefault();
                if (findsupplier == null)
                {
                    db.tblSuppliers.Add(tblSupplier);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "Already Registered! Please Check.";
                }
            }
            return View(tblSupplier);
        }

        // GET: tblSuppliers/Edit/5
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
                tblSupplier tblSupplier = db.tblSuppliers.Find(id);
                if (tblSupplier == null)
                {
                    return RedirectToAction("EP404", "EP");
                }
                return View(tblSupplier);
            }
            catch
            {
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: tblSuppliers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblSupplier tblSupplier)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int userid = 0;
            userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            tblSupplier.UserID = userid;
            if (ModelState.IsValid)
            {
                var findsupplier = db.tblSuppliers.Where(s => s.SupplierName == tblSupplier.SupplierName && s.SupplierConatctNo == tblSupplier.SupplierConatctNo && s.BranchID == tblSupplier.BranchID && s.SupplierID != tblSupplier.SupplierID).FirstOrDefault();
                if (findsupplier == null)
                {
                    db.Entry(tblSupplier).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "Already Registered! Please Check.";
                }
            }
            return View(tblSupplier);
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
