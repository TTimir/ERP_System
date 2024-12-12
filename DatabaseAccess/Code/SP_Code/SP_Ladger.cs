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
    public class SP_Ladger
    {
        private CloudErpVEntities db = new CloudErpVEntities();
        public List<AccountLadgerModel> GetLadger(int CompanyID, int BranchID, int FinancialYearID)
        {
            int sno = 0;
            var ladger = new List<AccountLadgerModel>();
            SqlCommand cmd = new SqlCommand("GetLadger", DatabaseQuery.ConnOpen());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BranchID", BranchID);
            cmd.Parameters.AddWithValue("@CompanyID", CompanyID);
            cmd.Parameters.AddWithValue("@FinancialYearID", FinancialYearID);
            var journaldt = new DataTable();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(journaldt);
            if (journaldt == null)
            {
                ladger.Clear();
                return ladger;
            }
            if (journaldt.Rows.Count == 0)
            {
                ladger.Clear();
                return ladger;
            }

            double debit = 0;
            double credit = 0;
            int totalrecords = 0;
            string accountname = string.Empty;
            foreach (DataRow row in journaldt.Rows)
            {
                double ldebit = 0;
                double lcredit = 0;
                if (accountname == Convert.ToString(row[7]).Trim()) // Check Account Title
                {
                    var createrow = new AccountLadgerModel();
                    createrow.Sno = sno;
                    createrow.Date = Convert.ToString(row[9]);
                    createrow.Description = Convert.ToString(row[10]);
                    double.TryParse(Convert.ToString(row[11]), out ldebit);
                    debit = debit + ldebit;
                    createrow.Debit = Convert.ToString(row[11]); // Debit
                    double.TryParse(Convert.ToString(row[12]), out lcredit);
                    credit = credit + lcredit;
                    createrow.Credit = Convert.ToString(row[12]); // Credit
                    ladger.Add(createrow);
                }
                else
                {
                    if (!string.IsNullOrEmpty(accountname))
                    {
                        var totalrow = new AccountLadgerModel();

                        totalrow.Sno = sno;
                        sno = sno + 1;
                        totalrow.Date = "Total"; // Account Control Name
                        if (credit >= debit)
                        {
                            totalrow.Credit = Convert.ToString(debit - credit).Replace('-', ' ');
                        }
                        else if (credit <= debit)
                        {
                            totalrow.Debit = Convert.ToString(debit - credit).Replace('-', ' ');
                        }
                        totalrow.Date = "Total Balance"; // Account Control Name
                        ladger.Add (totalrow);
                        debit = 0;
                        credit = 0;
                    }

                    var headerrow = new AccountLadgerModel();
                    headerrow.Sno = sno;
                    sno += 1;
                    headerrow.Account = Convert.ToString(row[7]); // Account Control Name
                    headerrow.Date = "Date";
                    headerrow.Description = "Description";
                    headerrow.Debit = "Debit";
                    headerrow.Credit = "Credit";
                    ladger.Add(headerrow);

                    var createrow = new AccountLadgerModel();
                    createrow.Sno = sno;
                    sno += 1;
                    createrow.Date = Convert.ToString(row[9]); // Transaction Date
                    createrow.Description = Convert.ToString(row[10]); // Transaction Title
                    double.TryParse(Convert.ToString(row[11]), out ldebit);
                    debit += ldebit;
                    createrow.Debit = Convert.ToString(row[11]); // Debit
                    double.TryParse(Convert.ToString(row[12]), out lcredit);
                    credit += + lcredit;
                    createrow.Credit = Convert.ToString(row[12]); // Credit
                    ladger.Add(createrow);
                    accountname = Convert.ToString(row[7]).Trim();
                }

                totalrecords = totalrecords + 1;
                if (totalrecords == journaldt.Rows.Count)
                {
                    var totalrow = new AccountLadgerModel();
                    totalrow.Sno = sno;
                    sno += 1;
                    totalrow.Date = "Total"; // Account Control Name
                    if (credit >= debit)
                    {
                        totalrow.Credit = Convert.ToString(debit - credit).Replace('-', ' ');
                    }
                    else if (credit <= debit)
                    {
                        totalrow.Debit = Convert.ToString(debit - credit).Replace('-', ' ');
                    }
                    totalrow.Date = "Total Balance"; // Account Control Name
                    ladger.Add(totalrow);
                    debit = 0;
                    credit = 0;
                }
            }

            return ladger;
        }

    }
}
