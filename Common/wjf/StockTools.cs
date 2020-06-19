using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Common.wjf
{
    /// <summary>
    /// 网际风获取接口
    /// </summary>
    public class StockTools
    {
        /// <summary>
        /// 文件结构
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct tagRCV_FILE_HEADEx
        {
            public int m_dwAttrib;// 文件子类型
            public int m_dwLen;// 文件长度
            public int m_dwSerialNoorTime;//文件序列号或时间.
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
            public char[] m_szFileName;// 文件名 or URL
            //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            //public string m_szFileName;
        }

        /// <summary>
        /// 文件数据结构
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct tagRCV_DATA
        {
            public int m_wDataType;// 文件类型
            public int m_nPacketNum;// 记录数,参见注一
            [MarshalAs(UnmanagedType.Struct)]
            public tagRCV_FILE_HEADEx m_File;// 文件接口
            public int m_bDISK;// 文件是否已存盘的文件
            public IntPtr m_pData;
        } ;

        /// <summary>
        /// 补历史(日线)数据
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct tagRCV_HISTORY_STRUCTEx
        {
            public Int32 m_time;//UCT
            public Single m_fOpen;//开盘
            public Single m_fHigh;//最高
            public Single m_fLow;//最低
            public Single m_fClose;//收盘
            public Single m_fVolume;//量
            public Single m_fAmount;//额
            public UInt16 m_wAdvance;//涨数,仅大盘有效
            public UInt16 m_wDecline;//跌数,仅大盘有效
        };

        /// <summary>
        /// 补除权除息数据
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct tagRCV_POWER_STRUCTEx
        {
            public Int32 m_time;//UCT
            public float m_fGive;//每股送
            public float m_fPei;//每股配
            public float m_fPeiPrice;//配股价,仅当 m_fPei!=0.0f 时有效
            public float m_fProfit;//每股红利
        };

        /// <summary>
        /// 补除权除息数据
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct tagRCV_POWER_STRUCTEx2
        {
            public Int32 m_time;//UCT
            public Single m_fGive;//每股送
            public Single m_fPei;//每股配
            public Single m_fPeiPrice;//配股价,仅当 m_fPei!=0.0f 时有效
            public Single m_fProfit;//每股红利
        };

        /// <summary>
        /// 补充分时线数据
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct tagRCV_MINUTE_STRUCTEx
        {
            public Int32 m_time;// UCT
            public Single m_fPrice;
            public Single m_fVolume;
            public Single m_fAmount;
        };

        /// <summary>
        /// 数据头
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct tagRCV_EKE_HEADEx
        {
            public UInt32 m_dwHeadTag;// = EKE_HEAD_TAG
            //public UInt16 m_wMarket;// 市场类型
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public char[] m_wMarket;//市场类型
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
            public string m_szLabel;//代码,以'\0'结尾   数组大小为STKLABEL_LEN，在c++描述中为char[10]
        };

        /// <summary>
        /// 行情结构
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct tagRCV_REPORT_STRUCTEx//: public CObject
        {
            public UInt16 m_cbSize;//结构大小
            public Int32 m_time;//交易时间
            //public UInt16 m_wMarket;//股票市场类型
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public char[] m_wMarket;// 股票市场类型
            //[MarshalAs(   UnmanagedType.ByValArray,   SizeConst=10)]   
            //public   char[]  m_szLabel; //代码,以'\0'结尾   数组大小为STKLABEL_LEN，在c++描述中为char[10]     
            //[MarshalAs(   UnmanagedType.ByValArray,   SizeConst=32)]   
            // public   char[]  m_szName;  //名称,以'\0'结尾   数组大小为STKNAME_LEN,在c++描述中为char[32]     
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
            public string m_szLabel;//代码,以'\0'结尾   数组大小为STKLABEL_LEN，在c++描述中为char[10]     
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string m_szName;//名称,以'\0'结尾   数组大小为STKNAME_LEN,在c++描述中为char[32]     

            public Single m_fLastClose;//昨收
            public Single m_fOpen;//今开
            public Single m_fHigh;//最高
            public Single m_fLow;//最低
            public Single m_fNewPrice;//最新
            public Single m_fVolume;//成交量
            public Single m_fAmount;//成交额

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public Single[] m_fBuyPrice;//申买价1,2,3
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public Single[] m_fBuyVolume; //申买量1,2,3
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public Single[] m_fSellPrice;//申卖价1,2,3
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public Single[] m_fSellVolume;//申卖量1,2,3

            public Single m_fBuyPrice4; //申买价4
            public Single m_fBuyVolume4;//申买量4
            public Single m_fSellPrice4;//申卖价4
            public Single m_fSellVolume4;//申卖量4

            public Single m_fBuyPrice5;//申买价5
            public Single m_fBuyVolume5;//申买量5
            public Single m_fSellPrice5;//申卖价5
            public Single m_fSellVolume5;//申卖量5
        };
    }
}
