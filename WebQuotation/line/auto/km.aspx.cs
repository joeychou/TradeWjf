using Common;
using System;

namespace WebQuotation.line.auto
{
    public partial class km : System.Web.UI.Page
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
                string code = Utils.GetFullStockCode(stock_code, flag);
                var stockList = RedisHelper.GetValues(code).Split(',');
                if (stockList.Length > 30)
                {
                    decimal yesterday_price = decimal.Parse(stockList[2]);//昨收价格
                    decimal open_price = decimal.Parse(stockList[1]);//开盘价格
                    decimal now_price_api = decimal.Parse(stockList[3]);//当前价格
                    decimal now_price_h = decimal.Parse(stockList[4]);//今日最高价
                    decimal now_price_l = decimal.Parse(stockList[5]);//今日最低价 
                    if (yesterday_price == 0 && open_price == 0 && now_price_api == 0 && now_price_h == 0 && now_price_l == 0)
                    {
                        Response.Write("该票已退市，无法提供行情信息");
                        Response.End();
                        return;
                    }
                }


            }
        }
    }
}