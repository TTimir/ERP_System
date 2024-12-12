using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CloudERP_Model.Models
{
    public class DashboardValueMV
    {
        [Display(Name = "Current Month Revenue")]
        public double CurrentMonthRevenue { get; set; }

        [Display(Name = "Current Month Expenses")]
        public double CurrentMonthExpenses { get; set; }

        [Display(Name = "Net Income")]
        public double NetIncome { get; set; }

        [Display(Name = "Capital")]
        public double Capital { get; set; }

        [Display(Name = "Current Month Recovery")]
        public double CurrentMonthRecovery { get; set; }

        [Display(Name = "Cash/Bank Balance")]
        public double CashPlusBankAccount { get; set; }

        [Display(Name = "Total Receivable")]
        public double TotalReceivable { get; set; }

        [Display(Name = "Total Payable")]
        public double TotalPayable { get; set; }
    }
}