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
    
    public partial class tblAccountSubControl
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblAccountSubControl()
        {
            this.tblAccountSettings = new HashSet<tblAccountSetting>();
            this.tblTransactions = new HashSet<tblTransaction>();
        }
    
        public int AccountSubControlID { get; set; }
        public int AccountHeadID { get; set; }
        public int AccountControlID { get; set; }
        public int CompanyID { get; set; }
        public int BranchID { get; set; }
        public string AccountSubControlName { get; set; }
        public int UserID { get; set; }
    
        public virtual tblAccountControl tblAccountControl { get; set; }
        public virtual tblAccountHead tblAccountHead { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblAccountSetting> tblAccountSettings { get; set; }
        public virtual tblBranch tblBranch { get; set; }
        public virtual tblUser tblUser { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblTransaction> tblTransactions { get; set; }
    }
}
