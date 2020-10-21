using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wjf.tools
{
    /// <summary>
    /// 行情接口
    /// </summary>
    public class api_ajax : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string action = AppRequest.GetQueryString("act");
            switch (action)
            {
                case "stock_api": //股票代码查询
                    stock_api(context);
                    break;
                case "stock_t": //股票成交量成交额
                    stock_t(context);
                    break;
                case "is_trade_time": //交易时间检查
                    is_trade_time(context);
                    break;
                case "ck_phone": //检查黑名单数据
                    ck_phone(context);
                    break;
                case "ck_weakpwd": //检查弱密码
                    ck_weakpwd(context);
                    break;
                default:
                    break;
            }
        }
        private void ck_weakpwd(HttpContext context)
        {
            string pwd = AppRequest.GetQueryString("pwd");//密码
            if (string.IsNullOrEmpty(pwd))
            {
                context.Response.Write(JsonHelper.ObjectToJSON(new { succeess = false, msg = "弱密码" }));
                return;
            }
            try
            {
                if (pwd.Contains("123456"))
                {
                    context.Response.Write(JsonHelper.ObjectToJSON(new { succeess = false, msg = "弱密码" }));
                    return;
                }
                if (pwd.Length != 32)
                {
                    string ckStatus = Utils.HttpGetUtf("http://monitor.zouhongsoft.com/api/userWeakpwd/ckUserWeakPwd?password=" + pwd);
                    Dictionary<string, object> dicCK = JsonHelper.DataRowFromJSON(ckStatus);
                    if (dicCK.Count > 0)
                    {
                        bool succuss = (bool)dicCK["succeess"];
                        string msg = dicCK["msg"].ToString();
                        if (!succuss)
                        {
                            context.Response.Write(JsonHelper.ObjectToJSON(new { succeess = false, msg = msg }));
                            return;
                        }
                    }
                    else
                    {
                        context.Response.Write(JsonHelper.ObjectToJSON(new { succeess = false, msg = "弱密码" }));
                        return;
                    }
                }
                else
                {
                    if (LimitPhoneHelper.GetWeakPwdList().Contains(pwd))
                    {
                        context.Response.Write(JsonHelper.ObjectToJSON(new { succeess = false, msg = "弱密码" }));
                        return;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            context.Response.Write(JsonHelper.ObjectToJSON(new { succeess = true, msg = "正常密码" }));
            return;
        }
        private void ck_phone(HttpContext context)
        {
            string phone = AppRequest.GetQueryString("phone", true);//电话号码
            if (!Regexlib.IsValidMobile(phone))
            {
                context.Response.Write(JsonHelper.ObjectToJSON(new { succeess = false, msg = "电话号码格式不正确" }));
                return;
            }
            try
            {
                if (LimitPhoneHelper.IsContains(phone))
                {
                    context.Response.Write(JsonHelper.ObjectToJSON(new { succeess = false, msg = "注册失败，网络错误" }));
                    return;
                }
                //string ckStatus = Utils.HttpGetUtf("http://monitor.zouhongsoft.com/api/userBlacklist/ckUserBlackList?UserPhone=" + phone);
                //Dictionary<string, object> dicCK = JsonHelper.DataRowFromJSON(ckStatus);
                //if (dicCK.Count > 0)
                //{
                //    bool succuss = (bool)dicCK["succeess"];
                //    string msg = dicCK["msg"].ToString();
                //    if (!succuss)
                //    {
                //        context.Response.Write(JsonHelper.ObjectToJSON(new { succeess = false, msg = msg }));
                //        return;
                //    }
                //}
                //else
                //{
                //    context.Response.Write(JsonHelper.ObjectToJSON(new { succeess = false, msg = "注册失败，网络错误" }));
                //    return;
                //}
            }
            catch (Exception ex)
            {

            }
            context.Response.Write(JsonHelper.ObjectToJSON(new { succeess = true, msg = "该号码可以注册" }));
            return;
        }
        private void stock_api(HttpContext context)
        {
            int flag = AppRequest.GetQueryInt("flag", 0);
            string code = AppRequest.GetQueryString("code", true);//股票代码
            if (code.Length != 6)
            {
                context.Response.Write("fail");
                return;
            }
            string stock_info = Utils.GetStockData(code, 0, flag);
            var obj = new { stock_info = stock_info, time = int.Parse(DateTime.Now.ToString("HHmm"), 0) };
            context.Response.Write(JsonHelper.ObjectToJSON(obj));
            return;
        }
        private void stock_t(HttpContext context)
        {
            int flag = AppRequest.GetQueryInt("flag", 0);
            string code = AppRequest.GetQueryString("code", true);//股票代码
            if (code.Length != 6)
            {
                context.Response.Write("fail");
                return;
            }
            string json_str = "", key;
            if (flag == 100)
            {
                key = code + "t_bak" + (flag > 0 ? flag.ToString() : "");
            }
            else
            {
                key = code + "t" + (flag > 0 ? flag.ToString() : "");
            }
            if (Utils.IsTradeTime() == 1)
            {
                decimal open = 0, preClose = 0, highest = 0, lowest = 0, fprice = 0, deal_num = 0, deal_price = 0;
                string[] stock_info = Utils.GetStockData(code, 0, flag).Split(',');
                if (stock_info.Length > 30)
                {
                    #region ====================更新数据
                    //redis-Key值、旧值
                    DateTime dt = DateTime.Now;
                    string old_str = RedisHelper.GetValues(key);//旧数据
                    string time_hhmm = dt.ToString("HHmm");//行情时间;

                    //开盘价、收盘价、最高、最低、当前价、成交量、成交额
                    open = Utils.StrToDecimal(stock_info[1], 0);
                    preClose = Utils.StrToDecimal(stock_info[2], 0);
                    highest = Utils.StrToDecimal(stock_info[4], 0);
                    lowest = Utils.StrToDecimal(stock_info[5], 0);
                    fprice = Utils.StrToDecimal(stock_info[3], 0);
                    deal_num = Utils.StrToDecimal(stock_info[8], 0) / 100;
                    deal_price = Utils.StrToDecimal(stock_info[9], 0);

                    if (old_str.Length > 32)
                    {
                        Model.StockTLine modelt = JsonHelper.ParseFromJson<Model.StockTLine>(old_str);
                        bool is_has = modelt.mins.Any(p => p.time == time_hhmm);
                        if (is_has == true)
                        {
                            modelt.mins.Remove(modelt.mins.Where(p => p.time == time_hhmm).Single());
                        }

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

                        if (modelt.mins.Count > 0)
                        {
                            decimal deal_num_his = modelt.mins.Sum(p => p.volume);//以前的成交量
                            decimal volume = (deal_num - deal_num_his) / 100;
                            Model.mins mins = new Model.mins()
                            {
                                price = fprice,
                                volume = volume < 0 ? 0 : volume,
                                amount = deal_price,
                                time = time_hhmm
                            };
                            modelt.quote = quote;
                            modelt.mins.Add(mins);
                        }
                        json_str = JsonHelper.GetJson<Model.StockTLine>(modelt);
                    }
                    #endregion
                }
            }
            else
            {
                json_str = RedisHelper.GetValues(key);//旧数据
            }
            context.Response.Write(json_str);
            return;
        }
        private void is_trade_time(HttpContext context)
        {
            var obj = new { is_trade_time = Utils.IsTradeTime() };//判断是否是交易时间：返回1即是交易时间
            context.Response.Write(JsonHelper.ObjectToJSON(obj));
            return;
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}