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
    public class SaleCartController : Controller
    {
        private CloudErpVEntities db = new CloudErpVEntities();
        private SaleEntry saleEntry = new SaleEntry();

        // GET: SaleCart
        public ActionResult NewSale()
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

            var findpurchase = db.tblSaleCartDetails.Where(i => i.BranchID == branchid && i.CompanyID == companyid && i.UserID == userid);
            foreach (var item in findpurchase)
            {
                totalAmount += (item.SaleQuantity * item.SaleUnitPrice);
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

            var checkQty = db.tblStocks.Find(PID);
            if (Qty > checkQty.Quantity)
            {
                ViewBag.Message = "Sale Quantity Must be Less Then or Equal to AVL Qty!";
                return RedirectToAction("NewSale");
            }

            var findproduct = db.tblSaleCartDetails.Where(i => i.ProductID == PID && i.BranchID == branchid && i.CompanyID == companyid).FirstOrDefault();
            if (findproduct == null)
            {
                if (PID > 0 && Qty > 0 && Price > 0)
                {
                    var newItem = new tblSaleCartDetail()
                    {
                        ProductID = PID,
                        SaleQuantity = Qty,
                        SaleUnitPrice = Price,
                        BranchID = branchid,
                        CompanyID = companyid,
                        UserID = userid,
                    };

                    db.tblSaleCartDetails.Add(newItem);
                    db.SaveChanges();
                    ViewBag.Message = "Item Add Succesfully!";
                }
            }
            else
            {
                ViewBag.Message = "Already Exist! Please Check";
            }

            return RedirectToAction("NewSale");
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
                list.Add(new ProductCartMV { Name = item.ProductName + " <b>(AVL Qty: " + item.Quantity + " )</b>", ProductID = item.ProductID });
            }

            return Json(new { data = list }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetProductDetails(int? id)
        {
            var product = db.tblStocks.Find(id);
            return Json(new { data = product.SaleUnitPrice }, JsonRequestBehavior.AllowGet);
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
            var product = db.tblSaleCartDetails.Find(id);

            if (product != null)
            {
                db.Entry(product).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
                ViewBag.Message = "Item Deleted Successfully!";
                return RedirectToAction("NewSale");
            }

            ViewBag.Message = "Some Unexpected issue is occure, please contact to concern person!";
            var find = db.tblSaleCartDetails.Where(i => i.BranchID == branchid && i.CompanyID == companyid && i.UserID == userid);

            return View(find.ToList());

        }

        public ActionResult CancelSale()
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
            var list = db.tblSaleCartDetails.Where(p => p.BranchID == branchid && p.CompanyID == companyid && p.UserID == userid).ToList();
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
                ViewBag.Message = "Sale is Canceled as per your Request!";
                return RedirectToAction("NewSale");
            }
            ViewBag.Message = "Some Unexpected issue is occure, please contact to concern person!";
            return RedirectToAction("NewSale");
        }

        public ActionResult SelectCustomer()
        {
            Session["SaleErrorMessage"] = string.Empty;
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
            var saledetails = db.tblSaleCartDetails.Where(pd => pd.BranchID == branchid && pd.CompanyID == companyid).FirstOrDefault();
            if (saledetails == null)
            {
                Session["SaleErrorMessage"] = "Sale Cart is Empty!";
                return RedirectToAction("NewSale");
            }
            var customers = db.tblCustomers.Where(s => s.CompanyID == companyid && s.BranchID == branchid).ToList();
            return View(customers);
        }

        [HttpPost]
        public ActionResult SaleConfirm(FormCollection collection)
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

            int customerid = 0;
            bool isPayment = false;
            string[] keys = collection.AllKeys;
            foreach (var name in keys)
            {
                if (name.Contains("name"))
                {
                    string idname = name;
                    string[] valueids = idname.Split(' ');
                    customerid = Convert.ToInt32(valueids[1]);
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
            var customer = db.tblCustomers.Find(customerid);
            var saledetails = db.tblSaleCartDetails.Where(pd => pd.BranchID == branchid && pd.CompanyID == companyid).ToList();
            double totalamount = 0;
            foreach (var item in saledetails)
            {
                totalamount = totalamount + (item.SaleQuantity * item.SaleUnitPrice);
            }
            if (totalamount == 0)
            {
                ViewBag.Message = "Sale Cart is Empty!";
                return View("NewSale");
            }
            string Invoiceno = "INV" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;

            var invoiceHeader = new tblCustomerInvoice()
            {
                BranchID = branchid,
                Title = "Sale Inovice" + customer.Customername,
                CompanyID = companyid,
                Description = Description,
                InvoiceDate = DateTime.Now,
                InvoiceNo = Invoiceno,
                CustomerID = customerid,
                UserID = userid,
                TotalAmount = totalamount,
            };

            db.tblCustomerInvoices.Add(invoiceHeader);
            db.SaveChanges();

            foreach (var item in saledetails)
            {
                var sdetails = new tblCustomerInvoiceDetail()
                {
                    ProductID = item.ProductID,
                    SaleQuantity = item.SaleQuantity,
                    SaleUnitPrice = item.SaleUnitPrice,
                    CustomerInvoiceID = invoiceHeader.CustomerInvoiceID,
                };
                db.tblCustomerInvoiceDetails.Add(sdetails);
                db.SaveChanges();
            }
            string Message = saleEntry.ConfirmSale(companyid, branchid, userid, Invoiceno, invoiceHeader.CustomerInvoiceID.ToString(), (float)totalamount, customerid.ToString(), customer.Customername, isPayment);
            if (Message.Contains("Success"))
            {
                foreach (var item in saledetails)
                {
                    var stockItem = db.tblStocks.Find(item.ProductID);
                    stockItem.Quantity = stockItem.Quantity - item.SaleQuantity;
                    db.Entry(stockItem).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
            }
            if (Message.Contains("Success"))
            {
                return RedirectToAction("PrintSaleInvoice", "SalePayment", new { id = invoiceHeader.CustomerInvoiceID });
            }
            Session["Message"] = Message;
            return RedirectToAction("NewSale");
        }
    }
}