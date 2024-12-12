using CloudERP_Model.Models;
using DatabaseAccess.Code.SP_Code;
using DatabaseAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DatabaseAccess.Code;

namespace CloudERP_Model.Controllers
{
    public class IncomeStatmentController : Controller
    {
        private CloudErpVEntities db = new CloudErpVEntities();
        private IncomeStatment income = new IncomeStatment();
        // GET: IncomeStatment
        public ActionResult GetIncomeStatment()
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
            var incomestatment = income.GetIncomeStatment(companyid, branchid, FinancialYear.FinancialYearID);
            return View(incomestatment);
        }

        [HttpPost]
        public ActionResult GetIncomeStatment(int? id)
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
            var incomestatment = income.GetIncomeStatment(companyid, branchid, (int)id);
            return View(incomestatment);
        }

        public ActionResult GetSubIncomeStatment(string bchid)
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
            var incomestatment = income.GetIncomeStatment(companyid, branchid, FinancialYear.FinancialYearID);
            return View(incomestatment);
        }

        [HttpPost]
        public ActionResult GetSubIncomeStatment(int? id)
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
            var incomestatment = income.GetIncomeStatment(companyid, branchid, (int)id);
            return View(incomestatment);
        }
    }
}