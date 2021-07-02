using log4net;
using log4net.Config;
using MedicalBeautyDataServ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MedicalBeautyDataServ.Controllers
{
    public class CounterMainController : Controller
    {
        private static ILog log = LogManager.GetLogger(typeof(CounterMainController));
        public CounterMainController()
        {
            XmlConfigurator.Configure(new System.IO.FileInfo("./log4net.config"));
        }
        // GET: CounterMain
        public ActionResult Index()
        {
            if (Session["Account"] == null || string.IsNullOrEmpty(((Account)Session["Account"]).AccountId.ToString()))
            {
                //Log.Error(ex + ex.StackTrace);
                TempData["SessionExipred"] = "true";
                //TempData["error"] = ex + ex.StackTrace;
                //tran.Rollback();
                return RedirectToAction("Index", "Home", null);
            }
            return View();
        }
    }
}