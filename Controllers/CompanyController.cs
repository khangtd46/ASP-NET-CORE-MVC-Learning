using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using WebApplication1.Models;
using WebApplication1.Security;
using WebApplication1.ViewModels;


namespace WebApplication1.Controllers
{
    public class CompanyController: Controller
    {
        private IEmployeeRepository _employeeRepository;
		private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IDataProtector protector;

		public CompanyController(IEmployeeRepository employeeRepository,
                                 IWebHostEnvironment webHostEnvironment,
								 DataProtectionPurposeStrings dataProtectionPurposeStrings,
                                 IDataProtectionProvider dataProtectionProvider) 
        {
            _employeeRepository = employeeRepository;
			this.webHostEnvironment = webHostEnvironment;
            protector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.EmployeeIdRouteValue);
		}

		[AllowAnonymous]
		public IActionResult Index()
        {
			HomeIndexViewModel homeIndexViewModel = new HomeIndexViewModel()
            {

                Employees = _employeeRepository.GetAllEmployee().Select(e =>
                {
                    e.EncryptedId = protector.Protect(e.Id.ToString());
                    return e;
                }),
                PageTitle = "Index Page",

		    };
            
            return View(homeIndexViewModel);
        }
		[AllowAnonymous]
      
		public IActionResult Details(string id)
        {
            int DecryotedId = 0;
            if (id.IsNullOrEmpty())
            {
                DecryotedId = 1;
            }
            else
            {
                DecryotedId = int.Parse(protector.Unprotect(id));
            }
            Employee employee = _employeeRepository.GetEmployee(DecryotedId);
            if (employee != null)
            {
                HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
                {
                    Employee = employee,
                    PageTitle = "Employee Details"
                };
                return View(homeDetailsViewModel);
            }
            else
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound", id);
            }

        }
        [HttpGet]
		public IActionResult Create()
        {
           // _employeeRepository.addEmployee(employee);
            return View();
        }
		[HttpPost]
		public IActionResult Create(EmployeeCreateViewModel model)
		{
            if (ModelState.IsValid)
            {
                string uniqueFileName = ProcessUploadFile(model);
                Employee emp = new Employee()
                {
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    PhotoPath = uniqueFileName
				};
				_employeeRepository.addEmployee(emp);

                return RedirectToAction("Details", new { id = emp.Id });
			}
            else 
            { 
                return View(); 
            }			
		}

        [HttpGet]
		[Authorize]
		public IActionResult Edit(int id)
        {
            Employee employee =  _employeeRepository.GetEmployee(id);
            EmployeeEditViewModel model = new EmployeeEditViewModel()
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                ExistingPhotoPath = employee.PhotoPath,
            };
            return View(model);
        }
		[HttpPost]
        [Authorize]
		public IActionResult Edit(EmployeeEditViewModel model)
		{
			if (ModelState.IsValid)
            {
                Employee emp = _employeeRepository.GetEmployee(model.Id);
                emp.Name = model.Name;
                emp.Email = model.Email;
                emp.Department = model.Department;
                if (model.Photo != null)
                {
                    if (model.ExistingPhotoPath != null)
                    {
                        string deleteFile = Path.Combine(webHostEnvironment.WebRootPath, "images" , model.ExistingPhotoPath);
                        System.IO.File.Delete(deleteFile);
                    }
                    emp.PhotoPath = ProcessUploadFile(model);
                }



                _employeeRepository.updateEmployee(emp);

                return RedirectToAction("Details", new { id = emp.Id });
            }
            else
			{
				return View();
			}
		}

        private string ProcessUploadFile(EmployeeCreateViewModel model)
        {
            string uniqueFileName = null;
            if (model.Photo != null)
            {
                string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string filePath = Path.Combine(uploadFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                { 
                    model.Photo.CopyTo(fileStream); 
                }
            }

            return uniqueFileName;
        }

    }
}
