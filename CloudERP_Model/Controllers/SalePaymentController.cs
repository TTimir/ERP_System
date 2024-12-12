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
    public class SalePaymentController : Controller
    {
        private CloudErpVEntities db = new CloudErpVEntities();
        SP_Sale sale = new SP_Sale();
        private SaleEntry saleEntry = new SaleEntry();

        // GET: SalePayment
        public ActionResult RemainingPaymentList()
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
            var list = sale.RemainingPayment(companyid, branchid);
            return View(list.ToList());
        }
        public ActionResult PaidHistory(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])) || string.IsNullOrEmpty(Convert.ToString(id)))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyid = 0;
            int branchid = 0;
            int userid = 0;
            branchid = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            var list = sale.SalePaymentHistory((int)id);
            var returndetails = db.tblCustomerReturnInvoices.Where(r => r.CustomerInvoiceID == id).ToList();
            if (returndetails != null)
            {
                if (returndetails.Count > 0)
                {
                    ViewData["ReturnSaleDetails"] = returndetails;
                }
            }
            double remainingamount = 0;
            double totalinvoiceamount = db.tblCustomerInvoices.Find(id).TotalAmount;
            double totalpaidamount = db.tblCustomerPayments.Where(p => p.CustomerInvoiceID == id).Sum(p => p.PaidAmount);
            remainingamount = totalinvoiceamount - totalpaidamount;

            //foreach (var item in list)
            //{
            //    remainingamount = item.RemainingBalance;
            //}
            //if (remainingamount == 0)
            //{
            //    var invoice = db.tblCustomerInvoices.Find(id);
            //    if (invoice != null)
            //    {
            //        remainingamount = invoice.TotalAmount;
            //    }
            //    else
            //    {
            //        return HttpNotFound("Invoice not found.");
            //    }
            //}
            ViewBag.PreviousRemainingAmount = remainingamount;
            ViewBag.InvoiceID = id;
            return View(list.ToList());
        }
        public ActionResult PaidAmount(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])) || string.IsNullOrEmpty(Convert.ToString(id)))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyid = 0;
            int branchid = 0;
            int userid = 0;
            branchid = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            //var list = sale.RemainingPayment(companyid, branchid).Where(p => p.CustomerInvoiceID == id).ToList();
            var list = sale.SalePaymentHistory((int)id);
            double remainingamount = 0;
            foreach (var item in list)
            {
                remainingamount = item.RemainingBalance;
            }
            if (remainingamount == 0)
            {
                var invoice = db.tblCustomerInvoices.Find(id);
                if (invoice != null)
                {
                    remainingamount = invoice.TotalAmount;
                }
                else
                {
                    return HttpNotFound("Invoice not found.");
                }
            }
            ViewBag.PreviousRemainingAmount = remainingamount;
            ViewBag.InvoiceID = id;
            return View(list);
        }

        [HttpPost]
        public ActionResult PaidAmount(int? id, float previousRemainingAmount, float paidAmount)
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

                if (paidAmount > previousRemainingAmount)
                {
                    ViewBag.Message = "The payment amount must be less than or equal to the remaining amount.";
                    var list = sale.SalePaymentHistory((int)id);
                    double remainingamount = 0;
                    foreach (var item in list)
                    {
                        remainingamount = item.RemainingBalance;
                    }
                    if (remainingamount == 0)
                    {
                        var invoice = db.tblCustomerInvoices.Find(id);
                        if (invoice != null)
                        {
                            remainingamount = invoice.TotalAmount;
                        }
                        else
                        {
                            return HttpNotFound("Invoice not found.");
                        }
                    }
                    ViewBag.PreviousRemainingAmount = remainingamount;
                    ViewBag.InvoiceID = id;
                    return View(list);
                }
                string payinvoicenno = "INP" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
                var customer = db.tblCustomers.Find(db.tblCustomerInvoices.Find(id).CustomerID);

                var saleinvoice = db.tblCustomerInvoices.Find(id);
                var saleinvoicedetails = db.tblCustomerPayments.Where(p => p.CustomerInvoiceID == id);

                string message = saleEntry.SalePayment(companyid, branchid, userid,
                    payinvoicenno, Convert.ToString(id), (float)saleinvoice.TotalAmount,
                    paidAmount, Convert.ToString(customer.CustomerID), customer.Customername,
                    (previousRemainingAmount - paidAmount));

                Session["Message"] = message;
                return RedirectToAction("RemainingPaymentList");
            }
            catch
            {
                ViewBag.Message = "Please Try Again!";
                var list = sale.SalePaymentHistory((int)id);
                double remainingamount = 0;
                foreach (var item in list)
                {
                    remainingamount = item.RemainingBalance;
                }
                if (remainingamount == 0)
                {
                    var invoice = db.tblCustomerInvoices.Find(id);
                    if (invoice != null)
                    {
                        remainingamount = invoice.TotalAmount;
                    }
                    else
                    {
                        return HttpNotFound("Invoice not found.");
                    }
                }
                ViewBag.PreviousRemainingAmount = remainingamount;
                ViewBag.InvoiceID = id;
                return View(list);
            }
        }

        public ActionResult CustomeSaleHistory(DateTime FromDate, DateTime ToDate)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyid = 0;
            int branchid = 0;
            int userid = 0;
            int.TryParse(Convert.ToString(Session["BranchID"]), out branchid);
            companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            var list = sale.CustomeSaleList(companyid, branchid, FromDate, ToDate);
            if (FromDate == null || ToDate == null)
            {
                ViewBag.Message = "Please select both From and To dates.";
                return RedirectToAction("CustomeSaleHistory");
            }
            return View(list.ToList());
        }

        public ActionResult SubCustomeSaleHistory(DateTime FromDate, DateTime ToDate, int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
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
            var list = sale.CustomeSaleList(companyid, branchid, FromDate, ToDate);
            if (FromDate == null || ToDate == null)
            {
                ViewBag.Message = "Please select both From and To dates.";
                return RedirectToAction("CustomeSaleHistory");
            }
            return View(list.ToList());
        }

        public ActionResult SaleItemDetail(int? id)
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
            var list = db.tblCustomerInvoiceDetails.Where(d => d.CustomerInvoiceID == id);
            return View(list.ToList());
        }

        public ActionResult PrintSaleInvoice(int? id)
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
            var list = db.tblCustomerInvoiceDetails.Where(d => d.CustomerInvoiceID == id);
            return View(list.ToList());
        }
    }
}