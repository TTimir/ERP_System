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
    public class SP_TrialBalance
    {
        private CloudErpVEntities db = new CloudErpVEntities();

        public List<TrialBalanceModel> TrialBalances(int BranchID, int CompanyID, int FinancialYearID)
        {
            var trialBalance = new List<TrialBalanceModel>();
            SqlCommand cmd = new SqlCommand("GetTrialBalance", DatabaseQuery.ConnOpen());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BranchID", BranchID);
            cmd.Parameters.AddWithValue("@CompanyID", CompanyID);
            cmd.Parameters.AddWithValue("@FinancialYearID", FinancialYearID);
            var dt = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(dt);
            double totaldebit = 0;
            double totalcredit = 0;
            foreach (DataRow row in dt.Rows)
            {
                var balance = new TrialBalanceModel();
                balance.FinancialYearID = Convert.ToInt32(Convert.ToString(row[0]));
                balance.AccountSubControl = Convert.ToString(row[1]);
                balance.AccountSubControlID = Convert.ToInt32(Convert.ToString(row[2]));
                balance.Debit = Convert.ToDouble(row[3] == DBNull.Value ? 0 : row[3]);
                balance.Credit = Convert.ToDouble(row[4] == DBNull.Value ? 0 : row[4]);
                balance.BranchID = Convert.ToInt32(Convert.ToString(row[5]));
                balance.CompanyID = Convert.ToInt32(Convert.ToString(row[6]));
                totaldebit = totaldebit + Convert.ToDouble(row[3] == DBNull.Value ? 0 : row[3]);
                totalcredit = totalcredit + Convert.ToDouble(row[4] == DBNull.Value ? 0 : row[4]);
                if (balance.Debit > 0 || balance.Credit > 0)
                {
                    trialBalance.Add(balance);
                }
            }

            var totalBalance = new TrialBalanceModel();
            totalBalance.Credit = totalcredit;
            totalBalance.Debit = totaldebit;
            totalBalance.AccountSubControl = "Total";
            trialBalance.Add(totalBalance);
            return trialBalance;
        }
    }
}
