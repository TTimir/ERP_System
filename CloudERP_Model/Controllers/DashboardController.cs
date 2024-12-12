using CloudERP_Model.Models;
using DatabaseAccess.Code.SP_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CloudERP_Model.Controllers
{
    public class DashboardController : Controller
    {
        private readonly SP_Dashboard dashboardService = new SP_Dashboard();

        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            int companyId = Convert.ToInt32(Session["CompanyID"]);
            int branchId = Convert.ToInt32(Session["BranchID"]);
            string fromDate = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd");
            string toDate = DateTime.Now.ToString("yyyy-MM-dd");

            var dashboardData = dashboardService.GetDashboardValues(fromDate, toDate, companyId, branchId);

            var dashboardViewModel = new DashboardValueMV
            {
                CurrentMonthRevenue = dashboardData.CurrentMonthRevenue,
                CurrentMonthExpenses = dashboardData.CurrentMonthExpenses,
                NetIncome = dashboardData.NetIncome,
                Capital = dashboardData.Capital,
                CurrentMonthRecovery = dashboardData.CurrentMonthRecovery,
                CashPlusBankAccount = dashboardData.CashPlusBankAccount,
                TotalReceivable = dashboardData.TotalReceivable,
                TotalPayable = dashboardData.TotalPayable
            };

            return View(dashboardViewModel);
        }
    }
}