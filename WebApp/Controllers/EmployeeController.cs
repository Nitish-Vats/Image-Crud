using DAL;
using DAL.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Net;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class EmployeeController : Controller
    {
        AppDbContext _db;
        private readonly IWebHostEnvironment webHostEnvironment; 
        public EmployeeController(AppDbContext db, IWebHostEnvironment hostEnvironment) 
        { 
            _db = db; 
            webHostEnvironment = hostEnvironment; 
        }
        public IActionResult Index()
        {
            List<EmployeeViewModel> EmpDeptViewModellist = new List<EmployeeViewModel>(); 
            var empsList = (from e in _db.Employees select new {
                e.EmployeeId,
                    e.Name, 
                e.Address, 
                e.ImagePath 
            }).ToList(); 
            foreach (var item in empsList)
            {
                EmployeeViewModel objedvm = new EmployeeViewModel();//ViewModel
                objedvm.Name=item.Name; 
                objedvm.Address=item.Address; 
                objedvm.EmployeeId=item.EmployeeId; 
                objedvm.ImagePath=item.ImagePath; 
                EmpDeptViewModellist.Add(objedvm); 
            } 
            return View(EmpDeptViewModellist);
                
        }


        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(EmployeeViewModel emp)
        {
            ModelState.Remove("EmployeeId");
            if (ModelState.IsValid)
            {
                string filePath = UploadedFile(emp);
                Employee employee = new Employee
                {
                    Name = emp.Name,
                    Address = emp.Address,
                    ImagePath = filePath
                };
                _db.Employees.Add(employee);
                _db.SaveChanges();
                return RedirectToAction("Index");

                

            }
           


            return View();
        }

        private string UploadedFile(EmployeeViewModel emp)
        {
            string filePath = null;
            if (emp.Image != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                filePath = Guid.NewGuid().ToString() + "_" + emp.Image.FileName;
                string path = Path.Combine(uploadsFolder, filePath);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    emp.Image.CopyTo(fileStream);
                }
            }
            return filePath;
        }

        public IActionResult Edit(int id)
        {
            Employee data = _db.Employees.Find(id); 
            EmployeeViewModel model = new EmployeeViewModel();//ViewModel
            if(model!=null) 
            { 
                model.Name=data.Name; 
                model.Address=data.Address; 
                model.EmployeeId=data.EmployeeId; 
                model.ImagePath=data.ImagePath; 
            } 
            return View("Create",model);
        }

        [HttpPost] 
        public IActionResult Edit(EmployeeViewModel model) 
        { 
            if (ModelState.IsValid) 
            { 
                Employee employee = new Employee 
                { 
                    EmployeeId = model.EmployeeId, 
                    Name = model.Name, 
                    Address = model.Address 
                }; 
                if (model.Image != null) 
                { 
                    string filePath = UploadedFile(model); 
                    employee.ImagePath = filePath; 
                } 
                _db.Employees.Update(employee); 
                _db.SaveChanges(); 
                return RedirectToAction("Index"); 
            } 
            return View("Create", model); 
        }
        public IActionResult Delete(int id)
        {
            Employee model = _db.Employees.Find(id); 
            if (model != null)
            {
                _db.Employees.Remove(model);
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        
    }
}
