using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Model
{
    public class DashboardDbModel
    {
        public double CurrentMonthRevenue { get; set; }
        public double CurrentMonthExpenses { get; set; }
        public double NetIncome { get; set; }
        public double Capital { get; set; }
        public double CurrentMonthRecovery { get; set; }
        public double CashPlusBankAccount { get; set; }
        public double TotalReceivable { get; set; }
        public double TotalPayable { get; set; }
    }
}
