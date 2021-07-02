using log4net;
using log4net.Config;
using MedicalBeautyDataServ.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MedicalBeautyDataServ.Controllers
{
    public class MedTreatController : Controller
    {
        private static ILog log = LogManager.GetLogger(typeof(AccountManageController));
        public MedTreatController()
        {
            XmlConfigurator.Configure(new System.IO.FileInfo("./log4net.config"));
        }
        // GET: MedTreat
        public ActionResult Index(int page = 1)
        {
            if (Session["Account"] == null || string.IsNullOrEmpty(((Account)Session["Account"]).AccountId.ToString()))
            {
                TempData["SessionExipred"] = "true";
                return RedirectToAction("Index", "Home", null);
            }
            MedTreat treat = new MedTreat();
            try
            {
                List<MedTreat> lst = MedTreat.getAllMedTreats();
                treat.medTreatList = lst.OrderBy(p => p.Name).ToPagedList(page, 10);
                TempData["menuId"] = "MedTreat";
            }
            catch (Exception ex)
            {
                log.Error(ex + ex.StackTrace);
                throw ex;
            }
            return View(treat);
        }
    }
}