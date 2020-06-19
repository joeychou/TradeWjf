using Common;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BLL
{
    public partial class tasks_opt
    {
        private static readonly object lockSysobj = new object();
        private static OdbcConnection ocConn = new OdbcConnection();
        private static string table = @"" + ConfigurationManager.AppSettings["file_path"];//接口数据位置
        private static string conn =
                       @"Driver={Microsoft dBASE Driver (*.dbf)};SourceType=DBF;" +
                       @"Data Source=" + table + ";Exclusive=No;NULL=NO;" +
                       @"Collate=Machine;BACKGROUNDFETCH=NO;DELETE=NO";
        public tasks_opt() { }
        /// <summary>s
        /// 线程开始工作
        /// </summary>
        public void Action()
        {
            AutoResetEvent ar = new AutoResetEvent(false);
            ThreadPool.RegisterWaitForSingleObject(ar, OptApi, null, 10000, false);//每3秒钟执行一次
        }
        private void RunEndCheck(string ff)
        {
            Console.WriteLine("任务ID:{0}，{1}，{2}", Thread.CurrentThread.ManagedThreadId, DateTime.Now, ff);
        }
        #region 获取网际风股票行情数据
        private void OptApi(Object param, bool sign)
        {
            try
            {
                Monitor.Enter(lockSysobj);
                DateTime dt1 = DateTime.Now;
                //if (Utils.IsTradeTime() == 1)
                //{
                    ocConn.ConnectionString = conn;
                    ocConn.Open();
                    string strSql = "SELECT * FROM " + table;
                    OdbcDataAdapter oda = new OdbcDataAdapter(strSql, ocConn);
                    DataTable dt = new DataTable();
                    oda.Fill(dt);
                    foreach (DataRow dr in dt.Rows)
                    {
                        //行情代码-行情数据
                        RedisHelper.Set<string>(dr["HQZQDM"].ToString(), dr["HQZQJC"].ToString() + "," + dr["HQJRKP"].ToString() + "," + dr["HQZRSP"].ToString()
                            + "," + dr["HQZJCJ"].ToString() + "," + dr["HQZGCJ"].ToString() + "," + dr["HQZDCJ"].ToString() + "," + dr["HQBJW1"].ToString() + "," + dr["HQSJW1"].ToString()
                            + "," + dr["HQCJSL"].ToString() + "," + dr["HQCJJE"].ToString() + "," + dr["HQBSL1"].ToString() + "," + dr["HQBJW1"].ToString() + "," + dr["HQBSL2"].ToString()
                            + "," + dr["HQBJW2"].ToString() + "," + dr["HQBSL3"].ToString() + "," + dr["HQBJW3"].ToString() + "," + dr["HQBSL4"].ToString() + "," + dr["HQBJW4"].ToString()
                            + "," + dr["HQBSL5"].ToString() + "," + dr["HQBJW5"].ToString() + "," + dr["HQSSL1"].ToString() + "," + dr["HQSJW1"].ToString() + "," + dr["HQSSL2"].ToString()
                            + "," + dr["HQSJW2"].ToString() + "," + dr["HQSSL3"].ToString() + "," + dr["HQSJW3"].ToString() + "," + dr["HQSSL4"].ToString() + "," + dr["HQSJW4"].ToString()
                            + "," + dr["HQSSL5"].ToString() + "," + dr["HQSJW5"].ToString() + "," + dt1.ToString("yyyy-MM-dd") + "," + dt1.ToString("HH:mm:ss") + ",00");
                    }
                    ocConn.Close();
                    DateTime dt2 = DateTime.Now;
                    int dt_int_1 = int.Parse(dt1.ToString("HHmmssfff")),
                    dt_int_2 = int.Parse(dt2.ToString("HHmmssfff"));
                    //RunEndCheck("获取其中一条：" + client.Get<string>("000001"));
                    //RunEndCheck("共" + dt.Rows.Count + "条数据");
                    //RunEndCheck(dt1.ToString("HH:mm:ss.fff"));
                    //RunEndCheck(dt2.ToString("HH:mm:ss.fff"));
                    RunEndCheck("更新行情共花费：" + (dt_int_2 - dt_int_1) + "毫秒");
                //}
            }
            catch (Exception ex)
            {
                Log.WriteLog("获取网际风股票行情数据报错：" + ex.ToString(), "");
            }
            finally
            {
                Monitor.Exit(lockSysobj);
            }
        }
        #endregion
    }
}
