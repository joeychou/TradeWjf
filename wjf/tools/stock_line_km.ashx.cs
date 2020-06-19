using Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;

namespace wjf.tools
{
    /// <summary>
    /// 月K线图
    /// </summary>
    public class stock_line_km : IHttpHandler, IHttpAsyncHandler
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
            string code = AppRequest.GetQueryString("code", true);
            int flag = AppRequest.GetQueryInt("flag", 0);
            if (code.Length != 6)
            {
                context.Response.Write("fail");
            }
            else
            {
                string key = code + "km" + (flag > 0 ? flag.ToString() : "");
                string hq = RedisHelper.GetValues(key);
                if (hq.Trim().Length < 32)
                {
                    RedisHelper.Remove(key);
                    StringBuilder sErrInfo = new StringBuilder(256);
                    StringBuilder sResult = new StringBuilder(1024 * 1024);
                    bool bRet = TradeX.TdxHq_Connect(AppKeys.QuotationAPI, 7709, sResult, sErrInfo);
                    if (bRet == true)
                    {
                        DateTime dt = DateTime.Now;
                        byte stock_market = 0;
                        string cyb = Utils.subStr(code, 0, 1);
                        if (flag == 0)
                        {
                            switch (cyb)
                            {
                                case "6":
                                    stock_market = 1;
                                    break;
                                case "5":
                                    stock_market = 1;
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            switch (cyb)
                            {
                                case "0":
                                    stock_market = 1;
                                    break;
                                default:
                                    break;
                            }
                        }
                        #region ====================查询接口数据
                        short nCount = 200;
                        if (flag == 1)
                        {
                            bRet = TradeX.TdxHq_GetIndexBars(6, stock_market, code, 0, ref nCount, sResult, sErrInfo);
                        }
                        else
                        {
                            bRet = TradeX.TdxHq_GetSecurityBars(6, stock_market, code, 0, ref nCount, sResult, sErrInfo);
                        }
                        if (bRet == true)
                        {
                            string[] k_list = sResult.ToString().Replace("\n", "#").Replace("\t", ",").Replace("-", "").Split('#');
                            if (k_list.Length > 2)
                            {
                                for (int z = 1; z < k_list.Length; z++)
                                {
                                    string[] k_split = k_list[z].Split(',');
                                    if (k_split.Length >= 7)
                                    {
                                        #region ====================更新redis，K线数据
                                        //开盘价、收盘价、最高、最低、当前价、成交量、成交额
                                        decimal open = Utils.StrToDecimal(k_split[1], 0), preClose = Utils.StrToDecimal(k_split[2], 0),
                                            highest = Utils.StrToDecimal(k_split[3], 0), lowest = Utils.StrToDecimal(k_split[4], 0),
                                            fprice = Utils.StrToDecimal(k_split[2], 0), deal_num = Utils.StrToDecimal(k_split[5], 0),
                                            deal_price = Utils.StrToDecimal(k_split[6], 0);
                                        //redis-Key值、行情时间、旧数据
                                        string old_str = RedisHelper.GetValues(key), time_hhmm = k_split[0];
                                        int time_hhmm_int = Utils.StrToInt(time_hhmm, 0);//时间数字化
                                        if (old_str.Trim().Length < 32)//第一次插入
                                        {
                                            #region ====================初始数据
                                            Model.StockKLine modelk = new Model.StockKLine();
                                            Model.kmins minsk = new Model.kmins()
                                            {
                                                time = time_hhmm_int,
                                                preClose = preClose,
                                                open = open,
                                                highest = highest,
                                                lowest = lowest,
                                                price = fprice,
                                                volume = deal_num,
                                                amount = deal_price
                                            };
                                            modelk.kmins.Add(minsk);
                                            string json_kline = JsonHelper.GetJson<Model.StockKLine>(modelk);
                                            RedisHelper.Set<string>(key, json_kline);
                                            #endregion;
                                        }
                                        else
                                        {
                                            #region ====================更新数据

                                            Model.StockKLine modelk = JsonHelper.ParseFromJson<Model.StockKLine>(old_str);
                                            bool is_has = modelk.kmins.Any(p => p.time == time_hhmm_int);
                                            if (is_has == true)
                                            {
                                                modelk.kmins.Remove(modelk.kmins.Where(p => p.time == time_hhmm_int).Single());
                                            }
                                            Model.kmins minsk = new Model.kmins()
                                            {
                                                time = time_hhmm_int,
                                                preClose = preClose,
                                                open = open,
                                                highest = highest,
                                                lowest = lowest,
                                                price = fprice,
                                                volume = deal_num,
                                                amount = deal_price
                                            };
                                            modelk.kmins.Add(minsk);
                                            string json_kline = JsonHelper.GetJson<Model.StockKLine>(modelk);
                                            RedisHelper.Set<string>(key, json_kline);
                                            #endregion
                                        }
                                        #endregion
                                    }
                                }
                            }
                        }
                        #endregion
                    }

                }
                context.Response.Write(RedisHelper.GetValues(key));
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