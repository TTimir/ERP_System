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
    public class SP_GeneralTransaction
    {
        private CloudErpVEntities db = new CloudErpVEntities();
        public List<AllAccountModel> GetAllAccounts(int CompanyID, int BranchID)
        {
            var accountlist = new List<AllAccountModel>();
            SqlCommand cmd = new SqlCommand("GetAllAccounts", DatabaseQuery.ConnOpen());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BranchID", BranchID);
            cmd.Parameters.AddWithValue("@CompanyID", CompanyID);
            var dt = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(dt);
            foreach (DataRow row in dt.Rows)
            {
                var account = new AllAccountModel();
                account.AccountHeadID = Convert.ToInt32(row[0].ToString());
                account.AccountHeadName = Convert.ToString(row[1]);
                account.AccountControlID = Convert.ToInt32(row[2].ToString());
                account.AccountControlName = Convert.ToString(row[3]);
                account.BranchID = Convert.ToInt32(row[4].ToString());
                account.CompanyID = Convert.ToInt32(row[5].ToString());
                account.AccountSubControlID = Convert.ToInt32(row[6].ToString());
                account.AccountSubControl = Convert.ToString(row[7]);
                accountlist.Add(account);
            }
            return accountlist;
        }

        public List<JournalModel> GetJournal(int CompanyID, int BranchID, DateTime FromDate, DateTime ToDate)
        {
            var journalEntry = new List<JournalModel>();
            SqlCommand cmd = new SqlCommand("GetJournals", DatabaseQuery.ConnOpen());
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
                var jEntry = new JournalModel();
                jEntry.TransectionDate = Convert.ToDateTime(row[0].ToString());
                jEntry.AccountSubControl = Convert.ToString(row[1]);
                jEntry.TransectionTitle = Convert.ToString(row[2]);
                jEntry.AccountSubControlID = Convert.ToInt32(row[3]);
                jEntry.InvoiceNo = Convert.ToString(row[4]);
                jEntry.Debit = Convert.ToDouble(row[5]);
                jEntry.Credit = Convert.ToDouble(row[6]);
                journalEntry.Add(jEntry);
            }
            return journalEntry;
        }

    }
}
