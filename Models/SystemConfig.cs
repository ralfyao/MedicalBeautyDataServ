using log4net;
using log4net.Config;
//using ProcessRequest.ProcessRequest.DB;
using ProcessRequest.Utility.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;

namespace MedicalBeautyDataServ.Models
{
    public class SystemConfig
    {
        private static ILog log = LogManager.GetLogger(typeof(SystemConfig));
        [Key]
        public string ConfigGroupName { get; set; }
        [Key]
        public string ConfigName { get; set; }
        [Key]
        public string ConfigValue { get; set; }
        public string ConfigDesc { get; set; }
        public static string getDesc(string configGroupName, string configName, string configValue)
        {
            string retStr = string.Empty;
            XmlConfigurator.Configure(new System.IO.FileInfo("./log4net.config"));
            try
            {
                SystemConfig systemConfig = new SystemConfig();
                systemConfig.ConfigGroupName = configGroupName;
                systemConfig.ConfigName = configName;
                systemConfig.ConfigValue = configValue;
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