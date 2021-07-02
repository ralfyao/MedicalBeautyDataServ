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
    public class HomeController : Controller
    {
        public static ILog log = LogManager.GetLogger(typeof(HomeController));
        public HomeController()
        {
            XmlConfigurator.Configure(new System.IO.FileInfo("./log4net.config"));
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Account account)
        {
            Account login = null;
            
            if ((login = Account.validateLogin(account)) != null)
            {
                Session["Account"] = login;
                if (login.AccountPriv == 3)
                    return RedirectToAction("Index", "Main");
                else if (login.AccountPriv == 2)
                    return RedirectToAction("Index", "Beautiflier");
                else if (login.AccountPriv == 1)
                    return RedirectToAction("Index", "CounterMain");
            }
            else
            {
                TempData["ErrMessage"] = string.Format("帳號:{0}密碼錯誤，或該帳號不存在", account.AccountId);
            }
            return RedirectToAction("Index", "Home", ViewBag) ;
        }
    }
}