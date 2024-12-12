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
    public class SalePaymentReturnController : Controller
    {
        private CloudErpVEntities db = new CloudErpVEntities();
        private SaleEntry saleEntry = new SaleEntry();
        SP_Sale sale = new SP_Sale();
        // GET: SalePaymentReturn
        public ActionResult ReturnSalePendingAmount(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            var list = sale.SaleReturnAmountPending(id);

            return View(list);
        }

        public ActionResult AllReturnSalePendingPayment()
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
            var list = sale.GetReturnSaleAmountPending(branchid, companyid);

            return View(list);
        }

        public ActionResult ReturnAmount(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            var list = db.tblCustomerReturnPayments.Where(r => r.CustomerReturnInvoiceID == id);
            double remainingamount = 0;
            foreach (var item in list)
            {
                remainingamount = item.RemainingBalance;
            }
            if (remainingamount == 0)
            {
                remainingamount = db.tblCustomerReturnInvoices.Find(id).TotalAmount;
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
                    var list = db.tblCustomerReturnPayments.Where(r => r.CustomerReturnInvoiceID == id);
                    double remainingamount = 0;
                    foreach (var item in list)
                    {
                        remainingamount = item.RemainingBalance;
                        if (remainingamount == 0)
                        {
                            return RedirectToAction("AllReturnSalePendingPayment");
                        }
                    }
                    if (remainingamount == 0)
                    {
                        remainingamount = db.tblCustomerReturnInvoices.Find(id).TotalAmount;
                    }
                    ViewBag.PreviousRemainingAmount = remainingamount;
                    ViewBag.InvoiceID = id;
                    return View(list);
                }
                string payinvoicenno = "RIP" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
                var customer = db.tblCustomers.Find(db.tblCustomerReturnInvoices.Find(id).CustomerID);

                var saleinvoice = db.tblCustomerReturnInvoices.Find(id);
                var saleinvoicedetails = db.tblCustomerReturnPayments.Where(p => p.CustomerReturnInvoiceID == id);

                string message = saleEntry.ReturnSalePayment(companyid, branchid, userid,
                    payinvoicenno, saleinvoice.CustomerInvoiceID.ToString(), saleinvoice.CustomerReturnInvoiceID,
                    (float)saleinvoice.TotalAmount, paymentAmount, Convert.ToString(customer.CustomerID),
                    customer.Customername, (previousRemainingAmount - paymentAmount));
                Session["SaleMessage"] = message;

                return RedirectToAction("ReturnAmount", new { id = id });
            }
            catch (Exception)
            {
                var list = db.tblCustomerReturnPayments.Where(r => r.CustomerReturnInvoiceID == id);
                double remainingamount = 0;
                foreach (var item in list)
                {
                    remainingamount = item.RemainingBalance;
                }
                if (remainingamount == 0)
                {
                    remainingamount = db.tblCustomerReturnInvoices.Find(id).TotalAmount;
                }
                ViewBag.PreviousRemainingAmount = remainingamount;
                ViewBag.InvoiceID = id;
                return View(list);
            }

        }
    }
}