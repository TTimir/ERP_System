using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Code
{
    public class SaleEntry
    {
        private CloudErpVEntities db = new CloudErpVEntities();
        private string connectionString = @"data source=TIMIRBHINGRADIY;initial catalog=CloudErpV;user id=sa;password=sqlserver;trustservercertificate=True;";
        public string selectcustomerid = string.Empty;
        DataTable dtEntries = null;
        public string ConfirmSale(int CompanyID, int BranchID, int UserID, string InvoiceNo, string CustomerInvoiceID, float Amount, string CustomerID, string Customername, bool isPayment)
        {
            try
            {
                dtEntries = null;
                string saletitle = "Sale To " + Customername.Trim();
                var financialCheck = DatabaseQuery.Retrive("select top 1 FinancialYearID from tblFinancialYear where IsActive = 1");
                string FinancialYearID = (financialCheck != null ? Convert.ToString(financialCheck.Rows[0][0]) : string.Empty);
                if (string.IsNullOrEmpty(FinancialYearID))
                {
                    return "Your Company Financial Year is not Set, Please Contact to Adminstrator!";
                }

                string successmessage = "Sale Success";

                string AccountHeadID = string.Empty;
                string AccountControlID = string.Empty;
                string AccountSubControlID = string.Empty;
                // Assests 1      increae(Debit)   decrese(Credit)
                // Liabilities 2     increae(Credit)   decrese(Debit)
                // Expenses 3     increae(Debit)   decrese(Credit)
                // Capital 4     increae(Credit)   decrese(Debit)
                // Revenue 5     increae(Credit)   decrese(Debit)

                var saleAccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 1 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                if (saleAccount == null || saleAccount.AccountHeadID == null || saleAccount.AccountControlID == null || saleAccount.AccountSubControlID == null)
                {
                    return "Sale Return Account Activity (i.e. Sale) entry not found in your Account Flow.";
                }
                //Credit Entry Sale
                AccountHeadID = Convert.ToString(saleAccount.AccountHeadID);
                AccountControlID = Convert.ToString(saleAccount.AccountControlID);
                AccountSubControlID = Convert.ToString(saleAccount.AccountSubControlID);

                string transectiontitle = string.Empty;

                transectiontitle = "Sale To " + Customername.Trim();
                //SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(), "0", Convert.ToString(Amount), DateTime.Now, transectiontitle);
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, CompanyID.ToString(), BranchID.ToString(), InvoiceNo, UserID.ToString(), Convert.ToString(Amount), "0", DateTime.Now, transectiontitle);


                //Debit Entry Sale
                saleAccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 11 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                if (saleAccount == null || saleAccount.AccountHeadID == null || saleAccount.AccountControlID == null || saleAccount.AccountSubControlID == null)
                {
                    return "Sale Return Account Activity (i.e. Sale Payment Pending) entry not found in your Account Flow.";
                }
                AccountHeadID = Convert.ToString(saleAccount.AccountHeadID);
                AccountControlID = Convert.ToString(saleAccount.AccountControlID);
                AccountSubControlID = Convert.ToString(saleAccount.AccountSubControlID);

                transectiontitle = Customername.Trim() + " , Sale Payment is Pending!";
                //SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(), "0", Convert.ToString(Amount), DateTime.Now, transectiontitle);
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, CompanyID.ToString(), BranchID.ToString(), InvoiceNo, UserID.ToString(), "0", Convert.ToString(Amount),  DateTime.Now, transectiontitle);

                if (isPayment == true)
                {
                    string payinvoicenno = "INP" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;

                    saleAccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 11 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                    if (saleAccount == null || saleAccount.AccountHeadID == null || saleAccount.AccountControlID == null || saleAccount.AccountSubControlID == null)
                    {
                        return "Sale Account Activity (i.e. Sale Payment Pending) entry not found in your Account Flow.";
                    }
                    AccountHeadID = Convert.ToString(saleAccount.AccountHeadID);
                    AccountControlID = Convert.ToString(saleAccount.AccountControlID);
                    AccountSubControlID = Convert.ToString(saleAccount.AccountSubControlID);

                    transectiontitle = "Sale Payment Paid By " + Customername;
                    SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, CompanyID.ToString(), BranchID.ToString(), payinvoicenno, UserID.ToString(), Convert.ToString(Amount), "0", DateTime.Now, transectiontitle);

                    saleAccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 12 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                    if (saleAccount == null || saleAccount.AccountHeadID == null || saleAccount.AccountControlID == null || saleAccount.AccountSubControlID == null)
                    {
                        return "Sale Account Activity (i.e. Sale Payment Succeed) entry not found in your Account Flow.";
                    }
                    AccountHeadID = Convert.ToString(saleAccount.AccountHeadID);
                    AccountControlID = Convert.ToString(saleAccount.AccountControlID);
                    AccountSubControlID = Convert.ToString(saleAccount.AccountSubControlID);
                    transectiontitle = Customername + " , Sale Payment is Succesed!";
                    SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, CompanyID.ToString(), BranchID.ToString(), payinvoicenno, UserID.ToString(), "0", Convert.ToString(Amount), DateTime.Now, transectiontitle);

                    //string paymentquery = string.Format("insert into tblCustomerPayment(CustomerID,CustomerInvoiceID,UserID,invoiceNo,TotalAmount,PaidAmount,RemainingBalance,CompanyID,BranchID) " +
                    //"values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')",
                    //CustomerID, CustomerInvoiceID, UserID, payinvoicenno, Amount, Amount, "0", CompanyID, BranchID);
                    //DatabaseQuery.Insert(paymentquery);

                    // Insert payment record
                    string paymentQuery = "INSERT INTO tblCustomerPayment(CustomerID, CustomerInvoiceID, UserID, InvoiceNo, TotalAmount, PaidAmount, RemainingBalance, CompanyID, BranchID, InvoiceDate) " +
                             "VALUES (@CustomerID, @CustomerInvoiceID, @UserID, @InvoiceNo, @TotalAmount, @PaidAmount, @RemainingBalance, @CompanyID, @BranchID, @InvoiceDate)";

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(paymentQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@CustomerID", CustomerID);
                            cmd.Parameters.AddWithValue("@CustomerInvoiceID", CustomerInvoiceID);
                            cmd.Parameters.AddWithValue("@UserID", UserID);
                            cmd.Parameters.AddWithValue("@InvoiceNo", payinvoicenno);
                            cmd.Parameters.AddWithValue("@TotalAmount", Amount);
                            cmd.Parameters.AddWithValue("@PaidAmount", Amount);
                            cmd.Parameters.AddWithValue("@RemainingBalance", "0");
                            cmd.Parameters.AddWithValue("@CompanyID", CompanyID);
                            cmd.Parameters.AddWithValue("@BranchID", BranchID);
                            cmd.Parameters.AddWithValue("@InvoiceDate", DateTime.Now.ToString("yyyy/MM/dd"));

                            cmd.ExecuteNonQuery();
                        }
                    }

                    successmessage = successmessage + " with Payment.";
                }

                //foreach (DataRow entryrow in dtEntries.Rows)
                //{
                //    string entryquery = string.Format("insert into tblTransaction(FinancialYearID,AccountHeadID,AccountControlID,AccountSubControlID,UserID,InvoiceNo,Credit,Debit,TransectionDate,TransactionTitle,CompanyID,BranchID) values" +
                //                        "('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}')",
                //                        Convert.ToString(entryrow[0]), Convert.ToString(entryrow[1]), Convert.ToString(entryrow[2]), Convert.ToString(entryrow[3]), Convert.ToString(entryrow[4]), Convert.ToString(entryrow[5]), Convert.ToString(entryrow[6]), Convert.ToString(entryrow[7]), (entryrow[8]), Convert.ToString(entryrow[9]), CompanyID, BranchID);
                //    DatabaseQuery.Insert(entryquery);
                //}

                // Insert transactions
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    foreach (DataRow entryRow in dtEntries.Rows)
                    {
                        string entryQuery = "INSERT INTO tblTransaction (FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, UserID, InvoiceNo, Credit, Debit, TransectionDate, TransectionTitle, CompanyID, BranchID) " +
                            "VALUES (@FinancialYearID, @AccountHeadID, @AccountControlID, @AccountSubControlID, @UserID, @InvoiceNo, @Credit, @Debit, @TransectionDate, @TransectionTitle, @CompanyID, @BranchID)";

                        using (SqlCommand cmd = new SqlCommand(entryQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@FinancialYearID", entryRow[0]);
                            cmd.Parameters.AddWithValue("@AccountHeadID", entryRow[1]);
                            cmd.Parameters.AddWithValue("@AccountControlID", entryRow[2]);
                            cmd.Parameters.AddWithValue("@AccountSubControlID", entryRow[3]);
                            cmd.Parameters.AddWithValue("@UserID", UserID);
                            cmd.Parameters.AddWithValue("@InvoiceNo", entryRow[6]);

                            float credit = 0, debit = 0;
                            float.TryParse(entryRow[8].ToString(), out credit);
                            float.TryParse(entryRow[9].ToString(), out debit);

                            cmd.Parameters.AddWithValue("@Credit", credit);
                            cmd.Parameters.AddWithValue("@Debit", debit);
                            cmd.Parameters.AddWithValue("@TransectionDate", entryRow[10]);
                            cmd.Parameters.AddWithValue("@TransectionTitle", entryRow[11]);
                            cmd.Parameters.AddWithValue("@CompanyID", CompanyID);
                            cmd.Parameters.AddWithValue("@BranchID", BranchID);

                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                return successmessage;
            }
            catch (Exception ex)
            {
                return $"Unexpected Error: {ex.Message}. Please try Again!";
            }
        }

        public string SalePayment(int CompanyID, int BranchID, int UserID, string invoiceNo, string CustomerInvoiceID, float TotalAmount, float Amount, string CustomerID, string Customername, float RemainingBalance)
        {
            try
            {
                dtEntries = null;
                string saletitle = "Sale to " + Customername.Trim();
                var financialCheck = DatabaseQuery.Retrive("select top 1 FinancialYearID from tblFinancialYear where IsActive = 1");
                string FinancialYearID = (financialCheck != null ? Convert.ToString(financialCheck.Rows[0][0]) : string.Empty);
                if (string.IsNullOrEmpty(FinancialYearID))
                {
                    return "Your Company Financial Year is not Set, Please Contact to Adminstrator!";
                }

                string AccountHeadID = string.Empty;
                string AccountControlID = string.Empty;
                string AccountSubControlID = string.Empty;
                // Assests 1      increae(Debit)   decrese(Credit)
                // Liabilities 2     increae(Credit)   decrese(Debit)
                // Expenses 3     increae(Debit)   decrese(Credit)
                // Capital 4     increae(Credit)   decrese(Debit)
                // Revenue 5     increae(Credit)   decrese(Debit)

                string transectiontitle = string.Empty;

                var saleAccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 11 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                if (saleAccount == null || saleAccount.AccountHeadID == null || saleAccount.AccountControlID == null || saleAccount.AccountSubControlID == null)
                {
                    return "Sale Account Activity (i.e. Sale Payment Pending) entry not found in your Account Flow.";
                }
                AccountHeadID = Convert.ToString(saleAccount.AccountHeadID);
                AccountControlID = Convert.ToString(saleAccount.AccountControlID);
                AccountSubControlID = Convert.ToString(saleAccount.AccountSubControlID);

                transectiontitle = "Sale Payement Paid By " + Customername;
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, CompanyID.ToString(), BranchID.ToString(), invoiceNo, UserID.ToString(), Convert.ToString(Amount), "0", DateTime.Now, transectiontitle);

                saleAccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 12 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                if (saleAccount == null || saleAccount.AccountHeadID == null || saleAccount.AccountControlID == null || saleAccount.AccountSubControlID == null)
                {
                    return "Sale Account Activity (i.e. Sale Payment Succeed) entry not found in your Account Flow.";
                }
                AccountHeadID = Convert.ToString(saleAccount.AccountHeadID);
                AccountControlID = Convert.ToString(saleAccount.AccountControlID);
                AccountSubControlID = Convert.ToString(saleAccount.AccountSubControlID);
                transectiontitle = Customername + " , Sale Payment is Succesed!";
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, CompanyID.ToString(), BranchID.ToString(), invoiceNo, UserID.ToString(), "0", Convert.ToString(Amount), DateTime.Now, transectiontitle);

                //string paymentquery = string.Format("insert into tblCustomerPayment(CustomerID,CustomerInvoiceID,UserID,invoiceNo,TotalAmount,PaidAmount,RemainingBalance,CompanyID,BranchID) " +
                //"values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')",
                //CustomerID, CustomerInvoiceID, UserID, invoiceNo, TotalAmount, Amount, Convert.ToString(RemainingBalance), CompanyID, BranchID);
                //DatabaseQuery.Insert(paymentquery);

                // Insert payment record
                string paymentQuery = "INSERT INTO tblCustomerPayment(CustomerID, CustomerInvoiceID, UserID, InvoiceNo, TotalAmount, PaidAmount, RemainingBalance, CompanyID, BranchID, InvoiceDate) " +
                         "VALUES (@CustomerID, @CustomerInvoiceID, @UserID, @InvoiceNo, @TotalAmount, @PaidAmount, @RemainingBalance, @CompanyID, @BranchID, @InvoiceDate)";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(paymentQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@CustomerID", CustomerID);
                        cmd.Parameters.AddWithValue("@CustomerInvoiceID", CustomerInvoiceID);
                        cmd.Parameters.AddWithValue("@UserID", UserID);
                        cmd.Parameters.AddWithValue("@InvoiceNo", invoiceNo);
                        cmd.Parameters.AddWithValue("@TotalAmount", TotalAmount);
                        cmd.Parameters.AddWithValue("@PaidAmount", Amount);
                        cmd.Parameters.AddWithValue("@RemainingBalance", RemainingBalance);
                        cmd.Parameters.AddWithValue("@CompanyID", CompanyID);
                        cmd.Parameters.AddWithValue("@BranchID", BranchID);
                        cmd.Parameters.AddWithValue("@InvoiceDate", DateTime.Now.ToString("yyyy/MM/dd"));

                        cmd.ExecuteNonQuery();
                    }
                }

                // Insert transactions
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    foreach (DataRow entryRow in dtEntries.Rows)
                    {
                        string entryQuery = "INSERT INTO tblTransaction (FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, UserID, InvoiceNo, Credit, Debit, TransectionDate, TransectionTitle, CompanyID, BranchID) " +
                            "VALUES (@FinancialYearID, @AccountHeadID, @AccountControlID, @AccountSubControlID, @UserID, @InvoiceNo, @Credit, @Debit, @TransectionDate, @TransectionTitle, @CompanyID, @BranchID)";

                        using (SqlCommand cmd = new SqlCommand(entryQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@FinancialYearID", entryRow[0]);
                            cmd.Parameters.AddWithValue("@AccountHeadID", entryRow[1]);
                            cmd.Parameters.AddWithValue("@AccountControlID", entryRow[2]);
                            cmd.Parameters.AddWithValue("@AccountSubControlID", entryRow[3]);
                            cmd.Parameters.AddWithValue("@UserID", UserID);
                            cmd.Parameters.AddWithValue("@InvoiceNo", entryRow[6]);

                            float credit = 0, debit = 0;
                            float.TryParse(entryRow[8].ToString(), out credit);
                            float.TryParse(entryRow[9].ToString(), out debit);

                            cmd.Parameters.AddWithValue("@Credit", credit);
                            cmd.Parameters.AddWithValue("@Debit", debit);
                            cmd.Parameters.AddWithValue("@TransectionDate", entryRow[10]);
                            cmd.Parameters.AddWithValue("@TransectionTitle", entryRow[11]);
                            cmd.Parameters.AddWithValue("@CompanyID", CompanyID);
                            cmd.Parameters.AddWithValue("@BranchID", BranchID);

                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                //foreach (DataRow entryrow in dtEntries.Rows)
                //{
                //    string entryquery = string.Format("insert into tblTransaction(FinancialYearID,AccountHeadID,AccountControlID,AccountSubControlID,UserID,InvoiceNo,Credit,Debit,TransectionDate,TransactionTitle,CompanyID,BranchID) values" +
                //                        "('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}')",
                //                        Convert.ToString(entryrow[0]), Convert.ToString(entryrow[1]), Convert.ToString(entryrow[2]), Convert.ToString(entryrow[3]), Convert.ToString(entryrow[4]), Convert.ToString(entryrow[5]), Convert.ToString(entryrow[6]), Convert.ToString(entryrow[7]), (entryrow[8]), Convert.ToString(entryrow[9]), CompanyID, BranchID);
                //    DatabaseQuery.Insert(entryquery);
                //}

                return "Paid Successfully.";
            }
            catch (Exception ex)
            {
                return $"Unexpected Error: {ex.Message}. Please try Again!";
            }
        }

        //Sale Return Function
        public string ReturnSale(int CompanyID, int BranchID, int UserID, string InvoiceNo, string CustomerInvoiceID, int CustomerReturnInvoiceID, float Amount, string CustomerID, string Customername, bool isPayment)
        {
            try
            {
                dtEntries = null;
                string returnsaletitle = "Return Sale From " + Customername.Trim();
                var financialCheck = DatabaseQuery.Retrive("select top 1 FinancialYearID from tblFinancialYear where IsActive = 1");
                string FinancialYearID = (financialCheck != null ? Convert.ToString(financialCheck.Rows[0][0]) : string.Empty);
                if (string.IsNullOrEmpty(FinancialYearID))
                {
                    return "Your Company Financial Year is not Set, Please Contact to Adminstrator!";
                }

                string successmessage = "Return Sale Success";

                string AccountHeadID = string.Empty;
                string AccountControlID = string.Empty;
                string AccountSubControlID = string.Empty;
                // Assests 1      increae(Debit)   decrese(Credit)
                // Liabilities 2     increae(Credit)   decrese(Debit)
                // Expenses 3     increae(Debit)   decrese(Credit)
                // Capital 4     increae(Credit)   decrese(Debit)
                // Revenue 5     increae(Credit)   decrese(Debit)

                var returnsaleAccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 2 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                if (returnsaleAccount == null || returnsaleAccount.AccountHeadID == null || returnsaleAccount.AccountControlID == null || returnsaleAccount.AccountSubControlID == null)
                {
                    return "Sale Return Account Activity (i.e. Sale Return) entry not found in your Account Flow.";
                }
                //Debit Entry Return Sale
                AccountHeadID = Convert.ToString(returnsaleAccount.AccountHeadID);
                AccountControlID = Convert.ToString(returnsaleAccount.AccountControlID);
                AccountSubControlID = Convert.ToString(returnsaleAccount.AccountSubControlID);

                string transectiontitle = string.Empty;

                transectiontitle = "Return Sale From " + Customername.Trim();
                //SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(), "0", Convert.ToString(Amount), DateTime.Now, transectiontitle);
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, CompanyID.ToString(), BranchID.ToString(), InvoiceNo, UserID.ToString(), "0", Convert.ToString(Amount), DateTime.Now, transectiontitle);


                //Credit Entry Return Sale
                returnsaleAccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 16 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                if (returnsaleAccount == null || returnsaleAccount.AccountHeadID == null || returnsaleAccount.AccountControlID == null || returnsaleAccount.AccountSubControlID == null)
                {
                    return "Sale Return Account Activity (i.e. Sale Return Payment Pending) entry not found in your Account Flow.";
                }
                AccountHeadID = Convert.ToString(returnsaleAccount.AccountHeadID);
                AccountControlID = Convert.ToString(returnsaleAccount.AccountControlID);
                AccountSubControlID = Convert.ToString(returnsaleAccount.AccountSubControlID);

                transectiontitle = Customername.Trim() + " , Return Sale Payment is Pending!";
                //SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(), "0", Convert.ToString(Amount), DateTime.Now, transectiontitle);
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, CompanyID.ToString(), BranchID.ToString(), InvoiceNo, UserID.ToString(), Convert.ToString(Amount), "0", DateTime.Now, transectiontitle);

                if (isPayment == true)
                {
                    string payinvoicenno = "RIP" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;

                    returnsaleAccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 16 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                    if (returnsaleAccount == null || returnsaleAccount.AccountHeadID == null || returnsaleAccount.AccountControlID == null || returnsaleAccount.AccountSubControlID == null)
                    {
                        return "Sale Return Account Activity (i.e. Sale Return Payment Pending) entry not found in your Account Flow.";
                    }
                    AccountHeadID = Convert.ToString(returnsaleAccount.AccountHeadID);
                    AccountControlID = Convert.ToString(returnsaleAccount.AccountControlID);
                    AccountSubControlID = Convert.ToString(returnsaleAccount.AccountSubControlID);

                    transectiontitle = "Return Sale Payment By " + Customername;
                    SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, CompanyID.ToString(), BranchID.ToString(), payinvoicenno, UserID.ToString(), "0", Convert.ToString(Amount), DateTime.Now, transectiontitle);

                    returnsaleAccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 17 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                    if (returnsaleAccount == null || returnsaleAccount.AccountHeadID == null || returnsaleAccount.AccountControlID == null || returnsaleAccount.AccountSubControlID == null)
                    {
                        return "Sale Return Account Activity (i.e. Sale Return Payment Succeed) entry not found in your Account Flow.";
                    }
                    AccountHeadID = Convert.ToString(returnsaleAccount.AccountHeadID);
                    AccountControlID = Convert.ToString(returnsaleAccount.AccountControlID);
                    AccountSubControlID = Convert.ToString(returnsaleAccount.AccountSubControlID);
                    transectiontitle = Customername + " , Return Sale Payment is Succesed!";
                    SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, CompanyID.ToString(), BranchID.ToString(), payinvoicenno, UserID.ToString(), Convert.ToString(Amount), "0", DateTime.Now, transectiontitle);

                    // Insert payment record
                    string paymentQuery = "INSERT INTO tblCustomerReturnPayment(CustomerID, CustomerInvoiceID, UserID, InvoiceNo, TotalAmount, PaidAmount, RemainingBalance, CompanyID, BranchID, CustomerReturnInvoiceID, InvoiceDate) " +
                             "VALUES (@CustomerID, @CustomerInvoiceID, @UserID, @InvoiceNo, @TotalAmount, @PaidAmount, @RemainingBalance, @CompanyID, @BranchID, @CustomerReturnInvoiceID, @InvoiceDate)";

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(paymentQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@CustomerID", CustomerID);
                            cmd.Parameters.AddWithValue("@CustomerInvoiceID", CustomerInvoiceID);
                            cmd.Parameters.AddWithValue("@UserID", UserID);
                            cmd.Parameters.AddWithValue("@InvoiceNo", payinvoicenno);
                            cmd.Parameters.AddWithValue("@TotalAmount", Amount);
                            cmd.Parameters.AddWithValue("@PaidAmount", Amount);
                            cmd.Parameters.AddWithValue("@RemainingBalance", "0");
                            cmd.Parameters.AddWithValue("@CompanyID", CompanyID);
                            cmd.Parameters.AddWithValue("@BranchID", BranchID);
                            cmd.Parameters.AddWithValue("@CustomerReturnInvoiceID", CustomerReturnInvoiceID);
                            cmd.Parameters.AddWithValue("@InvoiceDate", DateTime.Now.ToString("yyyy/MM/dd"));

                            cmd.ExecuteNonQuery();
                        }
                    }

                    successmessage = successmessage + " with Payment.";
                }

                // Insert transactions
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    foreach (DataRow entryRow in dtEntries.Rows)
                    {
                        string entryQuery = "INSERT INTO tblTransaction (FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, UserID, InvoiceNo, Credit, Debit, TransectionDate, TransectionTitle, CompanyID, BranchID) " +
                            "VALUES (@FinancialYearID, @AccountHeadID, @AccountControlID, @AccountSubControlID, @UserID, @InvoiceNo, @Credit, @Debit, @TransectionDate, @TransectionTitle, @CompanyID, @BranchID)";

                        using (SqlCommand cmd = new SqlCommand(entryQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@FinancialYearID", entryRow[0]);
                            cmd.Parameters.AddWithValue("@AccountHeadID", entryRow[1]);
                            cmd.Parameters.AddWithValue("@AccountControlID", entryRow[2]);
                            cmd.Parameters.AddWithValue("@AccountSubControlID", entryRow[3]);
                            cmd.Parameters.AddWithValue("@UserID", UserID);
                            cmd.Parameters.AddWithValue("@InvoiceNo", entryRow[6]);

                            float credit = 0, debit = 0;
                            float.TryParse(entryRow[8].ToString(), out credit);
                            float.TryParse(entryRow[9].ToString(), out debit);

                            cmd.Parameters.AddWithValue("@Credit", credit);
                            cmd.Parameters.AddWithValue("@Debit", debit);
                            cmd.Parameters.AddWithValue("@TransectionDate", entryRow[10]);
                            cmd.Parameters.AddWithValue("@TransectionTitle", entryRow[11]);
                            cmd.Parameters.AddWithValue("@CompanyID", CompanyID);
                            cmd.Parameters.AddWithValue("@BranchID", BranchID);

                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                return successmessage;
            }
            catch (Exception ex)
            {
                return $"Unexpected Error: {ex.Message}. Please try Again!";
            }
        }

        public string ReturnSalePayment(int CompanyID, int BranchID, int UserID, string invoiceNo, string CustomerInvoiceID, int CustomerReturnInvoiceID, float TotalAmount, float Amount, string CustomerID, string Customername, float RemainingBalance)
        {
            try
            {
                dtEntries = null;
                string saletitle = "Return Sale From " + Customername.Trim();
                var financialCheck = DatabaseQuery.Retrive("select top 1 FinancialYearID from tblFinancialYear where IsActive = 1");
                string FinancialYearID = (financialCheck != null ? Convert.ToString(financialCheck.Rows[0][0]) : string.Empty);
                if (string.IsNullOrEmpty(FinancialYearID))
                {
                    return "Your Company Financial Year is not Set, Please Contact to Adminstrator!";
                }

                string AccountHeadID = string.Empty;
                string AccountControlID = string.Empty;
                string AccountSubControlID = string.Empty;
                // Assests 1      increae(Debit)   decrese(Credit)
                // Liabilities 2     increae(Credit)   decrese(Debit)
                // Expenses 3     increae(Debit)   decrese(Credit)
                // Capital 4     increae(Credit)   decrese(Debit)
                // Revenue 5     increae(Credit)   decrese(Debit)

                string transectiontitle = string.Empty;

                var returnsaleAccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 16 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                if (returnsaleAccount == null || returnsaleAccount.AccountHeadID == null || returnsaleAccount.AccountControlID == null || returnsaleAccount.AccountSubControlID == null)
                {
                    return "Sale Return Account Activity (i.e. Sale Return Payment Pending) entry not found in your Account Flow.";
                }
                AccountHeadID = Convert.ToString(returnsaleAccount.AccountHeadID);
                AccountControlID = Convert.ToString(returnsaleAccount.AccountControlID);
                AccountSubControlID = Convert.ToString(returnsaleAccount.AccountSubControlID);

                transectiontitle = "Return Sale Payment Paid to " + Customername;
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, CompanyID.ToString(), BranchID.ToString(), invoiceNo, UserID.ToString(), "0", Convert.ToString(Amount), DateTime.Now, transectiontitle);

                returnsaleAccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 17 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                if (returnsaleAccount == null || returnsaleAccount.AccountHeadID == null || returnsaleAccount.AccountControlID == null || returnsaleAccount.AccountSubControlID == null)
                {
                    return "Sale Return Account Activity (i.e. Sale Return Payment Succeed) entry not found in your Account Flow.";
                }
                AccountHeadID = Convert.ToString(returnsaleAccount.AccountHeadID);
                AccountControlID = Convert.ToString(returnsaleAccount.AccountControlID);
                AccountSubControlID = Convert.ToString(returnsaleAccount.AccountSubControlID);
                transectiontitle = Customername + " , Return Sale Payment is Succesed!";
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, CompanyID.ToString(), BranchID.ToString(), invoiceNo, UserID.ToString(), Convert.ToString(Amount), "0", DateTime.Now, transectiontitle);

                // Insert payment record
                string paymentQuery = "INSERT INTO tblCustomerReturnPayment(CustomerID, CustomerInvoiceID, UserID, InvoiceNo, TotalAmount, PaidAmount, RemainingBalance, CompanyID, BranchID, CustomerReturnInvoiceID, InvoiceDate) " +
                         "VALUES (@CustomerID, @CustomerInvoiceID, @UserID, @InvoiceNo, @TotalAmount, @PaidAmount, @RemainingBalance, @CompanyID, @BranchID, @CustomerReturnInvoiceID, @InvoiceDate)";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(paymentQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@CustomerID", CustomerID);
                        cmd.Parameters.AddWithValue("@CustomerInvoiceID", CustomerInvoiceID);
                        cmd.Parameters.AddWithValue("@UserID", UserID);
                        cmd.Parameters.AddWithValue("@InvoiceNo", invoiceNo);
                        cmd.Parameters.AddWithValue("@TotalAmount", TotalAmount);
                        cmd.Parameters.AddWithValue("@PaidAmount", Amount);
                        cmd.Parameters.AddWithValue("@RemainingBalance", RemainingBalance);
                        cmd.Parameters.AddWithValue("@CompanyID", CompanyID);
                        cmd.Parameters.AddWithValue("@BranchID", BranchID);
                        cmd.Parameters.AddWithValue("@CustomerReturnInvoiceID", CustomerReturnInvoiceID);
                        cmd.Parameters.AddWithValue("@InvoiceDate", DateTime.Now.ToString("yyyy/MM/dd"));

                        cmd.ExecuteNonQuery();
                    }
                }

                // Insert transactions
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    foreach (DataRow entryRow in dtEntries.Rows)
                    {
                        string entryQuery = "INSERT INTO tblTransaction (FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, UserID, InvoiceNo, Credit, Debit, TransectionDate, TransectionTitle, CompanyID, BranchID) " +
                            "VALUES (@FinancialYearID, @AccountHeadID, @AccountControlID, @AccountSubControlID, @UserID, @InvoiceNo, @Credit, @Debit, @TransectionDate, @TransectionTitle, @CompanyID, @BranchID)";

                        using (SqlCommand cmd = new SqlCommand(entryQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@FinancialYearID", entryRow[0]);
                            cmd.Parameters.AddWithValue("@AccountHeadID", entryRow[1]);
                            cmd.Parameters.AddWithValue("@AccountControlID", entryRow[2]);
                            cmd.Parameters.AddWithValue("@AccountSubControlID", entryRow[3]);
                            cmd.Parameters.AddWithValue("@UserID", UserID);
                            cmd.Parameters.AddWithValue("@InvoiceNo", entryRow[6]);

                            float credit = 0, debit = 0;
                            float.TryParse(entryRow[8].ToString(), out credit);
                            float.TryParse(entryRow[9].ToString(), out debit);

                            cmd.Parameters.AddWithValue("@Credit", credit);
                            cmd.Parameters.AddWithValue("@Debit", debit);
                            cmd.Parameters.AddWithValue("@TransectionDate", entryRow[10]);
                            cmd.Parameters.AddWithValue("@TransectionTitle", entryRow[11]);
                            cmd.Parameters.AddWithValue("@CompanyID", CompanyID);
                            cmd.Parameters.AddWithValue("@BranchID", BranchID);

                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                return "Paid Successfully.";
            }
            catch (Exception ex)
            {
                return $"Unexpected Error: {ex.Message}. Please try Again!";
            }
        }

        private void SetEntries(
            string FinancialYearID, string AccountControlID,
            string AccountSubControlID, string AccountHeadID,
            string CompanyID, string BranchID, string InvoiceNo,
            string UserID, string Credit, string Debit,
            DateTime TransacctionDate, string TransactionTitle)
        {
            if (dtEntries == null)
            {
                dtEntries = new DataTable();
                dtEntries.Columns.Add("FinancialYearID");
                dtEntries.Columns.Add("AccountControlID");
                dtEntries.Columns.Add("AccountSubControlID");
                dtEntries.Columns.Add("AccountHeadID");
                dtEntries.Columns.Add("CompanyID");
                dtEntries.Columns.Add("BranchID");
                dtEntries.Columns.Add("InvoiceNo");
                dtEntries.Columns.Add("UserID");
                dtEntries.Columns.Add("Credit");
                dtEntries.Columns.Add("Debit");
                dtEntries.Columns.Add("TransacctionDate");
                dtEntries.Columns.Add("TransactionTitle");
            }

            if (dtEntries != null)
            {
                dtEntries.Rows.Add(
                    FinancialYearID,
                    AccountControlID,
                    AccountSubControlID,
                    AccountHeadID,
                    CompanyID,
                    BranchID,
                    InvoiceNo,
                    UserID,
                    Credit,
                    Debit,
                    TransacctionDate,
                    TransactionTitle);
            }
        }
    }
}
