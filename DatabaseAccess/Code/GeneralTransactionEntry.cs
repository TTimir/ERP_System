using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Code
{
    public class GeneralTransactionEntry
    {
        private CloudErpVEntities db = new CloudErpVEntities();
        private string connectionString = @"data source=TIMIRBHINGRADIY;initial catalog=CloudErpV;user id=sa;password=sqlserver;trustservercertificate=True;";
        public string selectcustomerid = string.Empty;
        DataTable dtEntries = null;
        public string ConfirmGeneralTransaction(
            float TransferAmount,
            int UserID,
            int BranchID,
            int CompanyID,
            string invoiceNo,
            int DebitAccountControlID,
            int CreditAccountControlID,
            string Reason)
        {
            try
            {
                dtEntries = null;
                string transectiontitle = Reason;
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

                var account = db.tblAccountSubControls.Where(a => a.AccountSubControlID == DebitAccountControlID && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                AccountHeadID = Convert.ToString(account.AccountHeadID);
                AccountControlID = Convert.ToString(account.AccountControlID);
                AccountSubControlID = Convert.ToString(account.AccountSubControlID);
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, CompanyID.ToString(), BranchID.ToString(), invoiceNo, UserID.ToString(), "0", Convert.ToString(TransferAmount), DateTime.Now, transectiontitle);

                account = db.tblAccountSubControls.Where(a => a.AccountSubControlID == CreditAccountControlID && a.CompanyID == CompanyID && a.BranchID == BranchID).FirstOrDefault();
                AccountHeadID = Convert.ToString(account.AccountHeadID);
                AccountControlID = Convert.ToString(account.AccountControlID);
                AccountSubControlID = Convert.ToString(account.AccountSubControlID);
                transectiontitle = "General Transaction Succeed.";
                SetEntries(FinancialYearID, AccountHeadID, AccountControlID, AccountSubControlID, CompanyID.ToString(), BranchID.ToString(), invoiceNo, UserID.ToString(), Convert.ToString(TransferAmount), "0", DateTime.Now, transectiontitle);

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

                return "General Transaction Succeed.";
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
