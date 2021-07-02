using log4net;
using log4net.Config;
using Newtonsoft.Json;
using PagedList;
//using ProcessRequest.ProcessRequest.DB;
//using MedicalBeautyDataServ.Models;
//using ProcessRequest.Utility.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.SqlClient;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using System.Data;

namespace MedicalBeautyDataServ.Models
{
    public class Recipe
    {
        private static ILog log = LogManager.GetLogger(typeof(Account));
        public static string ConnStr = ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString;
        public Recipe()
        {
            XmlConfigurator.Configure(new System.IO.FileInfo("./log4net.config"));
        }

        public static Recipe getUnique(string recipeNo, SqlConnection conn = null, SqlTransaction tran = null)
        {
            Recipe ret = new Recipe();
            ret.RecipeNo = recipeNo;
            try
            {
                DBUtility<Recipe> db = new DBUtility<Recipe>();
                //if (account.CreateDate == null)
                //    account.CreateDate = DateTime.Now;
                string strSQL = "SELECT * FROM Recipe WHERE 1=1 AND RecipeNo='" + ret.RecipeNo + "'";
                log.Debug(strSQL);
                List<Recipe> accList = new List<Recipe>();
                if (conn != null && tran != null)
                {
                    accList = db.queryListBySql(ref conn, ref tran, strSQL, Constant.MYSQL_DB_TYPE, Account.ConnStr);
                }
                else
                    accList = db.queryListBySql(strSQL, Constant.MYSQL_DB_TYPE, Account.ConnStr);
                if (accList.Count > 0)
                {
                    ret = accList[0];
                }
            }
            catch (Exception ex)
            {
                log.Error(ex + ex.StackTrace);
                throw ex;
            }
            return ret;
        }

        public string getBeautifler(string accountId)
        {
            string beautiflierOptions = string.Empty;
            try
            {
                Account account = new Account();
                account.AccountId = accountId;
                account = Account.getUnique(account);

                string strSQL = string.Format("SELECT AccountId, AccountName FROM Account WHERE branch='{0}' AND AccountPriv = '1' AND AccountId <> '{1}'", account.branch, account.AccountId);
                DBUtility<Account> db = new DBUtility<Account>();
                List<Account> accounts = db.queryListBySql(strSQL, Constant.MYSQL_DB_TYPE, Account.ConnStr);
                accounts.ForEach(a => {
                    beautiflierOptions += string.Format(@"<option value=""{0}"">{1}</option>", a.AccountId, a.AccountName);
                });
            }
            catch (Exception ex)
            {
                log.Error(ex + ex.StackTrace);
                throw ex;
            }
            return beautiflierOptions;
        }
        public SqlConnection conn = null;
        public SqlTransaction tran = null;
        [DisplayName("三聯單號：")]
        [Key]
        public string RecipeNo { get; set; }
        public int? branch { get; set; }
        //[DisplayName("療程內容：")]
        //public int? MedTreatment { get; set; }
        //[DisplayName("部位：")]
        //public int? Position { get; set; }
        //[DisplayName("單位：")]
        //public int? Unit { get; set; }
        [DisplayName("客戶名稱：")]
        public Guid? CustomerId { get; set; }
        public Guid? MedTreatmentID { get; set; }
        [JsonProperty]
        public string MedTreatmentDesc { get; set; }
        [JsonProperty]
        public string PositionDesc { get; set; }
        [JsonProperty]
        public string UnitDesc { get; set; }
        public string Beautiflier { get; set; }

        public DateTime? CreateDate { get; set; }
        public string CreateUser { get; set; }
        public int? Done { get; set; }

        public static void UpdateDone(Recipe recipe, SqlConnection conn = null, SqlTransaction tran = null)
        {
            try
            {
                DBUtility<Recipe> db = new DBUtility<Recipe>();
                if (recipe.CreateDate == null)
                    recipe.CreateDate = DateTime.Now;
                string strSQL = string.Format("UPDATE Recipe SET Done = {0} WHERE RecipeNo='{1}'", recipe.Done, recipe.RecipeNo);
                log.Debug(strSQL);
                if (conn != null && tran != null)
                    db.executeBySql(ref conn, ref tran, strSQL, Constant.MYSQL_DB_TYPE, ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString);
                else
                    db.executeBySql(strSQL, Constant.MYSQL_DB_TYPE, ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString);
            }
            catch (Exception ex)
            {
                log.Error(ex + ex.StackTrace);
                throw ex;
            }
        }

        public static void Update(Recipe recipe, SqlConnection conn = null, SqlTransaction tran = null)
        {
            try
            {
                DBUtility<Recipe> db = new DBUtility<Recipe>();
                //if (recipe.CreateDate == null)
                    recipe.CreateDate = DateTime.Now;
                string strSQL = db.GenerateUpdateQuery(recipe, Constant.MYSQL_DB_TYPE);
                log.Debug(strSQL);
                if (conn != null && tran != null)
                    db.executeBySql(ref conn, ref tran, strSQL, Constant.MYSQL_DB_TYPE, ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString);
                else
                    db.executeBySql(strSQL, Constant.MYSQL_DB_TYPE, ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString);
            }
            catch (Exception ex)
            {
                log.Error(ex + ex.StackTrace);
                throw ex;
            }
        }

        //public DateTime? ModifyDate { get; set; }
        //public string ModifyUser { get; set; }
        //[DisplayName("上麻時間：")]
        //public DateTime? AnesthesiaTime { get; set; }
        //[DisplayName("發數：")]
        //public int? AnesthesiaCount { get; set; }
        //public int? Serviced { get; set; }
        //[JsonProperty]
        //public List<SelectListItem> selectMedTreatmentList
        //{
        //    get
        //    {
        //        DBUtility<SystemConfig> db = new DBUtility<SystemConfig>();
        //        List<SystemConfig> systemConfigs = db.queryListBySql(db.getDataListSQL(new SystemConfig() { ConfigGroupName = "Beautiflier", ConfigName = "MedTreat" }), Constant.MYSQL_DB_TYPE, ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString);
        //        List<SelectListItem> selectListItems = new List<SelectListItem>();
        //        foreach (SystemConfig conf in systemConfigs)
        //        {
        //            SelectListItem listItem = new SelectListItem();
        //            listItem.Text = conf.ConfigDesc;
        //            listItem.Value = conf.ConfigValue;
        //            if (MedTreatment != null && conf.ConfigValue == MedTreatment.ToString())
        //            {
        //                listItem.Selected = true;
        //            }
        //            selectListItems.Add(listItem);
        //        }
        //        return selectListItems;
        //    }
        //    set { }
        //}
        [JsonProperty]
        public List<MedTreatDetail> medTreatDetails { get 
            {
                List<MedTreatDetail> ret = new List<MedTreatDetail>();
                try
                {
                    MedTreatDetail detail = new MedTreatDetail();
                    detail.MedTreatmentId = MedTreatmentID;
                    DBUtility<MedTreatDetail> dB = new DBUtility<MedTreatDetail>();
                    string strSQL = dB.getDataListSQLNoKey(detail);
                    if (conn != null && tran != null)
                        ret = dB.queryListBySql(ref conn, ref tran, strSQL, Constant.MYSQL_DB_TYPE, Account.ConnStr);
                    else
                        ret = dB.queryListBySql(strSQL, Constant.MYSQL_DB_TYPE, Account.ConnStr);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return ret;
            } set { } 
        }
        public static string getPositionDesc(string position)
        {
            string position1 = string.Empty;
            try
            {
                string strSQL = string.Format("SELECT * FROM SystemConfig WHERE ConfigGroupName='{0}' AND ConfigName='{1}' AND ConfigValue='{2}'", "Beautiflier", "Position", position.ToString());
                DBUtility<SystemConfig> dB = new DBUtility<SystemConfig>();
                List<SystemConfig> list = dB.queryListBySql(strSQL, Constant.MYSQL_DB_TYPE, Account.ConnStr);
                if (list.Count > 0)
                {
                    position1 = list[0].ConfigDesc;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return position1;
        }
        public static void Insert(Recipe recipe)
        {
            string strSQL = string.Empty;
            try
            {
                DBUtility<Recipe> db = new DBUtility<Recipe>();
                if (recipe.CreateDate == null)
                    recipe.CreateDate = DateTime.Now;

                strSQL = db.GenerateInsertQueryAny(recipe);
                log.Debug(strSQL);
                db.executeBySql(strSQL, Constant.MYSQL_DB_TYPE, ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString);
            }
            catch (Exception ex)
            {
                ex.Data.Add("SQL", strSQL);
                log.Error(ex + ex.StackTrace);
                throw ex;
            }
        }
        public string getAllTreatmentClass()
        {
            string treatmentClassHTML = string.Empty;
            try
            {
                DBUtility<MedTreat> db = new DBUtility<MedTreat>();
                string strSQL = "SELECT DISTINCT MedClass FROM MedTreat";
                log.Debug(strSQL);
                List<MedTreat> ds = db.queryListBySql(strSQL, Constant.MYSQL_DB_TYPE, ConnStr);
                foreach (MedTreat row in ds)
                {
                    treatmentClassHTML += string.Format("<option value='{0}'>{1}</option>", row.MedClass, row.MedClass);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex + ex.StackTrace);
                throw ex;
            }
            return treatmentClassHTML;
        }
        [JsonProperty]
        public IPagedList<Recipe> ProcessedPagedList { get; set; }
        [JsonProperty]
        public List<Recipe> UnProcessedLists
        {
            get
            {
                List<Recipe> retLst = new List<Recipe>();
                try
                {
                    DBUtility<Recipe> db = new DBUtility<Recipe>();
                    string strSQL = string.Empty;
                    Account account = Account.getUnique(new Account() { AccountId = Beautiflier });
                    if (account.AccountPriv == 1)
                        strSQL = string.Format("SELECT a.* FROM Recipe a WHERE (a.Done = 0 OR a.Done = null) AND a.Beautiflier='{0}' AND (SELECT COUNT(0) FROM MedTreatDetail WHERE MedTreatmentId=a.MedTreatmentId AND AnesthesiaTime is null) > 0", Beautiflier);
                    if (account.AccountPriv == 2 || account.AccountId == null)
                        strSQL = string.Format("SELECT a.* FROM Recipe a WHERE (a.Done = 0 OR a.Done = null) AND (SELECT COUNT(0) FROM MedTreatDetail WHERE MedTreatmentId=a.MedTreatmentId AND AnesthesiaTime is null) = 0 AND a.branch=" + (this.branch == null ? "null" : this.branch.ToString()));
                    log.Debug(strSQL);
                    retLst = db.queryListBySql(strSQL, Constant.MYSQL_DB_TYPE, ConnStr);
                    retLst.ForEach(r =>
                    {
                        //r.MedTreatmentDesc = SystemConfig.getDesc("Beautiflier", "MedTreat", r.MedTreatment.ToString());
                        //r.PositionDesc = SystemConfig.getDesc("Beautiflier", "Position", r.Position.ToString());
                        //r.UnitDesc = SystemConfig.getDesc("Beautiflier", "Unit", r.Unit.ToString());
                    });
                }
                catch (Exception ex)
                {
                    log.Error(ex + ex.StackTrace);
                    throw ex;
                }
                return retLst;
            }
            set { }
        }
        public List<Recipe> ProcessedLists()
        {
            //get
            //{
                List<Recipe> retLst = new List<Recipe>();
                try
                {
                    DBUtility<Recipe> db = new DBUtility<Recipe>();
                    string strSQL = string.Empty;
                    Account account = Account.getUnique(new Account() { AccountId = Beautiflier });
                    if (account.AccountPriv == 1)
                        strSQL = string.Format("SELECT a.* FROM Recipe a WHERE a.Done = 1 AND a.Beautiflier='{0}' AND (SELECT COUNT(0) FROM MedTreatDetail WHERE MedTreatmentId=a.MedTreatmentId AND AnesthesiaTime is not null) > 0", Beautiflier);
                    if (account.AccountPriv == 2 || account.AccountId == null)
                        strSQL = string.Format("SELECT a.* FROM Recipe a WHERE a.Done = 1 AND CONVERT(VARCHAR, a.CreateDate, 112)='"+DateTime.Now.ToString("yyyyMMdd")+"' AND a.branch=" + (this.branch == null ? "null" : this.branch.ToString()));
                    log.Debug(strSQL);
                    retLst = db.queryListBySql(strSQL, Constant.MYSQL_DB_TYPE, ConnStr);
                }
                catch (Exception ex)
                {
                    log.Error(ex + ex.StackTrace);
                    throw ex;
                }
                return retLst;
            //}
            //set { }
        }
        //public List<Recipe> UnProcessedListsNurse
        //{
        //    get
        //    {
        //        List<Recipe> retLst = new List<Recipe>();
        //        try
        //        {
        //            DBUtility<Recipe> db = new DBUtility<Recipe>();
        //            string strSQL = string.Format("SELECT * FROM Recipe WHERE LEFT(CONVERT(VARCHAR,CreateDate, 120),10) = '{0}' AND branch=" + (this.branch == null ? "null" : this.branch.ToString()) + " UNION SELECT * FROM Recipe WHERE done=0 AND branch=" + (this.branch == null ? "null" : this.branch.ToString()), DateTime.Now.ToString("yyyy-MM-dd"));
        //            log.Debug(strSQL);
        //            retLst = db.queryListBySql(strSQL, Constant.MYSQL_DB_TYPE, ConnStr);
        //            retLst.ForEach(r =>
        //            {
        //                //r.MedTreatmentDesc = SystemConfig.getDesc("Beautiflier", "MedTreat", r.MedTreatment.ToString());
        //                //r.PositionDesc = SystemConfig.getDesc("Beautiflier", "Position", r.Position.ToString());
        //                //r.UnitDesc = SystemConfig.getDesc("Beautiflier", "Unit", r.Unit.ToString());
        //            });
        //        }
        //        catch (Exception ex)
        //        {
        //            log.Error(ex + ex.StackTrace);
        //            throw ex;
        //        }
        //        return retLst;
        //    }
        //    set { }
        //}
        public static bool isExist(Recipe recipe)
        {
            bool isExist = false;
            try
            {
                DBUtility<Recipe> db = new DBUtility<Recipe>();
                string strSQL = db.getDataListSQL(recipe);
                log.Debug(strSQL);
                List<Recipe> recipes = db.queryListBySql(strSQL, Constant.MYSQL_DB_TYPE, ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString);
                if (recipes.Count > 0)
                {
                    isExist = true;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex + ex.StackTrace);
            }
            return isExist;
        }

        //[JsonProperty]
        //public List<SelectListItem> selectPositionList
        //{
        //    get
        //    {
        //        DBUtility<SystemConfig> db = new DBUtility<SystemConfig>();
        //        List<SystemConfig> systemConfigs = db.queryListBySql(db.getDataListSQL(new SystemConfig() { ConfigGroupName = "Beautiflier", ConfigName = "Position" }), Constant.MYSQL_DB_TYPE, ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString);
        //        List<SelectListItem> selectListItems = new List<SelectListItem>();
        //        foreach (SystemConfig conf in systemConfigs)
        //        {
        //            SelectListItem listItem = new SelectListItem();
        //            listItem.Text = conf.ConfigDesc;
        //            listItem.Value = conf.ConfigValue;
        //            if (Position != null && conf.ConfigValue == Position.ToString())
        //            {
        //                listItem.Selected = true;
        //            }
        //            selectListItems.Add(listItem);
        //        }
        //        return selectListItems;
        //    }
        //    set { }
        //}
        //[JsonProperty]
        //public List<SelectListItem> selectUnitList
        //{
        //    get
        //    {
        //        DBUtility<SystemConfig> db = new DBUtility<SystemConfig>();
        //        List<SystemConfig> systemConfigs = db.queryListBySql(db.getDataListSQL(new SystemConfig() { ConfigGroupName = "Beautiflier", ConfigName = "Unit" }), Constant.MYSQL_DB_TYPE, ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString);
        //        List<SelectListItem> selectListItems = new List<SelectListItem>();
        //        foreach (SystemConfig conf in systemConfigs)
        //        {
        //            SelectListItem listItem = new SelectListItem();
        //            listItem.Text = conf.ConfigDesc;
        //            listItem.Value = conf.ConfigValue;
        //            if (Unit != null && conf.ConfigValue == Unit.ToString())
        //            {
        //                listItem.Selected = true;
        //            }
        //            selectListItems.Add(listItem);
        //        }
        //        return selectListItems;
        //    }
        //    set { }
        //}
    }
}