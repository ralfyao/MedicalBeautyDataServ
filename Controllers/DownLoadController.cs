using MedicalBeautyDataServ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MedicalBeautyDataServ.Controllers
{
    public class DownLoadController : Controller
    {
        // GET: DownLoad
        public ActionResult Index()
        {
            if (Session["Account"] == null || string.IsNullOrEmpty(((Account)Session["Account"]).AccountId.ToString()))
            {
                TempData["SessionExipred"] = "true";
                return RedirectToAction("Index", "Home", null);
            }
            var fileName = Server.MapPath("~/Files/客戶清單.xlsx");
            return File(fileName, "application/ms-excel", "客戶清單.xlsx");
        }
    }
}