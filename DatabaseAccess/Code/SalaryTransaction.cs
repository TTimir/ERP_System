using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Code
{
    public class SalaryTransaction
    {
        private CloudErpVEntities db = new CloudErpVEntities();
        private string connectionString = @"data source=TIMIRBHINGRADIY;initial catalog=CloudErpV;user id=sa;password=sqlserver;trustservercertificate=True;";
        DataTable dtEntries = null;
        public string Confirm(
            int EmployeeID,
            double TransferAmount,
            int UserID,
            int BranchID,
            int CompanyID,
            string invoiceNo,
            string SalaryMonth,
            string SalaryYear)
        {
            try
            {
                dtEntries = null;
                string transectiontitle = "Salary is Pending";
                var employee = db.tblEmployees.Find(EmployeeID);
                string employeename = string.Empty;
                if (employee != null)
                {
                    employeename = ", To" + employee.Name;
                }
                transectiontitle = transectiontitle + employeename;

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

                var account = db.tblAccountSettings.Where(a => a.AccountActivityID == 5 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                if (account == null || account.AccountHeadID == null || account.AccountControlID == null || account.AccountSubControlID == null)
                {
                    return "Salary Account (i.e. Salary Expenses) entry not found in your Account Flow.";
                }
                AccountHeadID = Convert.ToString(account.AccountHeadID);
                AccountControlID = Convert.ToString(account.AccountControlID);
                AccountSubControlID = Convert.ToString(account.AccountSubControlID);
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, CompanyID.ToString(), BranchID.ToString(), invoiceNo, UserID.ToString(), "0", Convert.ToString(TransferAmount), DateTime.Now, transectiontitle);

                account = db.tblAccountSettings.Where(a => a.AccountActivityID == 18 && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                if (account == null || account.AccountHeadID == null || account.AccountControlID == null || account.AccountSubControlID == null)
                {
                    return "Salary Account (i.e. Salary Payment Succeed) entry not found in your Account Flow.";
                }
                AccountHeadID = Convert.ToString(account.AccountHeadID);
                AccountControlID = Convert.ToString(account.AccountControlID);
                AccountSubControlID = Convert.ToString(account.AccountSubControlID);
                transectiontitle = "Salary Succeed" + employeename;
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, CompanyID.ToString(), BranchID.ToString(), invoiceNo, UserID.ToString(), Convert.ToString(TransferAmount), "0", DateTime.Now, transectiontitle);

                // Insert supplier payment
                string paymentQuery = "INSERT INTO tblPayroll(EmployeeID, BranchID, CompanyID, TransferAmount, PayrollInvoiceNo, PaymentDate, SalaryMonth, SalaryYear, UserID) " +
                    "VALUES (@EmployeeID, @BranchID, @CompanyID, @TransferAmount, @PayrollInvoiceNo, @PaymentDate, @SalaryMonth, @SalaryYear, @UserID)";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(paymentQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeID", EmployeeID);
                        cmd.Parameters.AddWithValue("@BranchID", BranchID);
                        cmd.Parameters.AddWithValue("@CompanyID", CompanyID);
                        cmd.Parameters.AddWithValue("@TransferAmount", TransferAmount);
                        cmd.Parameters.AddWithValue("@PayrollInvoiceNo", invoiceNo);
                        cmd.Parameters.AddWithValue("@PaymentDate", DateTime.Now.ToString("yyyy/MM/dd"));
                        cmd.Parameters.AddWithValue("@SalaryMonth", SalaryMonth);
                        cmd.Parameters.AddWithValue("@SalaryYear", SalaryYear);
                        cmd.Parameters.AddWithValue("@UserID", UserID);

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

                return "Salary Paid Succeed.";
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
