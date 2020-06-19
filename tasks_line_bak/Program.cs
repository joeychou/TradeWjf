using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace tasks_line_bak
{
    public class Program
    {
        delegate bool ConsoleCtrlDelegate(int dwCtrlType);
        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleCtrlHandler(ConsoleCtrlDelegate HandlerRoutine, bool Add);
        static void Main(string[] args)
        {
            #region ====================用API安装事件处理
            ConsoleCtrlDelegate newDelegate = new ConsoleCtrlDelegate(HandlerRoutine);
            bool bRet = SetConsoleCtrlHandler(newDelegate, true);
            if (bRet == false)  //安装事件处理失败
            {
                Log.WriteLog("添加响应关闭事件失败", "");
            }
            else
            {
                Log.WriteLog("添加响应关闭事件成功", "");
            }
            #endregion

            Console.WriteLine("提示：");
            Console.WriteLine("1、按回车键结束程序；");
            Console.WriteLine("2、获取K线和分时数据及初始化行情数据；");
            Console.WriteLine("3、程序执行中．．．");
            BLL.tasks_k_t_line_bak gp_bll = new BLL.tasks_k_t_line_bak();
            gp_bll.Action();
            Console.ReadLine();
        }

        #region ====================关闭事件处理
        //当用户点击按钮关闭关闭Console时，系统会发送次消息
        private const int CTRL_CLOSE_EVENT = 2;
        //Ctrl+C，系统会发送次消息
        private const int CTRL_C_EVENT = 0;
        //Ctrl+break，系统会发送次消息
        private const int CTRL_BREAK_EVENT = 1;
        //用户退出（注销），系统会发送次消息
        private const int CTRL_LOGOFF_EVENT = 5;
        //系统关闭，系统会发送次消息
        private const int CTRL_SHUTDOWN_EVENT = 6;
        /// <summary>
        /// 处理消息的事件
        /// </summary>
        private static bool HandlerRoutine(int CtrlType)
        {
            switch (CtrlType)
            {
                case CTRL_CLOSE_EVENT:
                    Log.WriteLog("用户点击按钮关闭，退出程序。", "");
                    break;
                case CTRL_C_EVENT:
                    Log.WriteLog("Ctrl+C，退出程序。", "");
                    break;
                case CTRL_BREAK_EVENT:
                    Log.WriteLog("Ctrl+break，退出程序。", "");
                    break;
                case CTRL_LOGOFF_EVENT:
                    Log.WriteLog("用户退出（注销），退出程序。", "");
                    break;
                case CTRL_SHUTDOWN_EVENT:
                    Log.WriteLog("系统关闭，退出程序。", "");
                    break;
            }
            return false;
        }
        #endregion
    }
}