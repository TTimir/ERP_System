//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DatabaseAccess
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class tblAccountSetting
    {
        public int AccountSettingID { get; set; }
        [Required(ErrorMessage = "*Required!")]
        public int AccountHeadID { get; set; }
        [Required(ErrorMessage = "*Required!")]
        public int AccountControlID { get; set; }
        [Required(ErrorMessage = "*Required!")]
        public int AccountSubControlID { get; set; }
        [Required(ErrorMessage = "*Required!")]
        public int AccountActivityID { get; set; }
        public int CompanyID { get; set; }
        public int BranchID { get; set; }

        public virtual tblAccountActivity tblAccountActivity { get; set; }
        public virtual tblAccountControl tblAccountControl { get; set; }
        public virtual tblAccountHead tblAccountHead { get; set; }
        public virtual tblAccountSubControl tblAccountSubControl { get; set; }
        public virtual tblBranch tblBranch { get; set; }
        public virtual tblCompany tblCompany { get; set; }
    }
}
