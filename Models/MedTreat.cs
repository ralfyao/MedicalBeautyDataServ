using log4net;
using log4net.Config;
using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;

namespace MedicalBeautyDataServ.Models
{
    public class MedTreat
    {
        private static ILog log = LogManager.GetLogger(typeof(MedTreat));
        public static string ConnStr = ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString;
        public MedTreat()
        {
            XmlConfigurator.Configure(new System.IO.FileInfo("./log4net.config"));
        }
        [Key]
        public Guid? id { get; set; }
        [DisplayName("療程類別：")]
        public string MedClass { get; set; }
        [DisplayName("療程名稱：")]
        public string Name { get; set; }
        [DisplayName("單位：")]
        public string Unit { get; set; }
        [DisplayName("建立日期：")]
        public DateTime? CreateDate { get; set; }
        [JsonProperty]
        public IPagedList<MedTreat> medTreatList { get; set; }
        public static List<MedTreat> getAllMedTreats()
        {
            List<MedTreat> retAcc = new List<MedTreat>();
            try
            {
                DBUtility<MedTreat> db = new DBUtility<MedTreat>();
                string strSQL = db.getDataListSQL(new MedTreat());
                List<MedTreat> medTreatDB = db.queryListBySql(strSQL, Constant.MYSQL_DB_TYPE, ConnStr);
                if (medTreatDB.Count > 0)
                {
                    retAcc = medTreatDB;//new PagedList<Account>(accountDB, 0, 10);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex + ex.StackTrace);
            }
            return retAcc;
        }

        public static void Insert(MedTreat customer)
        {
            try
            {
                DBUtility<MedTreat> db = new DBUtility<MedTreat>();
                if (customer.CreateDate == null)
                    customer.CreateDate = DateTime.Now;
                if (customer.id == null)
                {
                    customer.id = Guid.NewGuid();
                }
                string strSQL = db.GenerateInsertQueryAny(customer);
                log.Debug(strSQL);
                db.executeBySql(strSQL, Constant.MYSQL_DB_TYPE, ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString);
            }
            catch (Exception ex)
            {
                log.Error(ex + ex.StackTrace);
                throw ex;
            }
        }

        public static bool isExist(MedTreat customer)
        {
            bool isExist = false;
            try
            {
                DBUtility<MedTreat> db = new DBUtility<MedTreat>();
                string strSQL = db.getDataListSQLNoKey(customer);
                log.Debug(strSQL);
                List<MedTreat> custs = db.queryListBySql(strSQL, Constant.MYSQL_DB_TYPE, ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString);
                if (custs.Count > 0)
                {
                    isExist = true;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex + ex.StackTrace);
                throw ex;
            }
            return isExist;
        }

        public static MedTreat get(string medClass, string medName)
        {
            MedTreat treat = new MedTreat();
            try
            {
                treat.MedClass = medClass;
                treat.Name = medName;
                DBUtility<MedTreat> dB = new DBUtility<MedTreat>();
                string strSQL = dB.getDataListSQLNoKey(treat);
                List<MedTreat> lst = dB.queryListBySql(strSQL, Constant.MYSQL_DB_TYPE, Account.ConnStr);
                if (lst.Count > 0)
                {
                    treat = lst[0];
                }
            }
            catch (Exception ex) 
            {
                log.Error(ex + ex.StackTrace);
                throw ex;
            }
            return treat;
        }

        public static void Delete(string id)
        {
            try
            {
                DBUtility<MedTreat> db = new DBUtility<MedTreat>();
                MedTreat account = new MedTreat();
                account.id = Guid.Parse(id);
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

        public static void Update(MedTreat customer)
        {
            try
            {
                DBUtility<MedTreat> db = new DBUtility<MedTreat>();
                if (customer.CreateDate == null)
                    customer.CreateDate = DateTime.Now;
                string strSQL = db.GenerateUpdateQuery(customer, Constant.MYSQL_DB_TYPE);
                log.Debug(strSQL);
                db.executeBySql(strSQL, Constant.MYSQL_DB_TYPE, ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString);
            }
            catch (Exception ex)
            {
                log.Error(ex + ex.StackTrace);
                throw ex;
            }
        }

        public static MedTreat getUnique(string id)
        {
            MedTreat ret = new MedTreat();
            try
            {
                ret.id = Guid.Parse(id);
                DBUtility<MedTreat> db = new DBUtility<MedTreat>();
                //if (account.CreateDate == null)
                //    account.CreateDate = DateTime.Now;
                string strSQL = db.getDataListSQL(ret);
                log.Debug(strSQL);
                List<MedTreat> accList = db.queryListBySql(strSQL, Constant.MYSQL_DB_TYPE, ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString);
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
    }
}