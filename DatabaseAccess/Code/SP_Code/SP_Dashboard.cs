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
    public class SP_Dashboard
    {
        private CloudErpVEntities db = new CloudErpVEntities();

        public DashboardDbModel GetDashboardValues(string fromDate, string toDate, int companyId, int branchId)
        {
            var dashboardData = new DashboardDbModel();

            try
            {
                SqlCommand cmd = new SqlCommand("GetDashboardValue", DatabaseQuery.ConnOpen());
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = fromDate;
                        cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = toDate;
                        cmd.Parameters.Add("@CompanyID", SqlDbType.Int).Value = companyId;
                        cmd.Parameters.Add("@BranchID", SqlDbType.Int).Value = branchId;

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        dashboardData.CurrentMonthRevenue = reader["Current Month Revenue"] != DBNull.Value
                            ? Convert.ToDouble(reader["Current Month Revenue"])
                            : 0;
                        dashboardData.CurrentMonthExpenses = reader["Current Month Expenses"] != DBNull.Value
                            ? Convert.ToDouble(reader["Current Month Expenses"])
                            : 0;
                        dashboardData.NetIncome = reader["Net Income"] != DBNull.Value
                            ? Convert.ToDouble(reader["Net Income"])
                            : 0;
                        dashboardData.Capital = reader["Capital"] != DBNull.Value
                            ? Convert.ToDouble(reader["Capital"])
                            : 0;
                        dashboardData.CurrentMonthRecovery = reader["Current Month Recovery"] != DBNull.Value
                            ? Convert.ToDouble(reader["Current Month Recovery"])
                            : 0;
                        dashboardData.CashPlusBankAccount = reader["Cash/Bank Balance"] != DBNull.Value
                            ? Convert.ToDouble(reader["Cash/Bank Balance"])
                            : 0;
                        dashboardData.TotalReceivable = reader["Total Receivable"] != DBNull.Value
                            ? Convert.ToDouble(reader["Total Receivable"])
                            : 0;
                        dashboardData.TotalPayable = reader["Total Payable"] != DBNull.Value
                            ? Convert.ToDouble(reader["Total Payable"])
                            : 0;
                    }
                    else
                    {
                        Console.WriteLine("No data returned from the stored procedure.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }

            return dashboardData;
        }
    }
}
