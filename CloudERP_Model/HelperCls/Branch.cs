using CloudERP_Model.Models;
using DatabaseAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CloudERP_Model.HelperCls
{
    public class Branch
    {

        public static List<int> GetBranchids(int? brid, CloudErpVEntities db)
        {
            List<int> branchids = new List<int>();
            List<int> IsSubBranchs1 = new List<int>();
            List<int> IsSubBranchs2 = new List<int>();
            List<BranchsCustomerMV> list = new List<BranchsCustomerMV>();

            int branchid = 0;

            branchid = Convert.ToInt32(brid);
            var brnch = db.tblBranches.Where(b => b.BrchID == branchid);
            foreach (var item in brnch)
            {
                IsSubBranchs1.Add(item.BranchID);
            }

        subbranch:
            foreach (var item in IsSubBranchs1)
            {
                branchids.Add(item);
                foreach (var sub in db.tblBranches.Where(b => b.BrchID == item))
                {
                    IsSubBranchs2.Add(sub.BranchID);
                }
            }
            if (IsSubBranchs2.Count > 0)
            {
                IsSubBranchs1.Clear();
                foreach (var item in IsSubBranchs2)
                {
                    IsSubBranchs1.Add(item);
                }
                IsSubBranchs2.Clear();
                goto subbranch;
            }
            foreach (var item in branchids)
            {
                foreach (var customer in db.tblCustomers.Where(c => c.BranchID == item))
                {
                    var cus = new BranchsCustomerMV();
                    cus.BranchName = customer.tblBranch.BranchName;
                    cus.CompanyName = customer.tblCompany.Name;
                    cus.CustomerAddress = customer.CustomerAddress;
                    cus.CustomerArea = customer.CustomerArea;
                    cus.CustomerContact = customer.CustomerContact;
                    cus.Description = customer.Description;
                    cus.User = customer.tblUser.UserName;
                    list.Add(cus);
                }

            }

            return branchids;
        }
    }
}