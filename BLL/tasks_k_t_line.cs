using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using Common;
using System.IO;
namespace BLL
{
    public partial class tasks_k_t_line
    {
        private static readonly object lockInit = new object();
        private static readonly object lockInitK = new object();
        private static readonly object lockHq = new object();
        private static readonly object lockTline = new object();
        private static readonly object lockTlineClear = new object();
        private static readonly object lockKline = new object();
        private static readonly object lockKEndline = new object();
        private static readonly object lockRd = new object();
        private static readonly object lockSs = new object();
        private readonly static object lockStock = new object();
        static DAL.stock_codeDAL dal = new DAL.stock_codeDAL();
        static DAL.s_stock_codeDAL sys_dal = new DAL.s_stock_codeDAL();
        static DAL.stock_k_lineDAL dal_kline = new DAL.stock_k_lineDAL();
        public tasks_k_t_line() { }
        /// <summary>s
        /// 线程开始工作
        /// </summary>
        public void Action()
        {
            AutoResetEvent ar = new AutoResetEvent(false);
            ThreadPool.RegisterWaitForSingleObject(ar, OptOpenHq, null, 10 * 1000, false);//每3秒钟执行一次
            ThreadPool.RegisterWaitForSingleObject(ar, OptStockT, null, 10 * 1000, false);//每45秒钟执行一次
            //ThreadPool.RegisterWaitForSingleObject(ar, OptStockK, null, 30 * 60 * 1000, false);//每10分钟执行一次
            ThreadPool.RegisterWaitForSingleObject(ar, OptStockTClear, null, 5 * 60 * 1000, false);//每10分钟执行一次
            //ThreadPool.RegisterWaitForSingleObject(ar, OptStockKEnd, null, 10 * 60 * 1000, false);//每10分钟执行一次
            ThreadPool.RegisterWaitForSingleObject(ar, OptStockInit, null, 5 * 60 * 1000, false);//每5分钟执行一次
            ThreadPool.RegisterWaitForSingleObject(ar, OptStockInitK, null, 30 * 60 * 1000, false);//每30分钟执行一次
            ThreadPool.RegisterWaitForSingleObject(ar, OptRd, null, 5 * 60 * 1000, false);//每5分钟执行一次
            ThreadPool.RegisterWaitForSingleObject(ar, OptSs, null, 5 * 60 * 1000, false);//每5分钟执行一次
            ThreadPool.RegisterWaitForSingleObject(ar, OptStockInfo, null, 5 * 60 * 1000, false);//每5分钟执行一次
            ThreadPool.RegisterWaitForSingleObject(ar, OptExit, null, 2 * 60 * 1000, false);//每5分钟执行一次
        }
        private void RunEndCheck(string ff)
        {
            Console.WriteLine("任务ID:{0}，{1}，{2}", Thread.CurrentThread.ManagedThreadId, DateTime.Now, ff);
        }

        #region ===========更新股票最新名称
        private void OptStockInfo(Object param, bool sign)
        {
            try
            {
                Monitor.Enter(lockStock);
                DateTime dt = DateTime.Now;
                DateTime trade_time = Convert.ToDateTime(dt.ToString("HH:mm:ss"));
                if (trade_time >= Convert.ToDateTime("16:20:00") && trade_time < Convert.ToDateTime("16:30:01"))
                {
                    string strWhere = " 1=1 ";
                    int count = sys_dal.GetRecordCount(strWhere), pagesize = 50;
                    int pageall = (count + pagesize - 1) / pagesize;
                    for (int i = 1; i <= pageall; i++)
                    {
                        List<string> list_stock_code_s = new List<string>();//股票代码
                        List<string> list_stock_name = new List<string>();//股票名称
                        DataSet ds = dal.GetSysList(strWhere, "", pagesize, i);
                        if (!Utils.DataSetIsNull(ds))
                        {
                            //更新的股票
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                list_stock_code_s.Add(dr["stock_code_s"].ToString());
                                list_stock_name.Add(dr["stock_name"].ToString());
                            }
                            //股票最新行情
                            string[] stock_list_info = Utils.GetStockBySinaLot(string.Join(",", list_stock_code_s.ToArray())).Split(';');
                            if (stock_list_info.Length > 0)
                            {
                                List<string> stringLot = new List<string>();
                                int j = 0;
                                foreach (var stock_code_s in list_stock_code_s)
                                {
                                    //股票代码
                                    string stock_code = stock_code_s.Substring(2, 6);
                                    //单票行情
                                    string[] stock_info = Utils.GetHqInfo(1, stock_code_s, stock_code, stock_list_info[j]).Split(',');
                                    if (stock_info.Length > 30)
                                    {
                                        string stock_name_old = list_stock_name[j];//原股票名称
                                        string stock_name_api = stock_info[0];//接口股票名称
                                        if (stock_name_api != stock_name_old && stock_code == stock_info[32])
                                        {
                                            stringLot.Add("update sys_stock_code set stock_name='" + stock_name_api + "',stock_name_capital='"
                                                + Utils.GetSpellCode(stock_name_api) + "',input_time=now() where stock_code='" + stock_code + "';");
                                        }
                                    }
                                    j++;
                                }
                                ///多条更新
                                int num = dal.TranLot(stringLot);
                                int num2 = dal.TranLotTo(stringLot);
                                RunEndCheck("共更新【" + num + "】只股票最新名称");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteError("更新股票最新名称报错", ex.ToString());
                RunEndCheck(ex.ToString());
            }
            finally
            {
                Monitor.Exit(lockStock);
            }
        }
        #endregion


        #region ===========除权除息
        private void OptRd(Object param, bool sign)
        {
            try
            {
                Monitor.Enter(lockRd);
                DateTime dt = DateTime.Now;
                DateTime trade_time = Convert.ToDateTime(dt.ToString("HH:mm:ss"));
                if (trade_time >= Convert.ToDateTime("16:10:00") && trade_time < Convert.ToDateTime("16:20:01"))
                {
                    Log.WriteLog("除权降息", "除权信息");
                    List<string> stringLot = new List<string>();
                    BinaryReader sReader = new BinaryReader(File.Open(@"C:\股本变迁.TDX", FileMode.Open));
                    long lgt = sReader.BaseStream.Length;
                    long j = 0, z = lgt / 29;
                    sReader.BaseStream.Position = 29;
                    int dtt = Utils.StrToInt(DateTime.Now.AddDays(-2).ToString("yyyyMMdd"), 0);
                    for (int i = 0; i < z; i++)
                    {
                        j += 29;
                        if (i > 0)
                        {
                            string sc = Encoding.UTF8.GetString(sReader.ReadBytes(1)).ToLower();
                            string stock_code = Encoding.UTF8.GetString(sReader.ReadBytes(6)).ToLower();
                            string lit = Encoding.UTF8.GetString(sReader.ReadBytes(1)).ToLower();
                            int rd_deal = sReader.ReadInt32();
                            //float RQ = sReader.ReadSingle();
                            string xxlx = Encoding.UTF8.GetString(sReader.ReadBytes(1));
                            float F1 = sReader.ReadSingle();//每10股派几元
                            float F2 = sReader.ReadSingle();//配股价
                            float F3 = sReader.ReadSingle();//每10股送几股
                            float F4 = sReader.ReadSingle();//每10股配几股
                            float floatCount = F1 + F2 + F3 + F4;
                            if (stock_code.Length == 6 && rd_deal > dtt && floatCount < 2000)
                            {
                                float m_fProfit = F1;//每股红利
                                float m_fGive = F3;//每股送
                                string rd_txt = "";
                                if (m_fGive > 0)
                                {
                                    rd_txt = "10送" + (m_fGive) + "股";
                                }
                                if (m_fProfit > 0)
                                {
                                    rd_txt += "10派" + (m_fProfit) + "元(含税)";
                                }
                                stringLot.Add("update sys_stock_code set remark='" + rd_deal + "',rd_num="
                                + (m_fGive / 10) + ",rd_money=" + (m_fProfit / 10) + ",rd_txt='" + rd_txt + "',input_time=now() where stock_code='" + stock_code + "';");
                                stringLot.Add($"update sys_stock_code set rd_deal=remark where stock_code='{stock_code}';");
                                //Log.WriteLog("除权降息，手动操作", $"sc={sc},stock_code={stock_code},RQ={rd_deal},xxlx={xxlx},m_fProfit={m_fProfit},m_fGive={m_fGive},F3={F3},F4={F4},i={i},z={z},lgt={lgt}");
                            }
                        }
                    }
                    sReader.Close();
                    int num = dal.TranLot(stringLot);
                    int num2 = dal.TranLotTo(stringLot);
                    Log.WriteLog("通达信除权降息，手动操作", $"{string.Join("*******", stringLot.ToArray())}");
                    /*
                    List<string> stringLot = new List<string>();
                    string path = ConfigurationManager.AppSettings["file_path_rd"];//数据位置
                    BinaryReader sReader = new BinaryReader(File.Open(path, FileMode.Open));
                    sReader.BaseStream.Position = 12;
                    int k;
                    while (sReader.BaseStream.Length > sReader.BaseStream.Position)
                    {
                        string stock_code = Encoding.UTF8.GetString(sReader.ReadBytes(8)).ToLower().Replace("sh", "").Replace("sz", "");
                        sReader.BaseStream.Position += 8;
                        k = sReader.ReadInt32();
                        while (k > 0)
                        {
                            DateTime rd_deal = Utils.ConvertIntDatetime(k);
                            float m_fGive = sReader.ReadSingle();//每股送
                            float m_fPei = sReader.ReadSingle();//每股配
                            float m_fPeiPrice = sReader.ReadSingle();//配股价,仅当 m_fPei!=0.0f 时有效
                            float m_fProfit = sReader.ReadSingle();//每股红利
                            if (rd_deal > dt.AddDays(-2))
                            {
                                string rd_txt = "";
                                if (m_fGive > 0)
                                {
                                    rd_txt = "10送" + (m_fGive * 10) + "股";
                                }
                                if (m_fProfit > 0)
                                {
                                    rd_txt += "10派" + (m_fProfit * 10) + "元(含税)";
                                }
                                stringLot.Add("update sys_stock_code set rd_deal='" + rd_deal.ToString("yyyy-MM-dd HH:mm:ss") + "',rd_num="
                                    + m_fGive + ",rd_money=" + m_fProfit + ",rd_txt='" + rd_txt + "',input_time=now() where stock_code='" + stock_code + "';");
                            }
                            if (sReader.BaseStream.Length > sReader.BaseStream.Position)
                                k = sReader.ReadInt32();
                            else
                                break;
                        }
                    }
                    sReader.Close();
                    int num = dal.TranLot(stringLot);
                    int num2 = dal.TranLotTo(stringLot);
                    RunEndCheck(string.Format("除权除息共:{0}条", num));
                    Log.WriteLog("除权除息", "共" + num + "条");
                    */
                }
            }
            catch (Exception ex)
            {
                Log.WriteError("更新除权除息报错", ex.ToString());
                RunEndCheck(ex.ToString());
            }
            finally
            {
                Monitor.Exit(lockRd);
            }
        }
        #endregion

        #region ===========上市日期
        private void OptSs(Object param, bool sign)
        {
            try
            {
                Monitor.Enter(lockSs);
                DateTime dt = DateTime.Now;
                DateTime trade_time = Convert.ToDateTime(dt.ToString("HH:mm:ss"));
                if (trade_time >= Convert.ToDateTime("16:00:00") && trade_time < Convert.ToDateTime("16:10:01"))
                {
                    OdbcConnection ocConn = new OdbcConnection();
                    string table = ConfigurationManager.AppSettings["file_path_ss"];//数据位置
                    string conn =
                                  @"Driver={Microsoft dBASE Driver (*.dbf)};SourceType=DBF;" +
                                  @"Data Source=" + table + ";Exclusive=No;NULL=NO;" +
                                  @"Collate=Machine;BACKGROUNDFETCH=NO;DELETE=NO";
                    ocConn.ConnectionString = conn;
                    ocConn.Open();
                    string strSql = "SELECT * FROM " + table;
                    OdbcDataAdapter oda = new OdbcDataAdapter(strSql, ocConn);
                    DataTable dat = new DataTable();
                    oda.Fill(dat);
                    int DATENOW = Utils.StrToInt(dt.AddDays(-7).ToString("yyyyMMdd"), 0);
                    List<string> stringLot = new List<string>();
                    foreach (DataRow dr in dat.Rows)
                    {
                        //===========上市日期、股票代码
                        int SSDATE = Utils.StrToInt(dr["SSDATE"].ToString(), 0);
                        if (SSDATE > DATENOW)
                        {
                            stringLot.Add("update sys_stock_code set remark='" + dr["SSDATE"].ToString() + "',input_time=now() where stock_code='" + dr["GPDM"]
                                + "' and rd_decl<='1990-01-01';");
                        }
                    }
                    stringLot.Add("update sys_stock_code set rd_decl=remark;");
                    int num = dal.TranLot(stringLot);
                    int num2 = dal.TranLotTo(stringLot);
                    RunEndCheck(string.Format("更新基本面资料共:{0}条", num));
                    Log.WriteLog("更新基本面资料", "共" + num + "条");
                }
            }
            catch (Exception ex)
            {
                Log.WriteError("更新基本面资料报错", ex.ToString());
                RunEndCheck(ex.ToString());
            }
            finally
            {
                Monitor.Exit(lockSs);
            }
        }
        #endregion

        #region ===========退市票更新
        private void OptExit(Object param, bool sign)
        {
            try
            {
                Monitor.Enter(lockSs);
                DateTime dt = DateTime.Now;
                DateTime trade_time = Convert.ToDateTime(dt.ToString("HH:mm:ss"));
                if (trade_time >= Convert.ToDateTime("08:30:00") && trade_time < Convert.ToDateTime("18:40:01"))
                {
                    var stockInfo = Utils.GetHttpData("https://eastmoney.qianniusoft.com/tools/api_ajax.ashx?act=stock_exit");
                    if (!string.IsNullOrEmpty(stockInfo))
                    {
                        var stockList = stockInfo.Split(',');
                        for (int i = 0; i < stockList.Length; i++)
                        {
                            var stock = stockList[i].Split('#');
                            if (stock.Length == 2)
                            {
                                string stock_code = stock[0];
                                if (stock_code.Contains(".SZ"))
                                {
                                    stock_code = "sz" + stock_code;
                                }
                                else
                                {
                                    stock_code = "sh" + stock_code;
                                }
                                stock_code = stock_code.Replace(".SZ", "").Replace(".SH", "");
                                RedisHelper.Set<string>(stock_code, stock[1] + ",0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0");
                            }
                        }
                        Log.WriteLog("更新退市票", "共" + stockList.Length + "条：" + stockInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteError("更新基本面资料报错", ex.ToString());
                RunEndCheck(ex.ToString());
            }
            finally
            {
                Monitor.Exit(lockSs);
            }
        }
        #endregion

        #region ===========10档口行情
        private void OptOpenHq(Object param, bool sign)
        {
            try
            {
                Monitor.Enter(lockHq);
                if (Utils.IsTradeTime2() == 1)
                {
                    StringBuilder sErrInfo = new StringBuilder(256);
                    StringBuilder sResult = new StringBuilder(1024 * 1024);
                    DateTime dt = DateTime.Now;
                    RunEndCheck("10档开始" + dt.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    int connid = Utils.StrToInt(RedisHelper.GetValues("connid"), -1);
                    if (connid == -1)
                    {
                        bool isok = TradeX2.TdxL2Hq_Connect("120.77.76.11", 7709, "srx1314520", "zybc2017znzn", sResult, sErrInfo);
                        connid = (isok == true ? 0 : -1);
                        RedisHelper.Set<string>("connid", "0", dt.AddSeconds(30));
                        Log.WriteLog("10档链接", sErrInfo.ToString() + "#" + connid);
                    }
                    int count = Utils.StrToInt(RedisHelper.GetValues("hq_10_count"), 0), pagesize = 50, count_succ = 0;
                    int pageall = (count + pagesize - 1) / pagesize;
                    for (int i = 1; i <= pageall; i++)
                    {
                        Thread.Sleep(10);//延迟10毫秒
                        string[] hq_stock = RedisHelper.GetValues("hq_10_code_" + i).Split(',');
                        string[] hq_market_str = RedisHelper.GetValues("hq_10_market_" + i).Split(',');
                        int market_lgt = hq_market_str.Length;
                        short count_quo = (short)hq_stock.Length;
                        byte[] hq_market = new byte[market_lgt];
                        for (int j = 0; j < market_lgt; j++)
                        {
                            hq_market[j] = (byte)(hq_market_str[j] == "0" ? 0 : 1);
                        }
                        List<string> code_list = new List<string>(hq_stock);
                        bool isRet3 = TradeX2.TdxL2Hq_GetSecurityQuotes10(hq_market, hq_stock, ref count_quo, sResult, sErrInfo);
                        if (sErrInfo.ToString().Contains("发送数据失败") || sErrInfo.ToString().Contains("无效行情连接") || sErrInfo.ToString().Contains("连接断开"))
                        {
                            TradeX.TdxL2Hq_Disconnect();
                            //Log.WriteLog("10档报错", sErrInfo.ToString() + "#" + connid + "#" + (isRet3 == true ? "成功" : "失败"));
                        }
                        else
                        {
                            if (isRet3 == true)
                            {
                                string[] hq_10_list = sResult.ToString().Replace("\n", "#").Replace("\t", ",").Replace("-", "").Split('#');
                                for (int z = 1; z < hq_10_list.Length; z++)
                                {
                                    string[] hq_10_list_sub = hq_10_list[z].Split(',');
                                    if (hq_10_list_sub.Length > 60)
                                    {
                                        RedisHelper.Set<string>("ten" + hq_10_list_sub[1], hq_10_list[z]);
                                    }
                                }
                                count_succ++;
                            }
                        }
                    }
                    //TradeX.TdxL2Hq_Disconnect(connid);
                    RunEndCheck("10档，共成功" + count_succ + "页，" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                }
            }
            catch (Exception ex)
            {
                Log.WriteError("10档行情", ex.ToString());
                RunEndCheck(ex.ToString());
            }
            finally
            {
                Monitor.Exit(lockHq);
            }
            /*股票名称,今日开盘价,昨日收盘价,当前价格,今日最高价,今日最低价,当前成交手数,内盘,
             *买一量,买一报价,买2量,买2报价....买10量,买10报价,
             *卖1量,卖1报价,卖2量,卖2报价...卖10量,卖10报价,
             *调取api服务器数据的时间*/
        }
        #endregion

        #region ===========更新分时数据
        private void OptStockT(Object param, bool sign)
        {
            try
            {
                Monitor.Enter(lockTline);
                //if (Utils.IsTradeTime() == 1)
                if (true)
                {
                    DateTime dt = DateTime.Now;
                    string strWhere = " 1=1 ", time_hhmm = dt.ToString("HHmm");//行情时间;
                    int count = dal.GetRecordCount(strWhere), pagesize = 500;
                    int pageall = (count + pagesize - 1) / pagesize;
                    for (int i = 1; i <= pageall; i++)
                    {
                        DataSet ds = dal.GetSysList(strWhere, "id desc", pagesize, i);
                        if (!Utils.DataSetIsNull(ds))
                        {
                            int j = 0;
                            List<string> stringLot = new List<string>();
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                #region ====================行情数据
                                string stock_code_s = dr["stock_code_s"].ToString();
                                //股票最新行情

                                string[] stock_info = Utils.GetStockSinaData(stock_code_s).Split(','); // RedisHelper.GetValues(stock_code_s).Split(',');
                                if (stock_info.Length > 30)
                                {
                                    //开盘价、收盘价、最高、最低、当前价、成交量、成交额
                                    decimal open = Utils.StrToDecimal(stock_info[1], 0), preClose = Utils.StrToDecimal(stock_info[2], 0),
                                        highest = Utils.StrToDecimal(stock_info[4], 0), lowest = Utils.StrToDecimal(stock_info[5], 0),
                                        fprice = Utils.StrToDecimal(stock_info[3], 0), deal_num = Utils.StrToDecimal(stock_info[8], 0),
                                        deal_price = Utils.StrToDecimal(stock_info[9], 0);
                                    string stock_code = stock_code_s.Replace("sh", "").Replace("sz", "");
                                    string key = stock_code + "t_bak",//redis-Key值
                                        keyhis = stock_code + "this",//旧值
                                        old_str = RedisHelper.GetValues(key),//旧数据
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
                                        //modelt.mins.RemoveAt(modelt.mins.Count - 1);
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
                                        string json_first = JsonHelper.GetJson(modelt);
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
                            //多条更新
                            int num = dal.TranLot(stringLot);
                            RunEndCheck("共更新【" + j + "】只票的分时数据");
                        }
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
                if (dal.GetIsTradeTimeTline() == 0)
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
                            DataSet ds = dal.GetSysList(strWhere, "id desc", pagesize, i);
                            if (!Utils.DataSetIsNull(ds))
                            {
                                #region ====================K图初始化
                                RunEndCheck("更新K线数据开始...");

                                foreach (DataRow dr in ds.Tables[0].Rows)
                                {
                                    string stock_code = dr["stock_code"].ToString();
                                    int stock_market = Utils.StrToInt(dr["stock_market"].ToString(), 0);
                                    string key = stock_code + "k";
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
                        DataSet ds = dal.GetSysList(strWhere + "order by id desc");
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
                if (trade_time >= Convert.ToDateTime("09:20:00") && trade_time < Convert.ToDateTime("09:25:30"))
                {
                    if (dal.GetIsWeek(trade_time) == 0)
                    {
                        #region ====================分时图
                        DataSet ds = dal.GetSysList(" 1=1 ");
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
                                    highest = (decimal)0.01,
                                    lowest = (decimal)0.01,
                                    price = 0,
                                    volume = 0,
                                    amount = 0
                                }
                            };
                            Model.mins mins = new Model.mins()
                            {
                                price = (decimal)0.01,
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
                                RedisHelper.Set<string>(stock_code + "t_bak", json_first);
                                RedisHelper.Set<string>(stock_code + "t1", json_first);
                                //先清理
                                RedisHelper.Remove(stock_code + "k");
                                RedisHelper.Remove(stock_code + "kw");
                                RedisHelper.Remove(stock_code + "km");

                                RedisHelper.Remove(stock_code + "k1");
                                RedisHelper.Remove(stock_code + "kw1");
                                RedisHelper.Remove(stock_code + "km1");
                            }
                            RunEndCheck("清空分时结束");
                            #endregion
                        }

                        RedisHelper.Remove("000001t1");
                        RedisHelper.Remove("399001t1");
                        RedisHelper.Remove("399006t1");

                        RedisHelper.Remove("000001k1");
                        RedisHelper.Remove("399001k1");
                        RedisHelper.Remove("399006k1");
                        #endregion

                        Log.WriteError("清空分时，更新K线数据", "");

                    }
                }

                if (trade_time >= Convert.ToDateTime("15:00:00") && trade_time < Convert.ToDateTime("15:10:30"))
                {
                    if (dal.GetIsWeek(trade_time) == 0)
                    {

                        #region ====================K线图
                        DataSet ds = dal.GetSysList(" 1=1 ");
                        if (!Utils.DataSetIsNull(ds))
                        {

                            #region ====================清空K线图
                            RunEndCheck("清空K线开始");
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                string stock_code = dr["stock_code"].ToString();
                                RedisHelper.Remove(stock_code + "k");
                                RedisHelper.Remove(stock_code + "kw");
                                RedisHelper.Remove(stock_code + "km");


                                RedisHelper.Remove(stock_code + "k1");
                                RedisHelper.Remove(stock_code + "kw1");
                                RedisHelper.Remove(stock_code + "km1");
                            }
                            RunEndCheck("清空K线结束");
                            #endregion
                        }
                        #endregion

                        Log.WriteError("清空K线数据", "");

                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteError("清空K线数据错误:", ex.ToString());
                RunEndCheck(ex.ToString());
            }
            finally
            {
                Monitor.Exit(lockTlineClear);
            }
        }
        #endregion

        #region ===========初始化K线数据到数据库
        private void OptStockInitK(Object param, bool sign)
        {
            try
            {
                Monitor.Enter(lockInitK);
                DateTime dt = DateTime.Now;
                DateTime trade_time = Convert.ToDateTime(dt.ToString("HH:mm:ss"));
                if (trade_time >= Convert.ToDateTime("09:00:00") && trade_time < Convert.ToDateTime("09:30:30"))
                {
                    if (dal.GetIsWeek(trade_time) == 0)
                    {
                        #region ====================行情数据
                        string strWhere = " 1=1 ";
                        int count = dal.GetRecordCount(strWhere), pagesize = 50;
                        int pageall = (count + pagesize - 1) / pagesize;
                        StringBuilder sErrInfo = new StringBuilder(256);
                        StringBuilder sResult = new StringBuilder(1024 * 1024);
                        bool bRet = TradeX.TdxHq_Connect("14.17.75.71", 7709, sResult, sErrInfo);
                        if (bRet == true)
                        {
                            List<string> strLot = new List<string>();
                            strLot.Add("truncate c_stock_k_line;");
                            dal.TranLot(strLot);
                            RunEndCheck("清空K线历史数据" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                            RunEndCheck("更新K线数据开始" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                            for (int i = 1; i <= pageall; i++)
                            {
                                DataSet ds = dal.GetSysList(strWhere, "id desc", pagesize, i);
                                if (!Utils.DataSetIsNull(ds))
                                {
                                    foreach (DataRow dr in ds.Tables[0].Rows)
                                    {
                                        //更新的股票
                                        List<string> stringLot = new List<string>();
                                        string stock_code = dr["stock_code"].ToString();
                                        string stock_name = dr["stock_name"].ToString();
                                        byte stock_market = 0;
                                        if (Utils.subStr(stock_code, 0, 1) == "6")
                                        {
                                            stock_market = 1;
                                        }

                                        #region ====================查询接口数据
                                        short nCount = 50;
                                        bRet = TradeX.TdxHq_GetSecurityBars(4, stock_market, stock_code, 0, ref nCount, sResult, sErrInfo);
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
                                                        string time = DateTime.ParseExact(k_split[0], "yyyyMMdd", null).ToString("yyyy-MM-dd 00:00:00");
                                                        //后添加
                                                        stringLot.Add(" insert into c_stock_k_line(stock_code,stock_name,update_time,open,preClose,highest,lowest,deal_vol,deal_price,fprice) values ('"
                                                            + stock_code + "','" + stock_name + "','" + time + "'," + open + "," + preClose + "," + highest + "," + lowest + "," + deal_num + ","
                                                            + deal_price + "," + fprice + ");");
                                                        #endregion
                                                    }
                                                }
                                            }
                                        }
                                        #endregion

                                        //多条更新
                                        int num = dal.TranLot(stringLot);
                                        RunEndCheck("更新第【" + stock_name + "】的K线数据共" + num + "条");
                                    }
                                }
                            }
                            RunEndCheck("更新K线数据结束" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                        }
                        #endregion

                        Log.WriteLog("初始化K线数据到数据库", "");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteError("初始化K线数据到数据库:", ex.ToString());
                RunEndCheck(ex.ToString());
            }
            finally
            {
                Monitor.Exit(lockInitK);
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
                            List<string> market_list = new List<string>();//股票市场
                            List<string> list_stock_code_s = new List<string>();//股票代码
                            List<string> list_stock_code = new List<string>();//股票代码
                            DataSet ds = dal.GetSysList(strWhere, "id desc", pagesize, i);
                            if (!Utils.DataSetIsNull(ds))
                            {
                                //更新的股票
                                foreach (DataRow dr in ds.Tables[0].Rows)
                                {
                                    list_stock_code.Add(dr["stock_code"].ToString());
                                    list_stock_code_s.Add(dr["stock_code_s"].ToString());
                                    market_list.Add(dr["stock_market"].ToString());
                                }
                                //10档行情数据
                                RedisHelper.Set<string>("hq_10_count", count.ToString());
                                RedisHelper.Set<string>("hq_10_code_" + i, string.Join(",", list_stock_code.ToArray()));
                                RedisHelper.Set<string>("hq_10_market_" + i, string.Join(",", market_list.ToArray()));

                                Log.WriteLog("hq_10_code_" + i, string.Join(",", list_stock_code.ToArray()));
                                Log.WriteLog("hq_10_market_" + i, string.Join(",", market_list.ToArray()));

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