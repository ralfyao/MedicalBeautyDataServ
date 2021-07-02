using log4net;
using log4net.Config;
using MedicalBeautyDataServ.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MedicalBeautyDataServ.Controllers
{
    public class BeautiflierController : Controller
    {
        private static ILog log = LogManager.GetLogger(typeof(BeautiflierController));
        public BeautiflierController() 
        {
            XmlConfigurator.Configure(new System.IO.FileInfo("./log4net.config"));
        }
        // GET: Beautiflier
        public ActionResult Index(int page = 1)
        {
            XmlConfigurator.Configure(new System.IO.FileInfo("./log4net.config"));
            if (Session["Account"] == null || string.IsNullOrEmpty(((Account)Session["Account"]).AccountId.ToString()))
            {
                TempData["SessionExipred"] = "true";
                return RedirectToAction("Index", "Home", null);
            }
            Recipe r = new Recipe();
            r.branch = ((Account)Session["Account"]).branch;
            r.Beautiflier = ((Account)Session["Account"]).AccountId;
            r.ProcessedPagedList = r.ProcessedLists().OrderBy(p => p.RecipeNo).ToPagedList(page, 10);
            TempData["menuId"] = "beautiflier";
            return View(r);
        }
        public static object lockObj = new object();
        public ActionResult Update(FormCollection recipe)
        {
            if (Session["Account"] == null || string.IsNullOrEmpty(((Account)Session["Account"]).AccountId.ToString()))
            {
                TempData["SessionExipred"] = "true";
                return RedirectToAction("Index", "Home", null);
            }
            lock (lockObj)
            {
                try
                {
                    int itx = 0;
                    MedTreatDetail detail = null;
                    SqlConnection conn = (SqlConnection)DBUtility<object>.getConnection(Constant.MYSQL_DB_TYPE, Account.ConnStr);
                    SqlTransaction tran = conn.BeginTransaction();
                    foreach (string key in Request.Form.AllKeys)
                    {
                        if (key.IndexOf("MedTreatID") != -1)
                        {
                            detail = MedTreatDetail.getUnique(Request.Form["MedTreatID" + itx], conn, tran);
                            if (Request.Form["AnesthesiaTime" + itx] != null && !string.IsNullOrEmpty(Request.Form["AnesthesiaTime" + itx]))
                            {
                                try
                                {
                                    detail.AnesthesiaTime = DateTime.Parse(Request.Form["AnesthesiaTime" + itx].Replace("T", " "));
                                }
                                catch (Exception ex) {
                                    TempData["ErrMessage"] = "錯誤的日期格式在第"+itx+"列";
                                    return RedirectToAction("Index", "BeautiflierEdit", new { recipeNo = Request.Form["RecipeNo"] });
                                }
                            }
                            if (Request.Form["AnesthesiaCount" + itx] != null && !string.IsNullOrEmpty(Request.Form["AnesthesiaCount" + itx]))
                            {
                                detail.AnesthesiaCount = int.Parse(Request.Form["AnesthesiaCount" + itx]);
                            }
                            MedTreatDetail.UpdateAnesthesiaTimeAndCount(detail, conn, tran);
                            itx++;
                        }
                    }
                    DBUtility<MedTreatDetail> db = new DBUtility<MedTreatDetail>();
                    List<MedTreatDetail> medTreatDetails = 
                        db.queryListBySql(
                            ref conn, 
                            ref tran,
                            string.Format("SELECT * FROM MedTreatDetail WHERE MedTreatmentId IN (SELECT MedTreatmentId FROM Recipe WHERE RecipeNo='{0}')", Request.Form["RecipeNo"]), 
                            Constant.MYSQL_DB_TYPE, 
                            Account.ConnStr);
                    bool isComplete = true;
                    medTreatDetails.ForEach(m => {
                        //if (((Account)Session["Account"]).AccountPriv == 2)
                        //{
                            if (m.AnesthesiaCount == null || m.AnesthesiaCount == 0)
                            {
                                isComplete = false;
                            }
                        //}
                    });
                    if (isComplete)
                    {
                        Recipe r = Recipe.getUnique(Request.Form["RecipeNo"], conn, tran);
                        r.conn = conn;
                        r.tran = tran;
                        r.Done = 1;
                        Recipe.UpdateDone(r, conn, tran);
                        TempData["parentreload"] = "OK";
                    }
                    tran.Commit();
                    conn.Close();
                    if (((Account)Session["Account"]).AccountPriv == 1)
                    {
                        TempData["parentreload"] = "OK";
                    }
                    TempData["success"] = "OK";
                    //TempData["parentreload"] = "OK";
                }
                catch (Exception ex)
                {
                    TempData["ErrMessage"] = ex + ex.StackTrace + ex.Data["SQL"];
                    //throw ex;
                }
            }
            return RedirectToAction("Index", "BeautiflierEdit", new { recipeNo = Request.Form["RecipeNo"] });
        }
    }
}