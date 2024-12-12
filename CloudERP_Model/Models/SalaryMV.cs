using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CloudERP_Model.Models
{
    public class SalaryMV
    {
        public int EmployeeID { get; set; }
        [Required(ErrorMessage = "*")]
        public string EmployeeName { get; set; }
        public string CNIC { get; set; }
        public string Designation  { get; set; }
        [Required(ErrorMessage = "*")]
        [DataType(DataType.Currency)]
        public double TransferAmount { get; set; }
        [Required(ErrorMessage = "*")]
        public string SalaryMonth { get; set; }
        [Required(ErrorMessage = "*")]
        public string SalaryYear { get; set; }
    }
}