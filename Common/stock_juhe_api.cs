using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Common
{
    public class stock_juhe_api
    {
        /// <summary>
        /// 获取股票数据接口
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string GetStockDataByJuhe(string code)
        {
            string stock_code = "";
            if (code.Trim().Length == 6)
            {
                if (Utils.subStr(code, 0, 1) == "6")
                {
                    //上证A、B股
                    stock_code = "sh" + code;
                }
                else
                {
                    //沪深A股-深圳A、B股
                    stock_code = "sz" + code;
                }
            }
            //1.沪深股市
            var parameters1 = new Dictionary<string, string>();
            parameters1.Add("gid", stock_code); //股票编号，上海股市以sh开头，深圳股市以sz开头如：sh601009
            parameters1.Add("key", AppKeys.JUHE_API_KEY);//你申请的key
            StringBuilder sb = new StringBuilder();
            Dictionary<string, object> dic = JsonHelper.DataRowFromJSON(sendPost(AppKeys.JUHE_API_URL, parameters1, "get"));
            if (dic["reason"].ToString() == "SUCCESSED!")
            {
                ArrayList arr = (ArrayList)dic["result"];
                Dictionary<string, object> arrlist = (Dictionary<string, object>)arr[0];
                Dictionary<string, object> str = (Dictionary<string, object>)arrlist["data"];
                sb.Append(str["name"] + ","/*股票名称*/
                    + str["todayStartPri"] + ","/*今日开盘价*/
                    + str["yestodEndPri"] + ","/*昨日收盘价*/
                    + str["nowPri"] + ","/*当前价格*/
                    + str["todayMax"] + ","/*今日最高价*/
                    + str["todayMin"] + ","/*今日最低价*/
                    + str["buyOnePri"] + ","/*竞买价，即“买一”报价；*/
                    + str["sellOnePri"] + ","/*竞卖价，即“卖一”报价；*/
                     + str["traNumber"] + ","/*成交量*/
                     + str["traAmount"] + ","/*成交金额*/
                     + str["buyOne"] + ","/*买一*/
                     + str["buyOnePri"] + ","/*买一报价*/
                     + str["buyTwo"] + ","
                     + str["buyTwoPri"] + ","
                     + str["buyThree"] + ","
                     + str["buyThreePri"] + ","
                      + str["buyFour"] + ","
                      + str["buyFourPri"] + ","
                      + str["buyFive"] + ","
                      + str["buyFivePri"] + ","
                      + str["sellOne"] + ","/*卖一*/
                      + str["sellOnePri"] + ","/*卖一报价*/
                      + str["sellTwo"] + ","
                      + str["sellTwoPri"] + ","
                       + str["sellThree"] + ","
                       + str["sellThreePri"] + ","
                       + str["sellFour"] + ","
                       + str["sellFourPri"] + ","
                       + str["sellFive"] + ","
                       + str["sellFivePri"] + ","
                       + str["date"] + ","
                       + str["time"]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取股票名称
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string GetStockNameByJuhe(string code)
        {
            string name = "";
            object stock_name = CacheHelper.Get(code);
            if (stock_name != null)
            {
                name = stock_name.ToString();
            }
            else
            {
                string stock_code = "";
                if (code.Trim().Length == 6)
                {
                    if (Utils.subStr(code, 0, 1) == "6")
                    {
                        //上证A、B股
                        stock_code = "sh" + code;
                    }
                    else
                    {
                        //沪深A股-深圳A、B股
                        stock_code = "sz" + code;
                    }
                }
                //1.沪深股市
                var parameters1 = new Dictionary<string, string>();
                parameters1.Add("gid", stock_code); //股票编号，上海股市以sh开头，深圳股市以sz开头如：sh601009
                parameters1.Add("key", AppKeys.JUHE_API_KEY);//你申请的key
                name = Utils.GetHttpValue(sendPost(AppKeys.JUHE_API_URL, parameters1, "get"), "\"name\":\"", "\",\"nowPri\"");
                CacheHelper.Insert(code, name, 1440);
            }
            return name;
        }
        /// <summary>
        /// Http (GET/POST)
        /// </summary>
        /// <param name="url">请求URL</param>
        /// <param name="parameters">请求参数</param>
        /// <param name="method">请求方法</param>
        /// <returns>响应内容</returns>
        public static string sendPost(string url, IDictionary<string, string> parameters, string method)
        {
            if (method.ToLower() == "post")
            {
                HttpWebRequest req = null;
                HttpWebResponse rsp = null;
                System.IO.Stream reqStream = null;
                try
                {
                    req = (HttpWebRequest)WebRequest.Create(url);
                    req.Method = method;
                    req.KeepAlive = false;
                    req.ProtocolVersion = HttpVersion.Version10;
                    req.Timeout = 5000;
                    req.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
                    byte[] postData = Encoding.UTF8.GetBytes(BuildQuery(parameters, "utf8"));
                    reqStream = req.GetRequestStream();
                    reqStream.Write(postData, 0, postData.Length);
                    rsp = (HttpWebResponse)req.GetResponse();
                    Encoding encoding = Encoding.GetEncoding(rsp.CharacterSet);
                    return GetResponseAsString(rsp, encoding);
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
                finally
                {
                    if (reqStream != null) reqStream.Close();
                    if (rsp != null) rsp.Close();
                }
            }
            else
            {
                //创建请求
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + "?" + BuildQuery(parameters, "utf8"));
                //GET请求
                request.Method = "GET";
                request.ReadWriteTimeout = 5000;
                request.ContentType = "text/html;charset=UTF-8";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                //返回内容
                string retString = myStreamReader.ReadToEnd();
                return retString;
            }
        }

        /// <summary>
        /// 组装普通文本请求参数。
        /// </summary>
        /// <param name="parameters">Key-Value形式请求参数字典</param>
        /// <returns>URL编码后的请求数据</returns>
        public static string BuildQuery(IDictionary<string, string> parameters, string encode)
        {
            StringBuilder postData = new StringBuilder();
            bool hasParam = false;
            IEnumerator<KeyValuePair<string, string>> dem = parameters.GetEnumerator();
            while (dem.MoveNext())
            {
                string name = dem.Current.Key;
                string value = dem.Current.Value;
                // 忽略参数名或参数值为空的参数
                if (!string.IsNullOrEmpty(name))//&& !string.IsNullOrEmpty(value)
                {
                    if (hasParam)
                    {
                        postData.Append("&");
                    }
                    postData.Append(name);
                    postData.Append("=");
                    if (encode == "gb2312")
                    {
                        postData.Append(HttpUtility.UrlEncode(value, Encoding.GetEncoding("gb2312")));
                    }
                    else if (encode == "utf8")
                    {
                        postData.Append(HttpUtility.UrlEncode(value, Encoding.UTF8));
                    }
                    else
                    {
                        postData.Append(value);
                    }
                    hasParam = true;
                }
            }
            return postData.ToString();
        }

        /// <summary>
        /// 把响应流转换为文本。
        /// </summary>
        /// <param name="rsp">响应流对象</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>响应文本</returns>
        public static string GetResponseAsString(HttpWebResponse rsp, Encoding encoding)
        {
            System.IO.Stream stream = null;
            StreamReader reader = null;
            try
            {
                // 以字符流的方式读取HTTP响应
                stream = rsp.GetResponseStream();
                reader = new StreamReader(stream, encoding);
                return reader.ReadToEnd();
            }
            finally
            {
                // 释放资源
                if (reader != null) reader.Close();
                if (stream != null) stream.Close();
                if (rsp != null) rsp.Close();
            }
        }
    }
}
