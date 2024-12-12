using DatabaseAccess;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CloudERP_Model.Controllers
{
    public class CompanyRegistrationController : Controller
    {
        private CloudErpVEntities db = new CloudErpVEntities();

        // GET: CompanyRegistration
        public ActionResult RegistrationForm()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }
        [HttpPost]
        public ActionResult RegistrationForm(
            string UserName,
            string Password,
            string ConPassword,
            string EmpName,
            string EmpContactNo,
            string EmpEmail,
            string EmpCNIC,
            string EmpDesignation,
            float EmpMonthlySalary,
            string EmpAddress,
            string CName,
            string BranchName,
            string BranchContact,
            string BranchAddress
            )
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            try
            {
                if (!string.IsNullOrEmpty(UserName)
                       && !string.IsNullOrEmpty(Password)
                       && !string.IsNullOrEmpty(ConPassword)
                       && !string.IsNullOrEmpty(EmpName)
                       && !string.IsNullOrEmpty(EmpContactNo)
                       && !string.IsNullOrEmpty(EmpEmail)
                       && !string.IsNullOrEmpty(EmpCNIC)
                       && !string.IsNullOrEmpty(EmpDesignation)
                       && EmpMonthlySalary > 0
                       && !string.IsNullOrEmpty(EmpAddress)
                       && !string.IsNullOrEmpty(CName)
                       && !string.IsNullOrEmpty(BranchName)
                       && !string.IsNullOrEmpty(BranchContact)
                       && !string.IsNullOrEmpty(BranchAddress)
                       )
                {
                    var company = new tblCompany()
                    {
                        Name = CName,
                        Logo = string.Empty,
                    };
                    db.tblCompanies.Add(company);
                    db.SaveChanges();

                    var branch = new tblBranch()
                    {
                        BranchAddress = BranchAddress,
                        BranchContact = BranchContact,
                        BranchName = BranchName,
                        BranchTypeID = 1,
                        CompanyID = company.CompanyID,
                        BrchID = null,
                    };
                    db.tblBranches.Add(branch);
                    db.SaveChanges();

                    var user = new tblUser()
                    {
                        ContactNo = EmpContactNo,
                        Email = EmpEmail,
                        FullName = EmpName,
                        IsActive = true,
                        Password = Password,
                        UserName = UserName,
                        UserTypeID = 2,
                    };
                    db.tblUsers.Add(user);
                    db.SaveChanges();

                    var employee = new tblEmployee()
                    {
                        Address = EmpAddress,
                        BranchID = branch.BranchID,
                        CNIC = EmpCNIC,
                        CompanyID = company.CompanyID,
                        ContactNo = EmpContactNo,
                        Designation = EmpDesignation,
                        Email = EmpEmail,
                        MonthlySalary = EmpMonthlySalary,
                        UserID = user.UserID,
                        Name = EmpName,
                        Description = string.Empty,
                        Photo = string.Empty
                    };
                    db.tblEmployees.Add(employee);
                    db.SaveChanges();
                    ViewBag.Message = "Registration Successfull.";
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    ViewBag.Message = "Please Provide Correct Details!";
                    return View("RegistrationForm");
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Debug.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                    }
                }
                ViewBag.Message = "Please Provide Correct Details!";
                return View("RegistrationForm");
            }
            catch (Exception e)
            {
                ViewBag.Message = "Please Contact To Administrator!";
                return View(e);
            }

        }
    }
}