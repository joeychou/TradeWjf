using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wjf.tools
{
    /// <summary>
    /// 10档行情
    /// </summary>
    public class stock_hq_ten : IHttpHandler, IHttpAsyncHandler
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
                context.Response.Write(RedisHelper.GetValues(code));
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
    }
}