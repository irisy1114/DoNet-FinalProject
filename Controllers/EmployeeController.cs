using Microsoft.AspNetCore.Mvc;
using Northwind.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace Northwind.Controllers
{
    public class EmployeeController : Controller
    {
        // this controller depends on the NorthwindRepository & the UserManager
        private NorthwindContext _northwindContext;
        private UserManager<AppUser> _userManager;
        public EmployeeController(NorthwindContext db, UserManager<AppUser> usrMgr)
        {
            _northwindContext = db;
            _userManager = usrMgr;
        }
       
        [Authorize(Roles = "northwind-employee")]
         public IActionResult ManageDiscounts() => View(_northwindContext.Discounts.Where(d => d.StartTime <= DateTime.Now && d.EndTime > DateTime.Now));

        public ViewResult Create() => View();

        public IActionResult Edit(int id)
        {
            Discount discount = _northwindContext.Discounts.Find(id);
            return View(discount);
        }
        [HttpPost]
        public IActionResult Edit(Discount d)
        {
             _northwindContext.EditDiscount(d); 
            return RedirectToAction("ManageDiscounts");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var itemToDelete = _northwindContext.Discounts.Where(d => d.DiscountId == id);
            if(itemToDelete != null) 
            {
                Discount discount = _northwindContext.Discounts.Find(id);
                _northwindContext.Discounts.Remove(discount);
                _northwindContext.SaveChanges();
            }
            return RedirectToAction("ManageDiscounts");
        }
    
        [HttpPost]
        public IActionResult Create(Discount model)
        {
            if (ModelState.IsValid)
            {
                if (_northwindContext.Discounts.Any(d => d.Code == model.Code))
                {
                    ModelState.AddModelError("", "Discount code must be unique");
                    return View(model);
                }
                else {
                    Discount discount = new Discount    
                    {
                        Title = model.Title,
                        Code = model.Code,
                        Description = model.Description,
                        StartTime = model.StartTime,
                        EndTime = model.EndTime,
                        DiscountPercent = model.DiscountPercent,
                        ProductId = model.ProductId
                    };
                    _northwindContext.AddDiscount(discount);   
                    return RedirectToAction("ManageDiscounts"); 
                }        
            }
            return View(model);
        }
    
    }
}