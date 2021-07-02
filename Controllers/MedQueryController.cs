using MedicalBeautyDataServ.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MedicalBeautyDataServ.Controllers
{
    public class Select 
    {
        public string Text { get; set; }
        public string Value { get; set; }
    }
    public class MedQueryController : Controller
    {
        
        // GET: MedQuery
        [HttpPost]
        public ActionResult queryMed(string ItemSelected)
        {
            List<Select> list = new List<Select>();
            try
            {
                string strSQL = "SELECT DISTINCT Name FROM MedTreat WHERE MedClass='"+ ItemSelected + "'";
                DBUtility<MedTreat> db = new DBUtility<MedTreat>();
                List<MedTreat> medTreats = db.queryListBySql(strSQL, Constant.MYSQL_DB_TYPE, Account.ConnStr);
                Select s = null;
                medTreats.ForEach(m => {
                    s = new Select();
                    s.Text = m.Name;
                    s.Value = m.Name;
                    list.Add(s);
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            string result = JsonConvert.SerializeObject(list);
            return Json(result);
        }
        [HttpPost]
        public ActionResult queryLabel(string MedClassSelected, string MedSelected)
        {
            List<Select> list = new List<Select>();
            try
            {
                string strSQL = "SELECT DISTINCT Unit FROM MedTreat WHERE MedClass='" + MedClassSelected + "' AND Name='"+ MedSelected + "'";
                DBUtility<MedTreat> db = new DBUtility<MedTreat>();
                List<MedTreat> medTreats = db.queryListBySql(strSQL, Constant.MYSQL_DB_TYPE, Account.ConnStr);
                Select s = null;
                medTreats.ForEach(m => {
                    s = new Select();
                    s.Text = m.Unit;
                    s.Value = m.Unit;
                    list.Add(s);
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            string result = JsonConvert.SerializeObject(list);
            return Json(result);
        }
        [HttpPost]
        public ActionResult queryRecipeList(string recipeNo)
        {
            List<MedTreatDetail> list = new List<MedTreatDetail>();
            try
            {
                string strSQL = string.Format("SELECT a.* FROM MedTreatDetail a, Recipe b WHERE b.RecipeNo='{0}' and b.MedTreatmentID=a.MedTreatmentID", recipeNo);
                DBUtility<MedTreatDetail> dB = new DBUtility<MedTreatDetail>();
                list = dB.queryListBySql(strSQL, Constant.MYSQL_DB_TYPE, Account.ConnStr);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            string result = JsonConvert.SerializeObject(list);
            return Json(result);
        }
    }
}