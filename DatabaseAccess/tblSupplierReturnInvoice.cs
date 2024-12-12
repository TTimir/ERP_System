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
    
    public partial class tblSupplierReturnInvoice
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblSupplierReturnInvoice()
        {
            this.tblSupplierReturnInvoiceDetails = new HashSet<tblSupplierReturnInvoiceDetail>();
            this.tblSupplierReturnPayments = new HashSet<tblSupplierReturnPayment>();
        }
    
        public int SupplierReturnInvoiceID { get; set; }
        public int SupplierInvoiceID { get; set; }
        public int CompanyID { get; set; }
        public int BranchID { get; set; }
        public string InvoiceNo { get; set; }
        public double TotalAmount { get; set; }
        public System.DateTime InvoiceDate { get; set; }
        public string Description { get; set; }
        public int UserID { get; set; }
        public int SupplierID { get; set; }
    
        public virtual tblBranch tblBranch { get; set; }
        public virtual tblCompany tblCompany { get; set; }
        public virtual tblSupplier tblSupplier { get; set; }
        public virtual tblSupplierInvoice tblSupplierInvoice { get; set; }
        public virtual tblUser tblUser { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblSupplierReturnInvoiceDetail> tblSupplierReturnInvoiceDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblSupplierReturnPayment> tblSupplierReturnPayments { get; set; }
    }
}
