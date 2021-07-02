using ProcessRequest.ProcessRequest.DB;

namespace MedicalBeautyDataServ.Models
{
    public class Constant
    {
        public static DBType MYSQL_DB_TYPE = DBType.MsSql;

        public static string RETURN_SUCCESS_XML_FORMAT { get; internal set; } = ""; 
        public static string TXGSMART_DB_CONN_STR { get; internal set; }
    }
}