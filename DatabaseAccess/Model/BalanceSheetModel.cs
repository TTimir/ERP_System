using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Model
{
    public class BalanceSheetModel
    {
        public string Title { get; set; }
        public double TotalAssets { get; set; }
        public double ReturnEarning { get; set; }
        public double Total_Liablties_OwnerEquity_ReturnEarning { get; set; }
        public List<AccountHeadTotal> accountHeadTotals { get; set; }
    }

    public class AccountHeadTotal
    {
        public string AccountHeadTitle { get; set; }
        public double TotalAmount { get; set; }
        public List<AccountHeadDetails> AccountHeadDetails { get; set; }
    }

    public class AccountHeadDetails
    {
        public string AccountSubTitle { get; set; }
        public double TotalAmount { get; set; }
        public string Status { get; set; }
    }
}
