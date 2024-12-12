using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CloudERP_System.HelperCls;
using DatabaseAccess;

namespace CloudERP_Model.Controllers
{
    public class tblCompaniesController : Controller
    {
        private CloudErpVEntities db = new CloudErpVEntities();

        // GET: tblCompanies
        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            return View(db.tblCompanies.ToList());
        }

        // GET: tblCompanies/Details/5
        public ActionResult Details(int? id)
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
                tblCompany tblCompany = db.tblCompanies.Find(id);
                if (tblCompany == null)
                {
                    return RedirectToAction("EP404", "EP");
                }
                return View(tblCompany);
            }
            catch
            {
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: tblCompanies/Create
        public ActionResult Create()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            return View();
        }

        // POST: tblCompanies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblCompany tblCompany)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Debug.WriteLine("Checking LogoFile: " + (tblCompany.LogoFile != null));
                    if (tblCompany.LogoFile != null && tblCompany.LogoFile.ContentLength > 0)
                    {
                        // Check file extension
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                        var fileExtension = Path.GetExtension(tblCompany.LogoFile.FileName).ToLower();

                        if (allowedExtensions.Contains(fileExtension))
                        {
                            // Check file size (limit: 5 MB)
                            const int maxFileSize = 5 * 1024 * 1024; // 5 MB in bytes

                            if (tblCompany.LogoFile.ContentLength <= maxFileSize)
                            {
                                var folder = "~/Content/CLogos";
                                var uniqueFileName = Guid.NewGuid().ToString("N").Substring(0, 6); // Generate a unique GUID for the file name
                                                                                                   // "N" format removes the hyphens from the GUID
                                var file = string.Format("{0}.jpg", uniqueFileName);
                                Debug.WriteLine("Generated Unique File Name: " + file);

                                var response = FileHelpers.UploadPhoto(tblCompany.LogoFile, folder, file);
                                if (response)
                                {
                                    var pic = string.Format("{0}/{1}", folder, file);
                                    tblCompany.Logo = pic;
                                    Debug.WriteLine("Image uploaded and path set: " + pic);
                                }
                            }
                            else
                            {
                                // File size exceeds limit
                                ViewBag.Message = "File size should not exceed 5 MB.";
                                return View(tblCompany);
                            }
                        }
                        else
                        {
                            // Invalid file extension
                            ViewBag.Message = "Only .jpg, .jpeg, and .png files are allowed.";
                            return View(tblCompany);
                        }
                    }
                    else
                    {
                        // No file chosen
                        ViewBag.Message = "Please choose a file to upload.";
                        return View(tblCompany);
                    }

                    db.tblCompanies.Add(tblCompany);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error: " + ex.Message);
                }
            }

            return View(tblCompany);
        }


        // GET: tblCompanies/Edit/5
        public ActionResult Edit(int? id)
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
                tblCompany tblCompany = db.tblCompanies.Find(id);
                if (tblCompany == null)
                {
                    return RedirectToAction("EP404", "EP");
                }
                return View(tblCompany);
            }
            catch
            {
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: tblCompanies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblCompany tblCompany)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Fetch the existing company record from the database
                    var existingCompany = db.tblCompanies.AsNoTracking().FirstOrDefault(c => c.CompanyID == tblCompany.CompanyID);

                    if (tblCompany.LogoFile != null && tblCompany.LogoFile.ContentLength > 0)
                    {
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                        var fileExtension = Path.GetExtension(tblCompany.LogoFile.FileName).ToLower();

                        if (allowedExtensions.Contains(fileExtension))
                        {
                            // Check file size (limit: 5 MB)
                            const int maxFileSize = 5 * 1024 * 1024; // 5 MB in bytes

                            if (tblCompany.LogoFile.ContentLength <= maxFileSize)
                            {

                                // Check if there is an existing logo and delete it
                                if (!string.IsNullOrEmpty(existingCompany.Logo))
                                {
                                    // Get the full path of the existing image
                                    var existingImagePath = Server.MapPath(existingCompany.Logo);

                                    // Check if the file exists before attempting to delete it
                                    if (System.IO.File.Exists(existingImagePath))
                                    {
                                        System.IO.File.Delete(existingImagePath);
                                        Debug.WriteLine("Old image deleted: " + existingCompany.Logo);
                                    }
                                }

                                // Generate a unique file name for the new logo
                                var folder = "~/Content/CLogos";
                                var uniqueFileName = Guid.NewGuid().ToString("N").Substring(0, 6);
                                var file = string.Format("{0}.jpg", uniqueFileName);

                                // Upload the new logo
                                var response = FileHelpers.UploadPhoto(tblCompany.LogoFile, folder, file);
                                if (response)
                                {
                                    var pic = string.Format("{0}/{1}", folder, file);
                                    tblCompany.Logo = pic;
                                }
                            }
                            else
                            {
                                // File size exceeds limit
                                ViewBag.Message = "File size should not exceed 5 MB.";
                                return View(tblCompany);
                            }
                        }
                        else
                        {
                            // Invalid file extension
                            ViewBag.Message = "Only .jpg, .jpeg, and .png files are allowed.";
                            return View(tblCompany);
                        }
                    }
                    else
                    {
                        // Retain the existing logo if no new logo is uploaded
                        tblCompany.Logo = existingCompany.Logo;
                    }

                    // Update the company record
                    db.Entry(tblCompany).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error: " + ex.Message);
                    ModelState.AddModelError("", "An error occurred while updating the company information.");
                }
            }
            return View(tblCompany);
        }

        // GET: tblCompanies/Delete/5
        public ActionResult Delete(int? id)
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
                tblCompany tblCompany = db.tblCompanies.Find(id);
                if (tblCompany == null)
                {
                    return RedirectToAction("EP404", "EP");
                }
                return View(tblCompany);
            }
            catch
            {
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: tblCompanies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            tblCompany tblCompany = db.tblCompanies.Find(id);
            db.tblCompanies.Remove(tblCompany);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
