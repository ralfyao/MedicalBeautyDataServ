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
    public class BeautiflierEditController : Controller
    {
        private static ILog log = LogManager.GetLogger(typeof(BeautiflierEditController));
        public BeautiflierEditController()
        {
            XmlConfigurator.Configure(new System.IO.FileInfo("./log4net.config"));
        }
        // GET: BeautiflierEdit
        public ActionResult Index(string recipeNo)
        {
            Recipe r = new Recipe();
            try
            {
                if (Session["Account"] == null || string.IsNullOrEmpty(((Account)Session["Account"]).AccountId.ToString()))
                {
                    TempData["SessionExipred"] = "true";
                    return RedirectToAction("Index", "Home", null);
                }
                r = Recipe.getUnique(recipeNo);
                r.Beautiflier = ((Account)Session["Account"]).AccountId;
            }
            catch (Exception ex)
            {
                //log.Error(ex + ex.StackTrace);
                TempData["ErrMessage"] = ex + ex.StackTrace;
            }
            return View(r);
        }

        public ActionResult Complete(string recipeNo)
        {
            Recipe r = new Recipe();
            try
            {
                if (Session["Account"] == null || string.IsNullOrEmpty(((Account)Session["Account"]).AccountId.ToString()))
                {
                    TempData["SessionExipred"] = "true";
                    return RedirectToAction("Index", "Home", null);
                }
                r = Recipe.getUnique(recipeNo);
                r.Done = 1;
                Recipe.Update(r);
                TempData["success"] = "OK";
                TempData["parentreload"] = "OK";
            }
            catch (Exception ex)
            {
                //log.Error(ex + ex.StackTrace);
                TempData["ErrMessage"] = ex + ex.StackTrace;
            }
            return RedirectToAction("Index", "BeautiflierEdit") ;
        }
        public ActionResult UnComplete(string recipeNo)
        {
            Recipe r = new Recipe();
            try
            {
                if (Session["Account"] == null || string.IsNullOrEmpty(((Account)Session["Account"]).AccountId.ToString()))
                {
                    TempData["SessionExipred"] = "true";
                    return RedirectToAction("Index", "Home", null);
                }
                r = Recipe.getUnique(recipeNo);
                r.Done = 0;
                Recipe.Update(r);
                TempData["success"] = "OK";
                TempData["parentreload"] = "OK";
            }
            catch (Exception ex)
            {
                //log.Error(ex + ex.StackTrace);
                TempData["ErrMessage"] = ex + ex.StackTrace;
            }
            return RedirectToAction("Index", "BeautiflierEdit", new { recipeNo = recipeNo });
        }
    }
}