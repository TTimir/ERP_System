using DatabaseAccess.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting.Messaging;

namespace DatabaseAccess.Code.SP_Code
{
    public class SP_BalanceSheet
    {
        private CloudErpVEntities db = new CloudErpVEntities();
        public BalanceSheetModel GetBalanceSheet(int CompanyID, int BranchID, int FinancialYearID, List<int> HeadIDs)
        {
            var BalanceSheet = new BalanceSheetModel();
            double TotalAssets = 0;
            double TotalLiablities = 0;
            double TotalOwnerEquity = 0;
            double TotalReturnEarning = 0;

            // Return Earning Fields
            double TotalExpenses = 0;
            double TotalRevenue = 0;
            var AllHead = new List<AccountHeadTotal>();
            foreach (var HeadID in HeadIDs)
            {
                var AccountHead = new AccountHeadTotal();
                if (HeadID == 1 || HeadID == 2 || HeadID == 4) // Total Assets
                {
                    AccountHead = GetHeadAccountsWithTotal(CompanyID, BranchID, FinancialYearID, HeadID);
                    if (HeadID == 1)
                    {
                        TotalAssets = GetHeadAccountTotalAmount(CompanyID, BranchID, FinancialYearID, HeadID);
                    }
                    else if (HeadID == 2)
                    {
                        TotalLiablities = GetHeadAccountTotalAmount(CompanyID, BranchID, FinancialYearID, HeadID);
                    }
                    else if (HeadID == 4)
                    {
                        TotalOwnerEquity = GetHeadAccountTotalAmount(CompanyID, BranchID, FinancialYearID, HeadID);
                    }
                    AllHead.Add(AccountHead);
                }
                else if (HeadID == 3) // Total Expenses
                {
                    AccountHead = GetHeadAccountsWithTotal(CompanyID, BranchID, FinancialYearID, HeadID);
                    TotalExpenses = AccountHead.TotalAmount;
                }
                else if (HeadID == 5) // Total Revenue
                {
                    AccountHead = GetHeadAccountsWithTotal(CompanyID, BranchID, FinancialYearID, HeadID);
                    TotalRevenue = AccountHead.TotalAmount;
                }
            }
            TotalReturnEarning = TotalRevenue - TotalExpenses;
            BalanceSheet.Title = "Total Balance";
            BalanceSheet.ReturnEarning = TotalReturnEarning;
            BalanceSheet.Total_Liablties_OwnerEquity_ReturnEarning = TotalLiablities + TotalOwnerEquity + TotalReturnEarning;
            BalanceSheet.TotalAssets = TotalAssets;
            BalanceSheet.accountHeadTotals = AllHead;

            return BalanceSheet;
        }

        public double GetHeadAccountTotalAmount(int CompanyID, int BranchID, int FinancialYearID, int HeadID)
        {
            SqlCommand cmd = new SqlCommand("GetTotalByHeadAccount", DatabaseQuery.ConnOpen());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BranchID", BranchID);
            cmd.Parameters.AddWithValue("@CompanyID", CompanyID);
            cmd.Parameters.AddWithValue("@HeadID", HeadID);
            cmd.Parameters.AddWithValue("@FinancialYearID", FinancialYearID);
            var dt = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(dt);
            double TotalAmount = 0;
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    TotalAmount = Convert.ToDouble(dt.Rows[0][0]);
                }
            }
            return TotalAmount;
        }

        public AccountHeadTotal GetHeadAccountsWithTotal(int CompanyID, int BranchID, int FinancialYearID, int HeadID)
        {
            var AccountHeadWithDetails = new AccountHeadTotal();
            SqlCommand cmd = new SqlCommand("GetAccountHeadDetails", DatabaseQuery.ConnOpen());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BranchID", BranchID);
            cmd.Parameters.AddWithValue("@CompanyID", CompanyID);
            cmd.Parameters.AddWithValue("@HeadID", HeadID);
            cmd.Parameters.AddWithValue("@FinancialYearID", FinancialYearID);
            var dt = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(dt);
            var AccountList = new List<AccountHeadDetails>();
            double TotalAmount = 0;
            foreach (DataRow row in dt.Rows)
            {
                var account = new AccountHeadDetails();
                account.AccountSubTitle = Convert.ToString(row[0]);
                account.TotalAmount = Convert.ToDouble(row[1]);
                account.Status = Convert.ToString(row[2]);
                TotalAmount = TotalAmount - account.TotalAmount;
                if (account.TotalAmount > 0)
                {
                    AccountList.Add(account);
                }
            }

            var accountHead = db.tblAccountHeads.Find(HeadID);
            AccountHeadWithDetails.TotalAmount = TotalAmount;
            AccountHeadWithDetails.AccountHeadTitle = accountHead.AccountHeadName;
            AccountHeadWithDetails.AccountHeadDetails = AccountList;
            return AccountHeadWithDetails;
        }


    }
}
