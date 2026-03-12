using crud_project.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace crud_project.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home

        exampledbEntities db = new exampledbEntities();


        
        public ActionResult Index()
        {
            var data = db.employees.ToList();
            return View(data);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost] // this method runs when the form is submitted
        [ValidateAntiForgeryToken] // protects the form from CSRF attacks
        public ActionResult Create(employee e) // the form data will store in employee object
        {

            //if all fields are filled and data is correct so modelstate will be true 
            if(ModelState.IsValid == true)
            {
                string fileName = Path.GetFileNameWithoutExtension(e.ImageFile.FileName);
                string extension = Path.GetExtension(e.ImageFile.FileName);
                HttpPostedFileBase postedFile = e.ImageFile;
                int length = postedFile.ContentLength; // length will come in bytes

                if (extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" || extension.ToLower() == ".png")
                {
                      if(length <= 1000000)
                        {
                        fileName = fileName + extension;
                        e.image_path = "~/images/" + fileName; // saving path for database
                        fileName = Path.Combine(Server.MapPath("~/images/"), fileName); // gives full path of image where it is 
                        e.ImageFile.SaveAs(fileName); // saving images in images folder
                        db.employees.Add(e); // adding data to database
                        int a = db.SaveChanges();  // save changing
                        if(a > 0)
                        {
                            TempData["CreateMessage"] = "Data Inserted Successfully";
                            ModelState.Clear();
                            return RedirectToAction("Index", "Home");
                        } else
                        {
                            TempData["CreateMessage"] = "Data Not Inserted Successfully";

                        }
                    } 
                      else
                        {
                        TempData["SizeMessage"] = "Image Size Should be Less than 1Mb";

                    }
                } else
                {
                    TempData["ExtensionMessage"] = "Image Format Not Supported";
                }
            }

            return View();
        }

        public ActionResult Edit(int id)
        {
            var employeeRow = db.employees.Where(model => model.id == id).FirstOrDefault();
            Session["Image"] = employeeRow.image_path;
            return View(employeeRow);
        }


        [HttpPost]
        public ActionResult Edit(employee e)
        {
            if (ModelState.IsValid == true)
            {
                if (e.ImageFile != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(e.ImageFile.FileName);
                    string extension = Path.GetExtension(e.ImageFile.FileName);
                    HttpPostedFileBase postedFile = e.ImageFile;
                    int length = postedFile.ContentLength; // length will come in bytes

                    if (extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" || extension.ToLower() == ".png")
                    {
                        if (length <= 1000000)
                        {
                            fileName = fileName + extension;
                            e.image_path = "~/images/" + fileName; // saving path for database
                            fileName = Path.Combine(Server.MapPath("~/images/"), fileName); // gives full path of image where it is 
                            e.ImageFile.SaveAs(fileName); // saving images in images folder
                            db.Entry(e).State = EntityState.Modified; // updating data to database
                            int a = db.SaveChanges();  // save changing
                            if (a > 0)
                            {
                                string ImagePath = Request.MapPath(Session["Image"].ToString());
                                if (System.IO.File.Exists(ImagePath))
                                {
                                    System.IO.File.Delete(ImagePath);
                                }
                                TempData["UpdateMessage"] = "Data Updated Successfully";
                                ModelState.Clear();
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                TempData["UpdateMessage"] = "Data Not Updated Successfully";

                            }
                        }
                        else
                        {
                            TempData["SizeMessage"] = "Image Size Should be Less than 1Mb";

                        }
                    }
                    else
                    {
                        TempData["ExtensionMessage"] = "Image Format Not Supported";
                    }
                } else
                {
                    e.image_path = Session["Image"].ToString();
                    db.Entry(e).State = EntityState.Modified; // updating data to database
                    int a = db.SaveChanges();  // save changing
                    if (a > 0)
                    {
                        TempData["UpdateMessage"] = "Data Updated Successfully";
                        ModelState.Clear();
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["UpdateMessage"] = "Data Not Updated Successfully";

                    }
                }
            }
            return View();  
        }

        public ActionResult Delete(int id)
        {
            if(id > 0)
            {

            var employeeRow = db.employees.Where(model => model.id == id).FirstOrDefault();
                if (employeeRow != null) {
                    db.Entry(employeeRow).State = EntityState.Deleted;
                    int a = db.SaveChanges();

                    if(a > 0)
                    {
                        TempData["DeleteMessage"] = "Deleted Successfully";
                        string ImagePath = Request.MapPath(employeeRow.image_path.ToString());
                        if (System.IO.File.Exists(ImagePath)) 
                        { 
                            System.IO.File.Delete(ImagePath);    
                        }

                    } else
                    {
                        TempData["DeleteMessage"] = "Data not deleted successfully";
                    }
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Details(int id)
        {
            var EmployeeRow = db.employees.Where(model => model.id == id).FirstOrDefault();
            Session["Image2"] = EmployeeRow.image_path.ToString();
            return View(EmployeeRow);
        }


    }
}