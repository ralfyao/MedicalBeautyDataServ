using log4net;
using log4net.Config;
using log4net.Repository.Hierarchy;
using MedicalBeautyDataServ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MedicalBeautyDataServ.Controllers
{
    public class AccountAddController : Controller
    {
        private ILog log = LogManager.GetLogger(typeof(AccountAddController));
        public AccountAddController()
        {
            XmlConfigurator.Configure(new System.IO.FileInfo("./log4net.config"));
        }
        // GET: AccountAdd
        [ValidateAntiForgeryToken]
        public ActionResult Add(Account account)
        {
            if (Session["Account"] == null || string.IsNullOrEmpty(((Account)Session["Account"]).AccountId.ToString()))
            {
                TempData["SessionExipred"] = "true";
                return RedirectToAction("Index", "Home", null);
            }
            try
            {
                if (Account.isExist(account))
                {
                    TempData["ErrMessage"] = string.Format("帳號:{0}已存在", account.AccountId);
                    return RedirectToAction("Index", "AccountAdd");
                }
                Account.InsertAccount(account);
                TempData["success"] = "OK";
                TempData["parentreload"] = "OK";
            }
            catch (Exception ex)
            {
                log.Error(ex+ex.StackTrace);
                throw ex;
            }
            return RedirectToAction("Index", "AccountAdd");
        }
        public ActionResult Edit(Account account)
        {
            if (Session["Account"] == null || string.IsNullOrEmpty(((Account)Session["Account"]).AccountId.ToString()))
            {
                TempData["SessionExipred"] = "true";
                return RedirectToAction("Index", "Home", null);
            }
            try
            {
                Account.UpdateAccount(account);
                TempData["success"] = "OK";
                TempData["parentreload"] = "OK";
            }
            catch (Exception ex)
            {
                log.Error(ex + ex.StackTrace);
                throw ex;
            }
            return RedirectToAction("Index", "AccountAdd");
        }
        public ActionResult Index(string accountId)
        {
            Account account = null;
            if (Session["Account"] == null || string.IsNullOrEmpty(((Account)Session["Account"]).AccountId.ToString()))
            {
                TempData["SessionExipred"] = "true";
                return RedirectToAction("Index", "Home", null);
            }
            try
            {
                account = new Account();
                account.AccountId = accountId;
                account = Account.getUnique(account);
            }
            catch (Exception ex)
            {
                log.Error(ex + ex.StackTrace);
                throw ex;
            }
            //TempData["AccountUnique"] = account;
            return View(account);
        }
        public ActionResult Delete(string accountId) 
        {
            if (Session["Account"] == null || string.IsNullOrEmpty(((Account)Session["Account"]).AccountId.ToString()))
            {
                TempData["SessionExipred"] = "true";
                return RedirectToAction("Index", "Home", null);
            }
            try
            {
                Account.DeleteAccount(accountId);
                TempData["success"] = "OK";
                TempData["parentreload"] = "OK";
            }
            catch (Exception ex)
            {
                log.Error(ex + ex.StackTrace);
                throw ex;
            }
            return RedirectToAction("Index", "AccountAdd");
        }
    }
}