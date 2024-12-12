﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CloudERP_Model.Models
{
    public class BranchsCustomerMV
    {
        public string CompanyName { get; set; }
        public string BranchName { get; set; }
        public string Customername { get; set; }

        public int CustomerContact { get; set; }
        public string CustomerArea { get; set; }
        public string CustomerAddress { get; set; }
        public string Description { get; set; }

        public string User { get; set; }
    }
}