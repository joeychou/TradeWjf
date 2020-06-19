using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BLL
{
    public partial class tasks_test_opt
    {
        private readonly static object lockSysobj = new object();
        private readonly static object lockSysobj1 = new object();
        private readonly static object lockSysobj2 = new object();
        private readonly static object lockSysobj3 = new object();
        private readonly static object lockSysobj4 = new object();
        private readonly static object lockSysobj5 = new object();
        public tasks_test_opt() { }
        /// <summary>
        /// 线程开始工作
        /// </summary>
        public void Action()
        {
            AutoResetEvent ar = new AutoResetEvent(false);
            ThreadPool.RegisterWaitForSingleObject(ar, OptApi, null, 50, false);//每50毫秒执行一次
            ThreadPool.RegisterWaitForSingleObject(ar, OptApi1, null, 100, false);//每3秒钟执行一次
            ThreadPool.RegisterWaitForSingleObject(ar, OptApi2, null, 120, false);//每3秒钟执行一次
            ThreadPool.RegisterWaitForSingleObject(ar, OptApi3, null, 60, false);//每3秒钟执行一次
            ThreadPool.RegisterWaitForSingleObject(ar, OptApi4, null, 30, false);//每3秒钟执行一次
            ThreadPool.RegisterWaitForSingleObject(ar, OptApi5, null, 80, false);//每3秒钟执行一次
        }
        private void RunEndCheck(string ff)
        {
            Console.WriteLine("任务ID:{0}，{1}，{2}", Thread.CurrentThread.ManagedThreadId, DateTime.Now, ff);
        }
        #region 获取行情数据
        private void OptApi(Object param, bool sign)
        {
            try
            {
                Monitor.Enter(lockSysobj);
                DateTime dt1 = DateTime.Now;
                RunEndCheck("行情数据：" + Utils.GetStockData("000001",0,0));
                DateTime dt2 = DateTime.Now;
                int dt_int_1 = int.Parse(dt1.ToString("HHmmssfff")),
                dt_int_2 = int.Parse(dt2.ToString("HHmmssfff"));
                RunEndCheck("共花费：" + (dt_int_2 - dt_int_1) + "毫秒");
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
        private void OptApi1(Object param, bool sign)
        {
            try
            {
                Monitor.Enter(lockSysobj1);
                DateTime dt1 = DateTime.Now;
                RunEndCheck("行情数据：" + Utils.GetStockData("000002",0,0));
                DateTime dt2 = DateTime.Now;
                int dt_int_1 = int.Parse(dt1.ToString("HHmmssfff")),
                dt_int_2 = int.Parse(dt2.ToString("HHmmssfff"));
                RunEndCheck("共花费：" + (dt_int_2 - dt_int_1) + "毫秒");
            }
            catch (Exception ex)
            {
                Log.WriteLog("获取网际风股票行情数据报错：" + ex.ToString(), "");
            }
            finally
            {
                Monitor.Exit(lockSysobj1);
            }
        }
        private void OptApi2(Object param, bool sign)
        {
            try
            {
                Monitor.Enter(lockSysobj2);
                DateTime dt1 = DateTime.Now;
                RunEndCheck("行情数据：" + Utils.GetStockData("000004",0,0));
                DateTime dt2 = DateTime.Now;
                int dt_int_1 = int.Parse(dt1.ToString("HHmmssfff")),
                dt_int_2 = int.Parse(dt2.ToString("HHmmssfff"));
                RunEndCheck("共花费：" + (dt_int_2 - dt_int_1) + "毫秒");
            }
            catch (Exception ex)
            {
                Log.WriteLog("获取网际风股票行情数据报错：" + ex.ToString(), "");
            }
            finally
            {
                Monitor.Exit(lockSysobj2);
            }
        }
        private void OptApi3(Object param, bool sign)
        {
            try
            {
                Monitor.Enter(lockSysobj3);
                DateTime dt1 = DateTime.Now;
                RunEndCheck("行情数据：" + Utils.GetStockData("000006",0,0));
                DateTime dt2 = DateTime.Now;
                int dt_int_1 = int.Parse(dt1.ToString("HHmmssfff")),
                dt_int_2 = int.Parse(dt2.ToString("HHmmssfff"));
                RunEndCheck("共花费：" + (dt_int_2 - dt_int_1) + "毫秒");
            }
            catch (Exception ex)
            {
                Log.WriteLog("获取网际风股票行情数据报错：" + ex.ToString(), "");
            }
            finally
            {
                Monitor.Exit(lockSysobj3);
            }
        }
        private void OptApi4(Object param, bool sign)
        {
            try
            {
                Monitor.Enter(lockSysobj4);
                DateTime dt1 = DateTime.Now;
                RunEndCheck("行情数据：" + Utils.GetStockData("000008",0,0));
                DateTime dt2 = DateTime.Now;
                int dt_int_1 = int.Parse(dt1.ToString("HHmmssfff")),
                dt_int_2 = int.Parse(dt2.ToString("HHmmssfff"));
                RunEndCheck("共花费：" + (dt_int_2 - dt_int_1) + "毫秒");
            }
            catch (Exception ex)
            {
                Log.WriteLog("获取网际风股票行情数据报错：" + ex.ToString(), "");
            }
            finally
            {
                Monitor.Exit(lockSysobj4);
            }
        }
        private void OptApi5(Object param, bool sign)
        {
            try
            {
                Monitor.Enter(lockSysobj5);
                DateTime dt1 = DateTime.Now;
                RunEndCheck("行情数据：" + Utils.GetStockData("000009,300153,002052,002302,300480",0,0));
                DateTime dt2 = DateTime.Now;
                int dt_int_1 = int.Parse(dt1.ToString("HHmmssfff")),
                dt_int_2 = int.Parse(dt2.ToString("HHmmssfff"));
                RunEndCheck("共花费：" + (dt_int_2 - dt_int_1) + "毫秒");
            }
            catch (Exception ex)
            {
                Log.WriteLog("获取网际风股票行情数据报错：" + ex.ToString(), "");
            }
            finally
            {
                Monitor.Exit(lockSysobj5);
            }
        }
        #endregion
    }
}
