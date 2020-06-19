using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Common.wjf
{
    public class StockDrv
    {
        public const int FILE_HISTORY_EX = 2;//补日线数据
        public const int FILE_MINUTE_EX = 4;//补分钟线数据
        public const int FILE_POWER_EX = 6;//补充除权数据
        public const int FILE_5MINUTE_EX = 81;//补5分钟线数据
        public const int FILE_BASE_EX = 0x1000;//钱龙兼容基本资料文件,m_szFileName仅包含文件名
        public const int FILE_NEWS_EX = 0x1002;//新闻类,其类型由m_szFileName中子目录名来定
        public const int RCV_WORK_SENDMSG = 4; //工作方式类型定义-窗口消息方式
        public const int My_Msg_StkData = 0x8001; //指定使用的消息
        public const int RCV_REPORT = 0x3f001234;//股票行情
        public const int RCV_FILEDATA = 0x3f001235;//文件
        public const int RCV_FENBIDATA = 0x3f001301;//分笔数据
        public const UInt32 EKE_HEAD_TAG = 0xffffffff; //数据头结构标记
        /*市场代码说明*/
        public const int SH_MARKET_EX = 0x4853;//上海 SH 的 ASCII码为 53 48 下同
        public const int SZ_MARKET_EX = 0x5A53;//深圳
        public const int HK_MARKET_EX = 0x4B48;//香港
        public const int HQ_MARKET_EX = 0x5148;//香港权证
        public const int NQ_MARKET_EX = 0x514E;//国内期货
        public const int WQ_MARKET_EX = 0x5157;//国外期货
        public const int WH_MARKET_EX = 0x4857;//外汇直盘与交叉盘
        public const int HZ_MARKET_EX = 0x5A48;//恒生期指
        public const int ID_MARKET_EX = 0x4449;//全球指数
        public const int SF_MARKET_EX = 0x4653;//股指期货
        public const int SG_MARKET_EX = 0x4753;//黄金现货

        [DllImport("C:\\wjf380\\WJF380\\Stock.dll")]
        public static extern int Stock_Init(IntPtr nHwnd, int nMsg, int nWorkMode);
        [DllImport("C:\\wjf380\\WJF380\\Stock.dll")]
        public static extern int GetStockByCodeEx(string strCode, int nMarket, StockTools.tagRCV_REPORT_STRUCTEx sRcvReort);
        /// <summary>
        /// 构造函数
        /// </summary>
        public StockDrv()
        {
        }
    }
}
