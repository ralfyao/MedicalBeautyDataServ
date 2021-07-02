using ExcelDataReader;
using log4net;
using log4net.Config;
using MedicalBeautyDataServ.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MedicalBeautyDataServ.Controllers
{
    public class CustManageController : Controller
    {
        private static ILog log = LogManager.GetLogger(typeof(CustManageController));
        // GET: CustManage
        public ActionResult Index(string name, string gender, string birthday, string tel, int page = 1)
        {
            XmlConfigurator.Configure(new System.IO.FileInfo("./log4net.config"));
            if (Session["Account"] == null || string.IsNullOrEmpty(((Account)Session["Account"]).AccountId.ToString()))
            {
                TempData["SessionExipred"] = "true";
                return RedirectToAction("Index", "Home", null);
            }
            Customer cust = new Customer();
            List<Customer> lst = new List<Customer>();
            try
            {
                cust.Name = name;
                if (!string.IsNullOrEmpty(gender))
                    cust.Gender = int.Parse(gender);
                cust.BirthDate = birthday;
                cust.Tel = tel;
                lst = Customer.getList(cust);
                //if (!string.IsNullOrEmpty(name))
                //{
                //    cust.Name = name;
                //    lst = Customer.getCustomerByName(name);
                //}
                //else if (!string.IsNullOrEmpty(gender))
                //{
                //    cust.Gender = gender == "男" ? 1 :2;
                //}
                //else
                //{
                //    lst = Customer.getAllCustomer();
                //}
                //if (lst != null && lst.Count > 0)
                //{
                cust.custList = lst.OrderBy(p => p.id).ToPagedList(page, 10); ;
                //}
                TempData["menuId"] = "custMan";
            }
            catch (Exception ex)
            {
                log.Error(ex + ex.StackTrace);
                throw ex;
            }
            return View(cust);
        }
        public ActionResult UploadCustList(HttpPostedFileBase file_input_list)
        {
            XmlConfigurator.Configure(new System.IO.FileInfo("./log4net.config"));
            if (Session["Account"] == null || string.IsNullOrEmpty(((Account)Session["Account"]).AccountId.ToString()))
            {
                TempData["SessionExipred"] = "true";
                return RedirectToAction("Index", "Home", null);
            }
            DataSet result;
            DataRowCollection dataRow;
            DataColumnCollection dataColumn;
            int colIndex = 0;
            int rowIndex = 0;
            try
            {
                if (file_input_list == null || file_input_list.ContentLength == 0)
                {
                    TempData["ErrMessage"] = "請先上傳檔案!";
                    return RedirectToAction("Index", "CustManage", null);
                }
                if (file_input_list.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file_input_list.FileName);
                    //檔案明加上時間戳記
                    fileName = fileName.Split('.')[0] + DateTime.Now.ToString("yyyyMMddHHmmss") + "." + fileName.Split('.')[1];
                    var path = Path.Combine(Server.MapPath("~/FileUploads"), fileName);
                    file_input_list.SaveAs(path);
                    //Dictionary<string, string> chkCaseNo = new Dictionary<string, string>();
                    using (FileStream fileStream = System.IO.File.Open(path, FileMode.Open, FileAccess.Read))
                    {
                        using (IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream))
                        {
                            result = excelReader.AsDataSet(new ExcelDataSetConfiguration()
                            {
                                UseColumnDataType = false,
                                ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                                {
                                    //設定讀取資料時是否忽略標題
                                    UseHeaderRow = false
                                }
                            });
                            //result.Tables[0].DefaultView.Sort = result.Tables[0].Columns[0].ColumnName;
                            dataRow = result.Tables[0].Rows;
                            dataColumn = result.Tables[0].Columns;
                            for (int i = 1; i < dataRow.Count; i++)
                            {
                                try
                                {
                                    colIndex = 0;
                                    string name = result.Tables[0].Rows[i][colIndex].ToString();
                                    colIndex++;
                                    int? gender = result.Tables[0].Rows[i][colIndex].ToString() == "男" ? 1 : 2;
                                    colIndex++;
                                    string tel = result.Tables[0].Rows[i][colIndex].ToString();
                                    colIndex++;
                                    string birthDate = result.Tables[0].Rows[i][colIndex].ToString();
                                    colIndex++;
                                    Customer cust = new Customer();
                                    cust.id = Guid.NewGuid();
                                    cust.Name = name;
                                    cust.Gender = gender;
                                    cust.Tel = tel;
                                    cust.BirthDate = birthDate;
                                    Customer.Insert(cust);
                                }
                                catch (Exception ex)
                                {
                                    TempData["ErrMessage"] = "檔案格式錯誤，在第" + (rowIndex + 1).ToString() + "列第" + (colIndex + 1).ToString() + "欄";
                                    return RedirectToAction("Index", "CustManage", null);
                                }
                            }
                        }
                    }
                    TempData["success"] = "OK";
                    TempData["parentreload"] = "OK";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrMessage"] = ex + ex.StackTrace;
                log.Error(ex+ex.StackTrace);
            }
            return RedirectToAction("Index", "CustManage");
        }
    }
}