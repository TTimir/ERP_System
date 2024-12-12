using DatabaseAccess.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Code.SP_Code
{
    public class SP_Sale
    {
        private CloudErpVEntities db = new CloudErpVEntities();
        public List<SalePaymentModel> RemainingPayment(int CompanyID, int BranchID)
        {
            var remainingPaymentList = new List<SalePaymentModel>();
            SqlCommand cmd = new SqlCommand("GetCustomerRemainingPaymentRecord", DatabaseQuery.ConnOpen());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BranchID", BranchID);
            cmd.Parameters.AddWithValue("@CompanyID", CompanyID);
            var dt = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(dt);
            foreach (DataRow row in dt.Rows)
            {
                int customerid = Convert.ToInt32(Convert.ToString(row[4]));
                var customer = db.tblCustomers.Find(customerid);

                var payment = new SalePaymentModel();
                payment.CustomerInvoiceID = Convert.ToInt32(Convert.ToString(row[0]));
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
                payment.CustomerContactNo = customer.CustomerContact;
                payment.CustomerAddress = customer.CustomerAddress;
                payment.CustomerID = customer.CustomerID;
                payment.CustomerName = customer.Customername;
                payment.TotalAmount = totalamount;
                remainingPaymentList.Add(payment);
            }
            return remainingPaymentList;
        }

        public List<SalePaymentModel> CustomeSaleList(int CompanyID, int BranchID, DateTime FromDate, DateTime ToDate)
        {
            var remainingPaymentList = new List<SalePaymentModel>();
            SqlCommand cmd = new SqlCommand("GetSaleHistory", DatabaseQuery.ConnOpen());
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
                int customerid = Convert.ToInt32(Convert.ToString(row[4]));
                var customer = db.tblCustomers.Find(customerid);

                var payment = new SalePaymentModel();
                payment.CustomerInvoiceID = Convert.ToInt32(Convert.ToString(row[0]));
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
                payment.CustomerContactNo = customer.CustomerContact;
                payment.CustomerAddress = customer.CustomerAddress;
                payment.CustomerID = customer.CustomerID;
                payment.CustomerName = customer.Customername;
                payment.TotalAmount = totalamount;

                payment.ReturnProductAmount = returntotalamount;
                payment.ReturnPaymentAmount = returnpayamount;
                payment.AfterReturnTotalAmount = afterreturntotalamount;
                remainingPaymentList.Add(payment);
            }
            return remainingPaymentList;
        }

        public List<SalePaymentModel> SalePaymentHistory(int CustomerInvoiceID)
        {
            var remainingPaymentList = new List<SalePaymentModel>();
            SqlCommand cmd = new SqlCommand("GetCustomerPaymentHistory", DatabaseQuery.ConnOpen());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustomerInvoiceID", CustomerInvoiceID);
            var dt = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(dt);
            foreach (DataRow row in dt.Rows)
            {
                int customerid = Convert.ToInt32(Convert.ToString(row[4]));
                int userid = Convert.ToInt32(Convert.ToString(row[9]));
                var customer = db.tblCustomers.Find(customerid);
                var user = db.tblUsers.Find(userid);

                var payment = new SalePaymentModel();
                payment.CustomerInvoiceID = Convert.ToInt32(Convert.ToString(row[0]));
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
                payment.CustomerContactNo = customer.CustomerContact;
                payment.CustomerAddress = customer.CustomerAddress;
                payment.CustomerID = customer.CustomerID;
                payment.CustomerName = customer.Customername;
                payment.TotalAmount = totalamount;
                payment.UserID = user.UserID;
                payment.UserName = user.UserName;
                remainingPaymentList.Add(payment);
            }
            return remainingPaymentList;
        }

        public List<SalePaymentModel> GetReturnSaleAmountPending(int BranchID, int CompanyID)
        {
            var remainingPaymentList = new List<SalePaymentModel>();
            SqlCommand cmd = new SqlCommand("GetReturnSaleAmountPending", DatabaseQuery.ConnOpen());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BranchID", BranchID);
            cmd.Parameters.AddWithValue("@CompanyID", CompanyID);
            var dt = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(dt);
            foreach (DataRow row in dt.Rows)
            {
                int customerid = Convert.ToInt32(Convert.ToString(row[4]));
                var customer = db.tblCustomers.Find(customerid);

                var payment = new SalePaymentModel();
                payment.CustomerInvoiceID = Convert.ToInt32(Convert.ToString(row[0]));
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
                payment.CustomerContactNo = customer.CustomerContact;
                payment.CustomerAddress = customer.CustomerAddress;
                payment.CustomerID = customer.CustomerID;
                payment.CustomerName = customer.Customername;
                payment.TotalAmount = totalamount;

                payment.ReturnProductAmount = returntotalamount;
                payment.ReturnPaymentAmount = returnpayamount;
                payment.AfterReturnTotalAmount = afterreturntotalamount;
                remainingPaymentList.Add(payment);
            }
            return remainingPaymentList;
        }

        public List<CustomerReturnInvoiceModel> SaleReturnAmountPending(int? CustomerInvoiceID)
        {
            var remainingPaymentList = new List<CustomerReturnInvoiceModel>();
            SqlCommand cmd = new SqlCommand("GetCustomerReturnSalePaidPending", DatabaseQuery.ConnOpen());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustomerInvoiceID", (int)CustomerInvoiceID);
            var dt = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(dt);
            foreach (DataRow row in dt.Rows)
            {
                int customerid = Convert.ToInt32(Convert.ToString(row[5]));
                int userid = Convert.ToInt32(Convert.ToString(row[10]));
                var customer = db.tblCustomers.Find(customerid);
                var user = db.tblUsers.Find(userid);

                var payment = new CustomerReturnInvoiceModel();

                payment.CustomerReturnInvoiceID = Convert.ToInt32(Convert.ToString(row[0]));
                payment.CustomerInvoiceID = Convert.ToInt32(Convert.ToString(row[1]));
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
                payment.CustomerContactNo = customer.CustomerContact;
                payment.CustomerAddress = customer.CustomerAddress;
                payment.CustomerID = customer.CustomerID;
                payment.CustomerName = customer.Customername;
                payment.ReturnTotalAmount = totalamount;
                payment.UserID = user.UserID;
                payment.UserName = user.UserName;
                remainingPaymentList.Add(payment);
            }
            return remainingPaymentList;
        }
    }
}
