using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace WebQuotation.tools
{
    /// <summary>
    /// 分时
    /// </summary>
    public class stock_line_t : IHttpHandler, IHttpAsyncHandler
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
            string url = AppRequest.GetQueryString("url");
            string time = AppRequest.GetQueryString("time");
            string token = AppRequest.GetQueryString("token");
            int flag = AppRequest.GetQueryInt("flag", 0);
            if (!Utils.tokenCk(url, time, token))
            {
                context.Response.Write("token校验失败，请刷新页面重试");
            }
            else
            {
                if (flag == 100)
                {
                    #region 使用自身行情
                    string keys_bak = code + "t_bak";
                    context.Response.Write(RedisHelper.GetValues(keys_bak));
                    #endregion
                }
                else
                {
                    #region 使用接口行情

                    if (code.Length != 6)
                    {
                        context.Response.Write("fail");
                    }
                    else
                    {
                        string key = code + "t" + (flag > 0 ? flag.ToString() : "");
                        string hq = RedisHelper.GetValues(key);
                        int count = 0;
                        DateTime dt = DateTime.Now;
                        int time_hhmm = Utils.StrToInt(dt.ToString("HHmm"), 0);//行情时间;
                        if (hq.Length > 32)
                        {
                            Model.StockTLine modelt = JsonHelper.ParseFromJson<Model.StockTLine>(hq);
                            count = modelt.mins.Count;
                            if (count > 0)
                            {
                                //检查交易时间是否已经有数据
                                if (((time_hhmm > 930 && time_hhmm < 1130) || (time_hhmm > 1300 && time_hhmm < 1500)))
                                {
                                    int countCk = modelt.mins.Where(t => t.time == time_hhmm.ToString()).Count();
                                    if (countCk > 0)
                                    {
                                        count = 240;
                                    }
                                    else
                                    {
                                        count = 0;
                                    }
                                }
                            }
                        }
                        if (count < 240 || count > 245)
                        {
                            StringBuilder sErrInfo = new StringBuilder(256);
                            StringBuilder sResult = new StringBuilder(1024 * 1024);
                            bool bRet = TradeX.TdxHq_Connect(AppKeys.QuotationAPI, 7709, sResult, sErrInfo);
                            //Log.WriteLog("行情", sResult.ToString() + "++++" + sErrInfo.ToString());
                            if (bRet == true)
                            {
                                string stock_code_s = Utils.GetFullStockCode(code, flag);
                                if (time_hhmm >= 1500)
                                {
                                    dt = Utils.StrToDateTime(DateTime.Now.ToString("yyyy-MM-dd 15:00:00"));
                                }
                                byte stock_market = 0;
                                string cyb = Utils.subStr(code, 0, 1);
                                if (flag == 0)
                                {
                                    string cyb2 = Utils.subStr(code, 0, 2);
                                    if (cyb == "6" || cyb2 == "11" || cyb2 == "51" || cyb2 == "50")
                                    {
                                        stock_market = 1;
                                    }
                                }
                                else
                                {
                                    switch (cyb)
                                    {
                                        case "0":
                                            stock_market = 1;
                                            break;
                                        case "5":
                                            stock_market = 1;
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                //股票最新行情
                                string[] stock_info = RedisHelper.GetValues(stock_code_s).Split(',');
                                if (stock_info.Length < 30)
                                {
                                    stock_info = Utils.GetStockData(code, 0, 0).Split(',');
                                }
                                bool bRet2 = TradeX.TdxHq_GetMinuteTimeData(stock_market, code, sResult, sErrInfo);
                                if (bRet2 == true && stock_info.Length > 30)
                                {
                                    string[] hq_time = AppKeys.CACHE_HQ_TIME.ToString().Split(',');
                                    string[] k_list = sResult.ToString().Replace("\n", "#").Replace("\t", ",").Replace("-", "").Split('#');
                                    if (k_list.Length > 2 && hq_time.Length >= 240)
                                    {
                                        int lgt = k_list.Length, lgt_ck = hq_time.Length;
                                        if (lgt > lgt_ck)
                                        {
                                            lgt = lgt_ck;
                                        }
                                        for (int z = 1; z < lgt; z++)
                                        {
                                            string[] k_split = k_list[z].Split(',');
                                            if (k_split.Length >= 2)
                                            {
                                                //现价,成交量,保留
                                                decimal price_now = Utils.StrToDecimal(k_split[0], 0), price_num = Utils.StrToDecimal(k_split[1], 0),
                                                    price_keep = Utils.StrToDecimal(k_split[2], 0);
                                                //开盘价、收盘价、最高、最低、当前价、成交量、成交额
                                                decimal open = Utils.StrToDecimal(stock_info[1], 0), preClose = Utils.StrToDecimal(stock_info[2], 0),
                                                    highest = Utils.StrToDecimal(stock_info[4], 0), lowest = Utils.StrToDecimal(stock_info[5], 0),
                                                    fprice = Utils.StrToDecimal(stock_info[3], 0), deal_num = Utils.StrToDecimal(stock_info[8], 0),
                                                    deal_price = Utils.StrToDecimal(stock_info[9], 0);
                                                #region ====================行情数据
                                                if (z == 1)
                                                {
                                                    #region ====================初始数据

                                                    Model.StockTLine modelt = new Model.StockTLine()
                                                    {
                                                        quote = new Model.quote()
                                                        {
                                                            stock_name = stock_info[0],
                                                            time = dt.ToString("yyyyMMddHHmmss"),
                                                            open = open,
                                                            preClose = preClose,
                                                            highest = highest,
                                                            lowest = lowest,
                                                            price = fprice,
                                                            volume = deal_num,
                                                            amount = deal_price
                                                        }
                                                    };
                                                    Model.mins mins = new Model.mins()
                                                    {
                                                        price = price_now,
                                                        volume = price_num,
                                                        amount = 0,
                                                        time = hq_time[z - 1]
                                                    };
                                                    modelt.mins.Add(mins);
                                                    string json_first = JsonHelper.GetJson<Model.StockTLine>(modelt);
                                                    RedisHelper.Set<string>(key, json_first);
                                                    #endregion
                                                }
                                                else
                                                {
                                                    #region ====================更新数据
                                                    string old_str = RedisHelper.GetValues(key);//旧分时数据
                                                    if (!string.IsNullOrEmpty(old_str))
                                                    {
                                                        Model.StockTLine modelt = JsonHelper.ParseFromJson<Model.StockTLine>(old_str);
                                                        Model.quote quote = new Model.quote()
                                                        {
                                                            stock_name = stock_info[0],
                                                            time = dt.ToString("yyyyMMddHHmmss"),
                                                            open = open,
                                                            preClose = preClose,
                                                            highest = highest,
                                                            lowest = lowest,
                                                            price = fprice,
                                                            volume = deal_num,
                                                            amount = deal_price
                                                        };
                                                        Model.mins mins = new Model.mins()
                                                        {
                                                            price = price_now,
                                                            volume = price_num,
                                                            amount = 0,
                                                            time = hq_time[z - 1]
                                                        };
                                                        modelt.quote = quote;
                                                        modelt.mins.Add(mins);
                                                        string json_first = JsonHelper.GetJson<Model.StockTLine>(modelt);
                                                        RedisHelper.Set<string>(key, json_first);
                                                    }
                                                    #endregion
                                                }
                                                #endregion
                                            }
                                        }
                                    }

                                    //断开连接
                                    TradeX.TdxHq_Disconnect();

                                }
                            }
                            hq = RedisHelper.GetValues(key);
                        }
                        else
                        {
                            hq = RedisHelper.GetValues(key);
                        }
                        context.Response.Write(hq);
                    }
                    #endregion
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
    }
}