using log4net;
using log4net.Config;
using MedicalBeautyDataServ.Models;
using ProcessRequest.Utility.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace MedicalBeautyDataServ.Controllers
{
    public class AccountManageController : Controller
    {
        private static ILog log = LogManager.GetLogger(typeof(AccountManageController));
        public AccountManageController()
        {
            XmlConfigurator.Configure(new System.IO.FileInfo("./log4net.config"));
        }
        // GET: AccountManage
        public ActionResult Index(int page = 1)
        {
            XmlConfigurator.Configure(new System.IO.FileInfo("./log4net.config"));
            if (Session["Account"] == null || string.IsNullOrEmpty(((Account)Session["Account"]).AccountId.ToString()))
            {
                TempData["SessionExipred"] = "true";
                return RedirectToAction("Index", "Home", null);
            }
            Account account = new Account();
            try
            {
                List<Account> lst = Account.getAllAccount();
                account.accountList = lst.OrderBy(p =>p.AccountId).ToPagedList(page, 10); 
                TempData["menuId"] = "accMan";
            }
            catch (Exception ex)
            {
                log.Error(ex+ex.StackTrace);
                throw ex;
            }
            return View(account);
        }
        public ActionResult Add()
        {
            XmlConfigurator.Configure(new System.IO.FileInfo("./log4net.config"));
            if (Session["Account"] == null || string.IsNullOrEmpty(((Account)Session["Account"]).AccountId.ToString()))
            {
                TempData["SessionExipred"] = "true";
                return RedirectToAction("Index", "Home", null);
            }
            return RedirectToAction("Index", "AccountAdd");
        }
    }
}