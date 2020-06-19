using aliyun_api_gateway_sdk.Constant;
using aliyun_api_gateway_sdk.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace aliyun_api_gateway_sdk
{
    public class ShowapiRequest
    {
        String appcode = "";
        String url = "";
        String host = "";
        String path = "";

        int connectTimeout = 10000;//3秒
        int readTimeout = 15000;//15秒
        String char_set = "utf-8";
        Dictionary<String, String> ret_headers = new Dictionary<String, String>();
        Dictionary<String, String> querys = new Dictionary<string, string>();

        public ShowapiRequest(String url, String appcode)
        {
            Console.WriteLine("begin....");
            this.appcode = appcode;
            this.url = url;
            int ind = url.LastIndexOf("/");
            this.host = url.Substring(0, ind);
            this.path = url.Substring(ind);
            Console.WriteLine(this.path);

        }
        public void setRet_headers(Dictionary<String, String> ret_headers)
        {
            this.ret_headers = ret_headers;
        }

        /**
         * 添加post体的字符串参数
         */
        public ShowapiRequest addTextPara(String key, String value)
        {
            this.querys.Add(key, value);
            return this;
        }

        public String doGet()
        {
            Dictionary<String, String> headers = new Dictionary<string, string>();
            headers.Add(HttpHeader.HTTP_HEADER_CONTENT_TYPE, ContentType.CONTENT_TYPE_TEXT);
            headers.Add(HttpHeader.HTTP_HEADER_ACCEPT, ContentType.CONTENT_TYPE_TEXT);
            headers.Add("Authorization", "APPCODE " + this.appcode);

            String ret = "";
            using (HttpWebResponse response = HttpUtil.HttpGet(this.host, this.path, this.appcode, this.readTimeout, headers, this.querys))
            {
                Console.WriteLine(response.StatusCode);
                Console.WriteLine(response.Method);
                Console.WriteLine(response.Headers);
                Stream st = response.GetResponseStream();
                StreamReader reader = new StreamReader(st, Encoding.GetEncoding("utf-8"));
                ret = reader.ReadToEnd() + Constants.LF;
            }
            return ret;
        }

        public String doPost()
        {
            Dictionary<String, String> headers = new Dictionary<string, string>();
            headers.Add(HttpHeader.HTTP_HEADER_CONTENT_TYPE, ContentType.CONTENT_TYPE_FORM);
            headers.Add(HttpHeader.HTTP_HEADER_ACCEPT, ContentType.CONTENT_TYPE_JSON);
            headers.Add("Authorization", "APPCODE " + this.appcode);

            String ret = "";
            using (HttpWebResponse response = HttpUtil.HttpPost(this.host, this.path, this.appcode, this.readTimeout, headers, null, this.querys))
            {
                Console.WriteLine(response.StatusCode);
                Console.WriteLine(response.Method);
                Console.WriteLine(response.Headers);
                Stream st = response.GetResponseStream();
                StreamReader reader = new StreamReader(st, Encoding.GetEncoding("utf-8"));
                ret = reader.ReadToEnd() + Constants.LF;
                Common.Log.WriteLog("验证码", response.StatusCode + "+++" + response.Method + "+++" + response.Headers + "+++" + ret);
            }
            return ret;
        }
    }
}
