using CloudERP_Model.Models;
using DatabaseAccess;
using DatabaseAccess.Code.SP_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity; // For Entity Framework 6


namespace CloudERP_Model.Controllers
{
    public class PurchasePaymentController : Controller
    {
        private CloudErpVEntities db = new CloudErpVEntities();
        private SP_Purchase purchase = new SP_Purchase();
        private PurchaseEntry paymentEntry = new PurchaseEntry();

        // GET: PurchasePayment
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
            var list = purchase.RemainingPayment(companyid, branchid);
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
            var list = purchase.PurchasePaymentHistory((int)id);
            var returndetails = db.tblSupplierReturnInvoices.Where(r => r.SupplierInvoiceID == id).ToList();
            if (returndetails != null)
            {
                if (returndetails.Count > 0)
                {
                    ViewData["ReturnPurcahseDetails"] = returndetails;
                }
            }
            double remainingamount = 0;
            double totalinvoiceamount = db.tblSupplierInvoices.Find(id).TotalAmount;
            double totalpaidamount = db.tblSupplierPayments.Where(p => p.SupplierInvoiceID == id).Sum(p => p.PaymentAmount);
            remainingamount = totalinvoiceamount - totalpaidamount;

            //foreach (var item in list)
            //{
            //    remainingamount = item.RemainingBalance;
            //}
            //if (remainingamount == 0)
            //{
            //    var invoice = db.tblSupplierInvoices.Find(id);
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
            //var list = purchase.RemainingPayment(companyid, branchid).Where(p => p.SupplierInvoiceID == id).ToList();
            var list = purchase.PurchasePaymentHistory((int)id);
            var returndetails = db.tblSupplierReturnInvoices.Where(r => r.SupplierInvoiceID == id).ToList();
            if (returndetails != null)
            {
                if (returndetails.Count > 0)
                {
                    ViewData["ReturnPurcahseDetails"] = returndetails;
                }
            }
            double remainingamount = 0;
            double totalpaidamount = 0;
            double totalinvoiceamount = db.tblSupplierInvoices.Find(id).TotalAmount;
            if (db.tblSupplierPayments.Where(p => p.SupplierInvoiceID == id).FirstOrDefault() != null)
            {
                totalpaidamount = db.tblSupplierPayments.Where(p => p.SupplierInvoiceID == id).Sum(p => p.PaymentAmount);
            }
            remainingamount = totalinvoiceamount - totalpaidamount;

            //foreach (var item in list)
            //{
            //    remainingamount = item.RemainingBalance;
            //}
            //if (remainingamount == 0)
            //{
            //    var invoice = db.tblSupplierInvoices.Find(id);
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
            return View(list);
        }

        [HttpPost]
        public ActionResult PaidAmount(int? id, float previousRemainingAmount, float paymentAmount)
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
                    //var list = purchase.RemainingPayment(companyid, branchid).Where(p => p.SupplierInvoiceID == id).ToList();
                    var list = purchase.PurchasePaymentHistory((int)id);
                    var returndetails = db.tblSupplierReturnInvoices.Where(r => r.SupplierInvoiceID == id).ToList();
                    if (returndetails != null)
                    {
                        if (returndetails.Count > 0)
                        {
                            ViewData["ReturnPurcahseDetails"] = returndetails;
                        }
                    }
                    double remainingamount = 0;
                    double totalpaidamount = 0;
                    double totalinvoiceamount = db.tblSupplierInvoices.Find(id).TotalAmount;
                    if (db.tblSupplierPayments.Where(p => p.SupplierInvoiceID == id).FirstOrDefault() != null)
                    {
                        totalpaidamount = db.tblSupplierPayments.Where(p => p.SupplierInvoiceID == id).Sum(p => p.PaymentAmount);
                    }
                    remainingamount = totalinvoiceamount - totalpaidamount;

                    //foreach (var item in list)
                    //{
                    //    remainingamount = item.RemainingBalance;
                    //}
                    //if (remainingamount == 0)
                    //{
                    //    var invoice = db.tblSupplierInvoices.Find(id);
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
                    return View(list);
                }
                string payinvoicenno = "PAY" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
                var supplier = db.tblSuppliers.Find(db.tblSupplierInvoices.Find(id).SupplierID);

                var purchaseinvoice = db.tblSupplierInvoices.Find(id);
                var purchaseinvoicedetails = db.tblSupplierPayments.Where(p => p.SupplierInvoiceID == id);

                string message = paymentEntry.PurchasePayment(companyid, branchid, userid,
                    payinvoicenno, Convert.ToString(id), (float)purchaseinvoice.TotalAmount, paymentAmount,
                    Convert.ToString(supplier.SupplierID), supplier.SupplierName, (previousRemainingAmount - paymentAmount));
                Session["Message"] = message;

                return RedirectToAction("RemainingPaymentList");
            }
            catch
            {
                ViewBag.Message = "Please Try Again!";
                //var list = purchase.RemainingPayment(companyid, branchid).Where(p => p.SupplierInvoiceID == id).ToList();
                var list = purchase.PurchasePaymentHistory((int)id);
                var returndetails = db.tblSupplierReturnInvoices.Where(r => r.SupplierInvoiceID == id).ToList();
                if (returndetails != null)
                {
                    if (returndetails.Count > 0)
                    {
                        ViewData["ReturnPurcahseDetails"] = returndetails;
                    }
                }
                var paymentdetails = db.tblSupplierPayments.Where(r => r.SupplierInvoiceID == id).ToList();
                if (paymentdetails != null)
                {
                    if (paymentdetails.Count > 0)
                    {
                        ViewData["SupplierPaymentDetails"] = paymentdetails;
                    }
                }

                double remainingamount = 0;
                double totalinvoiceamount = db.tblSupplierInvoices.Find(id).TotalAmount;
                double totalpaidamount = db.tblSupplierPayments.Where(p => p.SupplierInvoiceID == id).Sum(p => (double?)p.PaymentAmount) ?? 0.0;
                remainingamount = totalinvoiceamount - totalpaidamount;

                //foreach (var item in list)
                //{
                //    remainingamount = item.RemainingBalance;
                //}
                //if (remainingamount == 0)
                //{
                //    var invoice = db.tblSupplierInvoices.Find(id);
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

                return View(list);
            }
        }

        public ActionResult CustomePurchaseHistory(DateTime FromDate, DateTime ToDate)
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
            var list = purchase.CustomePurchaseList(companyid, branchid, FromDate, ToDate);
            if (FromDate == null || ToDate == null)
            {
                ViewBag.Message = "Please select both From and To dates.";
                return RedirectToAction("CustomePurchaseHistory");

            }
            return View(list.ToList());
        }

        public ActionResult SubCustomePurchaseHistory(DateTime FromDate, DateTime ToDate, int? id)
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
            var list = purchase.CustomePurchaseList(companyid, branchid, FromDate, ToDate);
            if (FromDate == null || ToDate == null)
            {
                ViewBag.Message = "Please select both From and To dates.";
                return RedirectToAction("CustomePurchaseHistory");

            }
            return View(list.ToList());
        }

        public ActionResult PurchaseItemDetail(int? id)
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
            var list = db.tblSupplierInvoiceDetails.Where(d => d.SupplierInvoiceID == id);
            return View(list.ToList());
        }

        public ActionResult PrintPurchaseInvoice(int? id)
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
            var list = db.tblSupplierInvoiceDetails.Where(d => d.SupplierInvoiceID == id);
            return View(list.ToList());
        }
    }
}