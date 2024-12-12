using CloudERP_Model.HelperCls;
using CloudERP_Model.Models;
using DatabaseAccess;
using DatabaseAccess.Code.SP_Code;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CloudERP_Model.Controllers
{
    public class PurchasePaymentReturnController : Controller
    {
        private CloudErpVEntities db = new CloudErpVEntities();
        private PurchaseEntry purchaseEntry = new PurchaseEntry();
        SP_Purchase purchase = new SP_Purchase();
        // GET: PurchasePaymentReturn
        public ActionResult ReturnPurchasePendingAmount(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            var list = purchase.PurchaseReturnPaymentPending(id);

            return View(list);
        }

        public ActionResult AllReturnPurchasePendingPayment()
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
            var list = purchase.GetReturnPurchasePaymentPending(branchid, companyid);

            return View(list);
        }

        public ActionResult ReturnAmount(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            var list = db.tblSupplierReturnPayments.Where(r => r.SupplierReturnInvoiceID == id);
            double remainingamount = 0;
            foreach (var item in list)
            {
                remainingamount = item.RemainingBalance;
            }
            if (remainingamount == 0)
            {
                remainingamount = db.tblSupplierReturnInvoices.Find(id).TotalAmount;
            }
            ViewBag.PreviousRemainingAmount = remainingamount;
            ViewBag.InvoiceID = id;
            return View(list);

        }
        [HttpPost]
        public ActionResult ReturnAmount(int? id, float previousRemainingAmount, float paymentAmount)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"]))
                        || string.IsNullOrEmpty(Convert.ToString(id)))
                {
                    return RedirectToAction("Login", "Home");
                }

                int companyid = 0;
                int branchid = 0;
                int userid = 0;
                branchid = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
                companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
                userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));

                if (paymentAmount > previousRemainingAmount)
                {
                    ViewBag.Message = "Payment Amount Must be Less Than or Equal to Previous Remaining Amount.";
                    var list = db.tblSupplierReturnPayments.Where(r => r.SupplierReturnInvoiceID == id);
                    double remainingamount = 0;
                    foreach (var item in list)
                    {
                        remainingamount = item.RemainingBalance;
                        if (remainingamount == 0)
                        {
                            return RedirectToAction("AllReturnPurchasePendingPayment");
                        }
                    }
                    if (remainingamount == 0)
                    {
                        remainingamount = db.tblSupplierReturnInvoices.Find(id).TotalAmount;
                    }
                    ViewBag.PreviousRemainingAmount = remainingamount;
                    ViewBag.InvoiceID = id;
                    return View(list);
                }
                string payinvoicenno = "RPP" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
                var supplier = db.tblSuppliers.Find(db.tblSupplierReturnInvoices.Find(id).SupplierID);

                var purchaseinvoice = db.tblSupplierReturnInvoices.Find(id);
                var purchaseinvoicedetails = db.tblSupplierReturnPayments.Where(p => p.SupplierReturnInvoiceID == id);

                string message = purchaseEntry.ReturnPurchasePayment(companyid, branchid, userid,
                    payinvoicenno, purchaseinvoice.SupplierInvoiceID.ToString(), purchaseinvoice.SupplierReturnInvoiceID, 
                    (float)purchaseinvoice.TotalAmount, paymentAmount, Convert.ToString(supplier.SupplierID), 
                    supplier.SupplierName, (previousRemainingAmount - paymentAmount));
                Session["Message"] = message;

                return RedirectToAction("ReturnAmount", new { id = id });
            }
            catch (Exception)
            {
                var list = db.tblSupplierReturnPayments.Where(r => r.SupplierReturnInvoiceID == id);
                double remainingamount = 0;
                foreach (var item in list)
                {
                    remainingamount = item.RemainingBalance;
                }
                if (remainingamount == 0)
                {
                    remainingamount = db.tblSupplierReturnInvoices.Find(id).TotalAmount;
                }
                ViewBag.PreviousRemainingAmount = remainingamount;
                ViewBag.InvoiceID = id;
                return View(list);
            }

        }
    }
}