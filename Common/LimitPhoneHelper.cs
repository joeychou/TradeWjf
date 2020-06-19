using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace Common
{
    public class LimitPhoneHelper
    {
        #region 字段属性
        private static object _syncObj = new object();
        private static LimitPhoneHelper _instance { get; set; }
        private static List<DateModel> cacheDateList { get; set; }
        private LimitPhoneHelper() { }
        /// <summary>
        /// 获得单例对象,使用懒汉式（双重锁定）
        /// </summary>
        /// <returns></returns>
        public static LimitPhoneHelper GetInstance()
        {
            if (_instance == null)
            {
                lock (_syncObj)
                {
                    if (_instance == null)
                    {
                        _instance = new LimitPhoneHelper();
                    }
                }
            }
            return _instance;
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetFileContent(string filePath)
        {
            string result = "";
            if (File.Exists(filePath))
            {
                result = File.ReadAllText(filePath);
            }
            return result;
        }
        /// <summary>
        /// 获取配置的Json文件
        /// </summary>
        /// <returns>经过反序列化之后的对象集合</returns>
        public static List<DateModel> GetConfigList()
        {
            string path = string.Format("{0}/xmlconfig/phoneConfig.json", System.AppDomain.CurrentDomain.BaseDirectory);
            string fileContent = GetFileContent(path);
            if (!string.IsNullOrWhiteSpace(fileContent))
            {
                cacheDateList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DateModel>>(fileContent);
            }
            return cacheDateList;
        }
        /// <summary>
        /// 获取配置的Json文件
        /// </summary>
        /// <returns>经过反序列化之后的对象集合</returns>
        public static string GetWeakPwdList()
        {
            string path = string.Format("{0}/xmlconfig/weakpwd.json", System.AppDomain.CurrentDomain.BaseDirectory);
            return GetFileContent(path);
        }
        /// <summary>
        /// 获取指定手机对应数据
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static DateModel GetConfigDataByPhone(string phone)
        {
            if (cacheDateList == null)//取配置数据
                GetConfigList();
            DateModel result = cacheDateList.FirstOrDefault(m => m.user_phone == phone);
            return result;
        }
        /// <summary>
        /// 检查是否存在该手机号码
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static bool IsContains(string phone)
        {
            string path = string.Format("{0}/xmlconfig/phoneConfig.json", System.AppDomain.CurrentDomain.BaseDirectory);
            string fileContent = GetFileContent(path);
            if (fileContent.Contains(phone))
            {
                return true;
            }
            return false;
        }
        #endregion

    }
    public class DateModel
    {
        public string user_name { get; set; }
        public string user_phone { get; set; }
    }
}