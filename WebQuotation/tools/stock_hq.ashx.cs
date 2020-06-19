using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace WebQuotation.tools
{
    /// <summary>
    /// 10档行情
    /// </summary>
    public class stock_hq : IHttpHandler, IHttpAsyncHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            //异步处理器不执行该方法
        }
        public bool IsReusable
        {
            //设置允许重用对象
            get { return false; }
        }
        //请求开始时由ASP.NET调用此方法
        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            context.Response.ContentType = "text/plain";
            string code = AppRequest.GetQueryString("code");
            if (code.Length < 6)
            {
                context.Response.Write("fail");
            }
            else
            {
                StringBuilder sErrInfo = new StringBuilder(256);
                StringBuilder sResult = new StringBuilder(1024 * 1024);

                byte[] market = null;
                List<byte> market_list = new List<byte>();
                string[] code_list = code.Split(',');
                short count = (short)code_list.Length;
                if (count > 50 || count < 1)
                {
                    context.Response.Write("The array limit  1 to 50");
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        if (Utils.subStr(code_list[i], 0, 1) == "6")
                        {
                            market_list.Add(1);
                        }
                        else
                        {
                            market_list.Add(0);
                        }
                    }
                    market = market_list.ToArray();

                    string connid = RedisHelper.GetValues("connid");
                    if (connid == "-1" || connid == "")
                    {
                        connid = TradeX.TdxL2Hq_Connect("183.3.223.36", 7709, "srx1314520", "qaz852147wsx", sResult, sErrInfo).ToString();
                        if (sErrInfo.ToString().Contains("行情连接已满"))
                        {
                            connid = "0";
                        }
                        RedisHelper.Set<string>("connid", connid, DateTime.Now.AddSeconds(30));
                    }
                    bool isRet3 = TradeX.TdxL2Hq_GetSecurityQuotes10(market, code_list, ref count, sResult, sErrInfo);
                    if (isRet3 == true)
                    {
                        context.Response.Write(sResult);
                    }
                    else
                    {
                        if (sErrInfo.ToString().Contains("发送数据失败") || sErrInfo.ToString().Contains("无效行情连接"))
                        {
                            TradeX.TdxL2Hq_Disconnect();
                            context.Response.Write(ConnHq(market, code_list, count));
                        }
                        else
                        {
                            context.Response.Write(sErrInfo);
                        }
                    }
                }
            }
            //构建异步结果并返回
            var result = new WebAsyncResult(cb, context);
            result.SetComplete();
            return result;
        }

        //异步结束时，由ASP.NET调用此方法
        public void EndProcessRequest(IAsyncResult result)
        {
            WebAsyncResult webresult = (WebAsyncResult)result;
        }

        protected static string ConnHq(byte[] market, string[] code_list, short count)
        {
            StringBuilder sErrInfo = new StringBuilder(256);
            StringBuilder sResult = new StringBuilder(1024 * 1024);
            bool connid = TradeX.TdxL2Hq_Connect("183.3.223.36", 7709, "srx1314520", "qaz852147wsx", sResult, sErrInfo);
            bool isRet3 = TradeX.TdxL2Hq_GetSecurityQuotes10(market, code_list, ref count, sResult, sErrInfo);
            return sResult.ToString();
        }
    }
}