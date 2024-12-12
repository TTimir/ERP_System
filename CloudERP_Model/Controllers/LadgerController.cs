using DatabaseAccess.Code;
using DatabaseAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DatabaseAccess.Code.SP_Code;

namespace CloudERP_Model.Controllers
{
    public class LadgerController : Controller
    {
        private CloudErpVEntities db = new CloudErpVEntities();
        private SP_Ladger sp_ladger = new SP_Ladger();
        // GET: Ladger
        public ActionResult GetLadgers()
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
            var FinancialYear = db.tblFinancialYears.Where(f => f.IsActive == true).FirstOrDefault();
            if (FinancialYear == null)
            {
                ViewBag.Message = "Your Company Financial Year is not Set, Please Contact to Adminstrator!";
            }
            var ladger = sp_ladger.GetLadger(companyid, branchid, FinancialYear.FinancialYearID);
            return View(ladger);
        }

        [HttpPost]
        public ActionResult GetLadgers(int? id)
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
            var FinancialYear = db.tblFinancialYears.Where(f => f.IsActive == true).FirstOrDefault();
            if (FinancialYear == null)
            {
                ViewBag.Message = "Your Company Financial Year is not Set, Please Contact to Adminstrator!";
            }
            var ladger = sp_ladger.GetLadger(companyid, branchid, (int)id);
            return View(ladger);
        }

        public ActionResult GetSubLadgers(string bchid)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyid = 0;
            int branchid = 0;
            int userid = 0;
            if (bchid != null)
            {
                Session["SubBranchID"] = bchid;
            }
            int.TryParse(Convert.ToString(Session["SubBranchID"]), out branchid);
            companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            var FinancialYear = db.tblFinancialYears.Where(f => f.IsActive == true).FirstOrDefault();
            if (FinancialYear == null)
            {
                ViewBag.Message = "Your Company Financial Year is not Set, Please Contact to Adminstrator!";
            }
            var ladger = sp_ladger.GetLadger(companyid, branchid, FinancialYear.FinancialYearID);
            return View(ladger);
        }

        [HttpPost]
        public ActionResult GetSubLadgers(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyid = 0;
            int branchid = 0;
            int userid = 0;
            int.TryParse(Convert.ToString(Session["SubBranchID"]), out branchid);
            companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            var FinancialYear = db.tblFinancialYears.Where(f => f.IsActive == true).FirstOrDefault();
            if (FinancialYear == null)
            {
                ViewBag.Message = "Your Company Financial Year is not Set, Please Contact to Adminstrator!";
            }
            var ladger = sp_ladger.GetLadger(companyid, branchid, (int)id);
            return View(ladger);
        }
    }
}