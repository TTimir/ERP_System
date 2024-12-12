using CloudERP_System.HelperCls;
using DatabaseAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace CloudERP_Model.Controllers
{
    public class BranchEmployeeController : Controller
    {
        private CloudErpVEntities db = new CloudErpVEntities();

        // GET: BranchEmployee
        public ActionResult Employee()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyid = 0;
            int branchid = 0;
            branchid = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            var tblEmployee = db.tblEmployees.Where(c => c.CompanyID == companyid && c.BranchID == branchid);

            return View(tblEmployee);
        }

        public ActionResult EmployeeRegistration()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
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
            int branchid = 0;
            branchid = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            employee.BranchID = branchid;
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
            int branchid = 0;
            branchid = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyid = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            employee.BranchID = branchid;
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
                            // Optionally, add the error to the ModelState to show it on the view
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
    }
}