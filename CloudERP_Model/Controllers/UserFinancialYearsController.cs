using DatabaseAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CloudERP_Model.Controllers
{
    public class UserFinancialYearsController : Controller
    {
        private CloudErpVEntities db = new CloudErpVEntities();

        // GET: UserFinancialYears
        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int userid = 0;
            userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            var tblFinancialYears = db.tblFinancialYears.Where(f => f.IsActive == true);
            return View(tblFinancialYears.ToList());
        }
    }
}