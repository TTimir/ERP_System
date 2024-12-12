using CloudERP_Model.Models;
using DatabaseAccess;
using DatabaseAccess.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CloudERP_Model.Controllers
{
    public class SaleReturnController : Controller
    {
        private CloudErpVEntities db = new CloudErpVEntities();
        private SaleEntry saleEntry = new SaleEntry();
        // GET: SaleReturn
        public ActionResult FindSale()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            tblCustomerInvoice invoice;
            if (Session["SaleInvoiceNo"] != null)
            {
                var invoiceno = Convert.ToString(Session["SaleInvoiceNo"]);
                if (!string.IsNullOrEmpty(invoiceno))
                {
                    invoice = db.tblCustomerInvoices.Where(p => p.InvoiceNo == invoiceno.Trim()).FirstOrDefault();
                }
                else
                {
                    invoice = db.tblCustomerInvoices.Find(0);
                }
            }
            else
            {
                invoice = db.tblCustomerInvoices.Find(0);
            }
            return View(invoice);
        }

        [HttpPost]
        public ActionResult FindSale(string invoiceid)
        {
            Session["SaleInvoiceNo"] = string.Empty;
            Session["SaleReturnMessage"] = string.Empty;
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            var saleInvoice = db.tblCustomerInvoices.Where(p => p.InvoiceNo == invoiceid).FirstOrDefault();
            return View(saleInvoice);
        }

        [HttpPost]
        public ActionResult ReturnSaleConfirm(FormCollection collection)
        {
            Session["SaleInvoiceNo"] = string.Empty;
            Session["SaleReturnMessage"] = string.Empty;
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

            int customerid = 0;
            int CustomerInvoiceID = 0;
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

            string Description = "Sale Return";
            string[] CustomerInvoiceIDs = collection["customerInvoiceID"].Split(',');
            if (CustomerInvoiceIDs != null)
            {
                if (CustomerInvoiceIDs[0] != null)
                {
                    CustomerInvoiceID = Convert.ToInt32(CustomerInvoiceIDs[0]);
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
            var saledetails = db.tblCustomerInvoiceDetails.Where(pd => pd.CustomerInvoiceID == CustomerInvoiceID).ToList();
            for (int i = 0; i < saledetails.Count; i++)
            {
                foreach (var prodid in ProductIDs)
                {
                    if (prodid == saledetails[i].ProductID)
                    {
                        TotalAmount += ReturnQtys[i] * saledetails[i].SaleUnitPrice;
                    }
                }
            }
            var customerinvoice = db.tblCustomerInvoices.Find(CustomerInvoiceID);
            customerid = customerinvoice.CustomerID;
            if (TotalAmount == 0)
            {
                Session["SaleInvoiceNo"] = customerinvoice.InvoiceNo;
                Session["SaleReturnMessage"] = "Must Be At Least One Product Return Quantity!";
                return RedirectToAction("FindSale");
            }

            string Invoiceno = "RIN" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
            var returninvoiceHeader = new tblCustomerReturnInvoice()
            {
                BranchID = branchid,
                CompanyID = companyid,
                Description = Description,
                InvoiceDate = DateTime.Now,
                InvoiceNo = Invoiceno,
                CustomerID = customerid,
                UserID = userid,
                TotalAmount = TotalAmount,
                CustomerInvoiceID = CustomerInvoiceID
            };

            db.tblCustomerReturnInvoices.Add(returninvoiceHeader);
            db.SaveChanges();
            var customer = db.tblCustomers.Find(customerid);
            string Message = saleEntry.ReturnSale(companyid, branchid, userid, Invoiceno, returninvoiceHeader.CustomerInvoiceID.ToString(), returninvoiceHeader.CustomerReturnInvoiceID, (float)TotalAmount, customerid.ToString(), customer.Customername, isPayment);
            if (Message.Contains("Success"))
            {
                for (int i = 0; i < saledetails.Count; i++)
                {
                    foreach (var prodid in ProductIDs)
                    {
                        if (prodid == saledetails[i].ProductID)
                        {
                            if (ReturnQtys[i] > 0)
                            {
                                var returnsaledetails = new tblCustomerReturnInvoiceDetail();
                                returnsaledetails.CustomerInvoiceID = CustomerInvoiceID;
                                returnsaledetails.SaleReturnQuantity = ReturnQtys[i];
                                returnsaledetails.ProductID = prodid;
                                returnsaledetails.SaleReturnUnitPrice = saledetails[i].SaleUnitPrice;
                                returnsaledetails.CustomerReturnInvoiceID = returninvoiceHeader.CustomerReturnInvoiceID;
                                returnsaledetails.CustomerInvoiceDetailID = saledetails[i].CustomerInvoiceDetailID;
                                db.tblCustomerReturnInvoiceDetails.Add(returnsaledetails);
                                db.SaveChanges();

                                var stock = db.tblStocks.Find(prodid);
                                stock.Quantity = (stock.Quantity + ReturnQtys[i]);
                                db.Entry(stock).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();
                            }

                        }
                    }
                }
                Session["SaleInvoiceNo"] = customerinvoice.InvoiceNo;
                Session["ReturnSuccMessage"] = "Product Returned Suucessfully As Per Your Request!";
                return RedirectToAction("FindSale");
            }
            Session["SaleInvoiceNo"] = customerinvoice.InvoiceNo;
            Session["SaleReturnMessage"] = "Some unexpected issue is occure please contact to Adminstrator!";
            return RedirectToAction("FindSale");
        }
    }
}