using CloudERP_Model.Models;
using DatabaseAccess;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace CloudERP_Model.Controllers
{
    public class PurchaseCartController : Controller
    {
        private CloudErpVEntities db = new CloudErpVEntities();
        private PurchaseEntry purchaseEntry = new PurchaseEntry();

        // GET: PurchaseCart
        public ActionResult NewPurchase()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            double totalAmount = 0;
            int companyid = 0;
            int branchid = 0;
            int userid = 0;
            branchid = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            var findpurchase = db.tblPurchaseCartDetails.Where(i => i.BranchID == branchid && i.CompanyID == companyid && i.UserID == userid);
            foreach (var item in findpurchase)
            {
                totalAmount += (item.PurchaseQuantity * item.purchaseUnitPrice);
            }
            ViewBag.TotalAmount = totalAmount;

            return View(findpurchase.ToList());
        }

        [HttpPost]
        public ActionResult AddItem(int PID, int Qty, float Price)
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

            var findproduct = db.tblPurchaseCartDetails.Where(i => i.ProductID == PID && i.BranchID == branchid && i.CompanyID == companyid).FirstOrDefault();
            if (findproduct == null)
            {
                if (PID > 0 && Qty > 0 && Price > 0)
                {
                    var newItem = new tblPurchaseCartDetail()
                    {
                        ProductID = PID,
                        PurchaseQuantity = Qty,
                        purchaseUnitPrice = Price,
                        BranchID = branchid,
                        CompanyID = companyid,
                        UserID = userid,
                    };
                    db.tblPurchaseCartDetails.Add(newItem);
                    db.SaveChanges();
                    ViewBag.Message = "Item Add Succesfully!";
                }
            }
            else
            {
                ViewBag.Message = "Already Exist! Please Check";
            }

            return RedirectToAction("NewPurchase");
        }

        [HttpGet]
        public ActionResult GetProduct()
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
            List<ProductCartMV> list = new List<ProductCartMV>();
            var productList = db.tblStocks.Where(p => p.BranchID == branchid && p.CompanyID == companyid).ToList();
            foreach (var item in productList)
            {
                list.Add(new ProductCartMV { Name = item.ProductName, ProductID = item.ProductID });
            }

            return Json(new { data = list }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteConfirm(int? id)
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
            var product = db.tblPurchaseCartDetails.Find(id);

            if (product != null)
            {
                db.Entry(product).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
                ViewBag.Message = "Item Deleted Successfully!";
                return RedirectToAction("NewPurchase");
            }

            ViewBag.Message = "Some Unexpected issue is occure, please contact to concern person!";
            var find = db.tblPurchaseCartDetails.Where(i => i.BranchID == branchid && i.CompanyID == companyid && i.UserID == userid);

            return View(find.ToList());

        }
        public ActionResult CancelPurchase()
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
            var list = db.tblPurchaseCartDetails.Where(p => p.BranchID == branchid && p.CompanyID == companyid && p.UserID == userid).ToList();
            bool cancelStatus = false;
            foreach (var item in list)
            {
                db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                int noOfRecored = db.SaveChanges();
                if (cancelStatus == false)
                {
                    if (noOfRecored > 0)
                    {
                        cancelStatus = true;
                    }
                }
            }

            if (cancelStatus == true)
            {
                ViewBag.Message = "Purchase is Canceled as per your Request!";
                return RedirectToAction("NewPurchase");
            }
            ViewBag.Message = "Some Unexpected issue is occure, please contact to concern person!";
            return RedirectToAction("NewPurchase");
        }

        public ActionResult SelectSupplier()
        {
            Session["PurErrorMessage"] = string.Empty;
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            int companyid = 0;
            int branchid = 0;
            int userid = 0;
            branchid = Convert.ToInt32(Session["BranchID"].ToString());
            companyid = Convert.ToInt32(Session["CompanyID"].ToString());
            userid = Convert.ToInt32(Session["UserID"].ToString());

            var checkpurchasecart = db.tblPurchaseCartDetails.Where(pd => pd.BranchID == branchid && pd.CompanyID == companyid).FirstOrDefault();
            if (checkpurchasecart == null)
            {
                Session["PurErrorMessage"] = "Purchase Cart is Empty!";
                return RedirectToAction("NewPurchase");
            }
            var suppliers = db.tblSuppliers.Where(s => s.CompanyID == companyid && s.BranchID == branchid).ToList();
            return View(suppliers);
        }

        [HttpPost]
        public ActionResult PurchaseConfirm(FormCollection collection)
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

            int supplierid = 0;
            bool isPayment = false;
            string[] keys = collection.AllKeys;
            foreach (var name in keys)
            {
                if (name.Contains("name"))
                {
                    string idname = name;
                    string[] valueids = idname.Split(' ');
                    supplierid = Convert.ToInt32(valueids[1]);
                }
            }
            string Description = string.Empty;
            string[] Descriptionlist = collection["item.Description"].Split(',');
            if (Descriptionlist != null)
            {
                if (Descriptionlist[0] != null)
                {
                    Description = Descriptionlist[0];
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
            var supplier = db.tblSuppliers.Find(supplierid);
            var purchasedetails = db.tblPurchaseCartDetails.Where(pd => pd.BranchID == branchid && pd.CompanyID == companyid).ToList();
            double totalamount = 0;
            foreach (var item in purchasedetails)
            {
                totalamount = totalamount + (item.PurchaseQuantity * item.purchaseUnitPrice);
            }
            if (totalamount == 0)
            {
                ViewBag.Message = "Purchase Cart is Empty!";
                return View("NewPurchase");
            }
            string Invoiceno = "PUR" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;

            var invoiceHeader = new tblSupplierInvoice()
            {
                BranchID = branchid,
                CompanyID = companyid,
                Description = Description,
                InvoiceDate = DateTime.Now,
                InvoiceNo = Invoiceno,
                SupplierID = supplierid,
                UserID = userid,
                TotalAmount = totalamount,
            };

            db.tblSupplierInvoices.Add(invoiceHeader);
            db.SaveChanges();

            foreach (var item in purchasedetails)
            {
                var purdetails = new tblSupplierInvoiceDetail()
                {
                    ProductID = item.ProductID,
                    PurchaseQuantity = item.PurchaseQuantity,
                    purchaseUnitPrice = item.purchaseUnitPrice,
                    SupplierInvoiceID = invoiceHeader.SupplierInvoiceID,
                };
                db.tblSupplierInvoiceDetails.Add(purdetails);
                db.SaveChanges();
            }
            string Message = purchaseEntry.ConfirmPurchase(companyid, branchid, userid, Invoiceno, invoiceHeader.SupplierInvoiceID.ToString(), (float)totalamount, supplierid.ToString(), supplier.SupplierName, isPayment);
            if (Message.Contains("Success"))
            {
                foreach (var item in purchasedetails)
                {
                    var stockItem = db.tblStocks.Find(item.ProductID);
                    stockItem.CurrentPurchaseUnitPrice = item.purchaseUnitPrice;
                    stockItem.Quantity = stockItem.Quantity + item.PurchaseQuantity;
                    db.Entry(stockItem).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
            }
            if (Message.Contains("Success"))
            {
                return RedirectToAction("PrintPurchaseInvoice", "PurchasePayment", new { id = invoiceHeader.SupplierInvoiceID });
            }
            Session["Message"] = Message;
            return RedirectToAction("NewPurchase");
        }

    }
}

