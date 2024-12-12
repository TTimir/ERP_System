using DatabaseAccess.Code.SP_Code;
using DatabaseAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DatabaseAccess.Code;
using System.EnterpriseServices;

namespace CloudERP_Model.Controllers
{
    public class BalanceSheetsController : Controller
    {
        private CloudErpVEntities db = new CloudErpVEntities();
        private SP_BalanceSheet bal_Sheet = new SP_BalanceSheet();
        // GET: BalanceSheets
        public ActionResult BalanceSheet()
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
            var balancesheet = bal_Sheet.GetBalanceSheet(companyid, branchid, FinancialYear.FinancialYearID, new List<int>() { 1, 2, 3, 4, 5 });
            return View(balancesheet);
        }

        [HttpPost]
        public ActionResult BalanceSheet(int? id)
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
            var balancesheet = bal_Sheet.GetBalanceSheet(companyid, branchid, (int)id, new List<int>() { 1, 2, 3, 4, 5 });
            return View(balancesheet);
        }

        public ActionResult SubBalanceSheet(string bchid)
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
            var balancesheet = bal_Sheet.GetBalanceSheet(companyid, branchid, FinancialYear.FinancialYearID, new List<int>() { 1, 2, 3, 4, 5 });
            return View(balancesheet);
        }

        [HttpPost]
        public ActionResult SubBalanceSheet(int? id)
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
            var balancesheet = bal_Sheet.GetBalanceSheet(companyid, branchid, (int)id, new List<int>() { 1, 2, 3, 4, 5 });
            return View(balancesheet);
        }
    }
}