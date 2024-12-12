using CloudERP_Model.Models;
using DatabaseAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Security.AccessControl;
using System.Web;

namespace CloudERP_Model.HelperCls
{
    public class ListHealper
    {
        //private CloudErpVEntities db = new CloudErpVEntities();
        //public List<AccountControlMV> AccountControl()
        //{
        //    var tblAccountControls = db.tblAccountControls.Include(t => t.tblBranch).Include(t => t.tblCompany).Include(t => t.tblUser).Where(a => a.CompanyID == companyid && a.BranchID == branchid);

        //    foreach (var item in tblAccountControls)
        //    {
        //        accountControls.Add(new AccountControlMV
        //        {
        //            AccountControlID = item.AccountControlID,
        //            AccountControlName = item.AccountControlName,
        //            AccountHeadID = item.AccountHeadID,
        //            AccountHeadName = db.tblAccountHeads.Find(item.AccountHeadID).AccountHeadName,
        //            BranchID = item.BranchID,
        //            BranchName = item.tblBranch.BranchName,
        //            CompanyID = item.CompanyID,
        //            Name = item.tblCompany.Name,
        //            UserID = item.UserID,
        //            UserName = item.tblUser.UserName
        //        });
        //    }
        //}
    }
}