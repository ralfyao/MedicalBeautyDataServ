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
    public class CustomerAddController : Controller
    {
        private ILog log = LogManager.GetLogger(typeof(CustomerAddController));
        public CustomerAddController()
        {
            XmlConfigurator.Configure(new System.IO.FileInfo("./log4net.config"));
        }
        // GET: CustomerAdd
        public ActionResult Index(string customerId)
        {
            Customer cust = null;
            if (Session["Account"] == null || string.IsNullOrEmpty(((Account)Session["Account"]).AccountId.ToString()))
            {
                TempData["SessionExipred"] = "true";
                return RedirectToAction("Index", "Home", null);
            }
            try
            {
                cust = new Customer();
                if (!string.IsNullOrEmpty(customerId))
                    cust.id = Guid.Parse(customerId);
                cust = Customer.getUnique(cust);
            }
            catch (Exception ex)
            {
                log.Error(ex + ex.StackTrace);
                throw ex;
            }
            //TempData["AccountUnique"] = account;
            return View(cust);
        }
        public ActionResult Add(Customer customer)
        {
            if (Session["Account"] == null || string.IsNullOrEmpty(((Account)Session["Account"]).AccountId.ToString()))
            {
                TempData["SessionExipred"] = "true";
                return RedirectToAction("Index", "Home", null);
            }
            try
            {
                if (Customer.isExist(customer))
                {
                    TempData["ErrMessage"] = string.Format("客戶:{0}已存在", customer.Name);
                    return RedirectToAction("Index", "AccountAdd");
                }
                Customer.Insert(customer, ((Account)Session["Account"]).AccountId);
                TempData["success"] = "OK";
                TempData["parentreload"] = "OK";
            }
            catch (Exception ex)
            {
                log.Error(ex + ex.StackTrace);
                throw ex;
            }
            return RedirectToAction("Index", "CustomerAdd");
        }
        public ActionResult Edit(Customer customer)
        {
            if (Session["Account"] == null || string.IsNullOrEmpty(((Account)Session["Account"]).AccountId.ToString()))
            {
                TempData["SessionExipred"] = "true";
                return RedirectToAction("Index", "Home", null);
            }
            try
            {
                Customer.Update(customer);
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
        public ActionResult Delete(string id)
        {
            if (Session["Account"] == null || string.IsNullOrEmpty(((Account)Session["Account"]).AccountId.ToString()))
            {
                TempData["SessionExipred"] = "true";
                return RedirectToAction("Index", "Home", null);
            }
            try
            {
                Customer.Delete(id);
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