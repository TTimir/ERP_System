using CloudERP_System.HelperCls;
using DatabaseAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CloudERP_Model.Models;
using DatabaseAccess.Code;

namespace CloudERP_Model.Controllers
{
    public class CompanyEmployeeController : Controller
    {
        private CloudErpVEntities db = new CloudErpVEntities();
        private SalaryTransaction salaryTransaction = new SalaryTransaction();

        // GET: CompanyEmployee
        public ActionResult Employee()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyid = 0;
            companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            var tblEmployee = db.tblEmployees.Where(c => c.CompanyID == companyid);
            return View(tblEmployee);
        }

        public ActionResult EmployeeRegistration()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyid = 0;
            companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            ViewBag.BranchID = new SelectList(db.tblBranches.Where(b => b.CompanyID == companyid), "BranchID", "BranchName", 0);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmployeeRegistration(tblEmployee employee)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyid = 0;
            //int branchid = 0;
            //branchid = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            employee.BranchID = employee.BranchID;
            employee.CompanyID = companyid;
            //var userId = Convert.ToInt32(Session["UserID"].ToString());
            employee.UserID = 0;

            if (ModelState.IsValid)
            {
                try
                {
                    if (employee.LogoFile != null && employee.LogoFile.ContentLength > 0)
                    {
                        // Check file extension
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                        var fileExtension = Path.GetExtension(employee.LogoFile.FileName).ToLower();

                        if (allowedExtensions.Contains(fileExtension))
                        {
                            // Check file size (limit: 5 MB)
                            const int maxFileSize = 5 * 1024 * 1024; // 5 MB in bytes

                            if (employee.LogoFile.ContentLength <= maxFileSize)
                            {
                                var folder = "~/Content/EmpPhotos";
                                var uniqueFileName = Guid.NewGuid().ToString("N").Substring(0, 6); // Generate a unique GUID for the file name
                                                                                                   // "N" format removes the hyphens from the GUID
                                var file = string.Format("{0}.jpg", uniqueFileName);

                                var response = FileHelpers.UploadPhoto(employee.LogoFile, folder, file);
                                if (response)
                                {
                                    var pic = string.Format("{0}/{1}", folder, file);
                                    employee.Photo = pic;
                                }
                            }
                            else
                            {
                                // File size exceeds limit
                                ViewBag.Message = "File size should not exceed 5 MB.";
                                return View(employee);
                            }
                        }
                        else
                        {
                            // Invalid file extension
                            ViewBag.Message = "Only .jpg, .jpeg, and .png files are allowed.";
                            return View(employee);
                        }
                    }
                    else
                    {
                        // No file chosen
                        ViewBag.Message = "Please choose a file to upload.";
                        return View(employee);
                    }


                    db.tblEmployees.Add(employee);
                    db.SaveChanges();
                    return RedirectToAction("Employee");
                }
                catch (Exception ex)
                {
                    Console.Write("Error: " + ex.Message);
                }
            }

            return View(employee);
        }

        public ActionResult EmployeeUpdation(int? id)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }
                if (id == null)
                {
                    return RedirectToAction("EP500", "EP");
                }
                var employee = db.tblEmployees.Find(id);
                if (employee == null)
                {
                    return RedirectToAction("EP404", "EP");
                }
                return View(employee);
            }
            catch
            {
                return RedirectToAction("EP500", "EP");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmployeeUpdation(tblEmployee employee)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyid = 0;
            //int branchid = 0;
            //branchid = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            employee.BranchID = employee.BranchID;
            employee.CompanyID = companyid;
            //var userId = Convert.ToInt32(Session["UserID"].ToString());
            employee.UserID = 0;

            if (ModelState.IsValid)
            {
                try
                {
                    // Fetch the existing company record from the database
                    var existingEmployee = db.tblEmployees.AsNoTracking().FirstOrDefault(c => c.EmployeeID == employee.EmployeeID);

                    if (employee.LogoFile != null && employee.LogoFile.ContentLength > 0)
                    {
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                        var fileExtension = Path.GetExtension(employee.LogoFile.FileName).ToLower();

                        if (allowedExtensions.Contains(fileExtension))
                        {
                            // Check file size (limit: 5 MB)
                            const int maxFileSize = 5 * 1024 * 1024; // 5 MB in bytes

                            if (employee.LogoFile.ContentLength <= maxFileSize)
                            {

                                // Check if there is an existing logo and delete it
                                if (!string.IsNullOrEmpty(existingEmployee.Photo))
                                {
                                    // Get the full path of the existing image
                                    var existingImagePath = Server.MapPath(existingEmployee.Photo);

                                    // Check if the file exists before attempting to delete it
                                    if (System.IO.File.Exists(existingImagePath))
                                    {
                                        System.IO.File.Delete(existingImagePath);
                                    }
                                }

                                // Generate a unique file name for the new logo
                                var folder = "~/Content/EmpPhotos";
                                var uniqueFileName = Guid.NewGuid().ToString("N").Substring(0, 6);
                                var file = string.Format("{0}.jpg", uniqueFileName);

                                // Upload the new logo
                                var response = FileHelpers.UploadPhoto(employee.LogoFile, folder, file);
                                if (response)
                                {
                                    var pic = string.Format("{0}/{1}", folder, file);
                                    employee.Photo = pic;
                                }
                            }
                            else
                            {
                                // File size exceeds limit
                                ViewBag.Message = "File size should not exceed 5 MB.";
                                return View(employee);
                            }
                        }
                        else
                        {
                            // Invalid file extension
                            ViewBag.Message = "Only .jpg, .jpeg, and .png files are allowed.";
                            return View(employee);
                        }
                    }
                    else
                    {
                        // Retain the existing logo if no new logo is uploaded
                        employee.Photo = existingEmployee.Photo;
                    }

                    // Update the company record
                    db.Entry(employee).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Employee");
                }
                catch (DbEntityValidationException ex)
                {
                    // Iterate through the validation errors and log them
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            Debug.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                            ModelState.AddModelError(validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                    // Return the view with the current employee data
                    return View(employee);
                }
                catch (Exception ex)
                {
                    // Handle any other exceptions
                    Debug.WriteLine("Error: " + ex.Message);
                    Debug.WriteLine("Stack Trace: " + ex.StackTrace);

                    ModelState.AddModelError("", "An unexpected error occurred. Please try again or contact support.");
                    return View(employee);
                }

            }

            return View(employee);
        }

        public ActionResult ViewProfile(int? id)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }
                if (id == null)
                {
                    return RedirectToAction("EP500", "EP");
                }
                int companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
                var employee = db.tblEmployees.Where(e => e.CompanyID == companyid && e.EmployeeID == id).FirstOrDefault();
                if (employee == null)
                {
                    return RedirectToAction("EP404", "EP");
                }
                return View(employee);
            }
            catch
            {
                return RedirectToAction("EP500", "EP");
            }
        }

        public ActionResult EmployeeSalary()
        {
            if (Session["SalaryMessage"] == null)
            {
                Session["SalaryMessage"] = string.Empty;
            }
            if (Session["ErrMessage"] == null)
            {
                Session["ErrMessage"] = string.Empty;
            }
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyid = 0;
            int branchid = 0;
            int userid = 0;
            var salary = new SalaryMV();
            branchid = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            salary.SalaryMonth = DateTime.Now.AddMonths(-1).ToString("MMMM");
            salary.SalaryYear = DateTime.Now.AddYears(-1).ToString("yyyy");
            return View(salary);
        }

        [HttpPost]
        public ActionResult EmployeeSalary(SalaryMV salary) // CNIC
        {
            Session["SalaryMessage"] = string.Empty;
            Session["ErrMessage"] = string.Empty;
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyid = 0;
            int branchid = 0;
            int userid = 0;
            branchid = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            var employeeList = db.tblEmployees.Where(p => p.CNIC == salary.CNIC).FirstOrDefault();
            salary.SalaryMonth = DateTime.Now.AddMonths(-1).ToString("MMMM");
            salary.SalaryYear = DateTime.Now.AddYears(-1).ToString("yyyy");
            if (employeeList != null)
            {
                salary.EmployeeID = employeeList.EmployeeID;
                salary.EmployeeName = employeeList.Name;
                salary.Designation = employeeList.Designation;
                salary.CNIC = employeeList.CNIC;
                salary.TransferAmount = employeeList.MonthlySalary;
            }
            else
            {
                Session["ErrMessage"] = "No employee found with the provided CNIC.";
                // Set default values for the salary object
                salary.SalaryMonth = DateTime.Now.AddMonths(-1).ToString("MMMM");
                salary.SalaryYear = DateTime.Now.AddYears(-1).ToString("yyyy");
            }
            return View(salary);
        }

        [HttpPost]
        public ActionResult EmployeeSalaryConfirm(SalaryMV salary)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }
                int companyid = 0;
                int branchid = 0;
                int userid = 0;
                branchid = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
                companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
                userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));
                salary.SalaryMonth = salary.SalaryMonth.ToLower();
                var emp = db.tblPayrolls.Where(p => p.EmployeeID == salary.EmployeeID && p.BranchID == branchid && p.CompanyID == companyid && p.SalaryMonth == salary.SalaryMonth && p.SalaryYear == salary.SalaryYear).FirstOrDefault();
                if (emp == null)
                {
                    string Invoiceno = "ESA" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
                    string message = string.Empty;
                    if (ModelState.IsValid)
                    {
                        message = salaryTransaction.Confirm(salary.EmployeeID, salary.TransferAmount, userid, branchid, companyid, Invoiceno, salary.SalaryYear, salary.SalaryYear);
                    }
                    if (message.Contains("Succeed"))
                    {
                        Session["SalaryMessage"] = message;
                        int payrollno = db.tblPayrolls.Max(p => p.PayrollID);
                        return RedirectToAction("PrintSalaryInvoice", new { id = payrollno });
                    }
                    else
                    {
                        Session["SalaryMessage"] = "Some issue is occur, re-login and try again after sometime!";
                    }
                }
                else
                {
                    Session["SalaryMessage"] = "Salary is Already Paid!";
                }
                return RedirectToAction("EmployeeSalary");
            }
            catch (Exception)
            {
                Session["SalaryMessage"] = "Some unexpected issue is occure please contact to Adminstrator!";
                return RedirectToAction("EmployeeSalary");
            }
        }

        public ActionResult SalaryHistory()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyid = 0;
            int branchid = 0;
            int userid = 0;
            branchid = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            userid = Convert.ToInt32(Convert.ToString(Session["UserID"]));

            var salaryList = db.tblPayrolls.Where(p => p.BranchID == branchid && p.CompanyID == companyid).OrderByDescending(p => p.PayrollID).ToList();
            return View(salaryList);
        }

        public ActionResult PrintSalaryInvoice(int id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            var salary = db.tblPayrolls.Where(p => p.PayrollID == id).FirstOrDefault();
            return View(salary);
        }
    }
}