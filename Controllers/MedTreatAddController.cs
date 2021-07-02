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
    public class MedTreatAddController : Controller
    {
        private ILog log = LogManager.GetLogger(typeof(AccountAddController));
        public MedTreatAddController()
        {
            XmlConfigurator.Configure(new System.IO.FileInfo("./log4net.config"));
        }
        public ActionResult Add(MedTreat account)
        {
            if (Session["Account"] == null || string.IsNullOrEmpty(((Account)Session["Account"]).AccountId.ToString()))
            {
                TempData["SessionExipred"] = "true";
                return RedirectToAction("Index", "Home", null);
            }
            try
            {
                if (MedTreat.isExist(account))
                {
                    TempData["ErrMessage"] = string.Format("療程:{0}已存在", account.MedClass + account.Name);
                    return RedirectToAction("Index", "AccountAdd");
                }
                MedTreat.Insert(account);
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
        // GET: MedTreatAdd
        public ActionResult Index(string id)
        {
            if (Session["Account"] == null || string.IsNullOrEmpty(((Account)Session["Account"]).AccountId.ToString()))
            {
                TempData["SessionExipred"] = "true";
                return RedirectToAction("Index", "Home", null);
            }
            MedTreat treat = new MedTreat();
            if (!string.IsNullOrEmpty(id))
            {
                treat = MedTreat.getUnique(id);
            }
            return View(treat);
        }
        public ActionResult Edit(MedTreat customer)
        {
            if (Session["Account"] == null || string.IsNullOrEmpty(((Account)Session["Account"]).AccountId.ToString()))
            {
                TempData["SessionExipred"] = "true";
                return RedirectToAction("Index", "Home", null);
            }
            try
            {
                MedTreat.Update(customer);
                TempData["success"] = "OK";
                TempData["parentreload"] = "OK";
            }
            catch (Exception ex)
            {
                log.Error(ex + ex.StackTrace);
                throw ex;
            }
            return RedirectToAction("Index", "MedTreat");
        }
        public ActionResult Delete(string id)
        {
            if (Session["Account"] == null || string.IsNullOrEmpty(((Account)Session["Account"]).AccountId.ToString()))
            {
                TempData["SessionExipred"] = "true";
                return RedirectToAction("Index", "Home", null);
            }
            try
            {
                MedTreat.Delete(id);
                TempData["success"] = "OK";
                TempData["parentreload"] = "OK";
            }
            catch (Exception ex)
            {
                log.Error(ex + ex.StackTrace);
                throw ex;
            }
            return RedirectToAction("Index", "MedTreat");
        }
    }
}