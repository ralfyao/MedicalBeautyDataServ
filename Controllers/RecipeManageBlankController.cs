using MedicalBeautyDataServ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MedicalBeautyDataServ.Controllers
{
    public class RecipeManageBlankController : Controller
    {
        // GET: RecipeManageBlank
        public ActionResult Index(string customerId)
        {
            if (Session["Account"] == null || string.IsNullOrEmpty(((Account)Session["Account"]).AccountId.ToString()))
            {
                TempData["SessionExipred"] = "true";
                return RedirectToAction("Index", "Home", null);
            }
            Recipe recipe = new Recipe();
            recipe.CustomerId = Guid.Parse(customerId);
            //TempData["menuId"] = "RecipeManage";
            return View(recipe);
        }
    }
}