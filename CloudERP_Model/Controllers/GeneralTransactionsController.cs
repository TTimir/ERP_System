using CloudERP_Model.Models;
using DatabaseAccess.Code.SP_Code;
using DatabaseAccess.Code;
using DatabaseAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace CloudERP_Model.Controllers
{
    public class GeneralTransactionsController : Controller
    {
        private CloudErpVEntities db = new CloudErpVEntities();
        SP_GeneralTransaction accounts = new SP_GeneralTransaction();
        private GeneralTransactionEntry generalEntry = new GeneralTransactionEntry();
        // GET: GeneralTransactions
        public ActionResult GeneralTransaction(GeneralTransactionMV transaction)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            if (Session["GNMessage"] != null)
            {
                Session["GNMessage"] = string.Empty;
            }
            int companyid = 0;
            int branchid = 0;
            int userid = 0;
            branchid = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));

            ViewBag.CreditAccountControlID = new SelectList(accounts.GetAllAccounts(companyid, branchid), "AccountSubControlID", "AccountSubControl", "0");
            ViewBag.DebitAccountControlID = new SelectList(accounts.GetAllAccounts(companyid, branchid), "AccountSubControlID", "AccountSubControl", "0");
            return View(transaction);
        }

        public ActionResult SaveGeneralTransaction(GeneralTransactionMV transaction)
        {
            try
            {
                Session["GNMessage"] = string.Empty;
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
                if (ModelState.IsValid)
                {
                    string payinvoiceno = "GEN" + DateTime.Now.ToString("yyyyMMddHHmmssmm");
                    var message = generalEntry.ConfirmGeneralTransaction(transaction.TransferAmount,
                        userid, branchid, companyid, payinvoiceno, transaction.DebitAccountControlID,
                        transaction.CreditAccountControlID, transaction.Reason);
                    if (message.Contains("Succeed"))
                    {
                        Session["GNMessage"] = message;
                        //return RedirectToAction("Journal");
                    }
                    else
                    {
                        Session["GNMessage"] = "Some issue is occur, re-login and try again after sometime!";
                    }

                }
                ViewBag.CreditAccountControlID = new SelectList(accounts.GetAllAccounts(companyid, branchid), "AccountSubControlID", "AccountSubControl", "0");
                ViewBag.DebitAccountControlID = new SelectList(accounts.GetAllAccounts(companyid, branchid), "AccountSubControlID", "AccountSubControl", "0");
                return RedirectToAction("GeneralTransaction", new { transaction = transaction });
            }
            catch (Exception)
            {
                Session["GNMessage"] = "Some unexpected issue is occure please contact to Adminstrator!";
                return RedirectToAction("GeneralTransaction", new { transaction = transaction });
            }
        }

        public ActionResult Journal()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            if (Session["GNMessage"] != null)
            {
                Session["GNMessage"] = string.Empty;
            }
            int companyid = 0;
            int branchid = 0;
            int userid = 0;
            branchid = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));

            var list = accounts.GetJournal(companyid, branchid, DateTime.Now, DateTime.Now);
            return View(list);
        }

        [HttpPost]
        public ActionResult Journal(DateTime FromDate, DateTime ToDate)
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
            var list = accounts.GetJournal(companyid, branchid, FromDate, ToDate);
            if (FromDate == null || ToDate == null)
            {
                ViewBag.JournalMessage = "Please select both From and To dates.";
                return RedirectToAction("CustomeJournalHistory");

            }
            return View(list.ToList());
        }

        public ActionResult SubJournal(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            if (Session["GNMessage"] != null)
            {
                Session["GNMessage"] = string.Empty;
            }
            int companyid = 0;
            int branchid = 0;
            int userid = 0;
            if (id != null)
            {
                Session["SubBranchID"] = id;
            }
            int.TryParse(Convert.ToString(Session["SubBranchID"]), out branchid);
            companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));

            var list = accounts.GetJournal(companyid, branchid, DateTime.Now, DateTime.Now);
            return View(list);
        }

        [HttpPost]
        public ActionResult SubJournal(DateTime FromDate, DateTime ToDate)
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
            var list = accounts.GetJournal(companyid, branchid, FromDate, ToDate);
            if (FromDate == null || ToDate == null)
            {
                ViewBag.JournalMessage = "Please select both From and To dates.";
                return RedirectToAction("CustomeJournalHistory");

            }
            return View(list.ToList());
        }

    }
}