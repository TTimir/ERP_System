using DatabaseAccess;
using DatabaseAccess.Code;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace CloudERP_Model.Models
{
    public class PurchaseEntry
    {
        private CloudErpVEntities db = new CloudErpVEntities();
        private string connectionString = @"data source=TIMIRBHINGRADIY;initial catalog=CloudErpV;user id=sa;password=sqlserver;trustservercertificate=True;";
        public string selectsupplierid = string.Empty;
        DataTable dtEntries = null;
        public string ConfirmPurchase(int CompanyID, int BranchID, int UserID, string InvoiceNo, string SupplierInvoiceID, float Amount, string SupplierID, string SupplierName, bool isPayment)
        {
            try
            {
                dtEntries = null;
                string pruchasetitle = "Purchase From " + SupplierName.Trim();
                var financialCheck = DatabaseQuery.Retrive("select top 1 FinancialYearID from tblFinancialYear where IsActive = 1");
                string FinancialYearID = (financialCheck != null ? Convert.ToString(financialCheck.Rows[0][0]) : string.Empty);
                if (string.IsNullOrEmpty(FinancialYearID))
                {
                    return "Your Company Financial Year is not Set, Please Contact to Adminstrator!";
                }

                string successmessage = "Purchase Success";

                string AccountHeadID = string.Empty;
                string AccountControlID = string.Empty;
                string AccountSubControlID = string.Empty;
                // Assests 1      increae(Debit)   decrese(Credit)
                // Liabilities 2     increae(Credit)   decrese(Debit)
                // Expenses 3     increae(Debit)   decrese(Credit)
                // Capital 4     increae(Credit)   decrese(Debit)
                // Revenue 5     increae(Credit)   decrese(Debit)

                var purchaseAccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 3 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                if (purchaseAccount == null || purchaseAccount.AccountHeadID == null || purchaseAccount.AccountControlID == null || purchaseAccount.AccountSubControlID == null)
                {
                    return "Purchase Account (i.e. Purchase) entry not found in your Account Flow.";
                }
                //Debit Entry Purchase
                AccountHeadID = Convert.ToString(purchaseAccount.AccountHeadID);
                AccountControlID = Convert.ToString(purchaseAccount.AccountControlID);
                AccountSubControlID = Convert.ToString(purchaseAccount.AccountSubControlID);

                string transectiontitle = string.Empty;

                transectiontitle = "Purchase From " + SupplierName.Trim();
                //SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(), "0", Convert.ToString(Amount), DateTime.Now, transectiontitle);
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, CompanyID.ToString(), BranchID.ToString(), InvoiceNo, UserID.ToString(), "0", Convert.ToString(Amount), DateTime.Now, transectiontitle);


                //Credit Entry Purchase
                purchaseAccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 8 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                if (purchaseAccount == null || purchaseAccount.AccountHeadID == null || purchaseAccount.AccountControlID == null || purchaseAccount.AccountSubControlID == null)
                {
                    return "Purchase Account (i.e. Purchase Payment Pending/Succeed) entry not found in your Account Flow.";
                }
                AccountHeadID = Convert.ToString(purchaseAccount.AccountHeadID);
                AccountControlID = Convert.ToString(purchaseAccount.AccountControlID);
                AccountSubControlID = Convert.ToString(purchaseAccount.AccountSubControlID);

                transectiontitle = SupplierName.Trim() + " , Purchase Payment is Pending!";
                //SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(), "0", Convert.ToString(Amount), DateTime.Now, transectiontitle);
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, CompanyID.ToString(), BranchID.ToString(), InvoiceNo, UserID.ToString(), Convert.ToString(Amount), "0", DateTime.Now, transectiontitle);

                if (isPayment == true)
                {
                    string payinvoicenno = "PAY" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;

                    purchaseAccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 8 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                    if (purchaseAccount == null || purchaseAccount.AccountHeadID == null || purchaseAccount.AccountControlID == null || purchaseAccount.AccountSubControlID == null)
                    {
                        return "Purchase Account (i.e. Purchase Payment Pending) entry not found in your Account Flow.";
                    }
                    AccountHeadID = Convert.ToString(purchaseAccount.AccountHeadID);
                    AccountControlID = Convert.ToString(purchaseAccount.AccountControlID);
                    AccountSubControlID = Convert.ToString(purchaseAccount.AccountSubControlID);

                    transectiontitle = "Payement Paid to " + SupplierName;
                    if (purchaseAccount != null)
                    {
                        SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, CompanyID.ToString(), BranchID.ToString(), payinvoicenno, UserID.ToString(), "0", Convert.ToString(Amount), DateTime.Now, transectiontitle);
                    }
                    purchaseAccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 9 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                    if (purchaseAccount == null || purchaseAccount.AccountHeadID == null || purchaseAccount.AccountControlID == null || purchaseAccount.AccountSubControlID == null)
                    {
                        return "Purchase Account (i.e. Purchase Payment Succeed) entry not found in your Account Flow.";
                    }
                    AccountHeadID = Convert.ToString(purchaseAccount.AccountHeadID);
                    AccountControlID = Convert.ToString(purchaseAccount.AccountControlID);
                    AccountSubControlID = Convert.ToString(purchaseAccount.AccountSubControlID);
                    transectiontitle = SupplierName + " , Purchase Payment is Succesed!";
                    if (purchaseAccount != null)
                    {
                        SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, CompanyID.ToString(), BranchID.ToString(), payinvoicenno, UserID.ToString(), Convert.ToString(Amount), "0", DateTime.Now, transectiontitle);
                    }

                    //string paymentquery = string.Format("insert into tblSupplierPayment(SupplierID,SupplierInvoiceID,UserID,InvoiceNo,TotalAmount,PaymentAmount,RemainingBalance,CompanyID,BranchID) " +
                    //"values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')",
                    //SupplierID, SupplierInvoiceID, UserID, InvoiceNo, Amount, Amount, "0", CompanyID, BranchID);
                    //DatabaseQuery.Insert(paymentquery);

                    // Insert supplier payment
                    string paymentQuery = "INSERT INTO tblSupplierPayment(SupplierID, SupplierInvoiceID, UserID, InvoiceNo, TotalAmount, PaymentAmount, RemainingBalance, CompanyID, BranchID, InvoiceDate) " +
                        "VALUES (@SupplierID, @SupplierInvoiceID, @UserID, @InvoiceNo, @TotalAmount, @PaymentAmount, @RemainingBalance, @CompanyID, @BranchID, @InvoiceDate)";

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(paymentQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@SupplierID", SupplierID);
                            cmd.Parameters.AddWithValue("@SupplierInvoiceID", SupplierInvoiceID);
                            cmd.Parameters.AddWithValue("@UserID", UserID);
                            cmd.Parameters.AddWithValue("@InvoiceNo", InvoiceNo);
                            cmd.Parameters.AddWithValue("@TotalAmount", Amount);
                            cmd.Parameters.AddWithValue("@PaymentAmount", Amount);
                            cmd.Parameters.AddWithValue("@RemainingBalance", "0");
                            cmd.Parameters.AddWithValue("@CompanyID", CompanyID);
                            cmd.Parameters.AddWithValue("@BranchID", BranchID);
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

        public string PurchasePayment(int CompanyID, int BranchID, int UserID, string InvoiceNo, string SupplierInvoiceID, float TotalAmount, float Amount, string SupplierID, string SupplierName, float RemainingBalance)
        {
            try
            {
                dtEntries = null;
                string pruchasetitle = "Purchase From " + SupplierName.Trim();
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

                var purchaseAccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 8 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                if (purchaseAccount == null || purchaseAccount.AccountHeadID == null || purchaseAccount.AccountControlID == null || purchaseAccount.AccountSubControlID == null)
                {
                    return "Purchase Account (i.e. Purchase Payment Pending) entry not found in your Account Flow.";
                }
                AccountHeadID = Convert.ToString(purchaseAccount.AccountHeadID);
                AccountControlID = Convert.ToString(purchaseAccount.AccountControlID);
                AccountSubControlID = Convert.ToString(purchaseAccount.AccountSubControlID);

                transectiontitle = "Payement Paid to " + SupplierName;
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, CompanyID.ToString(), BranchID.ToString(), InvoiceNo, UserID.ToString(), Convert.ToString(RemainingBalance), Convert.ToString(Amount), DateTime.Now, transectiontitle);

                purchaseAccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 9 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                if (purchaseAccount == null || purchaseAccount.AccountHeadID == null || purchaseAccount.AccountControlID == null || purchaseAccount.AccountSubControlID == null)
                {
                    return "Purchase Account (i.e. Purchase Payment Succeed) entry not found in your Account Flow.";
                }
                AccountHeadID = Convert.ToString(purchaseAccount.AccountHeadID);
                AccountControlID = Convert.ToString(purchaseAccount.AccountControlID);
                AccountSubControlID = Convert.ToString(purchaseAccount.AccountSubControlID);
                transectiontitle = SupplierName + " , Purchase Payment is Succesed!";
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, CompanyID.ToString(), BranchID.ToString(), InvoiceNo, UserID.ToString(), Convert.ToString(Amount), "0", DateTime.Now, transectiontitle);

                //string paymentquery = string.Format("insert into tblSupplierPayment(SupplierID,SupplierInvoiceID,UserID,InvoiceNo,TotalAmount,PaymentAmount,RemainingBalance,CompanyID,BranchID) " +
                //"values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')",
                //SupplierID, SupplierInvoiceID, UserID, InvoiceNo, TotalAmount, Amount, Convert.ToString(RemainingBalance), CompanyID, BranchID);
                //DatabaseQuery.Insert(paymentquery);

                // Insert payment record
                string paymentQuery = "INSERT INTO tblSupplierPayment(SupplierID, SupplierInvoiceID, UserID, InvoiceNo, TotalAmount, PaymentAmount, RemainingBalance, CompanyID, BranchID, InvoiceDate) " +
                         "VALUES (@SupplierID, @SupplierInvoiceID, @UserID, @InvoiceNo, @TotalAmount, @PaymentAmount, @RemainingBalance, @CompanyID, @BranchID, @InvoiceDate)";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(paymentQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@SupplierID", SupplierID);
                        cmd.Parameters.AddWithValue("@SupplierInvoiceID", SupplierInvoiceID);
                        cmd.Parameters.AddWithValue("@UserID", UserID);
                        cmd.Parameters.AddWithValue("@InvoiceNo", InvoiceNo);
                        cmd.Parameters.AddWithValue("@TotalAmount", TotalAmount);
                        cmd.Parameters.AddWithValue("@PaymentAmount", Amount);
                        cmd.Parameters.AddWithValue("@RemainingBalance", RemainingBalance);
                        cmd.Parameters.AddWithValue("@CompanyID", CompanyID);
                        cmd.Parameters.AddWithValue("@BranchID", BranchID);
                        cmd.Parameters.AddWithValue("@InvoiceDate", DateTime.Now.ToString("yyyy/MM/dd"));

                        cmd.ExecuteNonQuery();
                    }
                }

                // Insert transaction records
                string transactionQuery = "INSERT INTO tblTransaction (FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, UserID, InvoiceNo, Credit, Debit, TransectionDate, TransectionTitle, CompanyID, BranchID) " +
                                          "VALUES (@FinancialYearID, @AccountHeadID, @AccountControlID, @AccountSubControlID, @UserID, @InvoiceNo, @Credit, @Debit, @TransectionDate, @TransectionTitle, @CompanyID, @BranchID)";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    foreach (DataRow entryRow in dtEntries.Rows)
                    {
                        using (SqlCommand cmd = new SqlCommand(transactionQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@FinancialYearID", entryRow[0]);
                            cmd.Parameters.AddWithValue("@AccountHeadID", entryRow[1]);
                            cmd.Parameters.AddWithValue("@AccountControlID", entryRow[2]);
                            cmd.Parameters.AddWithValue("@AccountSubControlID", entryRow[3]);
                            cmd.Parameters.AddWithValue("@UserID", UserID);
                            cmd.Parameters.AddWithValue("@InvoiceNo", entryRow[6]);
                            cmd.Parameters.AddWithValue("@Credit", entryRow[8]);
                            cmd.Parameters.AddWithValue("@Debit", entryRow[9]);
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
                //    string entryquery = string.Format("insert into tblTransaction(FinancialYearID,AccountHeadID,AccountControlID,AccountSubControlID,UserID,InvoiceNo,Credit,Debit,TransectionDate,TransectionTitle,CompanyID,BranchID) values " +
                //                        "('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}')",
                //                        Convert.ToString(entryrow[0]), Convert.ToString(entryrow[1]), Convert.ToString(entryrow[2]), Convert.ToString(entryrow[3]), Convert.ToString(entryrow[4]), Convert.ToString(entryrow[5]), Convert.ToString(entryrow[6]), Convert.ToString(entryrow[7]), (entryrow[8]), Convert.ToString(entryrow[9]), CompanyID, BranchID);
                //    DatabaseQuery.Insert(entryquery);
                //}

                return "Payment is Paid.";
            }
            catch (Exception ex)
            {
                return $"Unexpected Error: {ex.Message}. Please try Again!";
            }
        }

        // Purchase Return Function
        public string ReturnPurchase(int CompanyID, int BranchID, int UserID, string InvoiceNo, string SupplierInvoiceID, int SupplierReturnInvoiceID, float Amount, string SupplierID, string SupplierName, bool isPayment)
        {
            try
            {
                dtEntries = null;
                string pruchasetitle = "Return Purchase To " + SupplierName.Trim();
                var financialCheck = DatabaseQuery.Retrive("select top 1 FinancialYearID from tblFinancialYear where IsActive = 1");
                string FinancialYearID = (financialCheck != null ? Convert.ToString(financialCheck.Rows[0][0]) : string.Empty);
                if (string.IsNullOrEmpty(FinancialYearID))
                {
                    return "Your Company Financial Year is not Set, Please Contact to Adminstrator!";
                }

                string successmessage = "Return Purchase Success";

                string AccountHeadID = string.Empty;
                string AccountControlID = string.Empty;
                string AccountSubControlID = string.Empty;
                // Assests 1      increae(Debit)   decrese(Credit)
                // Liabilities 2     increae(Credit)   decrese(Debit)
                // Expenses 3     increae(Debit)   decrese(Credit)
                // Capital 4     increae(Credit)   decrese(Debit)
                // Revenue 5     increae(Credit)   decrese(Debit)

                var returnpurchaseAccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 4 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                if (returnpurchaseAccount == null || returnpurchaseAccount.AccountHeadID == null || returnpurchaseAccount.AccountControlID == null || returnpurchaseAccount.AccountSubControlID == null)
                {
                    return "Purchase Return Account Activity (i.e. Purchase Purchase Return) entry not found in your Account Flow.";
                }
                //Credit Entry Return Purchase
                AccountHeadID = Convert.ToString(returnpurchaseAccount.AccountHeadID);
                AccountControlID = Convert.ToString(returnpurchaseAccount.AccountControlID);
                AccountSubControlID = Convert.ToString(returnpurchaseAccount.AccountSubControlID);

                string transectiontitle = string.Empty;

                transectiontitle = "Return Purchase To " + SupplierName.Trim();
                //SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(), "0", Convert.ToString(Amount), DateTime.Now, transectiontitle);
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, CompanyID.ToString(), BranchID.ToString(), InvoiceNo, UserID.ToString(), Convert.ToString(Amount), "0", DateTime.Now, transectiontitle);


                //Debit Entry Return Purchase
                returnpurchaseAccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 14 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                if (returnpurchaseAccount == null || returnpurchaseAccount.AccountHeadID == null || returnpurchaseAccount.AccountControlID == null || returnpurchaseAccount.AccountSubControlID == null)
                {
                    return "Purchase Return Account Activity (i.e. Purchase Return Payment Pending) entry not found in your Account Flow.";
                }
                AccountHeadID = Convert.ToString(returnpurchaseAccount.AccountHeadID);
                AccountControlID = Convert.ToString(returnpurchaseAccount.AccountControlID);
                AccountSubControlID = Convert.ToString(returnpurchaseAccount.AccountSubControlID);

                transectiontitle = SupplierName.Trim() + " , Return Purchase Payment is Pending!";
                //SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, InvoiceNo, UserID.ToString(), "0", Convert.ToString(Amount), DateTime.Now, transectiontitle);
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, CompanyID.ToString(), BranchID.ToString(), InvoiceNo, UserID.ToString(), "0", Convert.ToString(Amount), DateTime.Now, transectiontitle);

                if (isPayment == true)
                {
                    string payinvoicenno = "RPP" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;

                    returnpurchaseAccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 14 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                    if (returnpurchaseAccount == null || returnpurchaseAccount.AccountHeadID == null || returnpurchaseAccount.AccountControlID == null || returnpurchaseAccount.AccountSubControlID == null)
                    {
                        return "Purchase Return Account Activity (i.e. Purchase Return Payment Pending) entry not found in your Account Flow.";
                    }
                    AccountHeadID = Convert.ToString(returnpurchaseAccount.AccountHeadID);
                    AccountControlID = Convert.ToString(returnpurchaseAccount.AccountControlID);
                    AccountSubControlID = Convert.ToString(returnpurchaseAccount.AccountSubControlID);

                    transectiontitle = "Return Payement From " + SupplierName;
                    if (returnpurchaseAccount != null)
                    {
                        SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, CompanyID.ToString(), BranchID.ToString(), payinvoicenno, UserID.ToString(), Convert.ToString(Amount), "0", DateTime.Now, transectiontitle);
                    }
                    returnpurchaseAccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 15 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                    if (returnpurchaseAccount == null || returnpurchaseAccount.AccountHeadID == null || returnpurchaseAccount.AccountControlID == null || returnpurchaseAccount.AccountSubControlID == null)
                    {
                        return "Purchase Return Account Activity (i.e. Purchase Return Payment Succeed) entry not found in your Account Flow.";
                    }
                    AccountHeadID = Convert.ToString(returnpurchaseAccount.AccountHeadID);
                    AccountControlID = Convert.ToString(returnpurchaseAccount.AccountControlID);
                    AccountSubControlID = Convert.ToString(returnpurchaseAccount.AccountSubControlID);
                    transectiontitle = SupplierName + " , Return Purchase Payment is Succesed!";
                    if (returnpurchaseAccount != null)
                    {
                        SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, CompanyID.ToString(), BranchID.ToString(), payinvoicenno, UserID.ToString(), "0", Convert.ToString(Amount), DateTime.Now, transectiontitle);
                    }

                    // Insert supplier payment
                    string paymentQuery = "INSERT INTO tblSupplierReturnPayment(SupplierID, SupplierInvoiceID, UserID, InvoiceNo, TotalAmount, PaymentAmount, RemainingBalance, CompanyID, BranchID, SupplierReturnInvoiceID, InvoiceDate) " +
                        "VALUES (@SupplierID, @SupplierInvoiceID, @UserID, @InvoiceNo, @TotalAmount, @PaymentAmount, @RemainingBalance, @CompanyID, @BranchID, @SupplierReturnInvoiceID, @InvoiceDate)";

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(paymentQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@SupplierID", SupplierID);
                            cmd.Parameters.AddWithValue("@SupplierInvoiceID", SupplierInvoiceID);
                            cmd.Parameters.AddWithValue("@UserID", UserID);
                            cmd.Parameters.AddWithValue("@InvoiceNo", payinvoicenno);
                            cmd.Parameters.AddWithValue("@TotalAmount", Amount);
                            cmd.Parameters.AddWithValue("@PaymentAmount", Amount);
                            cmd.Parameters.AddWithValue("@RemainingBalance", "0");
                            cmd.Parameters.AddWithValue("@CompanyID", CompanyID);
                            cmd.Parameters.AddWithValue("@BranchID", BranchID);
                            cmd.Parameters.AddWithValue("@SupplierReturnInvoiceID", SupplierReturnInvoiceID);
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

        public string ReturnPurchasePayment(int CompanyID, int BranchID, int UserID, string InvoiceNo, string SupplierInvoiceID, int SupplierReturnInvoiceID, float TotalAmount, float Amount, string SupplierID, string SupplierName, float RemainingBalance)
        {
            try
            {
                dtEntries = null;
                string returnpruchasetitle = "Return Purchase To " + SupplierName.Trim();
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

                var returnpurchaseAccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 14 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                if (returnpurchaseAccount == null || returnpurchaseAccount.AccountHeadID == null || returnpurchaseAccount.AccountControlID == null || returnpurchaseAccount.AccountSubControlID == null)
                {
                    return "Purchase Return Account Activity (i.e. Purchase Return Payment Pending) entry not found in your Account Flow.";
                }
                AccountHeadID = Convert.ToString(returnpurchaseAccount.AccountHeadID);
                AccountControlID = Convert.ToString(returnpurchaseAccount.AccountControlID);
                AccountSubControlID = Convert.ToString(returnpurchaseAccount.AccountSubControlID);

                transectiontitle = "Return Payement From " + SupplierName;
                if (returnpurchaseAccount != null)
                {
                    SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, CompanyID.ToString(), BranchID.ToString(), InvoiceNo, UserID.ToString(), Convert.ToString(Amount), "0", DateTime.Now, transectiontitle);
                }
                returnpurchaseAccount = db.tblAccountSettings.Where(a => a.AccountActivityID == 15 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                if (returnpurchaseAccount == null || returnpurchaseAccount.AccountHeadID == null || returnpurchaseAccount.AccountControlID == null || returnpurchaseAccount.AccountSubControlID == null)
                {
                    return "Purchase Return Account Activity (i.e. Purchase Return Payment Succeed) entry not found in your Account Flow.";
                }
                AccountHeadID = Convert.ToString(returnpurchaseAccount.AccountHeadID);
                AccountControlID = Convert.ToString(returnpurchaseAccount.AccountControlID);
                AccountSubControlID = Convert.ToString(returnpurchaseAccount.AccountSubControlID);
                transectiontitle = SupplierName + " , Return Purchase Payment is Succesed!";
                if (returnpurchaseAccount != null)
                {
                    SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, CompanyID.ToString(), BranchID.ToString(), InvoiceNo, UserID.ToString(), "0", Convert.ToString(Amount), DateTime.Now, transectiontitle);
                }

                // Insert supplier payment
                string paymentQuery = "INSERT INTO tblSupplierReturnPayment(SupplierID, SupplierInvoiceID, UserID, InvoiceNo, TotalAmount, PaymentAmount, RemainingBalance, CompanyID, BranchID, SupplierReturnInvoiceID, InvoiceDate) " +
                    "VALUES (@SupplierID, @SupplierInvoiceID, @UserID, @InvoiceNo, @TotalAmount, @PaymentAmount, @RemainingBalance, @CompanyID, @BranchID, @SupplierReturnInvoiceID, @InvoiceDate)";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(paymentQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@SupplierID", SupplierID);
                        cmd.Parameters.AddWithValue("@SupplierInvoiceID", SupplierInvoiceID);
                        cmd.Parameters.AddWithValue("@UserID", UserID);
                        cmd.Parameters.AddWithValue("@InvoiceNo", InvoiceNo);
                        cmd.Parameters.AddWithValue("@TotalAmount", TotalAmount);
                        cmd.Parameters.AddWithValue("@PaymentAmount", Amount);
                        cmd.Parameters.AddWithValue("@RemainingBalance", RemainingBalance);
                        cmd.Parameters.AddWithValue("@CompanyID", CompanyID);
                        cmd.Parameters.AddWithValue("@BranchID", BranchID);
                        cmd.Parameters.AddWithValue("@SupplierReturnInvoiceID", SupplierReturnInvoiceID);
                        cmd.Parameters.AddWithValue("@InvoiceDate", DateTime.Now.ToString("yyyy/MM/dd"));

                        cmd.ExecuteNonQuery();
                    }
                }

                // Insert transaction records
                string transactionQuery = "INSERT INTO tblTransaction (FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, UserID, InvoiceNo, Credit, Debit, TransectionDate, TransectionTitle, CompanyID, BranchID) " +
                                          "VALUES (@FinancialYearID, @AccountHeadID, @AccountControlID, @AccountSubControlID, @UserID, @InvoiceNo, @Credit, @Debit, @TransectionDate, @TransectionTitle, @CompanyID, @BranchID)";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    foreach (DataRow entryRow in dtEntries.Rows)
                    {
                        using (SqlCommand cmd = new SqlCommand(transactionQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@FinancialYearID", entryRow[0]);
                            cmd.Parameters.AddWithValue("@AccountHeadID", entryRow[1]);
                            cmd.Parameters.AddWithValue("@AccountControlID", entryRow[2]);
                            cmd.Parameters.AddWithValue("@AccountSubControlID", entryRow[3]);
                            cmd.Parameters.AddWithValue("@UserID", UserID);
                            cmd.Parameters.AddWithValue("@InvoiceNo", entryRow[6]);
                            cmd.Parameters.AddWithValue("@Credit", entryRow[8]);
                            cmd.Parameters.AddWithValue("@Debit", entryRow[9]);
                            cmd.Parameters.AddWithValue("@TransectionDate", entryRow[10]);
                            cmd.Parameters.AddWithValue("@TransectionTitle", entryRow[11]);
                            cmd.Parameters.AddWithValue("@CompanyID", CompanyID);
                            cmd.Parameters.AddWithValue("@BranchID", BranchID);

                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                return "Return Purchase Payment is Paid.";
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