using log4net;
using log4net.Config;
using Newtonsoft.Json;
using PagedList;
//using MedicalBeautyDataServ.Models;//ProcessRequest.ProcessRequest.DB;
//using ProcessRequest.Utility.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MedicalBeautyDataServ.Models
{
    public class Account
    {
        private static ILog log = LogManager.GetLogger(typeof(Account));
        public static string ConnStr = ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString;
        public Account()
        {
            XmlConfigurator.Configure(new System.IO.FileInfo("./log4net.config"));
        }
        [Key]
        [DisplayName("帳號：")]
        [Required(ErrorMessage = "帳號未輸入")]
        public string AccountId { get; set; }
        [DisplayName("密碼：")]
        [Required(ErrorMessage = "密碼未輸入")]
        [DataType(DataType.Password)]
        public string AccountPwd { get; set; }
        [DisplayName("帳號名稱：")]
        public string AccountName { get; set; }

        public static bool isExist(Account account)
        {
            bool isExist = false;
            try
            {
                DBUtility<Account> db = new DBUtility<Account>();
                string strSQL = db.getDataListSQL(account);
                log.Debug(strSQL);
                List<Account> accounts = db.queryListBySql(strSQL, Constant.MYSQL_DB_TYPE, ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString);
                if (accounts.Count > 0)
                {
                    isExist = true;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex+ex.StackTrace);
            }
            return isExist;
        }

        [DisplayName("權限：")]
        public int? AccountPriv { get; set; }
        [DisplayName("分店：")]
        public int? branch { get; set; }
        public DateTime? CreateDate { get; set; }
        [JsonProperty]
        public IPagedList<Account> accountList { get; set; }

        public static Account validateLogin(Account account)
        {
            Account retAcc = null;
            try
            {
                DBUtility<Account> db = new DBUtility<Account>();
                string strSQL = db.getDataListSQL(account);
                List<Account> accountDB = db.queryListBySql(strSQL, Constant.MYSQL_DB_TYPE, ConnStr);
                if (accountDB.Count > 0)
                {
                    retAcc = accountDB[0];
                    if (retAcc.AccountPwd != account.AccountPwd)
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex+ex.StackTrace);
            }
            return retAcc;
        }

        public static void UpdateAccount(Account account)
        {
            try
            {
                DBUtility<Account> db = new DBUtility<Account>();
                if (account.CreateDate == null)
                    account.CreateDate = DateTime.Now;
                string strSQL = db.GenerateUpdateQuery(account, Constant.MYSQL_DB_TYPE);
                log.Debug(strSQL);
                db.executeBySql(strSQL, Constant.MYSQL_DB_TYPE, ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString);
            }
            catch (Exception ex)
            {
                log.Error(ex + ex.StackTrace);
                throw ex;
            }
        }

        public static void DeleteAccount(string accountId)
        {
            try
            {
                DBUtility<Account> db = new DBUtility<Account>();
                Account account = new Account();
                account.AccountId = accountId;
                string strSQL = db.GenDeleteSQL(account);
                log.Debug(strSQL);
                db.executeBySql(strSQL, Constant.MYSQL_DB_TYPE, ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString);
            }
            catch (Exception ex)
            {
                log.Error(ex + ex.StackTrace);
                throw ex;
            }
        }

        public static Account getUnique(Account account)
        {
            Account ret = new Account();
            try
            {
                DBUtility<Account> db = new DBUtility<Account>();
                //if (account.CreateDate == null)
                //    account.CreateDate = DateTime.Now;
                string strSQL = db.getDataListSQL(account) + " AND AccountId='"+ account.AccountId + "'";
                log.Debug(strSQL);
                List<Account> accList = db.queryListBySql(strSQL, Constant.MYSQL_DB_TYPE, ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString);
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

        public static void InsertAccount(Account account)
        {
            try
            {
                DBUtility<Account> db = new DBUtility<Account>();
                if (account.CreateDate == null)
                    account.CreateDate = DateTime.Now;
                string strSQL = db.GenerateInsertQueryAny(account);
                log.Debug(strSQL);
                db.executeBySql(strSQL, Constant.MYSQL_DB_TYPE, ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString);
            }
            catch (Exception ex)
            {
                log.Error(ex+ex.StackTrace);
                throw ex;
            }
        }

        public static List<Account> getAllAccount()
        {
            List<Account> retAcc = null;
            try
            {
                DBUtility<Account> db = new DBUtility<Account>();
                string strSQL = db.getDataListSQL(new Account());
                List<Account> accountDB = db.queryListBySql(strSQL, Constant.MYSQL_DB_TYPE, ConnStr);
                if (accountDB.Count > 0)
                {
                    retAcc = accountDB;//new PagedList<Account>(accountDB, 0, 10);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex + ex.StackTrace);
            }
            return retAcc;
        }
        [JsonProperty]
        public List<SelectListItem> branchList
        {
            get
            {
                DBUtility<SystemConfig> db = new DBUtility<SystemConfig>();
                List<SystemConfig> systemConfigs = db.queryListBySql(db.getDataListSQL(new SystemConfig() { ConfigGroupName = "Account", ConfigName = "Branch" }), Constant.MYSQL_DB_TYPE, ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString);
                List<SelectListItem> selectListItems = new List<SelectListItem>();
                foreach (SystemConfig conf in systemConfigs)
                {
                    SelectListItem listItem = new SelectListItem();
                    listItem.Text = conf.ConfigDesc;
                    listItem.Value = conf.ConfigValue;
                    if (branch != null && conf.ConfigValue == branch.ToString())
                    {
                        listItem.Selected = true;
                    }
                    selectListItems.Add(listItem);
                }
                return selectListItems;
            }
            set { }
        }
        [JsonProperty]
        public List<SelectListItem> selectPrivList { 
            get 
            {
                DBUtility<SystemConfig> db = new DBUtility<SystemConfig>();
                List<SystemConfig> systemConfigs = db.queryListBySql(db.getDataListSQL(new SystemConfig() { ConfigGroupName= "Account", ConfigName= "Privilege" }), Constant.MYSQL_DB_TYPE, ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString);
                List<SelectListItem> selectListItems = new List<SelectListItem>();
                foreach (SystemConfig conf in systemConfigs)
                {
                    SelectListItem listItem = new SelectListItem();
                    listItem.Text = conf.ConfigDesc;
                    listItem.Value = conf.ConfigValue;
                    if (AccountPriv != null && conf.ConfigValue == AccountPriv.ToString())
                    {
                        listItem.Selected = true;
                    }
                    selectListItems.Add(listItem);
                }
                return selectListItems;
            }
            set { }
        }
        public static string getPrivilegeName(int? privilegeInt)
        {
            string retStr = string.Empty;
            try
            {
                if (privilegeInt == null)
                    return string.Empty;
                SystemConfig systemConfig = new SystemConfig();
                systemConfig.ConfigGroupName = "Account";
                systemConfig.ConfigName = "Privilege";
                systemConfig.ConfigValue = privilegeInt.ToString();
                DBUtility<SystemConfig> db = new DBUtility<SystemConfig>();
                string strSQL = db.getDataListSQLNoKey(systemConfig);
                log.Debug(strSQL);
                List<SystemConfig> systemConfigs = db.queryListBySql(strSQL, Constant.MYSQL_DB_TYPE, ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString);
                if (systemConfigs.Count > 0)
                {
                    retStr = systemConfigs[0].ConfigDesc;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex+ex.StackTrace);
            }
            return retStr;
        }
        public static string getBranchName(int? privilegeInt)
        {
            string retStr = string.Empty;
            try
            {
                if (privilegeInt == null)
                    return string.Empty;
                SystemConfig systemConfig = new SystemConfig();
                systemConfig.ConfigGroupName = "Account";
                systemConfig.ConfigName = "Branch";
                systemConfig.ConfigValue = privilegeInt.ToString();
                DBUtility<SystemConfig> db = new DBUtility<SystemConfig>();
                string strSQL = db.getDataListSQLNoKey(systemConfig);
                log.Debug(strSQL);
                List<SystemConfig> systemConfigs = db.queryListBySql(strSQL, Constant.MYSQL_DB_TYPE, ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString);
                if (systemConfigs.Count > 0)
                {
                    retStr = systemConfigs[0].ConfigDesc;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex + ex.StackTrace);
            }
            return retStr;
        }
    }
}