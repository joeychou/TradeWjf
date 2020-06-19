using Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BLL
{
    public partial class tasks_k_t_line_bak
    {
        private static readonly object lockInit = new object();
        private static readonly object lockTline = new object();
        private static readonly object lockTlineClear = new object();
        private static readonly object lockKline = new object();
        private static readonly object lockKEndline = new object();
        static DAL.stock_codeDAL dal = new DAL.stock_codeDAL();
        static DAL.stock_k_lineDAL dal_kline = new DAL.stock_k_lineDAL();
        public tasks_k_t_line_bak() { }
        /// <summary>s
        /// 线程开始工作
        /// </summary>
        public void Action()
        {
            AutoResetEvent ar = new AutoResetEvent(false);
            ThreadPool.RegisterWaitForSingleObject(ar, OptStockT, null, 58 * 1000, false);//每45秒钟执行一次
            ThreadPool.RegisterWaitForSingleObject(ar, OptStockK, null, 10 * 60 * 1000, false);//每10分钟执行一次
            ThreadPool.RegisterWaitForSingleObject(ar, OptStockTClear, null, 10 * 60 * 1000, false);//每10分钟执行一次
            ThreadPool.RegisterWaitForSingleObject(ar, OptStockKEnd, null, 10 * 60 * 1000, false);//每10分钟执行一次
            ThreadPool.RegisterWaitForSingleObject(ar, OptStockInit, null, 5 * 60 * 1000, false);//每5分钟执行一次
        }
        private void RunEndCheck(string ff)
        {
            Console.WriteLine("任务ID:{0}，{1}，{2}", Thread.CurrentThread.ManagedThreadId, DateTime.Now, ff);
        }

        #region ===========更新分时数据
        private void OptStockT(Object param, bool sign)
        {
            try
            {
                Monitor.Enter(lockTline);
                if (Utils.IsTradeTime() == 1)
                {
                    StringBuilder sErrInfo = new StringBuilder(256);
                    StringBuilder sResult = new StringBuilder(1024 * 1024);
                    bool bRet = TradeX.TdxHq_Connect("14.17.75.71", 7709, sResult, sErrInfo);
                    if (bRet == true)
                    {
                        DateTime dt = DateTime.Now;
                        string strWhere = " 1=1 ", time_hhmm = dt.ToString("HHmm");//行情时间;
                        int count = dal.GetRecordCount(strWhere), pagesize = 500;
                        int pageall = (count + pagesize - 1) / pagesize;
                        for (int i = 1; i <= pageall; i++)
                        {
                            DataSet ds = dal.GetSList(strWhere, "id desc", pagesize, i);
                            if (!Utils.DataSetIsNull(ds))
                            {
                                int j = 0;
                                List<string> stringLot = new List<string>();
                                foreach (DataRow dr in ds.Tables[0].Rows)
                                {
                                    string stock_code = dr["stock_code"].ToString();
                                    int stock_market = Utils.StrToInt(dr["stock_market"].ToString(), 0);
                                    string key = stock_code + "t", keyhis = stock_code + "this";
                                    bool bRet2 = TradeX.TdxHq_GetMinuteTimeData((byte)stock_market, stock_code, sResult, sErrInfo);
                                    if (bRet2 == true)
                                    {
                                        string[] k_list = sResult.ToString().Replace("\n", "#").Replace("\t", ",").Replace("-", "").Split('#');
                                        if (k_list.Length > 2)
                                        {
                                            for (int z = 1; z < k_list.Length; z++)
                                            {
                                                string[] k_split = k_list[z].Split(',');
                                                if (k_split.Length >= 2)
                                                {
                                                    if (z == 1)
                                                    {
                                                        #region ====================行情数据
                                                        string stock_code_s = dr["stock_code_s"].ToString();
                                                        //股票最新行情
                                                        string[] stock_info = RedisHelper.GetValues(stock_code_s).Split(',');
                                                        if (stock_info.Length > 30)
                                                        {
                                                            //开盘价、收盘价、最高、最低、当前价、成交量、成交额
                                                            decimal open = Utils.StrToDecimal(stock_info[1], 0), preClose = Utils.StrToDecimal(stock_info[2], 0),
                                                                highest = Utils.StrToDecimal(stock_info[4], 0), lowest = Utils.StrToDecimal(stock_info[5], 0),
                                                                fprice = Utils.StrToDecimal(stock_info[3], 0), deal_num = Utils.StrToDecimal(stock_info[8], 0),
                                                                deal_price = Utils.StrToDecimal(stock_info[9], 0);
                                                            string old_str = RedisHelper.GetValues(key),//旧数据
                                                                old_his_str = RedisHelper.GetValues(keyhis);//旧数据
                                                            //记录历史交易量、交易额
                                                            RedisHelper.Set<string>(keyhis, deal_num + "," + deal_price);
                                                            //获取上一条行情对比成交量成交额
                                                            string[] deal_his = old_his_str.Split(',');
                                                            if (deal_his.Length == 2)
                                                            {
                                                                deal_num = deal_num - Utils.StrToDecimal(deal_his[0], 0);
                                                                deal_price = deal_price - Utils.StrToDecimal(deal_his[1], 0);
                                                                deal_num = (deal_num < 0 ? 0 : deal_num);
                                                                deal_price = (deal_price < 0 ? 0 : deal_price);
                                                            }
                                                            if (time_hhmm == "0930" || old_str.Trim().Length < 32)
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
                                                                    price = fprice,
                                                                    volume = deal_num,
                                                                    amount = deal_price,
                                                                    time = time_hhmm
                                                                };
                                                                modelt.mins.Add(mins);
                                                                string json_first = JsonHelper.GetJson<Model.StockTLine>(modelt);
                                                                RedisHelper.Set<string>(key, json_first);
                                                                #endregion
                                                            }
                                                            else
                                                            {
                                                                #region ====================更新数据
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
                                                                Model.mins mins = new Model.mins()
                                                                {
                                                                    price = fprice,
                                                                    volume = deal_num,
                                                                    amount = deal_price,
                                                                    time = time_hhmm
                                                                };
                                                                modelt.quote = quote;
                                                                modelt.mins.Add(mins);
                                                                string json_first = JsonHelper.GetJson<Model.StockTLine>(modelt);
                                                                RedisHelper.Set<string>(key, json_first);
                                                                if (time_hhmm == "1130" || time_hhmm == "1500")
                                                                {
                                                                    //更新分时线
                                                                    old_str = RedisHelper.GetValues(key);
                                                                    stringLot.Add(" update c_stock_code set t_line='" + old_str + "' where stock_code='" + stock_code + "';");
                                                                }
                                                                #endregion
                                                            }
                                                            j++;
                                                        }
                                                        #endregion
                                                    }
                                                }
                                            }
                                        }

                                    }


                                }
                                //多条更新
                                int num = dal.TranLot(stringLot);
                                RunEndCheck("共更新【" + j + "】只票的分时数据");
                            }
                        }

                        TradeX.TdxHq_Disconnect();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteError("", ex.ToString());
                RunEndCheck(ex.ToString());
            }
            finally
            {
                Monitor.Exit(lockTline);
            }
        }
        #endregion

        #region ===========更新股票K线图数据
        private void OptStockK(Object param, bool sign)
        {
            try
            {
                Monitor.Enter(lockKline);
                if (dal.GetIsTradeTimeTline() == 1)
                {
                    StringBuilder sErrInfo = new StringBuilder(256);
                    StringBuilder sResult = new StringBuilder(1024 * 1024);
                    bool bRet = TradeX.TdxHq_Connect("14.17.75.71", 7709, sResult, sErrInfo);
                    if (bRet == true)
                    {
                        DateTime dt = DateTime.Now;
                        string strWhere = " 1=1 ";
                        int count = dal.GetRecordCount(strWhere), pagesize = 500;
                        int pageall = (count + pagesize - 1) / pagesize;
                        for (int i = 1; i <= pageall; i++)
                        {
                            DataSet ds = dal.GetSList(strWhere, "id desc", pagesize, i);
                            if (!Utils.DataSetIsNull(ds))
                            {

                                #region ====================K图初始化
                                RunEndCheck("更新K线数据开始...");

                                foreach (DataRow dr in ds.Tables[0].Rows)
                                {
                                    string stock_code = dr["stock_code"].ToString();
                                    int stock_market = Utils.StrToInt(dr["stock_market"].ToString(), 0);
                                    string key = stock_code + "k";

                                    RunEndCheck("更新K线数据..." + stock_code);

                                    #region ====================查询接口数据
                                    short nCount = 300;
                                    bRet = TradeX.TdxHq_GetSecurityBars(4, (byte)stock_market, stock_code, 0, ref nCount, sResult, sErrInfo);
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
                                #endregion


                                RunEndCheck("更新K线数据完成...");
                            }
                        }
                        RunEndCheck("共更新【" + count + "】只票的K线数据");
                        TradeX.TdxHq_Disconnect();
                    }
                }
            }
            catch (Exception ex)
            {
                RunEndCheck(ex.ToString());
            }
            finally
            {
                Monitor.Exit(lockKline);
            }
        }
        #endregion

        #region ===========收市后更新K线图数据
        private void OptStockKEnd(Object param, bool sign)
        {
            try
            {
                Monitor.Enter(lockKEndline);
                DateTime dt = DateTime.Now;
                DateTime trade_time = Convert.ToDateTime(dt.ToString("HH:mm:ss"));
                if (trade_time >= Convert.ToDateTime("15:05:00") && trade_time < Convert.ToDateTime("15:10:10"))
                {
                    if (dal.GetIsWeek(trade_time) == 0)
                    {
                        string strWhere = " 1=1 ";
                        DataSet ds = dal.GetSList(strWhere + "order by id desc");
                        if (!Utils.DataSetIsNull(ds))
                        {
                            int j = 0;
                            List<string> stringLot = new List<string>();
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                #region =================数据库
                                string stock_code_s = dr["stock_code_s"].ToString();
                                string[] stock_info = RedisHelper.GetValues(stock_code_s).Split(',');
                                if (stock_info.Length > 30)
                                {
                                    //行情时间
                                    string open = stock_info[1], //开盘价
                                        preClose = stock_info[2], //收盘价
                                        highest = stock_info[4], //最高
                                        lowest = stock_info[5], //最低
                                        fprice = stock_info[3], //当前价
                                        deal_num = stock_info[8], //成交量
                                        deal_price = stock_info[9],//成交额
                                        stock_name = stock_info[0];//股票名称
                                    string stock_code = stock_code_s.Replace("sh", "").Replace("sz", "");//股票代码

                                    //先删除
                                    stringLot.Add(" delete from c_stock_k_line where stock_code='" + stock_code + "' and update_time>='" + dt.ToString("yyyy-MM-dd 00:00:00") + "'");
                                    //后添加
                                    stringLot.Add(" insert into c_stock_k_line(stock_code,stock_name,update_time,open,preClose,highest,lowest,deal_vol,deal_price,fprice) values ('"
                                        + stock_code + "','" + stock_name + "',now()," + open + "," + preClose + "," + highest + "," + lowest + "," + deal_num + ","
                                        + deal_price + "," + fprice + ");");

                                    j++;
                                }
                                #endregion
                            }
                            //多条更新
                            int num = dal.TranLot(stringLot);
                            RunEndCheck("共更新【" + j + "】只票的K线数据");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                RunEndCheck(ex.ToString());
            }
            finally
            {
                Monitor.Exit(lockKEndline);
            }
        }
        #endregion

        #region ===========清空分时&K线数据
        private void OptStockTClear(Object param, bool sign)
        {
            try
            {
                Monitor.Enter(lockTlineClear);
                DateTime dt = DateTime.Now;
                DateTime trade_time = Convert.ToDateTime(dt.ToString("HH:mm:ss"));
                if (trade_time >= Convert.ToDateTime("09:10:00") && trade_time < Convert.ToDateTime("09:20:30"))
                {
                    if (dal.GetIsWeek(trade_time) == 0)
                    {

                        #region ====================分时图
                        DataSet ds = dal.GetSList(" 1=1 ");
                        if (!Utils.DataSetIsNull(ds))
                        {
                            #region ====================初始分时数据
                            Model.StockTLine modelt = new Model.StockTLine()
                            {
                                quote = new Model.quote()
                                {
                                    time = dt.ToString("yyyyMMddHHmmss"),
                                    open = 0,
                                    preClose = 0,
                                    highest = 0,
                                    lowest = 0,
                                    price = 0,
                                    volume = 0,
                                    amount = 0
                                }
                            };
                            Model.mins mins = new Model.mins()
                            {
                                price = 0,
                                volume = 0,
                                amount = 0,
                                time = "0"
                            };
                            modelt.mins.Add(mins);
                            string json_first = JsonHelper.GetJson<Model.StockTLine>(modelt);
                            #endregion

                            #region ====================清空分时
                            RunEndCheck("清空分时开始");
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                string stock_code = dr["stock_code"].ToString();
                                RedisHelper.Set<string>(stock_code + "t", json_first);
                            }
                            RunEndCheck("清空分时结束");
                            #endregion

                            #region ====================K图初始化

                            StringBuilder sErrInfo = new StringBuilder(256);
                            StringBuilder sResult = new StringBuilder(1024 * 1024);
                            bool bRet = TradeX.TdxHq_Connect("14.17.75.71", 7709, sResult, sErrInfo);
                            if (bRet == true)
                            {
                                RunEndCheck("更新K线数据开始...");

                                foreach (DataRow dr in ds.Tables[0].Rows)
                                {
                                    string stock_code = dr["stock_code"].ToString();
                                    int stock_market = Utils.StrToInt(dr["stock_market"].ToString(), 0);
                                    string key = stock_code + "k";
                                    //先清理
                                    RedisHelper.Remove(key);

                                    RunEndCheck("更新K线数据..." + stock_code);

                                    #region ====================查询接口数据
                                    short nCount = 300;
                                    bRet = TradeX.TdxHq_GetSecurityBars(4, (byte)stock_market, stock_code, 0, ref nCount, sResult, sErrInfo);
                                    if (bRet == true)
                                    {
                                        string[] k_list = sResult.ToString().Replace("\n", "#").Replace("\t", ",").Replace("-", "").Split('#');
                                        if (k_list.Length > 2)
                                        {
                                            for (int i = 1; i < k_list.Length; i++)
                                            {
                                                string[] k_split = k_list[i].Split(',');
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

                                TradeX.TdxHq_Disconnect();
                                RunEndCheck("更新K线数据完成...");
                            }
                            #endregion
                        }
                        #endregion

                        Log.WriteError("清空分时，更新K线数据", "");

                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteError("清空分时，更新K线数据错误:", ex.ToString());
                RunEndCheck(ex.ToString());
            }
            finally
            {
                Monitor.Exit(lockTlineClear);
            }
        }
        #endregion

        #region ===========初始化行情
        private void OptStockInit(Object param, bool sign)
        {
            try
            {
                Monitor.Enter(lockInit);
                DateTime dt = DateTime.Now;
                DateTime trade_time = Convert.ToDateTime(dt.ToString("HH:mm:ss"));
                if (trade_time >= Convert.ToDateTime("09:20:00") && trade_time < Convert.ToDateTime("09:25:10"))
                {
                    if (dal.GetIsWeek(trade_time) == 0)
                    {
                        #region ====================行情数据
                        string strWhere = " 1=1 ";
                        int count = dal.GetRecordCount(strWhere), pagesize = 50;
                        int pageall = (count + pagesize - 1) / pagesize;
                        for (int i = 1; i <= pageall; i++)
                        {
                            List<string> list_stock_code_s = new List<string>();//持仓股票代码
                            DataSet ds = dal.GetSList(strWhere, "id desc", pagesize, i);
                            if (!Utils.DataSetIsNull(ds))
                            {
                                //更新的股票
                                foreach (DataRow dr in ds.Tables[0].Rows)
                                {
                                    list_stock_code_s.Add(dr["stock_code_s"].ToString());
                                }
                                //股票最新行情
                                string[] stock_list_info = Utils.GetStockBySinaLot(string.Join(",", list_stock_code_s.ToArray())).Split(';');
                                if (stock_list_info.Length == list_stock_code_s.Count)
                                {
                                    List<string> stringLot = new List<string>();
                                    int j = 0;
                                    foreach (var stock_code_s in list_stock_code_s)
                                    {
                                        string line_info = stock_list_info[j];
                                        //单票行情
                                        string[] stock_info = line_info.Split(',');
                                        if (stock_info.Length > 30)
                                        {
                                            RedisHelper.Set<string>(stock_code_s, line_info);
                                        }
                                    }
                                }
                            }
                        }
                        #endregion

                        Log.WriteLog("初始化行情数据", "");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteError("初始化行情数据错误:", ex.ToString());
                RunEndCheck(ex.ToString());
            }
            finally
            {
                Monitor.Exit(lockInit);
            }
        }
        #endregion
    }
}
