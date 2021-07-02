using MySqlConnector;
using Newtonsoft.Json;
using NUnit.Framework.Constraints;
using ProcessRequest.ProcessRequest.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace MedicalBeautyDataServ.Models
{
    public class DBUtility<T> 
    {
        /// <summary>
        /// 取得該泛型類別所有的成員名稱，不包含function
        /// </summary>
        private IEnumerable<PropertyInfo> GetProperties => typeof(T).GetProperties();
        public string getWhereCondByObj(T t, bool useParm = false)
        {
            string strSQL = string.Empty;
            var WhereQuery = new StringBuilder();
            try
            {
                string tmpStr = string.Empty;
                IEnumerable<PropertyInfo> lstPropertys = GetProperties;
                if (lstPropertys.Count() == 0)
                {
                    lstPropertys = t.GetType().GetProperties().ToList();
                }
                foreach (PropertyInfo property in lstPropertys)
                {
                    var attribute1 = Attribute.GetCustomAttribute(property, typeof(JsonPropertyAttribute))
                       as JsonPropertyAttribute;
                    if (attribute1 == null && property.GetValue(t) != null && !string.IsNullOrEmpty(property.GetValue(t).ToString()))
                    {
                        tmpStr = property.GetValue(t) == null ? null : property.GetValue(t).ToString();
                        if (tmpStr != null)
                        {
                            if (useParm)
                            {

                            }
                            else
                            {
                                if (property.PropertyType.Equals(typeof(DateTime)))
                                {
                                    WhereQuery.Append($" AND {property.Name}='{((DateTime)property.GetValue(t)).ToString("yyyy-MM-dd HH:mm:ss").Replace(" ", "")}'");
                                }
                                else
                                {
                                    WhereQuery.Append($" AND {property.Name}='{tmpStr.Replace(" ", "")}'");
                                }
                            }
                        }
                    }
                }
                //加上分頁功能
                foreach (PropertyInfo property in lstPropertys)
                {
                    var attribute1 = Attribute.GetCustomAttribute(property, typeof(JsonPropertyAttribute))
                       as JsonPropertyAttribute;
                    if (attribute1 != null)
                    {
                        tmpStr = property.GetValue(t) == null ? null : property.GetValue(t).ToString();
                        if (tmpStr != null)
                        {
                            if (property.Name.ToUpper() == "PAGESIZE")
                            {
                                WhereQuery.Append($" LIMIT {tmpStr}");
                            }
                            if (property.Name.ToUpper() == "STARTIDX")
                            {
                                WhereQuery.Append($" OFFSET {tmpStr}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //log.Error(ex);
                throw ex;
            }
            strSQL = WhereQuery.ToString();
            return strSQL;
        }
        /// <summary>
        /// 傳入泛型類別，並依據該類別物件的成員，有指定值的Primary Key成員才加入查詢條件，產生SELECT * 的SQL COMMAND
        /// </summary>
        /// <param name="t">傳入的泛型類別</param>
        /// <returns>SQL COMMAND</returns>
        public string getDataListSQL(T t, bool parms = false)
        {
            string strSQL = $"SELECT * FROM {t.GetType().Name} WHERE 1=1 ";
            try
            {

                var WhereQuery = new StringBuilder();
                string tmpStr = string.Empty;
                IEnumerable<PropertyInfo> lstPropertys = GetProperties;
                if (lstPropertys.Count() == 0)
                {
                    lstPropertys = t.GetType().GetProperties().ToList();
                }
                foreach (PropertyInfo property in lstPropertys)
                {
                    var attribute = Attribute.GetCustomAttribute(property, typeof(KeyAttribute))
                        as KeyAttribute;

                    if (attribute != null) // This property has a KeyAttribute
                    {
                        tmpStr = property.GetValue(t) == null ? null : property.GetValue(t).ToString();
                        if (tmpStr != null)
                        {
                            if (parms)
                            {
                                WhereQuery.Append($" AND {property.Name}=@{property.Name}");
                            }
                            else
                            {
                                WhereQuery.Append($" AND {property.Name}='{tmpStr.Replace(" ", "")}'");
                            }
                        }

                    }
                    else
                    {
                        if (property.GetValue(t) != null)
                        {
                        }
                    }
                }
                foreach (PropertyInfo property in lstPropertys)
                {
                    var attribute1 = Attribute.GetCustomAttribute(property, typeof(JsonPropertyAttribute))
                       as JsonPropertyAttribute;
                    if (attribute1 != null)
                    {
                        tmpStr = property.GetValue(t) == null ? null : property.GetValue(t).ToString();
                        if (tmpStr != null)
                        {
                            if (property.Name.ToUpper() == "PAGESIZE")
                            {
                                WhereQuery.Append($" LIMIT {tmpStr}");
                            }
                            if (property.Name.ToUpper() == "STARTIDX")
                            {
                                WhereQuery.Append($" OFFSET {tmpStr}");
                            }
                        }
                    }
                }
                strSQL += WhereQuery.ToString();
            }
            catch (Exception ex)
            {
                //log.Error(ex);
                throw ex;
            }
            return strSQL;
        }
        /// <summary>
        /// If data exist, update it. If data not exist, insert it.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public string InsertOrUpdate(T t, bool emptyStringNull = false)
        {
            string retSQL = string.Empty;
            try
            {
                IEnumerable<T> obj = this.queryListBySql(getDataListSQL(t), Constant.MYSQL_DB_TYPE, Constant.TXGSMART_DB_CONN_STR);
                string rootPathVal = string.Empty;
                int dataCnt = 0;
                foreach (T record in obj)
                {
                    dataCnt++;
                }
                if (dataCnt == 0)
                {
                    retSQL = Insert(t, false, Constant.MYSQL_DB_TYPE, Constant.TXGSMART_DB_CONN_STR);
                    //InsertMemory(t);
                }
                else
                {
                    retSQL = Update(t, false, Constant.MYSQL_DB_TYPE, Constant.TXGSMART_DB_CONN_STR);
                    //UpdateMemory(t);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return retSQL;
        }
        public string GenInsertOrUpdateSQL(T t, bool emptyStringNull = false)
        {
            string retSQL = string.Empty;
            try
            {
                IEnumerable<T> obj = this.queryListBySql(getDataListSQL(t), Constant.MYSQL_DB_TYPE, Constant.TXGSMART_DB_CONN_STR);
                string rootPathVal = string.Empty;
                int dataCnt = 0;
                foreach (T record in obj)
                {
                    dataCnt++;
                }
                if (dataCnt == 0)
                {
                    retSQL = GenerateInsertQueryAny(t);
                    //InsertMemory(t);
                }
                else
                {
                    retSQL = GenerateUpdateQuery(t);
                    //UpdateMemory(t);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return retSQL;
        }
        public string InsertOrUpdateAny(T t, bool emptyStringNull = false)
        {
            string retSQL = string.Empty;
            try
            {
                IEnumerable<T> obj = this.queryListBySql(getDataListSQL(t), Constant.MYSQL_DB_TYPE, Constant.TXGSMART_DB_CONN_STR);
                string rootPathVal = string.Empty;
                int dataCnt = 0;
                foreach (T record in obj)
                {
                    dataCnt++;
                }
                if (dataCnt == 0)
                {
                    retSQL = InsertAny(t, false, Constant.MYSQL_DB_TYPE, Constant.TXGSMART_DB_CONN_STR);
                    //InsertMemory(t);
                }
                else
                {
                    retSQL = Update(t, false, Constant.MYSQL_DB_TYPE, Constant.TXGSMART_DB_CONN_STR);
                    //UpdateMemory(t);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return retSQL;
        }
        /// <summary>
        /// Dapper ORM Update function
        /// </summary>
        /// <param name="t"></param>
        public string Update(T t, bool emptyStringNull = false, DBType dBType = DBType.MySql, string connStr = "")
        {
            string updateQuery = string.Empty;
            try
            {
                updateQuery = GenerateUpdateQuery(t);
                if (emptyStringNull)
                    updateQuery = updateQuery.Replace("''", "null");
                executeBySql(updateQuery, dBType, connStr);
                return updateQuery;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Dapper ORM Insert function
        /// </summary>
        /// <param name="t"></param>
        public string Insert(T t, bool emptyStringNull = false, DBType dBType = DBType.MySql, string connStr = "")
        {
            string insertQuery = GenerateInsertQuery(t).Replace("上午", "").Replace("下午", "");
            try
            {
                if (emptyStringNull)
                    insertQuery = insertQuery.Replace("''", "null");
                executeBySql(insertQuery, dBType, connStr);
                return insertQuery;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string InsertAny(T t, bool emptyStringNull = false, DBType dBType = DBType.MySql, string connStr = "")
        {
            string insertQuery = GenerateInsertQueryAny(t).Replace("上午", "").Replace("下午", "");
            try
            {
                if (emptyStringNull)
                    insertQuery = insertQuery.Replace("''", "null");
                executeBySql(insertQuery, dBType, connStr);
                return insertQuery;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 將DataTable轉為List<T>
        /// </summary>
        /// <param name="dt">傳入的DataTable</param>
        /// <returns>轉換過的List<T></returns>
        public static List<T> BindList(DataTable dt)
        {
            // Example 1:
            // Get private fields + non properties
            //var fields = typeof(T).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            // Example 2: Your case
            // Get all public fields
            //日期無法轉換上下午寫入DB的問題，需要由OS端修改日期格式，將tt HH:mm:ss=改為HH:mm:ss
            //控制台\時鐘、語言和區域\變更日期、時間或數字格式-->修改"完整時間"
            var fields = typeof(T).GetProperties();
            List<T> lst = new List<T>();
            try
            {
                foreach (DataRow dr in dt.Rows)
                {
                    // Create the object of T
                    var ob = Activator.CreateInstance<T>();
                    foreach (DataColumn dc in dt.Columns)
                    {
                        foreach (var fieldInfo in fields)
                        {
                            // Matching the columns with fields
                            if (fieldInfo.Name == dc.ColumnName)
                            {
                                // Get the value from the datatable cell
                                object value = dr[dc.ColumnName];
                                // Set the value into the object
                                if (value.GetType() != typeof(DBNull))
                                {
                                    //if (value.GetType() == typeof(DateTime))
                                    //{
                                    //    fieldInfo.SetValue(ob, value);
                                    //}
                                    //else
                                    //{
                                    //    fieldInfo.SetValue(ob, value);
                                    //}
                                    if (value.GetType() == typeof(UInt64))
                                    {
                                        fieldInfo.SetValue(ob, Convert.ToInt32(value));
                                    }
                                    else
                                    {
                                        fieldInfo.SetValue(ob, value);
                                    }
                                }
                                break;
                            }
                        }
                    }
                    lst.Add(ob);
                }
                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 將DataTable轉為JSON
        /// </summary>
        /// <param name="dt">傳入的DataTable</param>
        /// <returns>轉換過的JSON</returns>
        public string dataTableToJson(DataTable lst)
        {
            string strXml = @"<response>
                                  <return_code>[RetCode]</return_code>
                                  <error_msg>[Msg]</error_msg>
                                  <data>[JSON]</data>
                              </response>";
            string strTmp = string.Empty;
            List<T> retList = BindList(lst);
            strTmp = JsonConvert.SerializeObject(retList);
            strXml = strXml.Replace("[RetCode]", "1").Replace("[Msg]", "").Replace("[JSON]", strTmp);
            return strXml;
        }
        public static IDbConnection getConnection(DBType dbTypeData, string dbConnStr)
        {
            IDbConnection _conn = null;
            try
            {
                switch (dbTypeData)
                {
                    case DBType.MySql:
                        MySqlConnection mySqlConnection = new MySqlConnection(dbConnStr);
                        mySqlConnection.Open();
                        _conn = mySqlConnection;
                        break;
                    case DBType.MsSql:
                        SqlConnection conn = new SqlConnection(dbConnStr);
                        conn.Open();
                        _conn = conn;
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _conn;
        }
        /// <summary>
        /// 查詢SQL並回傳List<T>
        /// </summary>
        /// <param name="strSQL">傳入的SQL</param>
        /// <param name="dbTypeData">DB類型：MsSql/MySql</param>
        /// <param name="dbConnStr">DB連線字串</param>
        /// <returns>查詢並轉換後的List<T></returns>
        public List<T> queryListBySql(string strSQL, DBType dbTypeData, string dbConnStr)
        {
            List<T> retObj = new List<T>();
            DataSet dataSet = new DataSet();
            string strXml = string.Empty;
            try
            {
                switch (dbTypeData)
                {
                    case DBType.MySql:
                        MySqlConnection mySqlConnection = new MySqlConnection(dbConnStr);
                        mySqlConnection.Open();
                        MySqlCommand mySqlCommand = new MySqlCommand();
                        mySqlCommand.Connection = mySqlConnection;
                        mySqlCommand.CommandText = strSQL;
                        mySqlCommand.CommandType = System.Data.CommandType.Text;
                        MySqlDataAdapter adapter = new MySqlDataAdapter(mySqlCommand);
                        adapter.Fill(dataSet);
                        mySqlConnection.Close();
                        break;
                    case DBType.MsSql:
                        SqlConnection conn = new SqlConnection(dbConnStr);
                        conn.Open();
                        SqlCommand command = new SqlCommand();
                        command.Connection = conn;
                        command.CommandText = strSQL;
                        command.CommandType = CommandType.Text;
                        SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                        dataAdapter.Fill(dataSet);
                        conn.Close();
                        break;
                }
                retObj = BindList(dataSet.Tables[0]);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return retObj;
        }

        public List<T> queryListBySql(string strSQL, DBType dbTypeData, string dbConnStr, Dictionary<string, string> parameter)
        {
            List<T> retObj = new List<T>();
            DataSet dataSet = new DataSet();
            string strXml = string.Empty;
            try
            {
                switch (dbTypeData)
                {
                    case DBType.MySql:
                        MySqlConnection mySqlConnection = new MySqlConnection(dbConnStr);
                        mySqlConnection.Open();
                        MySqlCommand mySqlCommand = new MySqlCommand();
                        mySqlCommand.Connection = mySqlConnection;
                        mySqlCommand.CommandText = strSQL;
                        foreach (string key in parameter.Keys)
                        {
                            MySqlParameter parm = new MySqlParameter();
                            parm.ParameterName = key;
                            parm.Value = parameter[key];
                            mySqlCommand.Parameters.Add(parm);
                        }
                        mySqlCommand.CommandType = System.Data.CommandType.Text;
                        MySqlDataAdapter adapter = new MySqlDataAdapter(mySqlCommand);
                        adapter.Fill(dataSet);
                        mySqlConnection.Close();
                        break;
                    case DBType.MsSql:
                        SqlConnection conn = new SqlConnection(dbConnStr);
                        conn.Open();
                        SqlCommand command = new SqlCommand();
                        command.Connection = conn;
                        command.CommandText = strSQL;
                        foreach (string key in parameter.Keys)
                        {
                            SqlParameter parm = new SqlParameter();
                            parm.ParameterName = key;
                            parm.Value = parameter[key];
                            command.Parameters.Add(parm);
                        }
                        command.CommandType = CommandType.Text;
                        SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                        dataAdapter.Fill(dataSet);
                        conn.Close();
                        break;
                }
                retObj = BindList(dataSet.Tables[0]);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return retObj;
        }

        public List<T> queryMySqlListBySql(ref MySqlConnection conn, ref MySqlTransaction tran, string strSQL, DBType dbTypeData, string dbConnStr)
        {
            List<T> retObj = new List<T>();
            DataSet dataSet = new DataSet();
            string strXml = string.Empty;
            try
            {
                switch (dbTypeData)
                {
                    case DBType.MySql:
                        MySqlCommand mySqlCommand = new MySqlCommand();
                        mySqlCommand.Connection = conn;
                        mySqlCommand.Transaction = tran;
                        mySqlCommand.CommandText = strSQL;
                        mySqlCommand.CommandType = System.Data.CommandType.Text;
                        MySqlDataAdapter adapter = new MySqlDataAdapter(mySqlCommand);
                        adapter.Fill(dataSet);
                        break;
                        //case DBType.MsSql:
                        //    SqlConnection conn = new SqlConnection(dbConnStr);
                        //    conn.Open();
                        //    SqlCommand command = new SqlCommand();
                        //    command.Connection = conn;
                        //    command.CommandText = strSQL;
                        //    command.CommandType = CommandType.Text;
                        //    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                        //    dataAdapter.Fill(dataSet);
                        //    conn.Close();
                        //    break;
                }
                retObj = BindList(dataSet.Tables[0]);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return retObj;
        }

        public List<T> queryListBySql(ref SqlConnection conn, ref SqlTransaction tran, string strSQL, DBType dbTypeData, string dbConnStr)
        {
            List<T> retObj = new List<T>();
            DataSet dataSet = new DataSet();
            string strXml = string.Empty;
            try
            {
                switch (dbTypeData)
                {
                    case DBType.MsSql:
                        SqlCommand mySqlCommand = new SqlCommand();
                        mySqlCommand.Connection = conn;
                        mySqlCommand.Transaction = tran;
                        mySqlCommand.CommandText = strSQL;
                        mySqlCommand.CommandType = System.Data.CommandType.Text;
                        SqlDataAdapter adapter = new SqlDataAdapter(mySqlCommand);
                        adapter.Fill(dataSet);
                        break;
                        //case DBType.MsSql:
                        //    SqlConnection conn = new SqlConnection(dbConnStr);
                        //    conn.Open();
                        //    SqlCommand command = new SqlCommand();
                        //    command.Connection = conn;
                        //    command.CommandText = strSQL;
                        //    command.CommandType = CommandType.Text;
                        //    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                        //    dataAdapter.Fill(dataSet);
                        //    conn.Close();
                        //    break;
                }
                retObj = BindList(dataSet.Tables[0]);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return retObj;
        }

        /// <summary>
        /// 傳入SQL回傳JSON字串
        /// </summary>
        /// <param name="strSQL">傳入的SQL</param>
        /// <param name="dbTypeData">DB類型：MsSql/MySql</param>
        /// <param name="dbConnStr">DB連線字串</param>
        /// <returns>查詢並轉換後的JSON字串</returns>
        public string queryBySql(string strSQL, DBType dbTypeData, string dbConnStr)
        {
            string strXml = string.Empty;
            DataSet dataSet = new DataSet();
            try
            {
                switch (dbTypeData)
                {
                    case DBType.MySql:
                        MySqlConnection mySqlConnection = new MySqlConnection(dbConnStr);
                        mySqlConnection.Open();
                        MySqlCommand mySqlCommand = new MySqlCommand();
                        mySqlCommand.Connection = mySqlConnection;
                        mySqlCommand.CommandText = strSQL;
                        mySqlCommand.CommandType = System.Data.CommandType.Text;
                        MySqlDataAdapter adapter = new MySqlDataAdapter(mySqlCommand);
                        adapter.Fill(dataSet);
                        mySqlConnection.Close();
                        break;
                    case DBType.MsSql:
                        SqlConnection conn = new SqlConnection(dbConnStr);
                        conn.Open();
                        SqlCommand command = new SqlCommand();
                        command.Connection = conn;
                        command.CommandText = strSQL;
                        command.CommandType = CommandType.Text;
                        SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                        dataAdapter.Fill(dataSet);
                        conn.Close();
                        break;
                }
                strXml = dataTableToJson(dataSet.Tables[0]);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return strXml;
        }
        /// <summary>
        /// 依SQL查詢DataSet物件回傳
        /// </summary>
        /// <param name="strSQL">要查詢的SQL</param>
        /// <param name="dbTypeData">DB種類Enum</param>
        /// <param name="dbConnStr">DB Connection 字串</param>
        /// <returns>查詢到的DataSet物件</returns>
        public DataSet queryDataSetBySql(string strSQL, DBType dbTypeData, string dbConnStr)
        {
            string strXml = string.Empty;
            DataSet dataSet = new DataSet();
            try
            {
                switch (dbTypeData)
                {
                    case DBType.MySql:
                        MySqlConnection mySqlConnection = new MySqlConnection(dbConnStr);
                        mySqlConnection.Open();
                        MySqlCommand mySqlCommand = new MySqlCommand();
                        mySqlCommand.Connection = mySqlConnection;
                        mySqlCommand.CommandText = strSQL;
                        mySqlCommand.CommandType = System.Data.CommandType.Text;
                        MySqlDataAdapter adapter = new MySqlDataAdapter(mySqlCommand);
                        adapter.Fill(dataSet);
                        mySqlConnection.Close();
                        break;
                    case DBType.MsSql:
                        SqlConnection conn = new SqlConnection(dbConnStr);
                        conn.Open();
                        SqlCommand command = new SqlCommand();
                        command.Connection = conn;
                        command.CommandText = strSQL;
                        command.CommandType = CommandType.Text;
                        SqlDataAdapter dataAdapter = new SqlDataAdapter();
                        dataAdapter.Fill(dataSet);
                        conn.Close();
                        break;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dataSet;
        }

        public DataSet queryDataSetBySql(ref MySqlConnection conn, ref MySqlTransaction tran, string strSQL, DBType dbTypeData, string dbConnStr)
        {
            string strXml = string.Empty;
            DataSet dataSet = new DataSet();
            try
            {
                switch (dbTypeData)
                {
                    case DBType.MySql:
                        //MySqlConnection mySqlConnection = new MySqlConnection(dbConnStr);
                        //mySqlConnection.Open();
                        MySqlCommand mySqlCommand = new MySqlCommand();
                        mySqlCommand.Connection = conn;
                        mySqlCommand.Transaction = tran;
                        mySqlCommand.CommandText = strSQL;
                        mySqlCommand.CommandType = System.Data.CommandType.Text;
                        MySqlDataAdapter adapter = new MySqlDataAdapter(mySqlCommand);
                        adapter.Fill(dataSet);
                        //mySqlConnection.Close();
                        break;
                        //case DBType.MsSql:
                        //    SqlConnection conn = new SqlConnection(dbConnStr);
                        //    conn.Open();
                        //    SqlCommand command = new SqlCommand();
                        //    command.Connection = conn;
                        //    command.CommandText = strSQL;
                        //    command.CommandType = CommandType.Text;
                        //    SqlDataAdapter dataAdapter = new SqlDataAdapter();
                        //    dataAdapter.Fill(dataSet);
                        //    conn.Close();
                        //    break;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dataSet;
        }
        public DataSet queryDataSetBySql(string strSQL, DBType dbTypeData, string dbConnStr, Dictionary<string, string> parms)
        {
            string strXml = string.Empty;
            DataSet dataSet = new DataSet();
            try
            {
                switch (dbTypeData)
                {
                    case DBType.MySql:
                        MySqlConnection mySqlConnection = new MySqlConnection(dbConnStr);
                        mySqlConnection.Open();
                        MySqlCommand mySqlCommand = new MySqlCommand();
                        mySqlCommand.Connection = mySqlConnection;
                        foreach (string key in parms.Keys)
                        {
                            mySqlCommand.Parameters.Add(new MySqlParameter() { ParameterName = "@" + key, Value = parms[key] });
                        }
                        mySqlCommand.CommandText = strSQL;
                        mySqlCommand.CommandType = System.Data.CommandType.Text;
                        MySqlDataAdapter adapter = new MySqlDataAdapter(mySqlCommand);
                        adapter.Fill(dataSet);
                        mySqlConnection.Close();
                        break;
                    case DBType.MsSql:
                        SqlConnection conn = new SqlConnection(dbConnStr);
                        conn.Open();
                        SqlCommand command = new SqlCommand();
                        command.Connection = conn;
                        foreach (string key in parms.Keys)
                        {
                            command.Parameters.Add(new SqlParameter() { ParameterName = "@" + key, Value = parms[key] });
                        }
                        command.CommandText = strSQL;
                        command.CommandType = CommandType.Text;
                        SqlDataAdapter dataAdapter = new SqlDataAdapter();
                        dataAdapter.Fill(dataSet);
                        conn.Close();
                        break;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dataSet;
        }

        public DataSet queryDataSetBySql(ref MySqlConnection conn, ref MySqlTransaction tran, string strSQL, DBType dbTypeData, string dbConnStr, Dictionary<string, string> parms)
        {
            string strXml = string.Empty;
            DataSet dataSet = new DataSet();
            try
            {
                switch (dbTypeData)
                {
                    case DBType.MySql:
                        //MySqlConnection mySqlConnection = new MySqlConnection(dbConnStr);
                        //mySqlConnection.Open();
                        MySqlCommand mySqlCommand = new MySqlCommand();
                        mySqlCommand.Connection = conn;
                        mySqlCommand.Transaction = tran;
                        foreach (string key in parms.Keys)
                        {
                            mySqlCommand.Parameters.Add(new MySqlParameter() { ParameterName = "@" + key, Value = parms[key] });
                        }
                        mySqlCommand.CommandText = strSQL;
                        mySqlCommand.CommandType = System.Data.CommandType.Text;
                        MySqlDataAdapter adapter = new MySqlDataAdapter(mySqlCommand);
                        adapter.Fill(dataSet);
                        //mySqlConnection.Close();
                        break;
                        //case DBType.MsSql:
                        //    SqlConnection conn = new SqlConnection(dbConnStr);
                        //    conn.Open();
                        //    SqlCommand command = new SqlCommand();
                        //    command.Connection = conn;
                        //    foreach (string key in parms.Keys)
                        //    {
                        //        command.Parameters.Add(new SqlParameter() { ParameterName = "@" + key, Value = parms[key] });
                        //    }
                        //    command.CommandText = strSQL;
                        //    command.CommandType = CommandType.Text;
                        //    SqlDataAdapter dataAdapter = new SqlDataAdapter();
                        //    dataAdapter.Fill(dataSet);
                        //    conn.Close();
                        //    break;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dataSet;
        }
        public DataSet queryDataSetBySql(string strSQL, DBType dbTypeData, string dbConnStr, List<IDbDataParameter> parm)
        {
            string strXml = string.Empty;
            DataSet dataSet = new DataSet();
            try
            {
                switch (dbTypeData)
                {
                    case DBType.MySql:
                        MySqlConnection mySqlConnection = new MySqlConnection(dbConnStr);
                        mySqlConnection.Open();
                        MySqlCommand mySqlCommand = new MySqlCommand();
                        mySqlCommand.Connection = mySqlConnection;
                        mySqlCommand.Parameters.AddRange(parm.ToArray());
                        mySqlCommand.CommandText = strSQL;
                        mySqlCommand.CommandType = System.Data.CommandType.Text;
                        MySqlDataAdapter adapter = new MySqlDataAdapter(mySqlCommand);
                        adapter.Fill(dataSet);
                        mySqlConnection.Close();
                        break;
                    case DBType.MsSql:
                        SqlConnection conn = new SqlConnection(dbConnStr);
                        conn.Open();
                        SqlCommand command = new SqlCommand();
                        command.Connection = conn;
                        command.Parameters.AddRange(parm.ToArray());
                        command.CommandText = strSQL;
                        command.CommandType = CommandType.Text;
                        SqlDataAdapter dataAdapter = new SqlDataAdapter();
                        dataAdapter.Fill(dataSet);
                        conn.Close();
                        break;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dataSet;
        }
        /// <summary>
        /// 產生Delete T資料的SQL並回傳
        /// </summary>
        /// <param name="t">要Delete的T</param>
        /// <returns>Delete SQL Command</returns>
        public string Delete(T t, DBType dBType = DBType.MySql, string connStr = "")
        {
            var WhereQuery = new StringBuilder();
            string tmpStr = string.Empty;
            foreach (PropertyInfo property in GetProperties)
            {
                var attribute = Attribute.GetCustomAttribute(property, typeof(KeyAttribute))
                    as KeyAttribute;

                if (attribute != null && property.GetValue(t) != null) // This property has a KeyAttribute
                {
                    tmpStr = property.GetValue(t).ToString();
                    WhereQuery.Append($"{property.Name}='{tmpStr}' and ");
                }
            }
            WhereQuery.Remove(WhereQuery.Length - 4, 4);
            try
            {
                string strSQL = $"DELETE FROM {t.GetType().Name} WHERE 1=1 AND " + WhereQuery.ToString();
                executeBySql(strSQL, dBType, connStr);
                return strSQL;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 依傳入的Command執行SQL，並回傳
        /// </summary>
        /// <param name="strSQL"></param>
        /// <param name="dbTypeData"></param>
        /// <param name="dbConnStr"></param>
        /// <returns></returns>
        public string executeBySql(string strSQL, DBType dbTypeData, string dbConnStr)
        {
            string strXml = Constant.RETURN_SUCCESS_XML_FORMAT.Replace("[SQL]", strSQL.Replace("<", "&lt;").Replace(">", "&gt;"));//.Replace("[Byte]", Constant.DEFAULT_BYTE_SIZE);
            DataSet dataSet = new DataSet();
            string strTmp = string.Empty;
            try
            {
                switch (dbTypeData)
                {
                    case DBType.MySql:
                        MySqlConnection mySqlConnection = new MySqlConnection(dbConnStr);
                        mySqlConnection.Open();
                        MySqlCommand mySqlCommand = new MySqlCommand();
                        mySqlCommand.Connection = mySqlConnection;
                        mySqlCommand.CommandText = strSQL;
                        mySqlCommand.CommandType = System.Data.CommandType.Text;
                        mySqlCommand.ExecuteNonQuery();
                        mySqlConnection.Close();
                        break;
                    case DBType.MsSql:
                        SqlConnection conn = new SqlConnection(dbConnStr);
                        conn.Open();
                        SqlCommand command = new SqlCommand();
                        command.Connection = conn;
                        command.CommandText = strSQL;
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                        conn.Close();
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return strXml;
        }
        /// <summary>
        /// 依傳入的Command執行SQL，並回傳
        /// </summary>
        /// <param name="strSQL"></param>
        /// <param name="dbTypeData"></param>
        /// <param name="dbConnStr"></param>
        /// <returns></returns>
        public string executeBySql(ref MySqlConnection conn, ref MySqlTransaction tran, string strSQL, DBType dbTypeData, string dbConnStr)
        {
            string strXml = Constant.RETURN_SUCCESS_XML_FORMAT.Replace("[SQL]", strSQL.Replace("<", "&lt;").Replace(">", "&gt;"));//.Replace("[Byte]", Constant.DEFAULT_BYTE_SIZE);
            DataSet dataSet = new DataSet();
            string strTmp = string.Empty;
            try
            {
                switch (dbTypeData)
                {
                    case DBType.MsSql:
                        //MySqlConnection mySqlConnection = new MySqlConnection(dbConnStr);
                        //mySqlConnection.Open();
                        MySqlCommand mySqlCommand = new MySqlCommand();
                        mySqlCommand.Connection = conn;
                        mySqlCommand.Transaction = tran;
                        mySqlCommand.CommandText = strSQL;
                        mySqlCommand.CommandType = System.Data.CommandType.Text;
                        mySqlCommand.ExecuteNonQuery();
                        //mySqlConnection.Close();
                        break;
                        //case DBType.MsSql:
                        //    SqlConnection conn = new SqlConnection(dbConnStr);
                        //    conn.Open();
                        //    SqlCommand command = new SqlCommand();
                        //    command.Connection = conn;
                        //    command.CommandText = strSQL;
                        //    command.CommandType = CommandType.Text;
                        //    SqlDataAdapter dataAdapter = new SqlDataAdapter();
                        //    dataAdapter.Fill(dataSet);
                        //    command.ExecuteNonQuery();
                        //    conn.Close();
                        //    break;
                }
            }
            catch (Exception ex)
            {
                strXml = @"<response>
                                  <return_code>0</return_code>
                                  <error_msg>" + ex.Message + @"</error_msg>
                                  <data>" + ex.StackTrace + @"</data>
                              </response>";
            }
            return strXml;
        }
        public string executeBySql(ref SqlConnection conn, ref SqlTransaction tran, string strSQL, DBType dbTypeData, string dbConnStr)
        {
            string strXml = Constant.RETURN_SUCCESS_XML_FORMAT.Replace("[SQL]", strSQL.Replace("<", "&lt;").Replace(">", "&gt;"));//.Replace("[Byte]", Constant.DEFAULT_BYTE_SIZE);
            DataSet dataSet = new DataSet();
            string strTmp = string.Empty;
            try
            {
                switch (dbTypeData)
                {
                    case DBType.MsSql:
                        //MySqlConnection mySqlConnection = new MySqlConnection(dbConnStr);
                        //mySqlConnection.Open();
                        SqlCommand mySqlCommand = new SqlCommand();
                        mySqlCommand.Connection = conn;
                        mySqlCommand.Transaction = tran;
                        mySqlCommand.CommandText = strSQL;
                        mySqlCommand.CommandType = System.Data.CommandType.Text;
                        mySqlCommand.ExecuteNonQuery();
                        //mySqlConnection.Close();
                        break;
                        //case DBType.MsSql:
                        //    SqlConnection conn = new SqlConnection(dbConnStr);
                        //    conn.Open();
                        //    SqlCommand command = new SqlCommand();
                        //    command.Connection = conn;
                        //    command.CommandText = strSQL;
                        //    command.CommandType = CommandType.Text;
                        //    SqlDataAdapter dataAdapter = new SqlDataAdapter();
                        //    dataAdapter.Fill(dataSet);
                        //    command.ExecuteNonQuery();
                        //    conn.Close();
                        //    break;
                }
            }
            catch (Exception ex)
            {
                strXml = @"<response>
                                  <return_code>0</return_code>
                                  <error_msg>" + ex.Message + @"</error_msg>
                                  <data>" + ex.StackTrace + @"</data>
                              </response>";
            }
            return strXml;
        }
        /// <summary>
        /// 傳入泛型類別，並依據該類別物件的成員，有指定值的成員(不一定要Primary Key)才加入查詢條件，產生SELECT * 的SQL COMMAND
        /// </summary>
        /// <param name="t">傳入的泛型類別</param>
        /// <returns>SQL COMMAND</returns>
        public string getDataListSQLNoKey(T t)
        {
            string strSQL = $"SELECT * FROM {t.GetType().Name} WHERE 1=1 ";
            try
            {

                var WhereQuery = new StringBuilder();
                string tmpStr = string.Empty;
                IEnumerable<PropertyInfo> lstPropertys = GetProperties;
                if (lstPropertys.Count() == 0)
                {
                    lstPropertys = t.GetType().GetProperties().ToList();
                }
                foreach (PropertyInfo property in lstPropertys)
                {
                    var attribute = Attribute.GetCustomAttribute(property, typeof(Attribute))
                        as Attribute;
                    //JsonProperty非DB欄位，不加入SQL Command
                    if (property.GetValue(t) != null)
                    {
                        var attribute1 = Attribute.GetCustomAttribute(property, typeof(JsonPropertyAttribute))
                       as JsonPropertyAttribute;
                        if (attribute1 == null && !string.IsNullOrEmpty(property.GetValue(t).ToString()))
                        {
                            tmpStr = property.GetValue(t) == null ? null : property.GetValue(t).ToString();
                            WhereQuery.Append($" AND {property.Name} = '{tmpStr}'");
                        }
                    }
                }
                //加上分頁功能
                foreach (PropertyInfo property in lstPropertys)
                {
                    var attribute1 = Attribute.GetCustomAttribute(property, typeof(JsonPropertyAttribute))
                       as JsonPropertyAttribute;
                    if (attribute1 != null)
                    {
                        tmpStr = property.GetValue(t) == null ? null : property.GetValue(t).ToString();
                        if (tmpStr != null)
                        {
                            if (property.Name.ToUpper() == "PAGESIZE")
                            {
                                WhereQuery.Append($" LIMIT {tmpStr}");
                            }
                            if (property.Name.ToUpper() == "STARTIDX")
                            {
                                WhereQuery.Append($" OFFSET {tmpStr}");
                            }
                        }
                    }
                }
                strSQL += WhereQuery.ToString();
            }
            catch (Exception ex)
            {
                //log.Error(ex);
                throw ex;
            }
            return strSQL;
        }
        public string getDataListSQLLike(T t)
        {
            string strSQL = $"SELECT * FROM {t.GetType().Name} WHERE 1=1 ";
            try
            {

                var WhereQuery = new StringBuilder();
                string tmpStr = string.Empty;
                IEnumerable<PropertyInfo> lstPropertys = GetProperties;
                if (lstPropertys.Count() == 0)
                {
                    lstPropertys = t.GetType().GetProperties().ToList();
                }
                foreach (PropertyInfo property in lstPropertys)
                {
                    var attribute = Attribute.GetCustomAttribute(property, typeof(Attribute))
                        as Attribute;
                    //JsonProperty非DB欄位，不加入SQL Command
                    if (property.GetValue(t) != null)
                    {
                        var attribute1 = Attribute.GetCustomAttribute(property, typeof(JsonPropertyAttribute))
                       as JsonPropertyAttribute;
                        if (attribute1 == null && !string.IsNullOrEmpty(property.GetValue(t).ToString()))
                        {
                            tmpStr = property.GetValue(t) == null ? null : property.GetValue(t).ToString();
                            WhereQuery.Append($" AND {property.Name} LIKE '%{tmpStr}%'");
                        }
                    }
                }
                //加上分頁功能
                foreach (PropertyInfo property in lstPropertys)
                {
                    var attribute1 = Attribute.GetCustomAttribute(property, typeof(JsonPropertyAttribute))
                       as JsonPropertyAttribute;
                    if (attribute1 != null)
                    {
                        tmpStr = property.GetValue(t) == null ? null : property.GetValue(t).ToString();
                        if (tmpStr != null)
                        {
                            if (property.Name.ToUpper() == "PAGESIZE")
                            {
                                WhereQuery.Append($" LIMIT {tmpStr}");
                            }
                            if (property.Name.ToUpper() == "STARTIDX")
                            {
                                WhereQuery.Append($" OFFSET {tmpStr}");
                            }
                        }
                    }
                }
                strSQL += WhereQuery.ToString();
            }
            catch (Exception ex)
            {
                //log.Error(ex);
                throw ex;
            }
            return strSQL;
        }
        /// <summary>
        /// 依據傳入的PropertyInfo List轉換為 string list
        /// </summary>
        /// <param name="listOfProperties">傳入的PropertyInfo List</param>
        /// <returns>轉換過後的 string list</returns>
        private static List<string> GenerateListOfProperties(IEnumerable<PropertyInfo> listOfProperties)
        {
            return (from prop in listOfProperties
                    let attributes = prop.GetCustomAttributes(typeof(DescriptionAttribute), false)
                    where attributes.Length <= 0 || (attributes[0] as DescriptionAttribute)?.Description != "ignore"
                    select prop.Name).ToList();
        }
        /// <summary>
        /// 傳入泛型類別，並依據該類別物件的成員，產生INSERT的SQL COMMAND
        /// </summary>
        /// <param name="t">傳入的泛型類別</param>
        /// <returns>INSERT SQL COMMAND</returns>
        public string GenerateInsertQuery(T t)
        {
            var obList = queryListBySql(getDataListSQL(t), Constant.MYSQL_DB_TYPE, Constant.TXGSMART_DB_CONN_STR);
            if (obList.Count > 0)
            {
                return "EXIST";
            }
            var insertQuery = new StringBuilder($"INSERT INTO {t.GetType().Name} ");
            insertQuery.Append("(");
            var properties = GenerateListOfProperties(GetProperties);
            if (properties.Count == 0)
            {
                var _properties = t.GetType().GetProperties();
                _properties.ToList().ForEach(
                    prop => {
                        var attribute1 = Attribute.GetCustomAttribute(prop, typeof(JsonPropertyAttribute))
                           as JsonPropertyAttribute;
                        if (prop.GetValue(t) != null)
                        {
                            if (attribute1 == null)
                            {
                                insertQuery.Append($"{prop.Name},");
                            }
                        }
                    }
                );
                insertQuery
                .Remove(insertQuery.Length - 1, 1)
                .Append(") VALUES (");
                _properties.ToList().ForEach(
                    prop => {
                        var attribute1 = Attribute.GetCustomAttribute(prop, typeof(JsonPropertyAttribute))
                           as JsonPropertyAttribute;
                        if (attribute1 == null)
                        {
                            if (prop.PropertyType.Equals(typeof(DateTime)))
                            {
                                if (prop.GetValue(t) == null)
                                {
                                    //insertQuery.Append($"null,");
                                }
                                else
                                {
                                    string tmpStr = ((DateTime)prop.GetValue(t)).ToString("yyyy-MM-dd HH:mm:ss");
                                    insertQuery.Append($"'{tmpStr.Replace("'", "''")}',");
                                }
                            }
                            else
                            {
                                if (prop.GetValue(t) == null)
                                {
                                    //insertQuery.Append($"null,");
                                }
                                else
                                {
                                    if (Numerics.IsNumericType(prop.GetValue(t)))
                                    {
                                        insertQuery.Append($"{prop.GetValue(t)},");
                                    }
                                    else
                                    {
                                        insertQuery.Append($"'{prop.GetValue(t).ToString().Replace("'", "''")}',");
                                    }
                                }
                            }
                        }
                    }
                );
            }
            else
            {
                GetProperties.ToList().ForEach(prop => {
                    var attribute1 = Attribute.GetCustomAttribute(prop, typeof(JsonPropertyAttribute))
                           as JsonPropertyAttribute;
                    if (attribute1 == null)
                    {
                        insertQuery.Append($"{prop.Name},");
                    }
                });
                insertQuery
                .Remove(insertQuery.Length - 1, 1)
                .Append(") VALUES (");
                GetProperties.ToList().ForEach(prop =>
                {
                    var attribute1 = Attribute.GetCustomAttribute(prop, typeof(JsonPropertyAttribute))
                           as JsonPropertyAttribute;
                    if (attribute1 == null)
                    {
                        if (prop.PropertyType.Equals(typeof(DateTime)))
                        {
                            if (prop.GetValue(t) == null)
                            {
                                insertQuery.Append($"null,");
                            }
                            else
                            {
                                string tmpStr = ((DateTime)prop.GetValue(t)).ToString("yyyy-MM-dd HH:mm:ss");
                                insertQuery.Append($"'{tmpStr.Replace("'", "''")}',");
                            }
                        }
                        else
                        {
                            if (prop.GetValue(t) == null)
                            {
                                insertQuery.Append($"null,");
                            }
                            else
                            {
                                if (Numerics.IsNumericType(prop.GetValue(t)))
                                {
                                    insertQuery.Append($"{prop.GetValue(t)},");
                                }
                                else
                                {
                                    insertQuery.Append($"'{prop.GetValue(t).ToString().Replace("'", "''")}',");
                                }
                            }
                        }
                    }
                });
            }
            insertQuery
                .Remove(insertQuery.Length - 1, 1)
                .Append(")");

            return insertQuery.ToString();
        }
        public string GenerateInsertQueryAny(T t)
        {
            var insertQuery = new StringBuilder($"INSERT INTO {t.GetType().Name} ");
            insertQuery.Append("(");
            var properties = GenerateListOfProperties(GetProperties);
            if (properties.Count == 0)
            {
                var _properties = t.GetType().GetProperties();
                _properties.ToList().ForEach(
                    prop => {
                        var attribute1 = Attribute.GetCustomAttribute(prop, typeof(JsonPropertyAttribute))
                           as JsonPropertyAttribute;
                        if (prop.GetValue(t) != null)
                        {
                            if (attribute1 == null)
                            {
                                insertQuery.Append($"{prop.Name},");
                            }
                        }
                    }
                );
                insertQuery
                .Remove(insertQuery.Length - 1, 1)
                .Append(") VALUES (");
                _properties.ToList().ForEach(
                    prop => {
                        var attribute1 = Attribute.GetCustomAttribute(prop, typeof(JsonPropertyAttribute))
                           as JsonPropertyAttribute;
                        if (attribute1 == null)
                        {
                            if (prop.PropertyType.Equals(typeof(DateTime)))
                            {
                                if (prop.GetValue(t) == null)
                                {
                                    //insertQuery.Append($"null,");
                                }
                                else
                                {
                                    string tmpStr = ((DateTime)prop.GetValue(t)).ToString("yyyy-MM-dd HH:mm:ss");
                                    insertQuery.Append($"'{tmpStr.Replace("'", "''")}',");
                                }
                            }
                            else
                            {
                                if (prop.GetValue(t) == null)
                                {
                                    //insertQuery.Append($"null,");
                                }
                                else
                                {
                                    if (Numerics.IsNumericType(prop.GetValue(t)))
                                    {
                                        insertQuery.Append($"{prop.GetValue(t)},");
                                    }
                                    else
                                    {
                                        insertQuery.Append($"'{prop.GetValue(t).ToString().Replace("'", "''")}',");
                                    }
                                }
                            }
                        }
                    }
                );
            }
            else
            {
                GetProperties.ToList().ForEach(prop => {
                    var attribute1 = Attribute.GetCustomAttribute(prop, typeof(JsonPropertyAttribute))
                           as JsonPropertyAttribute;
                    if (attribute1 == null)
                    {
                        insertQuery.Append($"{prop.Name},");
                    }
                });
                insertQuery
                .Remove(insertQuery.Length - 1, 1)
                .Append(") VALUES (");
                GetProperties.ToList().ForEach(prop =>
                {
                    var attribute1 = Attribute.GetCustomAttribute(prop, typeof(JsonPropertyAttribute))
                           as JsonPropertyAttribute;
                    if (attribute1 == null)
                    {
                        if (prop.PropertyType.Equals(typeof(DateTime)))
                        {
                            if (prop.GetValue(t) == null)
                            {
                                insertQuery.Append($"null,");
                            }
                            else
                            {
                                string tmpStr = ((DateTime)prop.GetValue(t)).ToString("yyyy-MM-dd HH:mm:ss");
                                insertQuery.Append($"'{tmpStr.Replace("'", "''")}',");
                            }
                        }
                        else
                        {
                            if (prop.GetValue(t) == null)
                            {
                                insertQuery.Append($"null,");
                            }
                            else
                            {
                                if (Numerics.IsNumericType(prop.GetValue(t)))
                                {
                                    insertQuery.Append($"{prop.GetValue(t)},");
                                }
                                else
                                {
                                    insertQuery.Append($"'{prop.GetValue(t).ToString().Replace("'", "''")}',");
                                }
                            }
                        }
                    }
                });
            }
            insertQuery
                .Remove(insertQuery.Length - 1, 1)
                .Append(")");

            return insertQuery.ToString().Replace("上午 ", "").Replace("下午 ", "");
        }
        /// <summary>
        /// 傳入泛型類別，並依據該類別物件的成員，產生UPDATE的SQL COMMAND
        /// </summary>
        /// <param name="t">傳入的泛型類別</param>
        /// <returns>UPDATE SQL COMMAND</returns>
        public string GenerateUpdateQuery(T t, string updateField = "")
        {
            var WhereQuery = new StringBuilder($" where ");
            var updateQuery = new StringBuilder($"UPDATE {t.GetType().Name} SET ");
            string tmpStr = string.Empty;
            var props = GetProperties;
            if (props.ToList().Count() == 0)
            {
                props = t.GetType().GetProperties();
            }
            foreach (PropertyInfo property in props)
            {
                var attribute = Attribute.GetCustomAttribute(property, typeof(KeyAttribute))
                    as KeyAttribute;

                if (attribute != null) // This property has a KeyAttribute
                {
                    #region 主鍵處理
                    if (property.GetValue(t) != null)
                    {
                        if (property.PropertyType.Equals(typeof(DateTime)))
                        {
                            if (property.GetValue(t) == null)
                            {
                                tmpStr = "null";
                            }
                            else
                            {
                                tmpStr = ((DateTime)property.GetValue(t)).ToString("yyyy-MM-dd HH:mm:ss");
                            }
                        }
                        else
                        {
                            tmpStr = property.GetValue(t).ToString();
                        }
                        if (Numerics.IsNumericType(property.GetValue(t)))
                        {
                            WhereQuery.Append($"{property.Name}={tmpStr} and ");
                        }
                        else
                        {
                            WhereQuery.Append($"{property.Name}='{tmpStr}' and ");
                        }
                    }
                    #endregion
                }
                else
                {
                    #region 欄位處理
                    if (!string.IsNullOrEmpty(updateField) && updateField.ToUpper().IndexOf(property.Name.ToUpper()) == -1)
                    {
                        continue;
                    }
                    if (property.GetValue(t) != null)
                    {
                        var attribute1 = Attribute.GetCustomAttribute(property, typeof(JsonPropertyAttribute))
                            as JsonPropertyAttribute;
                        if (attribute1 == null)
                        {
                            if (property.PropertyType.Equals(typeof(DateTime)))
                            {
                                if (property.GetValue(t) == null)
                                {
                                    updateQuery.Append($"null,");
                                }
                                else
                                {
                                    string dateTimeStr = ((DateTime)property.GetValue(t)).ToString("yyyy-MM-dd HH:mm:ss");
                                    updateQuery.Append($"{property.Name}='{dateTimeStr.Replace("'", "''")}',");
                                }
                            }
                            else
                            {
                                if (Numerics.IsNumericType(property.GetValue(t)))
                                {
                                    updateQuery.Append($"{property.Name}={property.GetValue(t).ToString()},");
                                }
                                else
                                {
                                    updateQuery.Append($"{property.Name}='{property.GetValue(t).ToString().Replace("'", "''")}',");
                                }

                            }
                            //updateQuery.Append($"{property.Name}=trim('{property.GetValue(t).ToString()}'),");
                        }
                    }
                    else
                    {
                        var attribute1 = Attribute.GetCustomAttribute(property, typeof(JsonPropertyAttribute))
                           as JsonPropertyAttribute;
                        if (attribute1 == null)
                        {
                            updateQuery.Append($"{property.Name}=null,");
                        }
                    }
                    #endregion
                }
            }
            updateQuery.Remove(updateQuery.Length - 1, 1); //remove last comma
            WhereQuery.Remove(WhereQuery.Length - 4, 4); //remove last comma
            updateQuery.Append(WhereQuery.ToString());
            
            return updateQuery.ToString().Replace("上午 ", "").Replace("下午 ", "");
        }
        public string GenerateUpdateQuery(T t, DBType dBType, string updateField = "")
        {
            var WhereQuery = new StringBuilder($" where ");
            var updateQuery = new StringBuilder($"UPDATE {t.GetType().Name} SET ");
            string tmpStr = string.Empty;
            var props = GetProperties;
            if (props.ToList().Count() == 0)
            {
                props = t.GetType().GetProperties();
            }
            foreach (PropertyInfo property in props)
            {
                var attribute = Attribute.GetCustomAttribute(property, typeof(KeyAttribute))
                    as KeyAttribute;

                if (attribute != null) // This property has a KeyAttribute
                {
                    #region 主鍵處理
                    if (property.GetValue(t) != null)
                    {
                        if (property.PropertyType.Equals(typeof(DateTime)))
                        {
                            if (property.GetValue(t) == null)
                            {
                                tmpStr = "null";
                            }
                            else
                            {
                                tmpStr = ((DateTime)property.GetValue(t)).ToString("yyyy-MM-dd HH:mm:ss");
                            }
                        }
                        else
                        {
                            tmpStr = property.GetValue(t).ToString();
                        }
                        if (dBType == DBType.MySql)
                        {
                            if (Numerics.IsNumericType(property.GetValue(t)))
                            {
                                WhereQuery.Append($"{property.Name}={tmpStr} and ");
                            }
                            else
                            {
                                WhereQuery.Append($"{property.Name}='{tmpStr}' and ");
                            }
                        }
                        if (dBType == DBType.MsSql)
                        {
                            if (Numerics.IsNumericType(property.GetValue(t)))
                            {
                                WhereQuery.Append($"{property.Name}={tmpStr} and ");
                            }
                            else
                            {
                                WhereQuery.Append($"{property.Name}='{tmpStr}' and ");
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    #region 欄位處理
                    if (!string.IsNullOrEmpty(updateField) && updateField.ToUpper().IndexOf(property.Name.ToUpper()) == -1)
                    {
                        continue;
                    }
                    if (property.GetValue(t) != null)
                    {
                        var attribute1 = Attribute.GetCustomAttribute(property, typeof(JsonPropertyAttribute))
                            as JsonPropertyAttribute;
                        if (attribute1 == null)
                        {
                            if (property.PropertyType.Equals(typeof(DateTime)))
                            {
                                if (property.GetValue(t) == null)
                                {
                                    updateQuery.Append($"null,");
                                }
                                else
                                {
                                    if (dBType == DBType.MySql)
                                    {
                                        string dateTimeStr = ((DateTime)property.GetValue(t)).ToString("yyyy-MM-dd HH:mm:ss");
                                        updateQuery.Append($"{property.Name}='{dateTimeStr.Replace("'", "''")}',");
                                    }
                                    if (dBType == DBType.MsSql)
                                    {
                                        string dateTimeStr = ((DateTime)property.GetValue(t)).ToString("yyyy-MM-dd HH:mm:ss");
                                        updateQuery.Append($"{property.Name}='{dateTimeStr.Replace("'", "''")}',");
                                    }
                                }
                            }
                            else
                            {
                                if (Numerics.IsNumericType(property.GetValue(t)))
                                {
                                    updateQuery.Append($"{property.Name}={property.GetValue(t).ToString()},");
                                }
                                else
                                {
                                    if (dBType == DBType.MySql)
                                    {
                                        updateQuery.Append($"{property.Name}='{property.GetValue(t).ToString().Replace("'", "''")}',");
                                    }
                                    if (dBType == DBType.MsSql)
                                    {
                                        updateQuery.Append($"{property.Name}='{property.GetValue(t).ToString().Replace("'", "''")}',");
                                    }
                                }

                            }
                            //updateQuery.Append($"{property.Name}=trim('{property.GetValue(t).ToString()}'),");
                        }
                    }
                    else
                    {
                        var attribute1 = Attribute.GetCustomAttribute(property, typeof(JsonPropertyAttribute))
                           as JsonPropertyAttribute;
                        if (attribute1 == null)
                        {
                            updateQuery.Append($"{property.Name}=null,");
                        }
                    }
                    #endregion
                }
            }
            updateQuery.Remove(updateQuery.Length - 1, 1); //remove last comma
            WhereQuery.Remove(WhereQuery.Length - 4, 4); //remove last comma
            updateQuery.Append(WhereQuery.ToString());
            return updateQuery.ToString().Replace("上午 ", "").Replace("下午 ", "");
        }
        /// <summary>
        /// 傳入泛型類別，並依據該類別物件的成員，產生DELETE的SQL COMMAND
        /// </summary>
        /// <param name="t">傳入的泛型類別</param>
        /// <returns>DELETE SQL COMMAND</returns>
        public string GenDeleteSQL(T t)
        {
            var WhereQuery = new StringBuilder();
            string tmpStr = string.Empty;
            var props = GetProperties;
            if (props.ToList().Count() == 0)
            {
                props = t.GetType().GetProperties();
            }
            foreach (PropertyInfo property in props)
            {
                var attribute = Attribute.GetCustomAttribute(property, typeof(KeyAttribute))
                    as KeyAttribute;

                if (attribute != null && property.GetValue(t) != null) // This property has a KeyAttribute
                {
                    tmpStr = property.GetValue(t).ToString();
                    if (Numerics.IsNumericType(property.GetValue(t)))
                    {
                        WhereQuery.Append($"{property.Name}={tmpStr} and ");
                    }
                    else
                    {
                        WhereQuery.Append($"{property.Name}='{tmpStr}' and ");
                    }
                }
            }
            if (WhereQuery.Length >= 4)
                WhereQuery.Remove(WhereQuery.Length - 4, 4);
            else
                WhereQuery.Append(" 1=1 ");
            string strSQL = $"DELETE FROM {t.GetType().Name} WHERE 1=1 AND " + WhereQuery.ToString();
            return strSQL;
        }
    }
}