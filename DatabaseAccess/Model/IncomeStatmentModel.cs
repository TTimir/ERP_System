using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Model
{
    public class IncomeStatmentModel
    {
        public string Title { get; set; }
        public double NetIncome { get; set; }
        public List<IncomeStatmentHead> incomeStatmentHeads { get; set; }
    }
    public class IncomeStatmentHead
    {
        public string Title { get; set; }
        public double TotalAmount { get; set; }
        public AccountHeadTotal AccountHead { get; set; }

    }
}
