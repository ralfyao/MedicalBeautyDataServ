using log4net;
using log4net.Config;
using PagedList;
using Newtonsoft.Json;
//using ProcessRequest.ProcessRequest.DB;
using ProcessRequest.Utility.DB;
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
    public class Customer
    {
        public static string ConnStr = ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString;
        private static ILog log = LogManager.GetLogger(typeof(Customer));
        [JsonProperty]
        public IPagedList<Customer> custList { get; set; }

        public Customer()
        {
            XmlConfigurator.Configure(new System.IO.FileInfo("./log4net.config"));
        }
        [Key]
        public Guid? id { get; set; }
        [DisplayName("姓名：")]
        public string Name { get; set; }
        [DisplayName("性別：")]
        public int? Gender { get; set; }
        [DisplayName("電話：")]
        public string Tel { get; set; }
        [DisplayName("生日：")]
        public string BirthDate { get; set; }
        public string CreateUser { get; set; }

        public static List<Customer> getList(Customer cust)
        {
            List<Customer> retAcc = new List<Customer>();
            try
            {
                DBUtility<Customer> db = new DBUtility<Customer>();
                string strSQL = db.getDataListSQLLike(cust);
                log.Debug(strSQL);
                List<Customer> accountDB = db.queryListBySql(strSQL, Constant.MYSQL_DB_TYPE, ConnStr);
                if (accountDB.Count > 0)
                {
                    retAcc = accountDB;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex + ex.StackTrace);
            }
            return retAcc;
        }

        public DateTime? CreateDate { get; set; }
        [JsonProperty]
        public List<SelectListItem> selectGenderList
        {
            get
            {
                DBUtility<SystemConfig> db = new DBUtility<SystemConfig>();
                List<SystemConfig> systemConfigs = db.queryListBySql(db.getDataListSQL(new SystemConfig() { ConfigGroupName = "Customer", ConfigName= "Gender" }), Constant.MYSQL_DB_TYPE, ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString);
                List<SelectListItem> selectListItems = new List<SelectListItem>();
                foreach (SystemConfig conf in systemConfigs)
                {
                    SelectListItem listItem = new SelectListItem();
                    listItem.Text = conf.ConfigDesc;
                    listItem.Value = conf.ConfigValue;
                    if (Gender != null && conf.ConfigValue == Gender.ToString())
                    {
                        listItem.Selected = true;
                    }
                    selectListItems.Add(listItem);
                }
                return selectListItems;
            }
            set { }
        }

        public static List<Customer> getCustomerByName(string name)
        {
            List<Customer> retAcc = new List<Customer>();
            try
            {
                DBUtility<Customer> db = new DBUtility<Customer>();
                Customer customer = new Customer();
                customer.Name = name;
                string strSQL = db.getDataListSQLLike(customer);
                log.Debug(strSQL);
                List<Customer> accountDB = db.queryListBySql(strSQL, Constant.MYSQL_DB_TYPE, ConnStr);
                if (accountDB.Count > 0)
                {
                    retAcc = accountDB;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex + ex.StackTrace);
            }
            return retAcc;
        }

        public static void Insert(Customer customer, string user = "")
        {
            try
            {
                DBUtility<Customer> db = new DBUtility<Customer>();
                if (customer.CreateDate == null)
                    customer.CreateDate = DateTime.Now;
                customer.CreateUser = user;
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

        public static void Update(Customer customer)
        {
            try
            {
                DBUtility<Customer> db = new DBUtility<Customer>();
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
        public static void Delete(string id)
        {
            try
            {
                DBUtility<Customer> db = new DBUtility<Customer>();
                Customer customer = new Customer();
                customer.id = Guid.Parse(id);
                string strSQL = db.GenDeleteSQL(customer);
                log.Debug(strSQL);
                db.executeBySql(strSQL, Constant.MYSQL_DB_TYPE, ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString);
            }
            catch (Exception ex)
            {
                log.Error(ex + ex.StackTrace);
                throw ex;
            }
        }

        public static bool isExist(Customer customer)
        {
            bool isExist = false;
            try
            {
                DBUtility<Customer> db = new DBUtility<Customer>();
                string strSQL = db.getDataListSQLNoKey(customer);
                log.Debug(strSQL);
                List<Customer> custs = db.queryListBySql(strSQL, Constant.MYSQL_DB_TYPE, ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString);
                if (custs.Count > 0)
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

        public static Customer getUnique(Customer cust)
        {
            Customer customr = new Customer();
            try
            {
                DBUtility<Customer> db = new DBUtility<Customer>();
                string strSQL = db.getDataListSQL(cust) + string.Format(" AND id='{0}'", cust.id);
                log.Debug(strSQL);
                List<Customer> customers = db.queryListBySql(strSQL, Constant.MYSQL_DB_TYPE, ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString);
                if (customers.Count > 0)
                {
                    customr = customers[0];
                }
            }
            catch (Exception ex)
            {
                log.Error(ex+ex.StackTrace);
            }
            return customr;
        }

        public string ModifyUser { get; set; }
        public DateTime? ModifyDate { get; set; }
        public static List<Customer> getAllCustomer()
        {
            List<Customer> retAcc = new List<Customer>();
            try
            {
                DBUtility<Customer> db = new DBUtility<Customer>();
                string strSQL = db.getDataListSQL(new Customer());
                List<Customer> accountDB = db.queryListBySql(strSQL, Constant.MYSQL_DB_TYPE, ConnStr);
                if (accountDB.Count > 0)
                {
                    retAcc = accountDB;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex + ex.StackTrace);
            }
            return retAcc;
        }
    }
}