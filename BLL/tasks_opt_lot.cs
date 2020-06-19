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
    public class tasks_opt_lot
    {
        private readonly static object lockStock = new object();
        private static readonly object lockOptNewStock = new object();
        private static readonly object lockOptRdStock = new object();
        static DAL.stock_codeDAL dal = new DAL.stock_codeDAL();
        static DAL.s_stock_codeDAL sys_dal = new DAL.s_stock_codeDAL();
        public tasks_opt_lot() { }
        /// <summary>
        /// 线程开始工作
        /// </summary>
        public void Action()
        {
            AutoResetEvent ar = new AutoResetEvent(false);
            ThreadPool.RegisterWaitForSingleObject(ar, OptNewStock, null, 30 * 60 * 1000, false);//每5分钟执行一次
            ThreadPool.RegisterWaitForSingleObject(ar, OptRDStock, null, 30 * 60 * 1000, false);//每5分钟执行一次
            ThreadPool.RegisterWaitForSingleObject(ar, OptStockCodeInfo, null, 20 * 60 * 1000, false);//每5分钟执行一次
        }
        private void RunEndCheck(string ff)
        {
            Console.WriteLine("任务ID:{0}，{1}，{2}", Thread.CurrentThread.ManagedThreadId, DateTime.Now, ff);
        }
        #region ===========更新股票代码除权信息表
        private void OptRDStock(Object param, bool sign)
        {
            try
            {
                Monitor.Enter(lockOptNewStock);
                DateTime dt = DateTime.Now;
                DateTime trade_time = Convert.ToDateTime(dt.ToString("HH:mm:ss"));
                if (trade_time >= Convert.ToDateTime("16:00:00") && trade_time < Convert.ToDateTime("16:30:01"))
                {
                    string strWhere = " input_time>='" + dt.ToString("yyyy-MM-dd 00:00:00") + "'";
                    DataSet ds = sys_dal.GetList(strWhere);
                    if (!Utils.DataSetIsNull(ds))
                    {
                        #region 更新sys_stock_code表
                        List<string> stringLot = new List<string>();
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            string stock_code = dr["stock_code"].ToString();
                            string stock_name = dr["stock_name"].ToString();
                            string stock_name_capital = dr["stock_name_capital"].ToString();
                            int count = sys_dal.GetSRecordCountTo(" stock_code='" + stock_code + "'");
                            //stringLot.Add("delete from sys_stock_code where stock_code='" + stock_code + "';");
                            if (count < 1)
                            {
                                stringLot.Add("insert into sys_stock_code(stock_code,stock_name,stock_name_capital,rd_decl,rd_deal,rd_num,rd_money,rd_txt,remark,input_time)"
                                    + " values ('" + stock_code + "','" + stock_name + "','" + stock_name_capital + "','"
                                    + Utils.StrToDateTime(dr["rd_decl"].ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "','"
                                    + Utils.StrToDateTime(dr["rd_deal"].ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "',"
                                    + dr["rd_num"] + "," + dr["rd_money"] + ",'" + dr["rd_txt"] + "','" + dr["remark"] + "',now());");
                            }
                            else
                            {
                                stringLot.Add("update sys_stock_code set stock_code='" + stock_code
                                    + "',stock_name='" + stock_name
                                    + "',stock_name_capital='" + stock_name_capital
                                    + "',rd_decl='" + Utils.StrToDateTime(dr["rd_decl"].ToString()).ToString("yyyy-MM-dd HH:mm:ss")
                                    + "',rd_deal='" + Utils.StrToDateTime(dr["rd_deal"].ToString()).ToString("yyyy-MM-dd HH:mm:ss")
                                    + "',rd_num=" + dr["rd_num"]
                                    + ",rd_money=" + dr["rd_money"]
                                    + ",rd_txt='" + dr["rd_txt"]
                                    + "',remark='" + dr["remark"]
                                    + "' where stock_code='" + stock_code + "';");
                            }
                        }
                        //多条更新
                        int num = dal.TranLotTo(stringLot);
                        RunEndCheck("共更新【" + num + "】只股票代码除权信息");
                        #endregion


                        #region 日志记录
                        string site_name = ConfigurationManager.AppSettings["site_name"];
                        List<string> stringLot2 = new List<string>();
                        stringLot2.Add("insert into sys_log(userid,logcode,update_time,remark)values (0,1,now(),'更新网站【" + site_name + "】数据库股票除权代码表共（"
                            + num + "）条记录');");
                        dal.TranLot(stringLot2);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteError("更新股票代码除权信息表报错", ex.ToString());
                RunEndCheck(ex.ToString());
            }
            finally
            {
                Monitor.Exit(lockOptNewStock);
            }
        }
        #endregion

        #region ===========更新股票代码表
        private void OptNewStock(Object param, bool sign)
        {
            try
            {
                Monitor.Enter(lockOptNewStock);
                DateTime dt = DateTime.Now;
                DateTime trade_time = Convert.ToDateTime(dt.ToString("HH:mm:ss"));
                if (trade_time >= Convert.ToDateTime("16:00:00") && trade_time < Convert.ToDateTime("16:30:01"))
                {
                    string strWhere = " input_time>='" + dt.ToString("yyyy-MM-dd 00:00:00") + "'";
                    DataSet ds = sys_dal.GetList(strWhere);
                    if (!Utils.DataSetIsNull(ds))
                    {
                        #region 更新c_stock_code表
                        List<string> stringLot = new List<string>();
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            string stock_code = dr["stock_code"].ToString();
                            string stock_name = dr["stock_name"].ToString();
                            string stock_name_capital = dr["stock_name_capital"].ToString();
                            stringLot.Add("insert into c_stock_code(stock_code,stock_name,stock_name_capital,stock_board,circulation_value,circulation) "
                                + "select stock_code,stock_name,stock_name_capital,'0',0,'100亿' from sys_stock_code where stock_code not in(select stock_code from c_stock_code);");
                            stringLot.Add("update c_stock_code set stock_code='" + stock_code + "',stock_name='" + stock_name
                                + "',stock_name_capital='" + stock_name_capital + "' where stock_code='" + stock_code + "';");
                        }
                        //多条更新
                        int num = dal.TranLotTo(stringLot);
                        RunEndCheck("共更新【" + num + "】只股票代码");
                        #endregion


                        #region 日志记录
                        string site_name = ConfigurationManager.AppSettings["site_name"];
                        List<string> stringLot2 = new List<string>();
                        stringLot2.Add("insert into sys_log(userid,logcode,update_time,remark)values (0,1,now(),'更新网站【" + site_name + "】数据库股票代码表共（"
                            + num + "）条记录');");
                        dal.TranLot(stringLot2);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteError("更新股票代码表报错", ex.ToString());
                RunEndCheck(ex.ToString());
            }
            finally
            {
                Monitor.Exit(lockOptNewStock);
            }
        }
        #endregion

        #region ================更新股票代码表持仓
        private void OptStockCodeInfo(Object param, bool sign)
        {
            try
            {

                Monitor.Enter(lockStock);
                DateTime dt = DateTime.Now;
                DateTime trade_time = Convert.ToDateTime(dt.ToString("HH:mm:ss"));
                if ((trade_time >= Convert.ToDateTime("17:30:00") && trade_time < Convert.ToDateTime("17:50:01")) || trade_time >= Convert.ToDateTime("08:30:00") && trade_time < Convert.ToDateTime("08:50:01"))
                {
                    string stock_api = ConfigurationManager.AppSettings["stock_api"];//股票接口
                    switch (stock_api)
                    {
                        case "1"://新浪多票
                            RunEndCheck("新浪多票...");
                            OptStockBySinaLot(param, sign);
                            break;
                        case "2"://腾讯多票
                            RunEndCheck("腾讯多票...");
                            OptStockByQQLot(param, sign);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    RunEndCheck("程序运行中...");
                }
            }
            catch (Exception ex)
            {
                RunEndCheck(ex.ToString());
            }
            finally
            {
                Monitor.Exit(lockStock);
            }
        }

        #region 更新股票持仓By新浪行情
        private void OptStockBySinaLot(Object param, bool sign)
        {
            string strWhere = " 1=1 ";
            string stock_ratecut = ConfigurationManager.AppSettings["stock_ratecut"];//今日需要除权除息的票;
            int count = sys_dal.GetSRecordCount(strWhere), pagesize = 50;
            int z = count / pagesize, y = count % pagesize, pageall = y > 0 ? (z + 1) : z;
            for (int i = 1; i <= pageall; i++)
            {
                StringBuilder sb = new StringBuilder();
                StringBuilder sb_now_price = new StringBuilder();
                DataSet ds = sys_dal.GetSList(strWhere, "", pagesize, i);
                if (!Utils.DataSetIsNull(ds))
                {
                    //更新的股票
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        sb.Append(dr["stock_code_s"].ToString() + ",");
                        sb_now_price.Append(dr["yesterday_price"].ToString() + ",");//收盘价格=当前股价
                    }
                    //股票最新行情
                    string[] stock_list_info = Utils.GetStockListSinaData(sb.ToString()).Split(';');
                    if (stock_list_info.Length > 0)
                    {
                        //漂池分开
                        string[] stock_list_s = sb.ToString().Split(',');
                        string[] now_price_list = sb_now_price.ToString().Split(',');
                        if (stock_list_s.Length > 0)
                        {
                            List<string> stringLot = new List<string>();
                            for (int j = 0; j < stock_list_s.Length - 1; j++)
                            {
                                //单票名称
                                string stock_code_s = stock_list_s[j], stock_code = stock_code_s.Substring(2, 6);
                                //单票行情
                                string[] stock_info = Utils.GetHqInfo(1, stock_code_s, stock_code, stock_list_info[j]).Split(',');
                                if (stock_info.Length > 30)
                                {
                                    //当前价格、昨日收盘价、涨停价、跌停价、行情当前价
                                    decimal now_price = decimal.Parse(now_price_list[j]),
                                        yesterday_price = decimal.Parse(stock_info[2]),
                                        now_price_api = decimal.Parse(stock_info[3]);//当前价格
                                    decimal rate = Utils.MathRoundTwo(yesterday_price > 0 ? ((now_price_api - yesterday_price) / yesterday_price) * 100 : 0);
                                    if (now_price_api <= 0)
                                    {
                                        now_price_api = yesterday_price;
                                    }
                                    //股票名称
                                    string stock_name_api = stock_info[0];
                                    if (now_price_api > 0 && stock_code == stock_info[33])
                                    {
                                        string stock_name_capital = Utils.GetSpellCode(stock_name_api);
                                        stringLot.Add($"update sys_stock_code set stock_name='{stock_name_api}',stock_name_capital='{stock_name_capital}',yesterday_price={now_price_api},rate={rate} where stock_code='{stock_code}';");
                                        stringLot.Add($"update c_stock_code set stock_name='{stock_name_api}',stock_name_capital='{stock_name_capital}',yesterday_price={now_price_api},rate={rate} where stock_code='{stock_code}';");
                                    }
                                }
                            }
                            //删除重复记录
                            stringLot.Add("delete from c_stock_code where id in (select a.id from (SELECT id FROM c_stock_code WHERE stock_code IN (SELECT stock_code FROM c_stock_code GROUP BY stock_code HAVING count(stock_code) > 1) GROUP BY stock_code) a);");
                            stringLot.Add("delete from sys_stock_code where id in (select a.id from (SELECT id FROM sys_stock_code WHERE stock_code IN (SELECT stock_code FROM sys_stock_code GROUP BY stock_code HAVING count(stock_code) > 1) GROUP BY stock_code) a);");
                            //多条更新
                            int num = dal.TranLotTo(stringLot);
                            RunEndCheck("共更新【" + num + "】只票的持仓数据");
                        }
                    }
                }
            }
        }
        #endregion

        #region 更新股票持仓By腾讯行情
        private void OptStockByQQLot(Object param, bool sign)
        {
            string strWhere = " 1=1 ", stock_ratecut = ConfigurationManager.AppSettings["stock_ratecut"];//今日需要除权除息的票;
            int count = sys_dal.GetSRecordCount(strWhere), pagesize = 50;
            int z = count / pagesize, y = count % pagesize, pageall = y > 0 ? (z + 1) : z;
            for (int i = 1; i <= pageall; i++)
            {
                StringBuilder sb = new StringBuilder();
                StringBuilder sb_now_price = new StringBuilder();
                DataSet ds = sys_dal.GetSList(strWhere, "", pagesize, i);
                if (!Utils.DataSetIsNull(ds))
                {
                    RunEndCheck("进入股票持仓更新...");
                    //更新的股票
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        sb.Append(dr["stock_code_s"].ToString() + ",");
                        sb_now_price.Append(dr["yesterday_price"].ToString() + ",");
                    }
                    //股票最新行情
                    string[] stock_list_info = Utils.GetStockListQQData(sb.ToString()).Split(';');
                    if (stock_list_info.Length > 0)
                    {
                        //漂池分开
                        string[] stock_list_s = sb.ToString().Split(',');
                        string[] now_price_list = sb_now_price.ToString().Split(',');
                        if (stock_list_s.Length > 0)
                        {
                            List<string> stringLot = new List<string>();
                            for (int j = 0; j < stock_list_s.Length - 1; j++)
                            {
                                //单票名称
                                string stock_code_s = stock_list_s[j], stock_code = stock_code_s.Substring(2, 6);
                                //单票行情
                                string[] stock_info = Utils.GetHqInfo(2, stock_code_s, stock_code, stock_list_info[j]).Split('~');
                                if (stock_info.Length > 30)
                                {
                                    //当前价格、昨日收盘价、涨停价、跌停价、行情当前价
                                    decimal now_price = decimal.Parse(now_price_list[j]),
                                        yesterday_price = decimal.Parse(stock_info[4]),
                                        now_price_api = decimal.Parse(stock_info[3]);
                                    //涨跌幅
                                    decimal rate = Utils.MathRoundTwo(yesterday_price > 0 ? ((now_price_api - yesterday_price) / yesterday_price) * 100 : 0);
                                    //股票名称
                                    string stock_name_api = stock_info[1];
                                    if (now_price_api > 0 && now_price != now_price_api && stock_code == stock_info[2])
                                    {
                                        string stock_name_capital = Utils.GetSpellCode(stock_name_api);
                                        stringLot.Add($"update sys_stock_code set stock_name='{stock_name_api}',stock_name_capital='{stock_name_capital}',yesterday_price={now_price_api},rate={rate} where stock_code='{stock_code}';");
                                        stringLot.Add($"update c_stock_code set stock_name='{stock_name_api}',stock_name_capital='{stock_name_capital}',yesterday_price={now_price_api},rate={rate} where stock_code='{stock_code}';");
                                    }
                                }
                            }
                            //删除重复记录
                            stringLot.Add("delete from c_stock_code where id in (select a.id from (SELECT id FROM c_stock_code WHERE stock_code IN (SELECT stock_code FROM c_stock_code GROUP BY stock_code HAVING count(stock_code) > 1) GROUP BY stock_code) a);");
                            stringLot.Add("delete from sys_stock_code where id in (select a.id from (SELECT id FROM sys_stock_code WHERE stock_code IN (SELECT stock_code FROM sys_stock_code GROUP BY stock_code HAVING count(stock_code) > 1) GROUP BY stock_code) a);");
                            //多条更新
                            int num = dal.TranLotTo(stringLot);
                            RunEndCheck("共更新【" + num + "】只票的持仓数据");
                        }
                    }
                }
            }
        }
        #endregion

        #endregion

    }
}
