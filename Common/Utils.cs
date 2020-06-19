using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using System.Configuration;
using System.Web;
using System.Security.Cryptography;
using System.Collections.Specialized;
using System.Collections;

namespace Common
{
    public class Utils
    {
        #region 系统版本
        /// <summary>
        /// 版本信息类
        /// </summary>
        public class VersionInfo
        {
            public int FileMajorPart
            {
                get { return 4; }
            }
            public int FileMinorPart
            {
                get { return 0; }
            }
            public int FileBuildPart
            {
                get { return 0; }
            }
            public string ProductName
            {
                get { return "DTcms"; }
            }
            public int ProductType
            {
                get { return 1; }
            }
        }
        public static string GetVersion()
        {
            return AppKeys.ASSEMBLY_VERSION;
        }
        #endregion

        #region MD5加密
        public static string MD5(string strcode)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.Default.GetBytes(strcode);
            byte[] md5data = md5.ComputeHash(data);
            md5.Clear();
            string str = "";
            for (int i = 0; i < md5data.Length; i++)
            {
                str += md5data[i].ToString("x").PadLeft(2, '0');

            }
            return str;
        }
        /// <summary>
        /// 对字符串或参数值进行MD5加密
        /// </summary>
        /// <param name="text">要加密的字符串或参数名称</param>
        /// <param name="charset">字符串编码格式</param>
        /// <param name="isArg">加密字符串类型　true:参数值 false:字符串</param>
        /// <returns></returns>
        public static string MD5(string text, string charset, bool isArg)
        {
            try
            {
                MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();
                if (isArg)
                {
                    NameValueCollection Collect = HttpUtility.ParseQueryString(HttpContext.Current.Request.Url.Query, Encoding.GetEncoding(charset));//使用Collect接收参数值
                    if (Collect[text] != null)
                    {
                        return BitConverter.ToString(MD5.ComputeHash(Encoding.GetEncoding(charset).GetBytes(Collect[text].ToString()))).Replace("-", "");
                    }
                }
                else
                {
                    return BitConverter.ToString(MD5.ComputeHash(Encoding.GetEncoding(charset).GetBytes(text))).Replace("-", "");
                }
            }
            catch { }
            return string.Empty;
        }
        #endregion

        #region 对象转换处理
        /// <summary>
        /// 判断对象是否为Int32类型的数字
        /// </summary>
        /// <param name="Expression"></param>
        /// <returns></returns>
        public static bool IsNumeric(object expression)
        {
            if (expression != null)
                return IsNumeric(expression.ToString());

            return false;

        }

        /// <summary>
        /// 判断对象是否为Int32类型的数字
        /// </summary>
        /// <param name="Expression"></param>
        /// <returns></returns>
        public static bool IsNumeric(string expression)
        {
            if (expression != null)
            {
                string str = expression;
                if (str.Length > 0 && str.Length <= 11 && Regex.IsMatch(str, @"^[-]?[0-9]*[.]?[0-9]*$"))
                {
                    if ((str.Length < 10) || (str.Length == 10 && str[0] == '1') || (str.Length == 11 && str[0] == '-' && str[1] == '1'))
                        return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 截取字符串长度，超出部分使用后缀suffix代替，比如abcdevfddd取前3位，后面使用...代替
        /// </summary>
        /// <param name="orginStr"></param>
        /// <param name="length"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static string SubStrAddSuffix(string orginStr, int length, string suffix)
        {
            string ret = orginStr;
            if (orginStr.Length > length)
            {
                ret = orginStr.Substring(0, length) + suffix;
            }
            return ret;
        }
        /// <summary>
        /// 是否为Double类型
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static bool IsDouble(object expression)
        {
            if (expression != null)
                return Regex.IsMatch(expression.ToString(), @"^([0-9])[0-9]*(\.\w*)?$");

            return false;
        }

        /// <summary>
        /// 检测是否符合email格式
        /// </summary>
        /// <param name="strEmail">要判断的email字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsValidEmail(string strEmail)
        {
            return Regex.IsMatch(strEmail, @"^[\w\.]+([-]\w+)*@[A-Za-z0-9-_]+[\.][A-Za-z0-9-_]");
        }
        public static bool IsValidDoEmail(string strEmail)
        {
            return Regex.IsMatch(strEmail, @"^@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        /// <summary>
        /// 检测是否是正确的Url
        /// </summary>
        /// <param name="strUrl">要验证的Url</param>
        /// <returns>判断结果</returns>
        public static bool IsURL(string strUrl)
        {
            return Regex.IsMatch(strUrl, @"^(http|https)\://([a-zA-Z0-9\.\-]+(\:[a-zA-Z0-9\.&%\$\-]+)*@)*((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])|localhost|([a-zA-Z0-9\-]+\.)*[a-zA-Z0-9\-]+\.(com|edu|gov|int|mil|net|org|biz|arpa|info|name|pro|aero|coop|museum|[a-zA-Z]{1,10}))(\:[0-9]+)*(/($|[a-zA-Z0-9\.\,\?\'\\\+&%\$#\=~_\-]+))*$");
        }

        /// <summary>
        /// 将字符串转换为数组
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>字符串数组</returns>
        public static string[] GetStrArray(string str)
        {
            return str.Split(new char[',']);
        }

        /// <summary>
        /// 将数组转换为字符串
        /// </summary>
        /// <param name="list">List</param>
        /// <param name="speater">分隔符</param>
        /// <returns>String</returns>
        public static string GetArrayStr(List<string> list, string speater)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                if (i == list.Count - 1)
                {
                    sb.Append(list[i]);
                }
                else
                {
                    sb.Append(list[i]);
                    sb.Append(speater);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// object型转换为bool型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的bool类型结果</returns>
        public static bool StrToBool(object expression, bool defValue)
        {
            if (expression != null)
                return StrToBool(expression, defValue);

            return defValue;
        }

        /// <summary>
        /// string型转换为bool型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的bool类型结果</returns>
        public static bool StrToBool(string expression, bool defValue)
        {
            if (expression != null)
            {
                if (string.Compare(expression, "true", true) == 0)
                    return true;
                else if (string.Compare(expression, "false", true) == 0)
                    return false;
            }
            return defValue;
        }

        /// <summary>
        /// 将对象转换为Int32类型
        /// </summary>
        /// <param name="expression">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static int ObjToInt(object expression, int defValue)
        {
            if (expression != null)
                return StrToInt(expression.ToString(), defValue);

            return defValue;
        }

        /// <summary>
        /// 将字符串转换为Int32类型
        /// </summary>
        /// <param name="expression">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static int StrToInt(string expression, int defValue)
        {
            if (string.IsNullOrEmpty(expression) || expression.Trim().Length >= 16 || !Regex.IsMatch(expression.Trim(), @"^([-]|[0-9])[0-9]*(\.\w*)?$"))
                return defValue;

            int rv;
            if (Int32.TryParse(expression, out rv))
                return rv;

            return Convert.ToInt32(StrToFloat(expression, defValue));
        }

        /// <summary>
        /// Object型转换为decimal型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的decimal类型结果</returns>
        public static decimal ObjToDecimal(object expression, decimal defValue)
        {
            if (expression != null)
                return StrToDecimal(expression.ToString(), defValue);

            return defValue;
        }

        /// <summary>
        /// string型转换为decimal型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的decimal类型结果</returns>
        public static decimal StrToDecimal(string expression, decimal defValue)
        {
            if ((expression == null) || (expression.Length > 30))
                return defValue;

            decimal intValue = defValue;
            if (expression != null)
            {
                bool IsDecimal = Regex.IsMatch(expression, @"^([-]|[0-9])[0-9]*(\.\w*)?$");
                if (IsDecimal)
                    decimal.TryParse(expression, out intValue);
            }
            return intValue;
        }

        /// <summary>
        /// Object型转换为float型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static float ObjToFloat(object expression, float defValue)
        {
            if (expression != null)
                return StrToFloat(expression.ToString(), defValue);

            return defValue;
        }

        /// <summary>
        /// string型转换为float型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static float StrToFloat(string expression, float defValue)
        {
            if ((expression == null) || (expression.Length > 16))
                return defValue;

            float intValue = defValue;
            if (expression != null)
            {
                bool IsFloat = Regex.IsMatch(expression, @"^([-]|[0-9])[0-9]*(\.\w*)?$");
                if (IsFloat)
                    float.TryParse(expression, out intValue);
            }
            return intValue;
        }

        /// <summary>
        /// 将对象转换为日期时间类型
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static DateTime StrToDateTime(string str, DateTime defValue)
        {
            if (!string.IsNullOrEmpty(str))
            {
                DateTime dateTime;
                if (DateTime.TryParse(str, out dateTime))
                    return dateTime;
            }
            return defValue;
        }

        /// <summary>
        /// 将对象转换为日期时间类型
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <returns>转换后的int类型结果</returns>
        public static DateTime StrToDateTime(string str)
        {
            return StrToDateTime(str, DateTime.Now);
        }

        /// <summary>
        /// 将对象转换为日期时间类型
        /// </summary>
        /// <param name="obj">要转换的对象</param>
        /// <returns>转换后的int类型结果</returns>
        public static DateTime ObjectToDateTime(object obj)
        {
            return StrToDateTime(obj.ToString());
        }

        /// <summary>
        /// 将对象转换为日期时间类型
        /// </summary>
        /// <param name="obj">要转换的对象</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static DateTime ObjectToDateTime(object obj, DateTime defValue)
        {
            return StrToDateTime(obj.ToString(), defValue);
        }

        /// <summary>
        /// 将对象转换为字符串
        /// </summary>
        /// <param name="obj">要转换的对象</param>
        /// <returns>转换后的string类型结果</returns>
        public static string ObjectToStr(object obj)
        {
            if (obj == null)
                return "";
            return obj.ToString().Trim();
        }

        /// <summary>
        /// 将对象转换为Int类型
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static int ObjToInt(object obj)
        {
            if (isNumber(obj))
            {
                return int.Parse(obj.ToString());
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// 判断对象是否可以转成int型
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool isNumber(object o)
        {
            int tmpInt;
            if (o == null)
            {
                return false;
            }
            if (o.ToString().Trim().Length == 0)
            {
                return false;
            }
            if (!int.TryParse(o.ToString(), out tmpInt))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// 判断访问来自移动端还是PC端
        /// </summary>
        /// <returns></returns>
        public static bool isMobile()
        {
            string str_u = HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"];
            Regex b = new Regex(@"android.+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|symbian|treo|up\.(browser|link)|vodafone|wap|windows (ce|phone)|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Regex v = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(di|rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            if (!(b.IsMatch(str_u) || v.IsMatch(str_u.Substring(0, 4))))
            {
                return false;
            }
            else
            {
                //手机访问   
                return true;
            }
        }
        #endregion

        #region 数字保留两位小数 不四舍五入
        public static decimal MathRound2(decimal number)
        {
            number = Math.Floor(number * 100) / 100;//取三位小数,2.345
            return number;
        }
        /// <summary>
        /// 新浪股票接口
        /// </summary>
        /// <param name="stockcode">股票代码</param>
        public static string GetStockSinaData(string stockcode)
        {
            string stock_info = "", cyb = "", profix = "";

            if (stockcode.Trim().Length == 6)
            {
                cyb = Utils.subStr(stockcode, 0, 1);
                if (cyb == "6")
                {
                    profix = "sh";
                }
                else
                {
                    //沪深A股-深圳A、B股
                    profix = "sz";
                }
                try
                {
                    stock_info = GetHttpSinaData($"http://hq.sinajs.cn/list={profix}{stockcode}");
                    var reg = "var hq_str_(sh|sz){1}\\d{6}=\"(.+)\";";
                    if (Regex.IsMatch(stock_info, reg))
                    {
                        return Regex.Match(stock_info, reg).Groups[2].Value;
                    }
                    else
                    {
                        return "";
                    }
                }
                catch (Exception)
                {
                    return "";
                }
            }
            return stock_info;
        }
        #endregion

        #region 数字保留3位小数 不四舍五入
        public static decimal MathRound3(decimal number)
        {
            number = Math.Floor(number * 1000) / 1000;//取三位小数,2.345
            return number;
        }
        #endregion

        #region 数字保留4位小数 不四舍五入
        public static decimal MathRound4(decimal number)
        {
            number = Math.Floor(number * 10000) / 10000;//取三位小数,2.345
            return number;
        }
        #endregion

        #region 数字保留两位小数 四舍五入
        public static decimal MathRoundTwo(decimal number)
        {
            number = Math.Round(number, 2, MidpointRounding.AwayFromZero);
            return number;
        }
        public static decimal MathRoundThree(decimal number)
        {
            number = Math.Round(number, 3, MidpointRounding.AwayFromZero);
            return number;
        }
        #endregion

        #region 科学计数法转换
        public static decimal ChangeDataToD(string strData)
        {
            decimal dData = 0.0M;
            if (strData.Contains("E"))
            {
                dData = Convert.ToDecimal(decimal.Parse(strData.ToString(), System.Globalization.NumberStyles.Float));
            }
            return dData;
        }
        #endregion

        #region UTC时间和本地时间转换(北京时间)
        public static DateTime ConvertIntDatetime(double utc)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            startTime = startTime.AddSeconds(utc);
            startTime = startTime.AddHours(8);//转化为北京时间(北京时间=UTC时间+8小时 )            
            return startTime;
        }
        #endregion

        #region 产品盈亏处理
        /// <summary>
        /// 计算盈亏
        /// </summary>
        /// <param name="profit">盈亏情况</param>
        /// <param name="money">本金</param>
        /// <returns></returns>
        public static decimal CalRate(decimal profit, decimal money)
        {
            decimal rate = Utils.MathRound2((profit / money - 1) * 100);
            return rate;
        }
        #endregion

        #region 分割字符串
        /// <summary>
        /// 分割字符串
        /// </summary>
        public static string[] SplitString(string strContent, string strSplit)
        {
            if (!string.IsNullOrEmpty(strContent))
            {
                if (strContent.IndexOf(strSplit) < 0)
                    return new string[] { strContent };

                return Regex.Split(strContent, Regex.Escape(strSplit), RegexOptions.IgnoreCase);
            }
            else
                return new string[0] { };
        }

        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <returns></returns>
        public static string[] SplitString(string strContent, string strSplit, int count)
        {
            string[] result = new string[count];
            string[] splited = SplitString(strContent, strSplit);

            for (int i = 0; i < count; i++)
            {
                if (i < splited.Length)
                    result[i] = splited[i];
                else
                    result[i] = string.Empty;
            }

            return result;
        }
        #endregion

        #region 删除最后结尾的一个逗号
        /// <summary>
        /// 删除最后结尾的一个逗号
        /// </summary>
        public static string DelLastComma(string str)
        {
            if (str.Length < 1)
            {
                return "";
            }
            return str.Substring(0, str.LastIndexOf(","));
        }
        #endregion

        #region 删除最后结尾的指定字符后的字符
        /// <summary>
        /// 删除最后结尾的指定字符后的字符
        /// </summary>
        public static string DelLastChar(string str, string strchar)
        {
            if (string.IsNullOrEmpty(str))
                return "";
            if (str.LastIndexOf(strchar) >= 0 && str.LastIndexOf(strchar) == str.Length - 1)
            {
                return str.Substring(0, str.LastIndexOf(strchar));
            }
            return str;
        }
        #endregion

        #region 生成指定长度的字符串
        /// <summary>
        /// 生成指定长度的字符串,即生成strLong个str字符串
        /// </summary>
        /// <param name="strLong">生成的长度</param>
        /// <param name="str">以str生成字符串</param>
        /// <returns></returns>
        public static string StringOfChar(int strLong, string str)
        {
            string ReturnStr = "";
            for (int i = 0; i < strLong; i++)
            {
                ReturnStr += str;
            }

            return ReturnStr;
        }
        #endregion

        #region 生成日期随机码
        /// <summary>
        /// 生成日期随机码
        /// </summary>
        /// <returns></returns>
        public static string GetRamCode()
        {
            #region
            return DateTime.Now.ToString("yyyyMMddHHmmssffff");
            #endregion
        }
        #endregion

        #region 生成随机字母或数字
        /// <summary>
        /// 生成随机数字
        /// </summary>
        /// <param name="length">生成长度</param>
        /// <returns></returns>
        public static string Number(int Length)
        {
            return Number(Length, false);
        }

        /// <summary>
        /// 生成随机数字
        /// </summary>
        /// <param name="Length">生成长度</param>
        /// <param name="Sleep">是否要在生成前将当前线程阻止以避免重复</param>
        /// <returns></returns>
        public static string Number(int Length, bool Sleep)
        {
            if (Sleep)
                System.Threading.Thread.Sleep(3);
            string result = "";
            System.Random random = new Random();
            for (int i = 0; i < Length; i++)
            {
                result += random.Next(10).ToString();
            }
            return result;
        }
        /// <summary>
        /// 生成随机字母字符串(数字字母混和)
        /// </summary>
        /// <param name="codeCount">待生成的位数</param>
        public static string GetCheckCode(int codeCount)
        {
            string str = string.Empty;
            int rep = 0;
            long num2 = DateTime.Now.Ticks + rep;
            rep++;
            Random random = new Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> rep)));
            for (int i = 0; i < codeCount; i++)
            {
                char ch;
                int num = random.Next();
                if ((num % 2) == 0)
                {
                    ch = (char)(0x30 + ((ushort)(num % 10)));
                }
                else
                {
                    ch = (char)(0x41 + ((ushort)(num % 0x1a)));
                }
                str = str + ch.ToString();
            }
            return str;
        }
        /// <summary>
        /// 根据日期和随机码生成订单号
        /// </summary>
        /// <returns></returns>
        public static string GetOrderNumber()
        {
            string num = DateTime.Now.ToString("yyMMddHHmmss");//yyyyMMddHHmmssms
            return num + Number(2, true).ToString();
        }
        private static int Next(int numSeeds, int length)
        {
            byte[] buffer = new byte[length];
            System.Security.Cryptography.RNGCryptoServiceProvider Gen = new System.Security.Cryptography.RNGCryptoServiceProvider();
            Gen.GetBytes(buffer);
            uint randomResult = 0x0;//这里用uint作为生成的随机数  
            for (int i = 0; i < length; i++)
            {
                randomResult |= ((uint)buffer[i] << ((length - 1 - i) * 8));
            }
            return (int)(randomResult % numSeeds);
        }
        #endregion

        #region 截取字符长度
        /// <summary>
        /// 截取字符长度
        /// </summary>
        /// <param name="inputString">字符</param>
        /// <param name="len">长度</param>
        /// <returns></returns>
        public static string CutString(string inputString, int len)
        {
            if (string.IsNullOrEmpty(inputString))
                return "";
            inputString = DropHTML(inputString);
            ASCIIEncoding ascii = new ASCIIEncoding();
            int tempLen = 0;
            string tempString = "";
            byte[] s = ascii.GetBytes(inputString);
            for (int i = 0; i < s.Length; i++)
            {
                if ((int)s[i] == 63)
                {
                    tempLen += 2;
                }
                else
                {
                    tempLen += 1;
                }

                try
                {
                    tempString += inputString.Substring(i, 1);
                }
                catch
                {
                    break;
                }

                if (tempLen > len)
                    break;
            }
            //如果截过则加上半个省略号 
            byte[] mybyte = System.Text.Encoding.Default.GetBytes(inputString);
            if (mybyte.Length > len)
                tempString += "…";
            return tempString;
        }
        /// <summary>
        /// 截取字符长度
        /// </summary>
        /// <param name="inputString">字符</param>
        /// <param name="len">长度</param>
        /// <returns></returns>
        public static string CutStrLen(string inputString, int len)
        {
            if (string.IsNullOrEmpty(inputString))
            {
                return "";
            }
            if (len > inputString.Length)
            {
                return "";
            }
            return inputString.Substring(0, len);
        }

        /// <summary>
        /// 自定义截取字符串方法
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <param name="index">起始位</param>
        /// <param name="count">截取字节长度</param>
        /// <returns></returns>
        public static string subStr(string str, int index, int count)
        {
            string retStr = string.Empty;
            byte[] strs = System.Text.Encoding.Default.GetBytes(str);
            retStr = System.Text.Encoding.Default.GetString(strs, index, count);
            return retStr;
        }
        #endregion

        #region 清除HTML标记
        public static string DropHTML(string Htmlstring)
        {
            if (string.IsNullOrEmpty(Htmlstring)) return "";
            //删除脚本  
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML  
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            Htmlstring.Replace("<", "");
            Htmlstring.Replace(">", "");
            Htmlstring.Replace("\r\n", "");
            Htmlstring.Replace("&emsp;", "");
            Htmlstring = HttpContext.Current.Server.HtmlEncode(Htmlstring).Trim();
            return Htmlstring;
        }
        #endregion

        #region 清除HTML标记且返回相应的长度
        public static string DropHTML(string Htmlstring, int strLen)
        {
            return CutString(DropHTML(Htmlstring), strLen);
        }
        #endregion

        #region TXT代码转换成HTML格式
        /// <summary>
        /// 字符串字符处理
        /// </summary>
        /// <param name="chr">等待处理的字符串</param>
        /// <returns>处理后的字符串</returns>
        /// //把TXT代码转换成HTML格式
        public static String ToHtml(string Input)
        {
            StringBuilder sb = new StringBuilder(Input);
            sb.Replace("'", "&apos;");
            sb.Replace("&", "&amp;");
            sb.Replace("<", "&lt;");
            sb.Replace(">", "&gt;");
            sb.Replace("\r\n", "<br />");
            sb.Replace("\n", "<br />");
            sb.Replace("\t", " ");
            //sb.Replace(" ", "&nbsp;");
            return sb.ToString();
        }
        #endregion

        #region HTML代码转换成TXT格式
        /// <summary>
        /// 字符串字符处理
        /// </summary>
        /// <param name="chr">等待处理的字符串</param>
        /// <returns>处理后的字符串</returns>
        /// //把HTML代码转换成TXT格式
        public static String ToTxt(String Input)
        {
            StringBuilder sb = new StringBuilder(Input);
            sb.Replace("&nbsp;", " ");
            sb.Replace("<br>", "\r\n");
            sb.Replace("<br>", "\n");
            sb.Replace("<br />", "\n");
            sb.Replace("<br />", "\r\n");
            sb.Replace("&lt;", "<");
            sb.Replace("&gt;", ">");
            sb.Replace("&amp;", "&");
            return sb.ToString();
        }
        #endregion

        #region 检测是否有Sql危险字符
        /// <summary>
        /// 检测是否有Sql危险字符
        /// </summary>
        /// <param name="str">要判断字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsSafeSqlString(string str)
        {
            return !Regex.IsMatch(str, @"[-|;|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
        }

        /// <summary>
        /// 检查危险字符
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static string Filter(string sInput)
        {
            if (sInput == null || sInput == "")
                return null;
            string sInput1 = sInput.ToLower();
            string output = sInput;
            string pattern = @"*|and|exec|insert|select|delete|update|count|master|truncate|declare|char(|mid(|chr(|'";
            if (Regex.Match(sInput1, Regex.Escape(pattern), RegexOptions.Compiled | RegexOptions.IgnoreCase).Success)
            {
                throw new Exception("字符串中含有非法字符!");
            }
            else
            {
                output = output.Replace("'", "''");
            }
            return output;
        }

        /// <summary> 
        /// 检查过滤设定的危险字符
        /// </summary> 
        /// <param name="InText">要过滤的字符串 </param> 
        /// <returns>如果参数存在不安全字符，则返回true </returns> 
        public static bool SqlFilter(string word, string InText)
        {
            if (InText == null)
                return false;
            foreach (string i in word.Split('|'))
            {
                if ((InText.ToLower().IndexOf(i + " ") > -1) || (InText.ToLower().IndexOf(" " + i) > -1))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region 过滤特殊字符
        /// <summary>
        /// 过滤特殊字符
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static string Htmls(string Input)
        {
            if (Input != string.Empty && Input != null)
            {
                string ihtml = Input.ToLower();
                ihtml = ihtml.Replace("<script", "&lt;script");
                ihtml = ihtml.Replace("script>", "script&gt;");
                ihtml = ihtml.Replace("<%", "&lt;%");
                ihtml = ihtml.Replace("%>", "%&gt;");
                ihtml = ihtml.Replace("<$", "&lt;$");
                ihtml = ihtml.Replace("$>", "$&gt;");
                return ihtml;
            }
            else
            {
                return string.Empty;
            }
        }
        #endregion

        #region 获得配置文件节点XML文件的绝对路径
        public static string GetXmlMapPath(string xmlName)
        {
            return GetMapPath(ConfigurationManager.AppSettings[xmlName].ToString());
        }
        #endregion

        #region 获得当前绝对路径
        /// <summary>
        /// 获得当前绝对路径
        /// </summary>
        /// <param name="strPath">指定的路径</param>
        /// <returns>绝对路径</returns>
        public static string GetMapPath(string strPath)
        {
            if (strPath.ToLower().StartsWith("http://"))
            {
                return strPath;
            }
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Server.MapPath(strPath);
            }
            else //非web程序引用
            {
                strPath = strPath.Replace("/", "\\");
                if (strPath.StartsWith("\\"))
                {
                    strPath = strPath.Substring(strPath.IndexOf('\\', 1)).TrimStart('\\');
                }
                return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strPath);
            }
        }
        #endregion

        #region 文件操作
        /// <summary>
        /// 删除单个文件
        /// </summary>
        /// <param name="_filepath">文件相对路径</param>
        public static bool DeleteFile(string _filepath)
        {
            if (string.IsNullOrEmpty(_filepath))
            {
                return false;
            }
            string fullpath = GetMapPath(_filepath);
            if (File.Exists(fullpath))
            {
                File.Delete(fullpath);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 删除上传的文件(及缩略图)
        /// </summary>
        /// <param name="_filepath"></param>
        public static void DeleteUpFile(string _filepath)
        {
            string _path = _filepath.ToString().Replace("thumb_", "");
            if (string.IsNullOrEmpty(_path))
            {
                return;
            }
            string fullpath = GetMapPath(_path); //原图
            if (File.Exists(fullpath))
            {
                File.Delete(fullpath);
            }
            if (_path.LastIndexOf("/") >= 0)
            {
                string thumbnailpath = _path.Substring(0, _path.LastIndexOf("/")) + "/thumb_" + _path.Substring(_path.LastIndexOf("/") + 1);
                string fullTPATH = GetMapPath(thumbnailpath); //宿略图1
                if (File.Exists(fullTPATH))
                {
                    File.Delete(fullTPATH);
                }

                string thumbnailpath2 = _path.Substring(0, _path.LastIndexOf("/")) + "/thumb2_" + _path.Substring(_path.LastIndexOf("/") + 1);
                string fullTPATH2 = GetMapPath(thumbnailpath2); //宿略图2
                if (File.Exists(fullTPATH2))
                {
                    File.Delete(fullTPATH2);
                }
            }
        }
        /// <summary>
        /// 删除上传的文件(及缩略图)
        /// </summary>
        /// <param name="_filepath"></param>
        public static void DeleteUpFileList(string _filepath)
        {
            string[] str = _filepath.Split(',');
            if (str.Length > 0)
            {
                for (int i = 0; i < str.Length; i++)
                {
                    string _path = str[i].ToString().Replace("thumb_", "");
                    if (string.IsNullOrEmpty(_path))
                    {
                        return;
                    }
                    string fullpath = GetMapPath(_path); //原图
                    if (File.Exists(fullpath))
                    {
                        File.Delete(fullpath);
                    }
                    if (_path.LastIndexOf("/") >= 0)
                    {
                        string thumbnailpath = _path.Substring(0, _path.LastIndexOf("/")) + "/thumb_" + _path.Substring(_path.LastIndexOf("/") + 1);
                        string fullTPATH = GetMapPath(thumbnailpath); //宿略图1
                        if (File.Exists(fullTPATH))
                        {
                            File.Delete(fullTPATH);
                        }

                        string thumbnailpath2 = _path.Substring(0, _path.LastIndexOf("/")) + "/thumb2_" + _path.Substring(_path.LastIndexOf("/") + 1);
                        string fullTPATH2 = GetMapPath(thumbnailpath2); //宿略图2
                        if (File.Exists(fullTPATH2))
                        {
                            File.Delete(fullTPATH2);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 删除内容图片
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="startstr">匹配开头字符串</param>
        public static void DeleteContentPic(string content, string startstr)
        {
            if (string.IsNullOrEmpty(content))
            {
                return;
            }
            Regex reg = new Regex("IMG[^>]*?src\\s*=\\s*(?:\"(?<1>[^\"]*)\"|'(?<1>[^\']*)')", RegexOptions.IgnoreCase);
            MatchCollection m = reg.Matches(content);
            foreach (Match math in m)
            {
                string imgUrl = math.Groups[1].Value;
                string fullPath = GetMapPath(imgUrl);
                try
                {
                    if (imgUrl.ToLower().StartsWith(startstr.ToLower()) && File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// 删除指定文件夹
        /// </summary>
        /// <param name="_dirpath">文件相对路径</param>
        public static bool DeleteDirectory(string _dirpath)
        {
            if (string.IsNullOrEmpty(_dirpath))
            {
                return false;
            }
            string fullpath = GetMapPath(_dirpath);
            if (Directory.Exists(fullpath))
            {
                Directory.Delete(fullpath, true);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 修改指定文件夹名称
        /// </summary>
        /// <param name="old_dirpath">旧相对路径</param>
        /// <param name="new_dirpath">新相对路径</param>
        /// <returns>bool</returns>
        public static bool MoveDirectory(string old_dirpath, string new_dirpath)
        {
            if (string.IsNullOrEmpty(old_dirpath))
            {
                return false;
            }
            string fulloldpath = GetMapPath(old_dirpath);
            string fullnewpath = GetMapPath(new_dirpath);
            if (Directory.Exists(fulloldpath))
            {
                Directory.Move(fulloldpath, fullnewpath);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 返回文件大小KB
        /// </summary>
        /// <param name="_filepath">文件相对路径</param>
        /// <returns>int</returns>
        public static int GetFileSize(string _filepath)
        {
            if (string.IsNullOrEmpty(_filepath))
            {
                return 0;
            }
            string fullpath = GetMapPath(_filepath);
            if (File.Exists(fullpath))
            {
                FileInfo fileInfo = new FileInfo(fullpath);
                return ((int)fileInfo.Length) / 1024;
            }
            return 0;
        }

        /// <summary>
        /// 返回文件扩展名，不含“.”
        /// </summary>
        /// <param name="_filepath">文件全名称</param>
        /// <returns>string</returns>
        public static string GetFileExt(string _filepath)
        {
            if (string.IsNullOrEmpty(_filepath))
            {
                return "";
            }
            if (_filepath.LastIndexOf(".") > 0)
            {
                return _filepath.Substring(_filepath.LastIndexOf(".") + 1); //文件扩展名，不含“.”
            }
            return "";
        }

        /// <summary>
        /// 返回文件名，不含路径
        /// </summary>
        /// <param name="_filepath">文件相对路径</param>
        /// <returns>string</returns>
        public static string GetFileName(string _filepath)
        {
            return _filepath.Substring(_filepath.LastIndexOf(@"/") + 1);
        }

        /// <summary>
        /// 文件是否存在
        /// </summary>
        /// <param name="_filepath">文件相对路径</param>
        /// <returns>bool</returns>
        public static bool FileExists(string _filepath)
        {
            string fullpath = GetMapPath(_filepath);
            if (File.Exists(fullpath))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获得远程字符串
        /// </summary>
        public static string GetDomainStr(string key, string uriPath)
        {
            string result = CacheHelper.Get(key) as string;
            if (result == null)
            {
                System.Net.WebClient client = new System.Net.WebClient();
                try
                {
                    client.Encoding = System.Text.Encoding.UTF8;
                    result = client.DownloadString(uriPath);
                }
                catch
                {
                    result = "暂时无法连接!";
                }
                CacheHelper.Insert(key, result, 60);
            }

            return result;
        }

        #endregion

        #region 读取或写入cookie
        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="strValue">值</param>
        public static void WriteCookie(string strName, string strValue)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            cookie.Value = UrlEncode(strValue);
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="strValue">值</param>
        public static void WriteCookie(string strName, string key, string strValue)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            cookie[key] = UrlEncode(strValue);
            cookie.Path = "/";
            cookie.HttpOnly = true;
            //cookie.Domain = "kscp8.com";
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="strValue">值</param>
        public static void WriteCookie(string strName, string key, string strValue, int expires)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            cookie[key] = UrlEncode(strValue);
            cookie.Expires = DateTime.Now.AddMinutes(expires);
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="strValue">值</param>
        /// <param name="strValue">过期时间(分钟)</param>
        public static void WriteCookie(string strName, string strValue, int expires)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            cookie.Value = UrlEncode(strValue);
            cookie.Path = "/";
            cookie.HttpOnly = true;
            //cookie.Domain = "kscp8.com";
            cookie.Expires = DateTime.Now.AddMinutes(expires);
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// 读cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <returns>cookie值</returns>
        public static string GetCookie(string strName)
        {
            if (HttpContext.Current.Request.Cookies != null && HttpContext.Current.Request.Cookies[strName] != null)
                return UrlDecode(HttpContext.Current.Request.Cookies[strName].Value.ToString());
            return "";
        }

        /// <summary>
        /// 读cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <returns>cookie值</returns>
        public static string GetCookie(string strName, string key)
        {
            if (HttpContext.Current.Request.Cookies != null && HttpContext.Current.Request.Cookies[strName] != null && HttpContext.Current.Request.Cookies[strName][key] != null)
                return UrlDecode(HttpContext.Current.Request.Cookies[strName][key].ToString());

            return "";
        }
        /// <summary>
        /// 清除cookie
        /// </summary>
        /// <param name="strName"></param>
        /// <returns></returns>
        public static string ClearCookie(string strName)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-1);
                //cookie.Domain = "kscp8.com";
                cookie.Path = "/";
                cookie.Values.Clear();
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
            return "";
        }
        #endregion

        #region URL处理
        /// <summary>
        /// URL字符编码
        /// </summary>
        public static string UrlEncode(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }
            str = str.Replace("'", "");
            return HttpContext.Current.Server.UrlEncode(str);
        }

        /// <summary>
        /// URL字符解码
        /// </summary>
        public static string UrlDecode(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }
            return HttpContext.Current.Server.UrlDecode(str);
        }
        /// <summary>
        /// 组合URL参数
        /// </summary>
        /// <param name="_url">页面地址</param>
        /// <param name="_keys">参数名称</param>
        /// <param name="_values">参数值</param>
        /// <returns>String</returns>
        public static string CombUrlTxt(string _url, string _keys, params string[] _values)
        {
            StringBuilder urlParams = new StringBuilder();
            try
            {
                string[] keyArr = _keys.Split(new char[] { '&' });
                for (int i = 0; i < keyArr.Length; i++)
                {
                    if (!string.IsNullOrEmpty(_values[i]) && _values[i] != "0")
                    {
                        _values[i] = UrlEncode(_values[i]);
                        urlParams.Append(string.Format(keyArr[i], _values) + "&");
                    }
                }
                if (!string.IsNullOrEmpty(urlParams.ToString()) && _url.IndexOf("?") == -1)
                    urlParams.Insert(0, "?");
            }
            catch
            {
                return _url;
            }
            return _url + DelLastChar(urlParams.ToString(), "&");
        }
        #endregion

        #region 替换指定的字符串
        /// <summary>
        /// 替换指定的字符串
        /// </summary>
        /// <param name="originalStr">原字符串</param>
        /// <param name="oldStr">旧字符串</param>
        /// <param name="newStr">新字符串</param>
        /// <returns></returns>
        public static string ReplaceStr(string originalStr, string oldStr, string newStr)
        {
            if (string.IsNullOrEmpty(oldStr))
            {
                return "";
            }
            return originalStr.Replace(oldStr, newStr);
        }
        #endregion

        #region 操作权限菜单
        /// <summary>
        /// 获取操作权限
        /// </summary>
        /// <returns>Dictionary</returns>
        public static Dictionary<string, string> ActionType()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("Show", "显示");
            dic.Add("View", "查看");
            dic.Add("Add", "添加");
            dic.Add("Edit", "修改");
            dic.Add("Delete", "删除");
            dic.Add("Audit", "审核");
            dic.Add("Reply", "回复");
            dic.Add("Confirm", "确认");
            dic.Add("Cancel", "取消");
            return dic;
        }
        #endregion

        #region  手机数字验证码
        public static string smsCode()
        {
            string chkCode = "";
            //验证码的字符集，去掉了一些容易混淆的字符 
            char[] character = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            Random rnd = new Random();
            //生成验证码字符串 
            for (int i = 0; i < 4; i++)
            {
                chkCode += character[rnd.Next(character.Length)];
            }
            return chkCode;
        }
        #endregion

        #region 手机号码中间4位替换成 ****
        public static string rpMobile(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }
            if (str.Trim().Length > 10)
            {
                str = str.Insert(3, "****").Remove(7, 4);
            }
            return str;
        }
        #endregion

        #region 身份证中间10位替换成 ****
        public static string rpIDCARD(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }
            if (str.Length == 18)
            {
                str = str.Replace(str.Substring(4, 10), "********");
            }
            else if (str.Length == 16)
            {
                str = str.Replace(str.Substring(4, 8), "********");
            }
            else
            {

            }
            return str;
        }
        #endregion

        #region Unix时间戳转换为DateTime
        public static DateTime ConvertToDateTime(string timestamp)
        {
            DateTime time = DateTime.MinValue;
            //精确到毫秒
            //时间戳转成时间
            DateTime start = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            try
            {
                time = timestamp.Length == 10 ? start.AddSeconds(long.Parse(timestamp)) : start.AddMilliseconds(long.Parse(timestamp));
            }
            catch (Exception ex)
            {
                return start;//转换失败
            }
            return time;
        }
        #endregion

        #region DateTime转换为Unix时间戳
        public static string ConvertTimestamp(DateTime time)
        {
            double intResult = 0;
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            intResult = (time - startTime).TotalMilliseconds;
            return Math.Round(intResult, 0).ToString();
        }
        #endregion

        #region 判断DataSet是否为空
        public static bool DataSetIsNull(System.Data.DataSet ds)
        {
            return ds != null && ds.Tables != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0 ? false : true;
        }
        #endregion

        #region  生成指定大小的整数组级
        /// <summary>
        /// 生成一个非重复的随机序列。
        /// </summary>
        /// <param name="low">序列最小值。</param>
        /// <param name="high">序列最大值。</param>
        /// <returns>序列。</returns>
        public static string BuildRandomSequence(int low, int high)
        {
            Random random = new Random();
            int x = 0, tmp = 0;
            if (low > high)
            {
                tmp = low;
                low = high;
                high = tmp;
            }
            int[] array = new int[high - low + 1];
            for (int i = low; i <= high; i++)
            {
                array[i - low] = i;
            }
            //第i张与任意一张牌换位子，换完一轮即可
            for (int i = array.Length - 1; i > 0; i--)
            {
                x = random.Next(0, i + 1);
                tmp = array[i];
                array[i] = array[x];
                array[x] = tmp;
            }
            StringBuilder sb = new StringBuilder();
            if (array.Length > 0)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    sb.Append(array[i].ToString() + ",");
                }
            }
            return sb.ToString();
        }

        #endregion

        #region 获取远程页面返回内容
        public static string GetHttpSinaData(string Url)
        {
            string sException = null;
            string sRslt = null;
            System.Net.WebResponse oWebRps = null;
            System.Net.WebRequest oWebRqst = System.Net.WebRequest.Create(Url);
            oWebRqst.Timeout = 50000;
            try
            {
                oWebRps = oWebRqst.GetResponse();
            }
            catch (System.Net.WebException e)
            {
                sException = e.Message.ToString();
            }
            catch (Exception e)
            {
                sException = e.ToString();
            }
            finally
            {
                if (oWebRps != null)
                {
                    System.IO.StreamReader oStreamRd = new StreamReader(oWebRps.GetResponseStream(), System.Text.Encoding.GetEncoding("gb2312"));
                    sRslt = oStreamRd.ReadToEnd();
                    oStreamRd.Close();
                    oWebRps.Close();
                }
            }
            return sRslt;
        }
        /// <summary>
        /// 获取远程页面返回内容
        /// </summary>
        /// <param name="stock_url"></param>
        /// <returns></returns>
        public static string GetHttpData(string stock_url)
        {
            WebClient client = new WebClient();
            client.Headers.Add("Content-Type", "text/html;charset=UTF-8");
            Stream data = client.OpenRead(stock_url);
            StreamReader reader = new StreamReader(data, Encoding.GetEncoding("UTF-8"));
            string str = reader.ReadToEnd();
            reader.Close();
            data.Close();
            return str;
        }
        /// <summary>
        /// 获得字符串中开始和结束字符串中间得值
        /// </summary>
        /// <param name="str"></param>
        /// <param name="s"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string GetHttpValue(string str, string s, string e)
        {
            Regex rg = new Regex("(?<=(" + s + "))[.\\s\\S]*?(?=(" + e + "))", RegexOptions.Multiline | RegexOptions.Singleline);
            return rg.Match(str).Value;
        }
        /// <summary>
        /// 获得字符串中开始和结束字符串中间得值
        /// </summary>
        /// <param name="str"></param>
        /// <param name="s"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string GetStr(string TxtStr, string FirstStr, string SecondStr)
        {
            if (FirstStr.IndexOf(SecondStr, 0) != -1)
                return "";
            int FirstSite = TxtStr.IndexOf(FirstStr, 0);
            int SecondSite = TxtStr.IndexOf(SecondStr, FirstSite + 1);
            if (FirstSite == -1 || SecondSite == -1)
                return "";
            return TxtStr.Substring(FirstSite + FirstStr.Length, SecondSite - FirstSite - FirstStr.Length);
        }
        #endregion

        #region URL请求数据
        /// <summary>
        /// HTTP POST方式请求数据
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="param">POST的数据</param>
        /// <returns></returns>
        public static string HttpPost(string url, string param)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Timeout = 15000;
            request.AllowAutoRedirect = false;

            StreamWriter requestStream = null;
            WebResponse response = null;
            string responseStr = null;

            try
            {
                requestStream = new StreamWriter(request.GetRequestStream());
                requestStream.Write(param);
                requestStream.Close();

                response = request.GetResponse();
                if (response != null)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding("gbk"));
                    responseStr = reader.ReadToEnd();
                    reader.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                request = null;
                requestStream = null;
                response = null;
            }

            return responseStr;
        }

        /// <summary>
        /// HTTP GET方式请求数据.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <returns></returns>
        public static string HttpGet(string url)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "GET";
            //request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Timeout = 600000;
            request.AllowAutoRedirect = false;

            WebResponse response = null;
            string responseStr = null;

            try
            {
                response = request.GetResponse();

                if (response != null)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding("gbk"));
                    responseStr = reader.ReadToEnd();
                    reader.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                request = null;
                response = null;
            }

            return responseStr;
        }

        /// <summary>
        /// HTTP GET方式请求数据.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <returns></returns>
        public static string HttpGetUtf(string url)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "GET";
            //request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Timeout = 600000;
            request.AllowAutoRedirect = false;

            WebResponse response = null;
            string responseStr = null;

            try
            {
                response = request.GetResponse();

                if (response != null)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding("utf-8"));
                    responseStr = reader.ReadToEnd();
                    reader.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                request = null;
                response = null;
            }

            return responseStr;
        }

        /// <summary>
        /// 执行URL获取页面内容
        /// </summary>
        public static string UrlExecute(string urlPath)
        {
            if (string.IsNullOrEmpty(urlPath))
            {
                return "error";
            }
            StringWriter sw = new StringWriter();
            try
            {
                HttpContext.Current.Server.Execute(urlPath, sw);
                return sw.ToString();
            }
            catch (Exception)
            {
                return "error";
            }
            finally
            {
                sw.Close();
                sw.Dispose();
            }
        }

        /// <summary>
        /// 发送HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="param">请求的参数</param>
        /// <returns>请求结果</returns>
        public static string request(string url, string param)
        {
            string strURL = url + '?' + param;
            System.Net.HttpWebRequest request;
            request = (System.Net.HttpWebRequest)WebRequest.Create(strURL);
            request.Method = "GET";
            // 添加header
            request.Headers.Add("apikey", "78f655cc9c4d7a44067a4234c11fb703");
            using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse())
            {
                System.IO.Stream s;
                s = response.GetResponseStream();
                string StrDate = "";
                string strValue = "";
                StreamReader Reader = new StreamReader(s, Encoding.UTF8);
                while ((StrDate = Reader.ReadLine()) != null)
                {
                    strValue += StrDate + "\r\n";
                }
                return strValue;
            }

        }

        #endregion

        #region 程序执行时间测试
        /// <summary>
        /// 程序执行时间测试
        /// </summary>
        /// <param name="dateBegin">开始时间</param>
        /// <param name="dateEnd">结束时间</param>
        /// <returns>返回(秒)单位，比如: 0.00239秒</returns>
        public static string ExecDateDiff(DateTime dateBegin, DateTime dateEnd)
        {
            TimeSpan ts1 = new TimeSpan(dateBegin.Ticks);
            TimeSpan ts2 = new TimeSpan(dateEnd.Ticks);
            TimeSpan ts3 = ts1.Subtract(ts2).Duration();
            return ts3.TotalMilliseconds.ToString();
        }
        #endregion

        #region 返回相册列表HMTL=========================
        public static string GetAlbumHtml(int is_view, string picStr)
        {
            string[] pics = picStr.Length > 0 ? picStr.Split(',') : new string[0];
            StringBuilder strTxt = new StringBuilder();
            if (pics != null && pics.Length > 0)
            {
                for (int i = 0; i < pics.Length; i++)
                {
                    strTxt.Append("<li><input type=\"hidden\" name=\"hid_photo_name\" value=\"0|" + pics[i].Replace("thumb_", "") + "|" + pics[i] + "\">");
                    strTxt.Append("<div class=\"img-box selected\" onclick=\"setFocusImg(this);\"><img src=\"" + pics[i] + "\" bigsrc=\"" + pics[i].Replace("thumb_", "") + "\"></div>");
                    if (is_view > 0)
                    {
                        strTxt.Append("<a href=\"" + pics[i].Replace("thumb_", "") + "\" target=\"_blank\">原图</a>");
                    }
                    strTxt.Append("<a href=\"javascript:;\" onclick=\"delImg(this);\">删除</a></li>");
                }
            }
            return strTxt.ToString();
        }
        #endregion

        #region 时间相差天数
        public static int getDayOfDiff(DateTime dateStart, DateTime dateEnd)
        {
            DateTime start = Convert.ToDateTime(dateStart.ToShortDateString());
            DateTime end = Convert.ToDateTime(dateEnd.ToShortDateString());
            TimeSpan sp = end.Subtract(start);
            return sp.Days;
        }
        #endregion

        #region 时间相差月数
        public static int getMonthOfDiff(DateTime dateStart, DateTime dateEnd)
        {
            DateTime startDate = Convert.ToDateTime(dateStart.ToShortDateString());
            DateTime endDate = Convert.ToDateTime(dateEnd.ToShortDateString());
            int totalMonth = endDate.Year * 12 + endDate.Month - startDate.Year * 12 - startDate.Month;
            return totalMonth;
        }
        #endregion

        #region 年月日计算
        public static string getMonthTimeDiff(DateTime dateStart, DateTime dateEnd, int type)
        {
            string my = "";
            DateTime start = Convert.ToDateTime(dateStart.ToShortDateString()),
                end = Convert.ToDateTime(dateEnd.ToShortDateString());
            int mon = getMonthOfDiff(start, end), day = getDayOfDiff(start, end);
            if (mon > 0)
            {
                my += type == 1 ? "<b class=\"f30\">" + mon.ToString() + "</b>个月" : "<span>" + mon.ToString() + "个月</span>";
            }
            else
            {
                my += type == 1 ? "<b class=\"f30\">" + day.ToString() + "</b>天" : "<span>" + day.ToString() + "天</span>";
            }
            return my;
        }
        #endregion

        #region 判断是否是奇数
        public static bool IsOdd(int n)
        {
            return Convert.ToBoolean(n % 2);
        }
        #endregion

        #region 统计字符串字符出现个数
        public static int StrCount(string WithinString, string search)
        {
            if (string.IsNullOrEmpty(search))
                throw new ArgumentNullException("search");
            int counter = 0;
            int index = WithinString.IndexOf(search, 0);
            while (index >= 0 && index < WithinString.Length)
            {
                counter++;
                index = WithinString.IndexOf(search, index + search.Length);
            }
            return counter;
        }
        #endregion

        #region 数据库链接字符串解密
        /// <summary>
        /// 数据库链接字符串解密
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <returns></returns>
        public static string ConnDecrypt(string ConnectionString)
        {
            string[] pwdstr = ConnectionString.Split(';');
            if (pwdstr.Length >= 5)
            {
                string oldpwd = pwdstr[2].Replace("pwd=", ""), newpwd = DESEncrypt.Decrypt(oldpwd);
                ConnectionString = ConnectionString.Replace(oldpwd, newpwd);
            }
            return ConnectionString;
        }
        #endregion

        #region 获取汉子首字母
        /// <summary> 
        /// 在指定的字符串列表CnStr中检索符合拼音索引字符串 
        /// </summary> 
        /// <param name="CnStr">汉字字符串</param> 
        /// <returns>相对应的汉语拼音首字母串</returns> 
        public static string GetSpellCode(string CnStr)
        {
            string strTemp = "";
            int iLen = CnStr.Length;
            int i = 0;

            for (i = 0; i <= iLen - 1; i++)
            {
                strTemp += GetCharSpellCode(CnStr.Substring(i, 1));
            }

            return strTemp;
        }


        /// <summary> 
        /// 得到一个汉字的拼音第一个字母，如果是一个英文字母则直接返回大写字母 
        /// </summary> 
        /// <param name="CnChar">单个汉字</param> 
        /// <returns>单个大写字母</returns> 
        private static string GetCharSpellCode(string CnChar)
        {
            long iCnChar;

            byte[] ZW = System.Text.Encoding.Default.GetBytes(CnChar);

            //如果是字母，则直接返回 
            if (ZW.Length == 1)
            {
                return CnChar.ToUpper();
            }
            else
            {
                // get the array of byte from the single char 
                int i1 = (short)(ZW[0]);
                int i2 = (short)(ZW[1]);
                iCnChar = i1 * 256 + i2;
            }

            //expresstion 
            //table of the constant list 
            // 'A'; //45217..45252 
            // 'B'; //45253..45760 
            // 'C'; //45761..46317 
            // 'D'; //46318..46825 
            // 'E'; //46826..47009 
            // 'F'; //47010..47296 
            // 'G'; //47297..47613 

            // 'H'; //47614..48118 
            // 'J'; //48119..49061 
            // 'K'; //49062..49323 
            // 'L'; //49324..49895 
            // 'M'; //49896..50370 
            // 'N'; //50371..50613 
            // 'O'; //50614..50621 
            // 'P'; //50622..50905 
            // 'Q'; //50906..51386 

            // 'R'; //51387..51445 
            // 'S'; //51446..52217 
            // 'T'; //52218..52697 
            //没有U,V 
            // 'W'; //52698..52979 
            // 'X'; //52980..53640 
            // 'Y'; //53689..54480 
            // 'Z'; //54481..55289 

            // iCnChar match the constant 
            if ((iCnChar >= 45217) && (iCnChar <= 45252))
            {
                return "A";
            }
            else if ((iCnChar >= 45253) && (iCnChar <= 45760))
            {
                return "B";
            }
            else if ((iCnChar >= 45761) && (iCnChar <= 46317))
            {
                return "C";
            }
            else if ((iCnChar >= 46318) && (iCnChar <= 46825))
            {
                return "D";
            }
            else if ((iCnChar >= 46826) && (iCnChar <= 47009))
            {
                return "E";
            }
            else if ((iCnChar >= 47010) && (iCnChar <= 47296))
            {
                return "F";
            }
            else if ((iCnChar >= 47297) && (iCnChar <= 47613))
            {
                return "G";
            }
            else if ((iCnChar >= 47614) && (iCnChar <= 48118))
            {
                return "H";
            }
            else if ((iCnChar >= 48119) && (iCnChar <= 49061))
            {
                return "J";
            }
            else if ((iCnChar >= 49062) && (iCnChar <= 49323))
            {
                return "K";
            }
            else if ((iCnChar >= 49324) && (iCnChar <= 49895))
            {
                return "L";
            }
            else if ((iCnChar >= 49896) && (iCnChar <= 50370))
            {
                return "M";
            }

            else if ((iCnChar >= 50371) && (iCnChar <= 50613))
            {
                return "N";
            }
            else if ((iCnChar >= 50614) && (iCnChar <= 50621))
            {
                return "O";
            }
            else if ((iCnChar >= 50622) && (iCnChar <= 50905))
            {
                return "P";
            }
            else if ((iCnChar >= 50906) && (iCnChar <= 51386))
            {
                return "Q";
            }
            else if ((iCnChar >= 51387) && (iCnChar <= 51445))
            {
                return "R";
            }
            else if ((iCnChar >= 51446) && (iCnChar <= 52217))
            {
                return "S";
            }
            else if ((iCnChar >= 52218) && (iCnChar <= 52697))
            {
                return "T";
            }
            else if ((iCnChar >= 52698) && (iCnChar <= 52979))
            {
                return "W";
            }
            else if ((iCnChar >= 52980) && (iCnChar <= 53640))
            {
                return "X";
            }
            else if ((iCnChar >= 53689) && (iCnChar <= 54480))
            {
                return "Y";
            }
            else if ((iCnChar >= 54481) && (iCnChar <= 55289))
            {
                return "Z";
            }
            else return ("?");
        }
        #endregion

        #region 判断是否是交易时间
        public static int IsTradeTime()
        {
            DateTime dt = DateTime.Now;
            int is_trade_time = 0;
            DateTime trade_time = Convert.ToDateTime(dt.ToString("HH:mm:ss"));
            if (trade_time >= Convert.ToDateTime("09:30:00") && trade_time <= Convert.ToDateTime("11:30:00"))
            {
                is_trade_time = 1;
            }
            if (trade_time >= Convert.ToDateTime("13:00:00") && trade_time <= Convert.ToDateTime("15:00:00"))
            {
                is_trade_time = 1;
            }
            if (is_trade_time == 1)
            {
                string day = dt.ToString("MMdd");
                switch (day)
                {
                    case "0204":
                        is_trade_time = 0;
                        break;
                    case "0205":
                        is_trade_time = 0;
                        break;
                    case "0206":
                        is_trade_time = 0;
                        break;
                    case "0207":
                        is_trade_time = 0;
                        break;
                    case "0208":
                        is_trade_time = 0;
                        break;
                    case "0405":
                        is_trade_time = 0;
                        break;
                    case "0501":
                        is_trade_time = 0;
                        break;
                    case "0607":
                        is_trade_time = 0;
                        break;
                    case "0913":
                        is_trade_time = 0;
                        break;
                    case "1001":
                        is_trade_time = 0;
                        break;
                    case "1002":
                        is_trade_time = 0;
                        break;
                    case "1003":
                        is_trade_time = 0;
                        break;
                    case "1004":
                        is_trade_time = 0;
                        break;
                    case "1005":
                        is_trade_time = 0;
                        break;
                    case "1006":
                        is_trade_time = 0;
                        break;
                    case "1007":
                        is_trade_time = 0;
                        break;
                    default:
                        break;
                }
                string dt_week = DateTime.Today.DayOfWeek.ToString();
                if (dt_week == "Saturday")
                {
                    is_trade_time = 0;
                }
                else if (dt_week == "Sunday")
                {
                    is_trade_time = 0;
                }
            }
            return is_trade_time;
        }
        public static int IsTradeTime2()
        {
            DateTime dt = DateTime.Now;
            int is_trade_time = 0;
            DateTime trade_time = Convert.ToDateTime(dt.ToString("HH:mm:ss"));
            if (trade_time >= Convert.ToDateTime("09:28:00") && trade_time <= Convert.ToDateTime("11:30:00"))
            {
                is_trade_time = 1;
            }
            if (trade_time >= Convert.ToDateTime("13:00:00") && trade_time <= Convert.ToDateTime("15:05:00"))
            {
                is_trade_time = 1;
            }
            if (is_trade_time == 1)
            {
                string day = dt.ToString("MMdd");
                switch (day)
                {
                    case "1001":
                        is_trade_time = 0;
                        break;
                    case "1002":
                        is_trade_time = 0;
                        break;
                    case "1003":
                        is_trade_time = 0;
                        break;
                    case "1004":
                        is_trade_time = 0;
                        break;
                    case "1005":
                        is_trade_time = 0;
                        break;
                    case "1006":
                        is_trade_time = 0;
                        break;
                    case "1007":
                        is_trade_time = 0;
                        break;
                    default:
                        break;
                }
                string dt_week = DateTime.Today.DayOfWeek.ToString();
                if (dt_week == "Saturday")
                {
                    is_trade_time = 0;
                }
                else if (dt_week == "Sunday")
                {
                    is_trade_time = 0;
                }
            }
            return is_trade_time;
        }
        public static int IsTradeTimeCK()
        {
            DateTime dt = DateTime.Now;
            int is_trade_time = 0;
            DateTime trade_time = Convert.ToDateTime(dt.ToString("HH:mm:ss"));
            if (trade_time >= Convert.ToDateTime("09:15:00") && trade_time <= Convert.ToDateTime("11:50:00"))
            {
                is_trade_time = 1;
            }
            if (trade_time >= Convert.ToDateTime("12:45:00") && trade_time <= Convert.ToDateTime("15:20:00"))
            {
                is_trade_time = 1;
            }
            if (is_trade_time == 1)
            {
                string day = dt.ToString("MMdd");
                switch (day)
                {
                    case "0204":
                        is_trade_time = 0;
                        break;
                    case "0205":
                        is_trade_time = 0;
                        break;
                    case "0206":
                        is_trade_time = 0;
                        break;
                    case "0207":
                        is_trade_time = 0;
                        break;
                    case "0208":
                        is_trade_time = 0;
                        break;
                    case "0405":
                        is_trade_time = 0;
                        break;
                    case "0501":
                        is_trade_time = 0;
                        break;
                    case "0607":
                        is_trade_time = 0;
                        break;
                    case "0913":
                        is_trade_time = 0;
                        break;
                    case "1001":
                        is_trade_time = 0;
                        break;
                    case "1002":
                        is_trade_time = 0;
                        break;
                    case "1003":
                        is_trade_time = 0;
                        break;
                    case "1004":
                        is_trade_time = 0;
                        break;
                    case "1005":
                        is_trade_time = 0;
                        break;
                    case "1006":
                        is_trade_time = 0;
                        break;
                    case "1007":
                        is_trade_time = 0;
                        break;
                    default:
                        break;
                }
                string dt_week = DateTime.Today.DayOfWeek.ToString();
                if (dt_week == "Saturday")
                {
                    is_trade_time = 0;
                }
                else if (dt_week == "Sunday")
                {
                    is_trade_time = 0;
                }
            }
            return is_trade_time;
        }
        #endregion

        #region 获取股票数据接口
        /// <summary>
        /// 新浪股票接口
        /// </summary>
        /// <param name="stockcode">股票代码</param>
        /// <param name="flag">股票代码</param>
        /// <returns></returns>
        public static string GetStockSina(string stockcode, int flag)
        {
            string stock_info = "";
            stockcode = GetFullStockCode(stockcode,flag);
            if (stockcode.Trim().Length == 8)
            {
                stock_info = GetHttpSinaData("http://hq.sinajs.cn/list=" + stockcode).Replace("var hq_str_" + stockcode + "=\"", "").Replace("\";", "").Trim();
            }
            return stock_info;
        }
        /// <summary>
        /// 腾讯股票接口：返回33
        /// </summary>
        /// <param name="stockcode"></param>
        /// <returns></returns>
        public static string GetStockQQ(string stockcode, int flag)
        {
            string cyb = "", stock = "";
            StringBuilder stock_info = new StringBuilder();
            stockcode = GetFullStockCode(stockcode, flag);
            if (stockcode.Trim().Length ==8)
            {
                stock = GetHttpSinaData("http://qt.gtimg.cn/q=" + stockcode).Replace("v_" + stockcode + "=\"", "").Replace("\";", "").Trim();

                string[] info = stock.Split('~');
                if (info.Length > 0)
                {
                    stock_info.Append(info[1] + ",");//名字
                    stock_info.Append(info[5] + ",");//今日开盘价 
                    stock_info.Append(info[4] + ",");//昨日收盘价
                    stock_info.Append(info[3] + ",");//当前价格
                    stock_info.Append(info[33] + ",");//今日最高价
                    stock_info.Append(info[34] + ",");//今日最低价
                    stock_info.Append(info[9] + ",");//竞买价，即“买一”报价
                    stock_info.Append(info[19] + ",");//竞卖价，即“卖一”报价
                    stock_info.Append(info[36] + ",");//成交的股票数
                    stock_info.Append(info[37] + ",");//成交金额
                    stock_info.Append(info[10] + ",");//“买一”申请4695股，即47手；
                    stock_info.Append(info[9] + ",");//“买一”报价；
                    stock_info.Append(info[12] + ",");//“买二”
                    stock_info.Append(info[11] + ",");//“买二”
                    stock_info.Append(info[14] + ",");//“买3”
                    stock_info.Append(info[13] + ",");//“买3”
                    stock_info.Append(info[16] + ",");//“买4”
                    stock_info.Append(info[15] + ",");//“买4”
                    stock_info.Append(info[18] + ",");//“买5”
                    stock_info.Append(info[17] + ",");//“买5”
                    stock_info.Append(info[20] + ",");//“卖一”申报3100股，即31手；
                    stock_info.Append(info[19] + ",");//“卖一”报价
                    stock_info.Append(info[22] + ",");//“卖2”
                    stock_info.Append(info[21] + ",");//“卖2”
                    stock_info.Append(info[24] + ",");//“卖3”
                    stock_info.Append(info[23] + ",");//“卖3”
                    stock_info.Append(info[26] + ",");//“卖4”
                    stock_info.Append(info[25] + ",");//“卖4”
                    stock_info.Append(info[28] + ",");//“卖5”
                    stock_info.Append(info[27] + ",");//“卖5”
                    stock_info.Append(DateTime.Now.ToString("yyyy-MM-dd") + ",");//日期
                    stock_info.Append(info[30] + ",");//时间
                    stock_info.Append(stockcode);//股票代码
                }
            }
            return stock_info.ToString();
        }
        /// <summary>
        /// 紫牛股票接口（网际风）
        /// </summary>
        /// <param name="stockcode">股票代码</param>
        /// <returns></returns>
        public static string GetStockZiniu(string stockcode, int flag)
        {
            string stock_info = "";
            if (stockcode.Trim().Length == 6)
            {
                stock_info = GetHttpSinaData("http://120.26.41.152:8012/tools/stock.ashx?code=" + stockcode + "&flag=" + flag);
            }
            return stock_info;
        }

        /// <summary>
        /// 单支股票查询接口
        /// </summary>
        /// <param name="stockcode">股票代码</param>
        /// <returns></returns>
        public static string GetStockData(string stockcode, int stock_api, int flag)
        {
            StringBuilder sbStockApi = new StringBuilder();
            switch (stock_api)
            {
                case 0:
                    sbStockApi.Append(Utils.GetStockSina(stockcode, flag));//新浪
                    break;
                case 1:
                    sbStockApi.Append(Utils.GetStockQQ(stockcode, flag));//腾讯
                    break;
                case 3:
                    sbStockApi.Append(Utils.GetStockZiniu(stockcode, flag));//紫牛
                    break;
                default:
                    break;
            }
            return sbStockApi.ToString();
        }
        #endregion

        #region 批量获取股票数据
        /// <summary>
        /// 新浪股票多票
        /// </summary>
        /// <param name="stockcode">股票代码列表</param>
        /// <returns></returns>
        public static string GetStockBySinaLot(string stock_code_list)
        {
            return GetHttpSinaData("http://hq.sinajs.cn/list=" + stock_code_list).Trim();
        }

        /// <summary>
        /// 格式化多票行情数据
        /// </summary>
        /// <returns></returns>
        public static string GetHqInfo(int stock_type, string stock_code_s, string stock_code, string stock_info)
        {
            switch (stock_type)
            {
                //新浪
                case 1:
                    stock_info = stock_info.Replace("var hq_str_" + stock_code_s + "=\"", "").Replace("\"", "").Trim() + "," + stock_code;
                    break;
                case 2:
                    stock_info = stock_info.Replace("v_" + stock_code_s + "=\"", "").Replace("\"", "").Trim();
                    break;
                default:
                    break;
            }
            return stock_info;
        }

        /// <summary>
        /// 获取股票完整代码（含指数）
        /// </summary>
        /// <param name="stockcode"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static string GetFullStockCode(string stockcode, int flag)
        {
            string cyb = "", cyb2 = "";
            if (stockcode.Length == 6)
            {
                cyb = Utils.subStr(stockcode, 0, 1);
                if (flag == 0)
                {
                    cyb2 = Utils.subStr(stockcode, 0, 2);
                    if (cyb == "6" || cyb2 == "11" || cyb2 == "51" || cyb2 == "50")
                    {
                        stockcode = "sh" + stockcode;//上证A、B股
                    }
                    else
                    {
                        stockcode = "sz" + stockcode;//沪深A股-深圳A、B股
                    }
                }
                else
                {
                    switch (cyb)
                    {
                        case "0":
                            stockcode = "sh" + stockcode;//上证A、B股
                            break;
                        default:
                            stockcode = "sz" + stockcode;//沪深A股-深圳A、B股
                            break;
                    }
                }
            }
            return stockcode;
        }
        /// <summary>
        /// 新浪获取多只股票行情
        /// </summary>
        /// <param name="stock_code_list">票池</param>
        /// <returns></returns>
        public static string GetStockListSinaData(string stock_code_list)
        {
            return GetHttpSinaData("http://hq.sinajs.cn/list=" + stock_code_list).Trim();
        }
        /// <summary>
        /// 腾讯获取多只票行情
        /// </summary>
        /// <param name="stock_code_list">票池</param>
        /// <returns></returns>
        public static string GetStockListQQData(string stock_code_list)
        {
            return GetHttpSinaData("http://qt.gtimg.cn/q=" + stock_code_list).Trim();
        }

        #endregion

        #region 获取一段文字中的手机号码
        public static string getPhoneNumbers(string text)
        {
            Regex Expression = new Regex("1+[3,4,5,6,7,8,9][0-9]{9}");
            MatchCollection match = Expression.Matches(text);
            foreach (Match m in match)
            {
                return m.Value;
            }
            return "";
        }
        #endregion

        #region Token验证
        /// <summary>
        /// TOKEN校验值
        /// </summary>
        /// <param name="url">来源域名</param>
        /// <param name="time">时间：精确至秒</param>
        /// <param name="token">生成TOKEN</param>
        /// <returns></returns>
        public static bool tokenCk(string url, string time, string token)
        {
            //20190102113906
            if (time.Length < 12)
            {
                return false;
            }
            var timeStr = time.Insert(4, "-");
            timeStr = timeStr.Insert(7, "-");
            timeStr = timeStr.Insert(10, " ");
            timeStr = timeStr.Insert(13, ":");
            timeStr = timeStr.Insert(16, ":");
            var reqTime = DateTime.Parse(timeStr);
            if ((DateTime.Now - reqTime).TotalSeconds > 1200)
            {
                return false;
            }
            string ip = AppRequest.GetIP();
            string ckStr = url + AppKeys.TOKEN_CHECK + time + ip;
            string toeknCk = Utils.MD5(ckStr, "utf-8", false);

            if (toeknCk == token || (toeknCk.Length == token.Length && ip == "127.0.0.1"))
            {
                return true;
            }
            else
            {
                var domainSafeList = ConfigurationManager.AppSettings["domainSafe"].Split(',');
                if (domainSafeList.Length > 0)
                {
                    for (int i = 0; i < domainSafeList.Length; i++)
                    {
                        if (url.Contains(domainSafeList[i]))
                        {
                            return true;
                        }
                    }
                }
            }
            //Log.WriteLog("",$"{toeknCk}=={token}=={ckStr}");
            return false;
        }
        #endregion

    }
}
