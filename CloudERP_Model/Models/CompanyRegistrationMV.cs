using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CloudERP_Model.Models
{
    public class CompanyRegistrationMV
    {
        // User Details
        public int UserTypeID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool UserStatus { get; set; }

        // Company Details
        [Required(ErrorMessage = "*Required!")]
        [StringLength(100, ErrorMessage = "Name must be under 100 characters")]
        public string CName { get; set; }

        //Branch Details
        public string BranchName { get; set; }
        public string BranchContact { get; set; }
        public string BranchAddress { get; set; }

        // Employee Details
        public int EmployeeID { get; set; }
        public string EmpName { get; set; }
        public string EmpContactNo { get; set; }
        public string EmpEmail { get; set; }
        public string EmpAddress { get; set; }
        public string EmpCNIC { get; set; }
        public string EmpDesignation { get; set; }
        public string EmpDescription { get; set; }
        public double EmpMonthlySalary { get; set; }
    }
}