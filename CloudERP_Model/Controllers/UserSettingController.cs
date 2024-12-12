﻿using DatabaseAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CloudERP_Model.Controllers
{
    public class UserSettingController : Controller
    {
        private CloudErpVEntities db = new CloudErpVEntities();

        // GET: UserSetting
        public ActionResult CreateUser(int? employeeid)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            Session["CEmployeeID"] = employeeid;
            var employee = db.tblEmployees.Find(employeeid);
            var user = new tblUser();
            user.Email = employee.Email;
            user.ContactNo = employee.ContactNo;
            user.FullName = employee.Name;
            user.IsActive = true;
            user.Password = employee.ContactNo;
            user.UserName = employee.Email;
            ViewBag.UserTypeID = new SelectList(db.tblUserTypes.ToList(), "UserTypeID", "UserType");
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUser(tblUser user)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            try
            {
                if (ModelState.IsValid)
                {
                    var guser = db.tblUsers.Where(u => u.Email == user.Email && u.UserID != user.UserID);
                    if (guser.Count() > 0)
                    {
                        ViewBag.Message = "Email is Already Registered!";
                    }
                    else
                    {
                        db.tblUsers.Add(user);
                        db.SaveChanges();
                        int? employeeid = Convert.ToInt32(Convert.ToString(Session["CEmployeeID"]));
                        var employee = db.tblEmployees.Find(employeeid);
                        employee.UserID = user.UserID;
                        db.Entry(employee).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        Session["CEmployeeID"] = null;
                        return RedirectToAction("Index", "tblUsers");
                    }
                }
                if (user == null)
                {
                    ViewBag.UserTypeID = new SelectList(db.tblUserTypes.ToList(), "UserTypeID", "UserType");
                }
                else
                {
                    ViewBag.UserTypeID = new SelectList(db.tblUserTypes.ToList(), "UserTypeID", "UserType", user.UserTypeID);
                }
                return View(user);
            }
            catch
            {
                return RedirectToAction("EP500","EP");
            }
        }

        public ActionResult UpdateUser(int? userid)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            var user = db.tblUsers.Find(userid);
            ViewBag.UserTypeID = new SelectList(db.tblUserTypes.ToList(), "UserTypeID", "UserType", user.UserTypeID);
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateUser(tblUser user)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            if (ModelState.IsValid)
            {
                var guser = db.tblUsers.Where(u => u.Email == user.Email && u.UserID != user.UserID);
                if (guser.Count() > 0)
                {
                    ViewBag.Message = "Email is Already Registered!";
                }
                else
                {
                    db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index", "tblUsers");
                }
            }
            if (user == null)
            {
                ViewBag.UserTypeID = new SelectList(db.tblUserTypes.ToList(), "UserTypeID", "UserType");
            }
            else
            {
                ViewBag.UserTypeID = new SelectList(db.tblUserTypes.ToList(), "UserTypeID", "UserType", user.UserTypeID);
            }
            return View(user);
        }


    }
}