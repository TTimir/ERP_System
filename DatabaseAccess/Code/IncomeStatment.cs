using DatabaseAccess.Code.SP_Code;
using DatabaseAccess.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Code
{
    public class IncomeStatment
    {
        SP_BalanceSheet income = new SP_BalanceSheet();
        public IncomeStatmentModel GetIncomeStatment(int CompanyID, int BranchID, int FinancialYearID)
        {
            var incomestatment = new IncomeStatmentModel();
            incomestatment.incomeStatmentHeads = new List<IncomeStatmentHead>();
            incomestatment.Title = "Net Income";
            
            var revenue = income.GetHeadAccountsWithTotal(CompanyID, BranchID, FinancialYearID, 5);
            var revenuedetails = new IncomeStatmentHead();
            revenuedetails.Title = "Total Revenue";
            revenuedetails.TotalAmount = Math.Abs(revenue.TotalAmount);
            revenuedetails.AccountHead = revenue;
            incomestatment.incomeStatmentHeads.Add(revenuedetails);


            var expenses = income.GetHeadAccountsWithTotal(CompanyID, BranchID, FinancialYearID, 3);
            var expensesdetails = new IncomeStatmentHead();
            expensesdetails.Title = "Total Expenses";
            expensesdetails.TotalAmount = Math.Abs(expenses.TotalAmount);
            expensesdetails.AccountHead = expenses;
            incomestatment.incomeStatmentHeads.Add(expensesdetails);

            incomestatment.NetIncome = Math.Abs(revenuedetails.TotalAmount) - Math.Abs(expensesdetails.TotalAmount);

            return incomestatment;
        }
    }
}
