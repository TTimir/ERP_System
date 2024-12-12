using DatabaseAccess.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Code.SP_Code
{
    public class SP_Purchase
    {
        private CloudErpVEntities db = new CloudErpVEntities();
        public List<PurchasePaymentModel> RemainingPayment(int CompanyID, int BranchID)
        {
            var remainingPaymentList = new List<PurchasePaymentModel>();
            SqlCommand cmd = new SqlCommand("GetSupplierRemainingPaymentRecord", DatabaseQuery.ConnOpen());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BranchID", BranchID);
            cmd.Parameters.AddWithValue("@CompanyID", CompanyID);
            var dt = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(dt);
            foreach (DataRow row in dt.Rows)
            {
                int supplierid = Convert.ToInt32(Convert.ToString(row[4]));
                var supplier = db.tblSuppliers.Find(supplierid);

                var payment = new PurchasePaymentModel();
                payment.SupplierInvoiceID = Convert.ToInt32(Convert.ToString(row[0]));
                payment.BranchID = Convert.ToInt32(Convert.ToString(row[1]));
                payment.CompanyID = Convert.ToInt32(Convert.ToString(row[2]));
                payment.InvoiceDate = Convert.ToDateTime(Convert.ToString(row[3]));
                payment.InvoiceNo = Convert.ToString(row[5]);
                double totalamount = 0;
                double.TryParse(Convert.ToString(row[6]), out totalamount);
                double returntotalamount = 0;
                double.TryParse(Convert.ToString(row[7]), out returntotalamount);
                double afterreturntotalamount = 0;
                double.TryParse(Convert.ToString(row[8]), out afterreturntotalamount);
                double payamount = 0;
                double.TryParse(Convert.ToString(row[9]), out payamount);
                double returnpayamount = 0;
                double.TryParse(Convert.ToString(row[10]), out returnpayamount);
                double remainingbalance = 0;
                double.TryParse(Convert.ToString(row[11]), out remainingbalance);

                payment.PaymentAmount = payamount;
                payment.RemainingBalance = remainingbalance;
                payment.SupplierContactNo = supplier.SupplierConatctNo;
                payment.SupplierAddress = supplier.SupplierAddress;
                payment.SupplierID = supplier.SupplierID;
                payment.SupplierName = supplier.SupplierName;
                payment.TotalAmount = totalamount;
                
                payment.ReturnProductAmount = returntotalamount;
                payment.ReturnPaymentAmount = returnpayamount;
                payment.AfterReturnTotalAmount = afterreturntotalamount;
                remainingPaymentList.Add(payment);
            }
            return remainingPaymentList;
        }

        public List<PurchasePaymentModel> CustomePurchaseList(int CompanyID, int BranchID, DateTime FromDate, DateTime ToDate)
        {
            var remainingPaymentList = new List<PurchasePaymentModel>();
            SqlCommand cmd = new SqlCommand("GetPurchaseHistory", DatabaseQuery.ConnOpen());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BranchID", BranchID);
            cmd.Parameters.AddWithValue("@CompanyID", CompanyID);
            cmd.Parameters.AddWithValue("@FromDate", FromDate.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@ToDate", ToDate.ToString("yyyy-MM-dd"));
            var dt = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(dt);
            foreach (DataRow row in dt.Rows)
            {
                int supplierid = Convert.ToInt32(Convert.ToString(row[4]));
                var supplier = db.tblSuppliers.Find(supplierid);

                var payment = new PurchasePaymentModel();
                payment.SupplierInvoiceID = Convert.ToInt32(Convert.ToString(row[0]));
                payment.BranchID = Convert.ToInt32(Convert.ToString(row[1]));
                payment.CompanyID = Convert.ToInt32(Convert.ToString(row[2]));
                payment.InvoiceDate = Convert.ToDateTime(Convert.ToString(row[3]));
                payment.InvoiceNo = Convert.ToString(row[5]);
                double totalamount = 0;
                double.TryParse(Convert.ToString(row[6]), out totalamount);
                double returntotalamount = 0;
                double.TryParse(Convert.ToString(row[7]), out returntotalamount);
                double afterreturntotalamount = 0;
                double.TryParse(Convert.ToString(row[8]), out afterreturntotalamount);
                double payamount = 0;
                double.TryParse(Convert.ToString(row[9]), out payamount);
                double returnpayamount = 0;
                double.TryParse(Convert.ToString(row[10]), out returnpayamount);
                double remainingbalance = 0;
                double.TryParse(Convert.ToString(row[11]), out remainingbalance);

                payment.PaymentAmount = payamount;
                payment.RemainingBalance = remainingbalance;
                payment.SupplierContactNo = supplier.SupplierConatctNo;
                payment.SupplierAddress = supplier.SupplierAddress;
                payment.SupplierID = supplier.SupplierID;
                payment.SupplierName = supplier.SupplierName;
                payment.TotalAmount = totalamount;
                
                payment.ReturnProductAmount = returntotalamount;
                payment.ReturnPaymentAmount = returnpayamount;
                payment.AfterReturnTotalAmount = afterreturntotalamount;
                remainingPaymentList.Add(payment);
            }
            return remainingPaymentList;
        }

        public List<PurchasePaymentModel> PurchasePaymentHistory(int SupplierInvoiceID)
        {
            var remainingPaymentList = new List<PurchasePaymentModel>();
            SqlCommand cmd = new SqlCommand("GetSupplierPaymentHistory", DatabaseQuery.ConnOpen());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SupplierInvoiceID", SupplierInvoiceID);
            var dt = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(dt);
            foreach (DataRow row in dt.Rows)
            {
                int supplierid = Convert.ToInt32(Convert.ToString(row[4]));
                int userid = Convert.ToInt32(Convert.ToString(row[9]));
                var supplier = db.tblSuppliers.Find(supplierid);
                var user = db.tblUsers.Find(userid);

                var payment = new PurchasePaymentModel();
                payment.SupplierInvoiceID = Convert.ToInt32(Convert.ToString(row[0]));
                payment.BranchID = Convert.ToInt32(Convert.ToString(row[1]));
                payment.CompanyID = Convert.ToInt32(Convert.ToString(row[2]));
                payment.InvoiceDate = Convert.ToDateTime(Convert.ToString(row[3]));
                payment.InvoiceNo = Convert.ToString(row[5]);
                double payamount = 0;
                double.TryParse(Convert.ToString(row[7]), out payamount);
                double remainingbalance = 0;
                double.TryParse(Convert.ToString(row[8]), out remainingbalance);
                double totalamount = 0;
                double.TryParse(Convert.ToString(row[6]), out totalamount);

                payment.PaymentAmount = payamount;
                payment.RemainingBalance = remainingbalance;
                payment.SupplierContactNo = supplier.SupplierConatctNo;
                payment.SupplierAddress = supplier.SupplierAddress;
                payment.SupplierID = supplier.SupplierID;
                payment.SupplierName = supplier.SupplierName;
                payment.TotalAmount = totalamount;
                payment.UserID = user.UserID;
                payment.UserName = user.UserName;
                remainingPaymentList.Add(payment);
            }
            return remainingPaymentList;
        }

        public List<SupplierReturnInvoiceModel> PurchaseReturnPaymentPending(int? SupplierInvoiceID)
        {
            var remainingPaymentList = new List<SupplierReturnInvoiceModel>();
            SqlCommand cmd = new SqlCommand("GetSupplierReturnPurchasePaymentPending", DatabaseQuery.ConnOpen());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SupplierInvoiceID", (int)SupplierInvoiceID);
            var dt = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(dt);
            foreach (DataRow row in dt.Rows)
            {
                int supplierid = Convert.ToInt32(Convert.ToString(row[5]));
                int userid = Convert.ToInt32(Convert.ToString(row[10]));
                var supplier = db.tblSuppliers.Find(supplierid);
                var user = db.tblUsers.Find(userid);

                var payment = new SupplierReturnInvoiceModel();

                payment.SupplierReturnInvoiceID = Convert.ToInt32(Convert.ToString(row[0]));
                payment.SupplierInvoiceID = Convert.ToInt32(Convert.ToString(row[1]));
                payment.BranchID = Convert.ToInt32(Convert.ToString(row[2]));
                payment.CompanyID = Convert.ToInt32(Convert.ToString(row[3]));
                payment.InvoiceDate = Convert.ToDateTime(Convert.ToString(row[4]));
                payment.InvoiceNo = Convert.ToString(row[6]);
                double payamount = 0;
                double.TryParse(Convert.ToString(row[8]), out payamount);
                double remainingbalance = 0;
                double.TryParse(Convert.ToString(row[9]), out remainingbalance);
                double totalamount = 0;
                double.TryParse(Convert.ToString(row[7]), out totalamount);

                payment.ReturnPaymentAmount = payamount;
                payment.RemainingBalance = remainingbalance;
                payment.SupplierContactNo = supplier.SupplierConatctNo;
                payment.SupplierAddress = supplier.SupplierAddress;
                payment.SupplierID = supplier.SupplierID;
                payment.SupplierName = supplier.SupplierName;
                payment.ReturnTotalAmount = totalamount;
                payment.UserID = user.UserID;
                payment.UserName = user.UserName;
                remainingPaymentList.Add(payment);
            }
            return remainingPaymentList;
        }

        public List<PurchasePaymentModel> GetReturnPurchasePaymentPending(int BranchID, int CompanyID)
        {
            var remainingPaymentList = new List<PurchasePaymentModel>();
            SqlCommand cmd = new SqlCommand("GetReturnPurchasePaymentPending", DatabaseQuery.ConnOpen());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BranchID", BranchID);
            cmd.Parameters.AddWithValue("@CompanyID", CompanyID);
            var dt = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(dt);
            foreach (DataRow row in dt.Rows)
            {
                int supplierid = Convert.ToInt32(Convert.ToString(row[4]));
                var supplier = db.tblSuppliers.Find(supplierid);

                var payment = new PurchasePaymentModel();
                payment.SupplierInvoiceID = Convert.ToInt32(Convert.ToString(row[0]));
                payment.BranchID = Convert.ToInt32(Convert.ToString(row[1]));
                payment.CompanyID = Convert.ToInt32(Convert.ToString(row[2]));
                payment.InvoiceDate = Convert.ToDateTime(Convert.ToString(row[3]));
                payment.InvoiceNo = Convert.ToString(row[5]);
                double totalamount = 0;
                double.TryParse(Convert.ToString(row[6]), out totalamount);
                double returntotalamount = 0;
                double.TryParse(Convert.ToString(row[7]), out returntotalamount);
                double afterreturntotalamount = 0;
                double.TryParse(Convert.ToString(row[8]), out afterreturntotalamount);
                double payamount = 0;
                double.TryParse(Convert.ToString(row[9]), out payamount);
                double returnpayamount = 0;
                double.TryParse(Convert.ToString(row[10]), out returnpayamount);
                double remainingbalance = 0;
                double.TryParse(Convert.ToString(row[11]), out remainingbalance);

                payment.PaymentAmount = payamount;
                payment.RemainingBalance = remainingbalance;
                payment.SupplierContactNo = supplier.SupplierConatctNo;
                payment.SupplierAddress = supplier.SupplierAddress;
                payment.SupplierID = supplier.SupplierID;
                payment.SupplierName = supplier.SupplierName;
                payment.TotalAmount = totalamount;

                payment.ReturnProductAmount = returntotalamount;
                payment.ReturnPaymentAmount = returnpayamount;
                payment.AfterReturnTotalAmount = afterreturntotalamount;
                remainingPaymentList.Add(payment);
            }
            return remainingPaymentList;
        }
    }
}
