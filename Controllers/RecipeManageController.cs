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
    public class RecipeManageController : Controller
    {
        private ILog log = LogManager.GetLogger(typeof(RecipeManageController));
        public RecipeManageController()
        {
            XmlConfigurator.Configure(new System.IO.FileInfo("./log4net.config"));
        }
        // GET: RecipeManage
        public ActionResult Index()
        {
            if (Session["Account"] == null || string.IsNullOrEmpty(((Account)Session["Account"]).AccountId.ToString()))
            {
                TempData["SessionExipred"] = "true";
                return RedirectToAction("Index", "Home", null);
            }
            TempData["menuId"] = "RecipeManage";
            return View(new Recipe());
        }
        static object obj = new object();
        public ActionResult Add(FormCollection recipe)
        {
            string strSQL = string.Empty;
            if (Session["Account"] == null || string.IsNullOrEmpty(((Account)Session["Account"]).AccountId.ToString()))
            {
                TempData["SessionExipred"] = "true";
                return RedirectToAction("Index", "Home", null);
            }
            try
            {
                lock (obj)
                {
                    Guid? id = null;
                    MedTreatDetail treatmentDetail = null;
                    MedTreat medTreat = null;
                    DBUtility<MedTreat> dbMedTreat = new DBUtility<MedTreat>();
                    int indx = 1;
                    if (!Recipe.isExist(new Recipe() { RecipeNo = recipe["RecipeNo"] }))
                    {
                        id = Guid.NewGuid();
                        Recipe.Insert(new Recipe()
                        {
                            RecipeNo = recipe["RecipeNo"],
                            CreateUser = ((Account)Session["Account"]).AccountId.ToString(),
                            CreateDate = DateTime.Now,
                            CustomerId = Guid.Parse(recipe["CustomerId"]),
                            MedTreatmentID = id,
                            Beautiflier = recipe["beautiflier"],
                            branch = ((Account)Session["Account"]).branch
                        });
                        foreach (string key in recipe.Keys)
                        {
                            
                            if (!string.IsNullOrEmpty(recipe["MedClass" + indx]))
                            {
                                treatmentDetail = new MedTreatDetail();
                                treatmentDetail.id = Guid.NewGuid();
                                treatmentDetail.MedTreatmentId = id;
                                treatmentDetail.MedTreatClass = recipe["MedClass" + indx];
                                treatmentDetail.MedName = recipe["Med" + indx];

                                medTreat = MedTreat.get(recipe["MedClass" + indx], recipe["Med" + indx]);
                                strSQL = dbMedTreat.getDataListSQLNoKey(medTreat).Replace("上午", "").Replace("下午", "");
                                log.Debug(strSQL);
                                List<MedTreat> lsttreat = dbMedTreat.queryListBySql(strSQL, Constant.MYSQL_DB_TYPE, Account.ConnStr);
                                if (lsttreat.Count > 0)
                                {
                                    medTreat = lsttreat[0];
                                }
                                if (!string.IsNullOrEmpty(recipe["Unit" + indx]))
                                {
                                    treatmentDetail.MedQuantity = float.Parse(recipe["Unit" + indx]);
                                }
                                treatmentDetail.MedPosition = recipe["MedPosition" + indx];
                                treatmentDetail.MedUnit = medTreat.Unit;
                                MedTreatDetail.Insert(treatmentDetail);
                            }
                            indx++;
                        }
                    }
                    else
                    {
                        Recipe erecipe = Recipe.getUnique(recipe["RecipeNo"]);
                        MedTreatDetail.Delete(erecipe.MedTreatmentID);
                        foreach (string key in recipe.Keys)
                        {
                            treatmentDetail = new MedTreatDetail();
                            treatmentDetail.id = Guid.NewGuid();
                            treatmentDetail.MedTreatmentId = erecipe.MedTreatmentID;
                            treatmentDetail.MedTreatClass = recipe["MedClass" + indx];
                            treatmentDetail.MedName = recipe["Med" + indx];
                            medTreat = MedTreat.get(recipe["MedClass" + indx], recipe["Med" + indx]);
                            strSQL = dbMedTreat.getDataListSQLNoKey(medTreat).Replace("上午", "").Replace("下午", "");
                            log.Debug(strSQL);
                            List<MedTreat> lsttreat = dbMedTreat.queryListBySql(strSQL, Constant.MYSQL_DB_TYPE, Account.ConnStr);
                            if (lsttreat.Count > 0)
                            {
                                medTreat = lsttreat[0];
                            }
                            if (!string.IsNullOrEmpty(recipe["Unit" + indx]))
                            {
                                treatmentDetail.MedQuantity = float.Parse(recipe["Unit" + indx]);
                            }
                            treatmentDetail.MedPosition = recipe["MedPosition" + indx];
                            treatmentDetail.MedUnit = medTreat.Unit;
                            MedTreatDetail.Insert(treatmentDetail);
                            indx++;
                        }
                    }
                }
                //recipe.CreateUser = ((Account)Session["Account"]).AccountId;
                //Recipe.Insert(recipe);
                TempData["success"] = "OK";
                TempData["parentreload"] = "OK";
                TempData["menuId"] = "RecipeManage";
            }
            catch (Exception ex)
            {
                log.Error(ex+ex.StackTrace);
                TempData["ErrMessage"] = ex + ex.StackTrace + strSQL;
            }
            return RedirectToAction("Index", "RecipeManage");
        }
    }
}