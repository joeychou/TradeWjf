using Common;
using System;

namespace WebQuotation.line.black
{
    public partial class kw : System.Web.UI.Page
    {
        protected string stock_code = AppRequest.GetQueryString("stock_code", true);
        protected string url = AppRequest.GetQueryString("url", true);
        protected string time = AppRequest.GetQueryString("time", true);
        protected string token = AppRequest.GetQueryString("token", true);
        protected int flag = AppRequest.GetQueryInt("flag", 0);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (stock_code.Length != 6)
                {
                    Response.Write("股票代码不正确");
                    Response.End();
                    return;
                }
                if (!Utils.tokenCk(url, time, token))
                {
                    Response.Write("token校验失败");
                    Response.End();
                    return;
                }
                if (!IPLimitHelper.CheckIsAble())
                {
                    Response.Write("访问频率过高，请稍后访问");
                    Response.End();
                    return;
                }
            }
        }
    }
}