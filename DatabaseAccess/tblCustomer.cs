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

    public partial class tblCustomer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblCustomer()
        {
            this.tblCustomerInvoices = new HashSet<tblCustomerInvoice>();
            this.tblCustomerPayments = new HashSet<tblCustomerPayment>();
            this.tblCustomerReturnInvoices = new HashSet<tblCustomerReturnInvoice>();
            this.tblCustomerReturnPayments = new HashSet<tblCustomerReturnPayment>();
        }

        public int CustomerID { get; set; }
        [Required(ErrorMessage = "*Required!")]
        public string Customername { get; set; }
        [Required(ErrorMessage = "*Required!")]
        [DataType(DataType.PhoneNumber)]
        public int CustomerContact { get; set; }
        [Required(ErrorMessage = "*Required!")]
        public string CustomerArea { get; set; }
        [Required(ErrorMessage = "*Required!")]
        public string CustomerAddress { get; set; }
        public string Description { get; set; }
        public int BranchID { get; set; }
        public int CompanyID { get; set; }
        public int UserID { get; set; }

        public virtual tblBranch tblBranch { get; set; }
        public virtual tblCompany tblCompany { get; set; }
        public virtual tblUser tblUser { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblCustomerInvoice> tblCustomerInvoices { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblCustomerPayment> tblCustomerPayments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblCustomerReturnInvoice> tblCustomerReturnInvoices { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblCustomerReturnPayment> tblCustomerReturnPayments { get; set; }
    }
}
