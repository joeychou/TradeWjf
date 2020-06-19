using Common;
using System;
namespace wjf.line.auto
{
    public partial class t : System.Web.UI.Page
    {
        protected string stock_code = AppRequest.GetQueryString("stock_code", true);
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
                if (AppRequest.isBlackList(""))
                {
                    Response.Write("您无访问权限");
                    Response.End();
                    return;
                }
            }
        }
    }
}