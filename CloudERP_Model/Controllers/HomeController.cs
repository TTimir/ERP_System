using DatabaseAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CloudERP_Model.Controllers
{
    public class HomeController : Controller
    {
        private CloudErpVEntities db = new CloudErpVEntities();

        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LoginUser(string email, string password)
        {
            var user = db.tblUsers.Where(u => u.Email == email && u.Password == password && u.IsActive == true).FirstOrDefault();
            if (user != null)
            {
                Session["UserID"] = user.UserID;
                Session["UserTypeID"] = user.UserTypeID;
                Session["FullName"] = user.FullName;
                Session["Email"] = user.Email;
                Session["ContactNo"] = user.ContactNo;
                Session["UserName"] = user.UserName;
                Session["Password"] = user.Password;
                Session["IsActive"] = user.IsActive;

                var EmployeeDetails = db.tblEmployees.Where(e => e.UserID == user.UserID).FirstOrDefault();
                if (EmployeeDetails == null)
                {
                    ViewBag.Message = "Please contact the administrator!";

                    Session["UserTypeID"] = string.Empty;
                    Session["FullName"] = string.Empty;
                    Session["Email"] = string.Empty;
                    Session["ContactNo"] = string.Empty;
                    Session["UserName"] = string.Empty;
                    Session["Password"] = string.Empty;
                    Session["IsActive"] = string.Empty;
                    Session["EmployeeID"] = string.Empty;
                    Session["EmpName"] = string.Empty;
                    Session["EmpPhoto"] = string.Empty;
                    Session["Designation"] = string.Empty;
                    Session["BranchID"] = string.Empty;
                    Session["CompanyID"] = string.Empty;
                    return View("Login");
                }

                Session["EmployeeID"] = EmployeeDetails.EmployeeID;
                Session["EmpName"] = EmployeeDetails.Name;
                Session["EmpPhoto"] = EmployeeDetails.Photo;
                Session["Designation"] = EmployeeDetails.Designation;
                Session["BranchID"] = EmployeeDetails.BranchID;
                Session["CompanyID"] = EmployeeDetails.CompanyID;

                var company = db.tblCompanies.Where(c => c.CompanyID == EmployeeDetails.CompanyID).FirstOrDefault();
                if (company == null)
                {
                    ViewBag.Message = "Please contact the administrator!";

                    Session["UserTypeID"] = string.Empty;
                    Session["FullName"] = string.Empty;
                    Session["Email"] = string.Empty;
                    Session["ContactNo"] = string.Empty;
                    Session["UserName"] = string.Empty;
                    Session["Password"] = string.Empty;
                    Session["IsActive"] = string.Empty;
                    Session["EmployeeID"] = string.Empty;
                    Session["EmpName"] = string.Empty;
                    Session["EmpPhoto"] = string.Empty;
                    Session["Designation"] = string.Empty;
                    Session["BranchID"] = string.Empty;
                    Session["CompanyID"] = string.Empty;
                    return View("Login", "Home");
                }

                Session["CName"] = company.Name;
                Session["CLogo"] = company.Logo;
                var BranchType = db.tblBranches.Where(b => b.BranchID == EmployeeDetails.BranchID).FirstOrDefault();
                if (BranchType == null)
                {
                    ViewBag.Message = "Please contact the administrator!";
                    return View("Login");
                }
                Session["BranchTypeID"] = BranchType.BranchTypeID;
                Session["BrchID"] = BranchType.BrchID == null ? 0 : BranchType.BrchID;
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Message = "Incorrect credentials! try again.";

                Session["UserTypeID"] = string.Empty;
                Session["FullName"] = string.Empty;
                Session["Email"] = string.Empty;
                Session["ContactNo"] = string.Empty;
                Session["UserName"] = string.Empty;
                Session["Password"] = string.Empty;
                Session["IsActive"] = string.Empty;
                Session["EmployeeID"] = string.Empty;
                Session["EmpName"] = string.Empty;
                Session["EmpPhoto"] = string.Empty;
                Session["Designation"] = string.Empty;
                Session["BranchID"] = string.Empty;
                Session["CompanyID"] = string.Empty;
                Session["BrchID"] = string.Empty;
            }

            return View("Login");
        }

        public ActionResult Logout()
        {
            Session["UserTypeID"] = string.Empty;
            Session["FullName"] = string.Empty;
            Session["Email"] = string.Empty;
            Session["ContactNo"] = string.Empty;
            Session["UserName"] = string.Empty;
            Session["Password"] = string.Empty;
            Session["IsActive"] = string.Empty;
            Session["EmployeeID"] = string.Empty;
            Session["EmpName"] = string.Empty;
            Session["EmpPhoto"] = string.Empty;
            Session["Designation"] = string.Empty;
            Session["BranchID"] = string.Empty;
            Session["CompanyID"] = string.Empty;
            Session["BrchID"] = string.Empty;
            Session.Clear();
            Session.Abandon();

            return View("Login");
        }

        public ActionResult ForgetPassword()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }
        [Route("LockScreen")]
        public ActionResult LS()
        {
            return View("Login");
        }

        public ActionResult About()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login");
            }

            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login");
            }

            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}