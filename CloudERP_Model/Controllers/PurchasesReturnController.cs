using CloudERP_Model.Models;
using DatabaseAccess;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CloudERP_Model.Controllers
{
    public class PurchasesReturnController : Controller
    {
        private CloudErpVEntities db = new CloudErpVEntities();
        private PurchaseEntry purchaseEntry = new PurchaseEntry();
        // GET: PurchasesReturn
        public ActionResult FindPurchase()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            tblSupplierInvoice invoice;
            if (Session["InvoiceNo"] != null)
            {
                var invoiceno = Convert.ToString(Session["InvoiceNo"]);
                if (!string.IsNullOrEmpty(invoiceno))
                {
                    invoice = db.tblSupplierInvoices.Where(p => p.InvoiceNo == invoiceno.Trim()).FirstOrDefault();
                }
                else
                {
                    invoice = db.tblSupplierInvoices.Find(0);
                }
            }
            else
            {
                invoice = db.tblSupplierInvoices.Find(0);
            }
            return View(invoice);
        }

        [HttpPost]
        public ActionResult FindPurchase(string invoiceid)
        {
            Session["InvoiceNo"] = string.Empty;
            Session["ReturnMessage"] = string.Empty;
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            var purchaseInvoice = db.tblSupplierInvoices.Where(p => p.InvoiceNo == invoiceid).FirstOrDefault();
            return View(purchaseInvoice);
        }

        [HttpPost]
        public ActionResult ReturnPurchaseConfirm(FormCollection collection)
        {
            Session["InvoiceNo"] = string.Empty;
            Session["ReturnMessage"] = string.Empty;
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

            int supplierid = 0;
            int SupplierInvoiceID = 0;
            bool isPayment = false;
            List<int> ProductIDs = new List<int>();
            List<int> ReturnQtys = new List<int>();

            string[] keys = collection.AllKeys;
            foreach (var name in keys)
            {
                if (name.Contains("ProductID "))
                {
                    string idname = name;
                    string[] valueids = idname.Split(' ');
                    ProductIDs.Add(Convert.ToInt32(valueids[1]));
                    ReturnQtys.Add(Convert.ToInt32(collection[idname].Split(',')[0]));
                }
            }

            string Description = "Purchase Return";
            string[] SupplierInvoiceIDs = collection["supplierInvoiceID"].Split(',');
            if (SupplierInvoiceIDs != null)
            {
                if (SupplierInvoiceIDs[0] != null)
                {
                    SupplierInvoiceID = Convert.ToInt32(SupplierInvoiceIDs[0]);
                }
            }
            if (collection["isPayment"] != null)
            {
                string[] isPaymentDirect = collection["isPayment"].Split(',');
                if (isPaymentDirect[0] == "on")
                {
                    isPayment = true;
                }
                else
                {
                    isPayment = false;
                }
            }
            else
            {
                isPayment = false;
            }

            double TotalAmount = 0;
            var purchasedetails = db.tblSupplierInvoiceDetails.Where(pd => pd.SupplierInvoiceID == SupplierInvoiceID).ToList();
            for (int i = 0; i < purchasedetails.Count; i++)
            {
                foreach (var prodid in ProductIDs)
                {
                    if (prodid == purchasedetails[i].ProductID)
                    {
                        TotalAmount += ReturnQtys[i] * purchasedetails[i].purchaseUnitPrice;
                    }
                }
            }
            var supplierinvoice = db.tblSupplierInvoices.Find(SupplierInvoiceID);
            supplierid = supplierinvoice.SupplierID;
            if (TotalAmount == 0)
            {
                Session["InvoiceNo"] = supplierinvoice.InvoiceNo;
                Session["ReturnMessage"] = "Must Be At Least One Product Return Quantity!";
                return RedirectToAction("FindPurchase");
            }

            string Invoiceno = "RPU" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
            var returninvoiceHeader = new tblSupplierReturnInvoice()
            {
                BranchID = branchid,
                CompanyID = companyid,
                Description = Description,
                InvoiceDate = DateTime.Now,
                InvoiceNo = Invoiceno,
                SupplierID = supplierid,
                UserID = userid,
                TotalAmount = TotalAmount,
                SupplierInvoiceID = SupplierInvoiceID
            };

            db.tblSupplierReturnInvoices.Add(returninvoiceHeader);
            db.SaveChanges();
            var supplier = db.tblSuppliers.Find(supplierid);
            string Message = purchaseEntry.ReturnPurchase(companyid, branchid, userid, Invoiceno, returninvoiceHeader.SupplierInvoiceID.ToString(), returninvoiceHeader.SupplierReturnInvoiceID, (float)TotalAmount, supplierid.ToString(), supplier.SupplierName, isPayment);
            if (Message.Contains("Success"))
            {
                for (int i = 0; i < purchasedetails.Count; i++)
                {
                    foreach (var prodid in ProductIDs)
                    {
                        if (prodid == purchasedetails[i].ProductID)
                        {
                            if (ReturnQtys[i] > 0)
                            {
                                var returnproductdetails = new tblSupplierReturnInvoiceDetail();
                                returnproductdetails.SupplierInvoiceID = SupplierInvoiceID;
                                returnproductdetails.PurchaseReturnQuantity = ReturnQtys[i];
                                returnproductdetails.ProductID = prodid;
                                returnproductdetails.purchaseReturnUnitPrice = purchasedetails[i].purchaseUnitPrice;
                                returnproductdetails.SupplierReturnInvoiceID = returninvoiceHeader.SupplierReturnInvoiceID;
                                returnproductdetails.SupplierInvoiceDetailID = purchasedetails[i].SupplierInvoiceDetailID;
                                db.tblSupplierReturnInvoiceDetails.Add(returnproductdetails);
                                db.SaveChanges();

                                var stock = db.tblStocks.Find(prodid);
                                stock.Quantity = (stock.Quantity - ReturnQtys[i]);
                                db.Entry(stock).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();
                            }

                        }
                    }
                }
                Session["InvoiceNo"] = supplierinvoice.InvoiceNo;
                Session["ReturnSuccMessage"] = "Product Returned Suucessfully As Per Your Request!";
                return RedirectToAction("FindPurchase");
            }
            Session["InvoiceNo"] = supplierinvoice.InvoiceNo;
            Session["ReturnMessage"] = "Some unexpected issue is occure please contact to Adminstrator!";
            return RedirectToAction("FindPurchase");
        }
    }
}