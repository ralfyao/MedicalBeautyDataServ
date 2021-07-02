using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

namespace MedicalBeautyDataServ.Models
{
    public class MedTreatDetail
    {
        private DateTime? anesthesiaTime = null;
        public Guid? MedTreatmentId { get; set; }
        public string MedTreatClass { get; set; }
        public string MedName { get; set; }
        public string MedPosition { get; set; }
        public double? MedQuantity { get; set; }
        public string MedUnit { get; set; }
        public DateTime? AnesthesiaTime { get { if (anesthesiaTime != null) { return DateTime.Parse(((DateTime)anesthesiaTime).ToString("yyyy-MM-dd'T'HH:mm")); } else { return null; } } set { this.anesthesiaTime = value; } }
        public int? AnesthesiaCount { get; set; }
        public string Anesthesiaer { get; set; }
        [Key]
        public Guid? id { get; set; }

        public static void Insert(MedTreatDetail treatmentDetail)
        {
            try
            {
                DBUtility<MedTreatDetail> dB = new DBUtility<MedTreatDetail>();
                string strSQL = dB.GenerateInsertQueryAny(treatmentDetail);
                if (treatmentDetail.id == null)
                    treatmentDetail.id = Guid.NewGuid();
                dB.executeBySql(strSQL, Constant.MYSQL_DB_TYPE, Account.ConnStr);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void Delete(Guid? medTreatmentID)
        {
            try
            {
                DBUtility<MedTreatDetail> dB = new DBUtility<MedTreatDetail>();
                MedTreatDetail treatDetail = new MedTreatDetail();
                string strSQL = dB.GenDeleteSQL(treatDetail) + " AND MedTreatmentId='" + medTreatmentID + "'";
                dB.executeBySql(strSQL, Constant.MYSQL_DB_TYPE, Account.ConnStr);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static MedTreatDetail getUnique(string id, SqlConnection conn = null, SqlTransaction tran = null)
        {
            MedTreatDetail ret = new MedTreatDetail();
            try
            {
                ret.id = Guid.Parse(id);
                DBUtility<MedTreatDetail> dB = new DBUtility<MedTreatDetail>();
                string strSQL = dB.getDataListSQL(ret);
                List<MedTreatDetail> list = new List<MedTreatDetail>();
                if (conn != null && tran != null)
                    list = dB.queryListBySql(ref conn, ref tran, strSQL, Constant.MYSQL_DB_TYPE, Account.ConnStr);
                else
                    list = dB.queryListBySql(strSQL, Constant.MYSQL_DB_TYPE, Account.ConnStr);
                if (list.Count > 0)
                {
                    ret = list[0];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ret;
        }

        public static void UpdateAnesthesiaTimeAndCount(MedTreatDetail detail, SqlConnection conn = null, SqlTransaction tran = null)
        {
            string strSQL = string.Empty;
            try
            {
                DBUtility<MedTreatDetail> dB = new DBUtility<MedTreatDetail>();
                //MedTreatDetail treatDetail = new MedTreatDetail();
                strSQL = string.Format("UPDATE MedTreatDetail SET AnesthesiaTime='{0}', AnesthesiaCount={2} WHERE id='{1}'", (detail.AnesthesiaTime != null ? ((DateTime)detail.AnesthesiaTime).ToString("yyyy-MM-dd HH:mm:ss") : "null"), detail.id, (detail.AnesthesiaCount == null ? "null": detail.AnesthesiaCount.ToString()));
                Log.Debug(strSQL);
                if (conn != null && tran != null)
                    dB.executeBySql(ref conn, ref tran, strSQL, Constant.MYSQL_DB_TYPE, Account.ConnStr);
                else
                    dB.executeBySql(strSQL, Constant.MYSQL_DB_TYPE, Account.ConnStr);
            }
            catch (Exception ex)
            {
                ex.Data["SQL"] = strSQL;
                throw ex;
            }
        }

        public static void Update(MedTreatDetail detail)
        {
            string strSQL = string.Empty;
            try
            {
                DBUtility<MedTreatDetail> dB = new DBUtility<MedTreatDetail>();
                //MedTreatDetail treatDetail = new MedTreatDetail();
                strSQL = dB.GenerateUpdateQuery(detail);
                Log.Debug(strSQL);
                dB.executeBySql(strSQL, Constant.MYSQL_DB_TYPE, Account.ConnStr);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}