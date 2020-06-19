using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Common
{
    /// <summary>
    /// 通达信交易接口定义
    /// </summary>
    public class TradeX
    {
        /// <summary>
        /// 打开通达信实例
        /// </summary>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        public static extern void OpenTdx();

        /// <summary>
        /// 关闭通达信实例
        /// </summary>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        public static extern void CloseTdx();

        /// <summary>
        /// 交易账户登录
        /// </summary>
        /// <param name="IP">券商交易服务器IP</param>
        /// <param name="Port">券商交易服务器端口</param>
        /// <param name="Version">设置通达信客户端的版本号:6.00或8.00</param>
        /// <param name="YybId">营业部编码：国泰君安为7</param>
        /// <param name="AccountNo">资金账号</param>
        /// <param name="TradeAccount">交易帐号与资金帐号相同</param>
        /// <param name="JyPassword">交易密码</param>
        /// <param name="TxPassword">通讯密码为空</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串</param>
        /// <returns>客户端ID，失败时返回-1。</returns>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        public static extern int Logon(string IP, short Port, string Version, short YybId, string AccountNo, string TradeAccount, string JyPassword, string TxPassword, StringBuilder ErrInfo);

        /// <summary>
        /// 交易账户注销
        /// </summary>
        /// <param name="ClientID">客户端ID</param>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        public static extern void Logoff(int ClientID);

        /// <summary>
        /// 查询各种交易数据
        /// </summary>
        /// <param name="ClientID">客户端ID</param>
        /// <param name="Category">表示查询信息的种类，0资金  1股份   2当日委托  3当日成交     4可撤单   5股东代码  6融资余额   7融券余额  8可融证券</param>
        /// <param name="Result">此API执行返回后，Result内保存了返回的查询数据, 形式为表格数据，行数据之间通过\n字符分割，列数据之间通过\t分隔。一般要分配1024*1024字节的空间。出错时为空字符串。</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串</param>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        public static extern void QueryData(int ClientID, int Category, StringBuilder Result, StringBuilder ErrInfo);

        /// <summary>
        /// 查询各种历史数据
        /// </summary>
        /// <param name="ClientID">客户端ID</param>
        /// <param name="Category">表示查询信息的种类，0历史委托  1历史成交   2交割单</param>
        /// <param name="StartDate">表示开始日期，格式为yyyyMMdd,比如2014年3月1日为  20140301</param>
        /// <param name="EndDate">表示结束日期，格式为yyyyMMdd,比如2014年3月1日为  20140301</param>
        /// <param name="Result">同上</param>
        /// <param name="ErrInfo">同上</param>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        public static extern void QueryHistoryData(int ClientID, int Category, string StartDate, string EndDate, StringBuilder Result, StringBuilder ErrInfo);

        /// <summary>
        /// 批量查询各种交易数据,用数组传入每个委托的参数，数组第i个元素表示第i个查询的相应参数。
        /// </summary>
        /// <param name="ClientID">客户端ID</param>
        /// <param name="Category">表示查询信息的种类，0资金  1股份   2当日委托  3当日成交     4可撤单   5股东代码  6融资余额   7融券余额  8可融证券</param>
        /// <param name="Count"></param>
        /// <param name="Result">同上</param>
        /// <param name="ErrInfo">同上</param>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        public static extern void QueryDatas(int ClientID, int[] Category, int Count, IntPtr[] Result, IntPtr[] ErrInfo);

        /// <summary>
        /// 下委托交易证券
        /// </summary>
        /// <param name="ClientID">客户端ID</param>
        /// <param name="Category">表示委托的种类，0买入 1卖出  2融资买入  3融券卖出   4买券还券   5卖券还款  6现券还券</param>
        /// <param name="PriceType">表示报价方式 0  上海限价委托 深圳限价委托 1深圳对方最优价格  2深圳本方最优价格  3深圳即时成交剩余撤销  4上海五档即成剩撤 深圳五档即成剩撤 5深圳全额成交或撤销 6上海五档即成转限价</param>
        /// <param name="Gddm">股东代码</param>
        /// <param name="Zqdm">证券代码</param>
        /// <param name="Price">委托价格</param>
        /// <param name="Quantity">委托数量</param>
        /// <param name="Result">同上</param>
        /// <param name="ErrInfo">同上</param>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        public static extern void SendOrder(int ClientID, int Category, int PriceType, string Gddm, string Zqdm, float Price, int Quantity, StringBuilder Result, StringBuilder ErrInfo);

        /// <summary>
        /// 批量下委托交易
        /// </summary>
        /// <param name="ClientID">客户端ID</param>
        /// <param name="Category">表示委托的种类，0买入 1卖出  2融资买入  3融券卖出   4买券还券   5卖券还款  6现券还券</param>
        /// <param name="PriceType">表示报价方式 0  上海限价委托 深圳限价委托 1深圳对方最优价格  2深圳本方最优价格  3深圳即时成交剩余撤销  4上海五档即成剩撤 深圳五档即成剩撤 5深圳全额成交或撤销 6上海五档即成转限价</param>
        /// <param name="Gddm">股东代码</param>
        /// <param name="Zqdm">证券代码</param>
        /// <param name="Price">委托价格</param>
        /// <param name="Quantity">委托数量</param>
        /// <param name="Count">批量下单数量</param>
        /// <param name="Result">同上</param>
        /// <param name="ErrInfo">同上</param>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        public static extern void SendOrders(int ClientID, int[] Category, int[] PriceType, string[] Gddm, string[] Zqdm, float[] Price, int[] Quantity, int Count, IntPtr[] Result, IntPtr[] ErrInfo);

        /// <summary>
        /// 撤委托
        /// </summary>
        /// <param name="ClientID">客户端ID</param>
        /// <param name="ExchangeID">交易所类别， 上海A1，深圳A0(招商证券普通账户深圳是2)</param>
        /// <param name="hth">委托编号</param>
        /// <param name="Result">同上</param>
        /// <param name="ErrInfo">同上</param>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        public static extern void CancelOrder(int ClientID, string ExchangeID, string hth, StringBuilder Result, StringBuilder ErrInfo);

        /// <summary>
        /// 批量撤单
        /// </summary>
        /// <param name="ClientID"></param>
        /// <param name="ExchangeID">交易所类别， 上海A1，深圳A0(招商证券普通账户深圳是2)</param>
        /// <param name="hth"></param>
        /// <param name="Count"></param>
        /// <param name="Result"></param>
        /// <param name="ErrInfo"></param>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        public static extern void CancelOrders(int ClientID, string[] ExchangeID, string[] hth, int Count, IntPtr[] Result, IntPtr[] ErrInfo);

        /// <summary>
        /// 获取证券的实时五档行情
        /// </summary>
        /// <param name="ClientID">客户端ID</param>
        /// <param name="Zqdm">证券代码</param>
        /// <param name="Result">同上</param>
        /// <param name="ErrInfo">同上</param>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        public static extern void GetQuote(int ClientID, string Zqdm, StringBuilder Result, StringBuilder ErrInfo);

        /// <summary>
        /// 批量获取证券的实时五档行情
        /// </summary>
        /// <param name="ClientID">客户端ID</param>
        /// <param name="Zqdm">证券代码</param>
        /// <param name="Count">证券合约数量</param>
        /// <param name="Result">同</param>
        /// <param name="ErrInfo">同</param>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        public static extern void GetQuotes(int ClientID, string[] Zqdm, int Count, IntPtr[] Result, IntPtr[] ErrInfo);

        /// <summary>
        /// 融资融券直接还款
        /// </summary>
        /// <param name="ClientID">客户端ID</param>
        /// <param name="Amount">还款金额</param>
        /// <param name="Result">同上</param>
        /// <param name="ErrInfo">同上</param>

        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        public static extern void Repay(int ClientID, string Amount, StringBuilder Result, StringBuilder ErrInfo);

        /// <summary>
        ///  连接通达信行情服务器,服务器地址可在券商软件登录界面中的通讯设置中查得
        /// </summary>
        /// <param name="IP">服务器IP</param>
        /// <param name="Port">服务器端口</param>
        /// <param name="Result">此API执行返回后，Result内保存了返回的查询数据, 形式为表格数据，行数据之间通过\n字符分割，列数据之间通过\t分隔。一般要分配1024*1024字节的空间。出错时为空字符串。</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串。</param>
        /// <returns>成功返货true, 失败返回false</returns>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool TdxHq_Connect(string IP, int Port, StringBuilder Result, StringBuilder ErrInfo);

        /// <summary>
        /// 断开同服务器的连接
        /// </summary>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        public static extern void TdxHq_Disconnect();

        /// <summary>
        /// 获取市场内所有证券的数量
        /// </summary>
        /// <param name="Market">市场代码,   0->深圳     1->上海</param>
        /// <param name="Result">此API执行返回后，Result内保存了返回的证券数量</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串。</param>
        /// <returns>成功返货true, 失败返回false</returns>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool TdxHq_GetSecurityCount(byte Market, ref short Result, StringBuilder ErrInfo);

        /// <summary>
        /// 获取市场内从某个位置开始的1000支股票的股票代码
        /// </summary>
        /// <param name="Market">市场代码,   0->深圳     1->上海</param>
        /// <param name="Start">股票开始位置,第一个股票是0, 第二个是1, 依此类推,位置信息依据TdxL2Hq_GetSecurityCount返回的证券总数确定</param>
        /// <param name="Count">API执行后,保存了实际返回的股票数目,</param>
        /// <param name="Result">此API执行返回后，Result内保存了返回的证券代码信息,形式为表格数据，行数据之间通过\n字符分割，列数据之间通过\t分隔。一般要分配1024*1024字节的空间。出错时为空字符串。</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串。</param>
        /// <returns>成功返货true, 失败返回false</returns>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool TdxHq_GetSecurityList(byte Market, short Start, ref short Count, StringBuilder Result, StringBuilder ErrInfo);

        /// <summary>
        /// 获取证券的K线数据
        /// </summary>
        /// <param name="Category">K线种类, 0->5分钟K线    1->15分钟K线    2->30分钟K线  3->1小时K线    4->日K线  5->周K线  6->月K线  7->1分钟    10->季K线  11->年K线< / param>
        /// <param name="Market">市场代码,   0->深圳     1->上海</param>
        /// <param name="Zqdm">证券代码</param>
        /// <param name="Start">K线开始位置,最后一条K线位置是0, 前一条是1, 依此类推</param>
        /// <param name="Count">API执行前,表示用户要请求的K线数目, API执行后,保存了实际返回的K线数目, 最大值800</param>
        /// <param name="Result">此API执行返回后，Result内保存了返回的查询数据, 形式为表格数据，行数据之间通过\n字符分割，列数据之间通过\t分隔。一般要分配1024*1024字节的空间。出错时为空字符串。</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串。</param>
        /// <returns>成功返货true, 失败返回false</returns>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool TdxHq_GetSecurityBars(byte Category, byte Market, string Zqdm, short Start, ref short Count, StringBuilder Result, StringBuilder ErrInfo);

        /// <summary>
        /// 获取指数的K线数据
        /// </summary>
        /// <param name="Category">K线种类, 0->5分钟K线    1->15分钟K线    2->30分钟K线  3->1小时K线    4->日K线  5->周K线  6->月K线  7->1分钟  8->1分钟K线  9->日K线  10->季K线  11->年K线< / param>
        /// <param name="Market">市场代码,   0->深圳     1->上海</param>
        /// <param name="Zqdm">证券代码</param>
        /// <param name="Start">K线开始位置,最后一条K线位置是0, 前一条是1, 依此类推</param>
        /// <param name="Count">API执行前,表示用户要请求的K线数目, API执行后,保存了实际返回的K线数目, 最大值800</param>
        /// <param name="Result">此API执行返回后，Result内保存了返回的查询数据, 形式为表格数据，行数据之间通过\n字符分割，列数据之间通过\t分隔。一般要分配1024*1024字节的空间。出错时为空字符串。</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串。</param>
        /// <returns>成功返货true, 失败返回false</returns>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool TdxHq_GetIndexBars(byte Category, byte Market, string Zqdm, short Start, ref short Count, StringBuilder Result, StringBuilder ErrInfo);

        /// <summary>
        /// 获取分时数据
        /// </summary>
        /// <param name="Market">市场代码,   0->深圳     1->上海</param>
        /// <param name="Zqdm">证券代码</param>
        /// <param name="Result">此API执行返回后，Result内保存了返回的查询数据, 形式为表格数据，行数据之间通过\n字符分割，列数据之间通过\t分隔。一般要分配1024*1024字节的空间。出错时为空字符串。</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串。</param>
        /// <returns>成功返货true, 失败返回false</returns>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool TdxHq_GetMinuteTimeData(byte Market, string Zqdm, StringBuilder Result, StringBuilder ErrInfo);

        /// <summary>
        /// 获取历史分时数据
        /// </summary>
        /// <param name="Market">市场代码,   0->深圳     1->上海</param>
        /// <param name="Zqdm">证券代码</param>
        /// <param name="Date">日期, 比如2014年1月1日为整数20140101</param>
        /// <param name="Result">此API执行返回后，Result内保存了返回的查询数据, 形式为表格数据，行数据之间通过\n字符分割，列数据之间通过\t分隔。一般要分配1024*1024字节的空间。出错时为空字符串。</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串。</param>
        /// <returns>成功返货true, 失败返回false</returns>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool TdxHq_GetHistoryMinuteTimeData(byte Market, string Zqdm, int Date, StringBuilder Result, StringBuilder ErrInfo);

        /// <summary>
        /// 获取分时成交数据
        /// </summary>
        /// <param name="Market">市场代码,   0->深圳     1->上海</param>
        /// <param name="Zqdm">证券代码</param>
        /// <param name="Start">K线开始位置,最后一条K线位置是0, 前一条是1, 依此类推</param>
        /// <param name="Count">API执行前,表示用户要请求的记录数目, API执行后,保存了实际返回的记录数目</param>
        /// <param name="Result">此API执行返回后，Result内保存了返回的查询数据, 形式为表格数据，行数据之间通过\n字符分割，列数据之间通过\t分隔。一般要分配1024*1024字节的空间。出错时为空字符串。</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串。</param>
        /// <returns>成功返货true, 失败返回false</returns>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool TdxHq_GetTransactionData(byte Market, string Zqdm, short Start, ref short Count, StringBuilder Result, StringBuilder ErrInfo);

        /// <summary>
        /// 获取历史分时成交数据
        /// </summary>
        /// <param name="Market">市场代码,   0->深圳     1->上海</param>
        /// <param name="Zqdm">证券代码</param>
        /// <param name="Start">K线开始位置,最后一条K线位置是0, 前一条是1, 依此类推</param>
        /// <param name="Count">API执行前,表示用户要请求的记录数目, API执行后,保存了实际返回的记录数目</param>
        /// <param name="Date">日期, 比如2014年1月1日为整数20140101</param>
        /// <param name="Result">此API执行返回后，Result内保存了返回的查询数据, 形式为表格数据，行数据之间通过\n字符分割，列数据之间通过\t分隔。一般要分配1024*1024字节的空间。出错时为空字符串。</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串。</param>
        /// <returns>成功返货true, 失败返回false</returns>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool TdxHq_GetHistoryTransactionData(byte Market, string Zqdm, short Start, ref short Count, int Date, StringBuilder Result, StringBuilder ErrInfo);

        /// <summary>
        /// 获取五档报价
        /// </summary>
        /// <param name="Market">市场代码,   0->深圳     1->上海</param>
        /// <param name="Zqdm">证券代码</param>
        /// <param name="Count">API执行前,表示证券代码的记录数目, API执行后,保存了实际返回的记录数目</param>
        /// <param name="Result">此API执行返回后，Result内保存了返回的查询数据, 形式为表格数据，行数据之间通过\n字符分割，列数据之间通过\t分隔。一般要分配1024*1024字节的空间。出错时为空字符串。</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串。</param>
        /// <returns>成功返货true, 失败返回false</returns>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool TdxHq_GetSecurityQuotes(byte[] Market, string[] Zqdm, ref short Count, StringBuilder Result, StringBuilder ErrInfo);

        /// <summary>
        /// 获取F10资料的分类
        /// </summary>
        /// <param name="Market">市场代码,   0->深圳     1->上海</param>
        /// <param name="Zqdm">证券代码</param>
        /// <param name="Result">此API执行返回后，Result内保存了返回的查询数据, 形式为表格数据，行数据之间通过\n字符分割，列数据之间通过\t分隔。一般要分配1024*1024字节的空间。出错时为空字符串。</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串。</param>
        /// <returns>成功返货true, 失败返回false</returns>
        /// 
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool TdxHq_GetCompanyInfoCategory(byte Market, string Zqdm, StringBuilder Result, StringBuilder ErrInfo);

        /// <summary>
        /// 获取F10资料的某一分类的内容
        /// </summary>
        /// <param name="Market">市场代码,   0->深圳     1->上海</param>
        /// <param name="Zqdm">证券代码</param>
        /// <param name="FileName">类目的文件名, 由TdxHq_GetCompanyInfoCategory返回信息中获取</param>
        /// <param name="Start">类目的开始位置, 由TdxHq_GetCompanyInfoCategory返回信息中获取</param>
        /// <param name="Length">类目的长度, 由TdxHq_GetCompanyInfoCategory返回信息中获取</param>
        /// <param name="Result">此API执行返回后，Result内保存了返回的查询数据,出错时为空字符串。</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串。</param>
        /// <returns>成功返货true, 失败返回false</returns>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool TdxHq_GetCompanyInfoContent(byte Market, string Zqdm, string FileName, int Start, int Length, StringBuilder Result, StringBuilder ErrInfo);

        /// <summary>
        /// 获取除权除息信息
        /// </summary>
        /// <param name="Market">市场代码,   0->深圳     1->上海</param>
        /// <param name="Zqdm">证券代码</param>
        /// <param name="Result">此API执行返回后，Result内保存了返回的查询数据,出错时为空字符串。</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串。</param>
        /// <returns>成功返货true, 失败返回false</returns>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool TdxHq_GetXDXRInfo(byte Market, string Zqdm, StringBuilder Result, StringBuilder ErrInfo);

        /// <summary>
        /// 获取财务信息
        /// </summary>
        /// <param name="Market">市场代码,   0->深圳     1->上海</param>
        /// <param name="Zqdm">证券代码</param>
        /// <param name="Result">此API执行返回后，Result内保存了返回的查询数据,出错时为空字符串。</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串。</param>
        /// <returns>成功返货true, 失败返回false</returns>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool TdxHq_GetFinanceInfo(byte Market, string Zqdm, StringBuilder Result, StringBuilder ErrInfo);

        // L2行情  ************************************************************************************************

        /// <summary>
        ///  连接通达信行情服务器,服务器地址可在券商软件登录界面中的通讯设置中查得
        /// </summary>
        /// <param name="IP">服务器IP</param>
        /// <param name="Port">服务器端口</param>
        /// <param name="Result">此API执行返回后，Result内保存了返回的查询数据, 形式为表格数据，行数据之间通过\n字符分割，列数据之间通过\t分隔。一般要分配1024*1024字节的空间。出错时为空字符串。</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串。</param>
        /// <returns>成功返货true, 失败返回false</returns>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        //[return: MarshalAs(UnmanagedType.I1)]
        public static extern bool TdxL2Hq_Connect(string IP, int Port, string L2User, string L2Password, StringBuilder Result, StringBuilder ErrInfo);

        /// <summary>
        /// 断开同服务器的连接
        /// </summary>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        public static extern void TdxL2Hq_Disconnect();

        /// <summary>
        /// 设置超时的函数
        /// 缺省connect - 3000ms, send - 1000ms, recv - 1000ms, 这些在Tdx*_Connect函数中缺省设置
        /// </summary>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        public static extern void TdxL2Hq_SetTimeout(int nReadTimeout, int nWriteTimeout);

        /// <summary>
        /// 获取市场内所有证券的数量
        /// </summary>
        /// <param name="Market">市场代码,   0->深圳     1->上海</param>
        /// <param name="Result">此API执行返回后，Result内保存了返回的证券数量</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串。</param>
        /// <returns>成功返货true, 失败返回false</returns>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool TdxL2Hq_GetSecurityCount(byte Market, ref short Result, StringBuilder ErrInfo);

        /// <summary>
        /// 获取市场内从某个位置开始的1000支股票的股票代码
        /// </summary>
        /// <param name="Market">市场代码,   0->深圳     1->上海</param>
        /// <param name="Start">股票开始位置,第一个股票是0, 第二个是1, 依此类推,位置信息依据TdxL2Hq_GetSecurityCount返回的证券总数确定</param>
        /// <param name="Count">API执行后,保存了实际返回的股票数目,</param>
        /// <param name="Result">此API执行返回后，Result内保存了返回的证券代码信息,形式为表格数据，行数据之间通过\n字符分割，列数据之间通过\t分隔。一般要分配1024*1024字节的空间。出错时为空字符串。</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串。</param>
        /// <returns>成功返货true, 失败返回false</returns>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool TdxL2Hq_GetSecurityList(byte Market, short Start, ref short Count, StringBuilder Result, StringBuilder ErrInfo);

        /// <summary>
        /// 获取证券的K线数据
        /// </summary>
        /// <param name="Category">K线种类, 0->5分钟K线    1->15分钟K线    2->30分钟K线  3->1小时K线    4->日K线  5->周K线  6->月K线  7->1分钟    10->季K线  11->年K线< / param>
        /// <param name="Market">市场代码,   0->深圳     1->上海</param>
        /// <param name="Zqdm">证券代码</param>
        /// <param name="Start">K线开始位置,最后一条K线位置是0, 前一条是1, 依此类推</param>
        /// <param name="Count">API执行前,表示用户要请求的K线数目, API执行后,保存了实际返回的K线数目, 最大值800</param>
        /// <param name="Result">此API执行返回后，Result内保存了返回的查询数据, 形式为表格数据，行数据之间通过\n字符分割，列数据之间通过\t分隔。一般要分配1024*1024字节的空间。出错时为空字符串。</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串。</param>
        /// <returns>成功返货true, 失败返回false</returns>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool TdxL2Hq_GetSecurityBars(byte Category, byte Market, string Zqdm, short Start, ref short Count, StringBuilder Result, StringBuilder ErrInfo);

        /// <summary>
        /// 获取指数的K线数据
        /// </summary>
        /// <param name="Category">K线种类, 0->5分钟K线    1->15分钟K线    2->30分钟K线  3->1小时K线    4->日K线  5->周K线  6->月K线  7->1分钟  8->1分钟K线  9->日K线  10->季K线  11->年K线< / param>
        /// <param name="Market">市场代码,   0->深圳     1->上海</param>
        /// <param name="Zqdm">证券代码</param>
        /// <param name="Start">K线开始位置,最后一条K线位置是0, 前一条是1, 依此类推</param>
        /// <param name="Count">API执行前,表示用户要请求的K线数目, API执行后,保存了实际返回的K线数目, 最大值800</param>
        /// <param name="Result">此API执行返回后，Result内保存了返回的查询数据, 形式为表格数据，行数据之间通过\n字符分割，列数据之间通过\t分隔。一般要分配1024*1024字节的空间。出错时为空字符串。</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串。</param>
        /// <returns>成功返货true, 失败返回false</returns>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool TdxL2Hq_GetIndexBars(byte Category, byte Market, string Zqdm, short Start, ref short Count, StringBuilder Result, StringBuilder ErrInfo);

        /// <summary>
        /// 获取分时数据
        /// </summary>
        /// <param name="Market">市场代码,   0->深圳     1->上海</param>
        /// <param name="Zqdm">证券代码</param>
        /// <param name="Result">此API执行返回后，Result内保存了返回的查询数据, 形式为表格数据，行数据之间通过\n字符分割，列数据之间通过\t分隔。一般要分配1024*1024字节的空间。出错时为空字符串。</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串。</param>
        /// <returns>成功返货true, 失败返回false</returns>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool TdxL2Hq_GetMinuteTimeData(byte Market, string Zqdm, StringBuilder Result, StringBuilder ErrInfo);

        /// <summary>
        /// 获取历史分时数据
        /// </summary>
        /// <param name="Market">市场代码,   0->深圳     1->上海</param>
        /// <param name="Zqdm">证券代码</param>
        /// <param name="Date">日期, 比如2014年1月1日为整数20140101</param>
        /// <param name="Result">此API执行返回后，Result内保存了返回的查询数据, 形式为表格数据，行数据之间通过\n字符分割，列数据之间通过\t分隔。一般要分配1024*1024字节的空间。出错时为空字符串。</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串。</param>
        /// <returns>成功返货true, 失败返回false</returns>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool TdxL2Hq_GetHistoryMinuteTimeData(byte Market, string Zqdm, int Date, StringBuilder Result, StringBuilder ErrInfo);

        /// <summary>
        /// 获取分时成交数据
        /// </summary>
        /// <param name="Market">市场代码,   0->深圳     1->上海</param>
        /// <param name="Zqdm">证券代码</param>
        /// <param name="Start">K线开始位置,最后一条K线位置是0, 前一条是1, 依此类推</param>
        /// <param name="Count">API执行前,表示用户要请求的记录数目, API执行后,保存了实际返回的记录数目</param>
        /// <param name="Result">此API执行返回后，Result内保存了返回的查询数据, 形式为表格数据，行数据之间通过\n字符分割，列数据之间通过\t分隔。一般要分配1024*1024字节的空间。出错时为空字符串。</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串。</param>
        /// <returns>成功返货true, 失败返回false</returns>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool TdxL2Hq_GetTransactionData(byte Market, string Zqdm, short Start, ref short Count, StringBuilder Result, StringBuilder ErrInfo);

        /// <summary>
        /// 获取历史分时成交数据
        /// </summary>
        /// <param name="Market">市场代码,   0->深圳     1->上海</param>
        /// <param name="Zqdm">证券代码</param>
        /// <param name="Start">K线开始位置,最后一条K线位置是0, 前一条是1, 依此类推</param>
        /// <param name="Count">API执行前,表示用户要请求的记录数目, API执行后,保存了实际返回的记录数目</param>
        /// <param name="Date">日期, 比如2014年1月1日为整数20140101</param>
        /// <param name="Result">此API执行返回后，Result内保存了返回的查询数据, 形式为表格数据，行数据之间通过\n字符分割，列数据之间通过\t分隔。一般要分配1024*1024字节的空间。出错时为空字符串。</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串。</param>
        /// <returns>成功返货true, 失败返回false</returns>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool TdxL2Hq_GetHistoryTransactionData(byte Market, string Zqdm, short Start, ref short Count, int Date, StringBuilder Result, StringBuilder ErrInfo);

        /// <summary>
        /// 获取五档报价
        /// </summary>
        /// <param name="Market">市场代码,   0->深圳     1->上海</param>
        /// <param name="Zqdm">证券代码</param>
        /// <param name="Count">API执行前,表示证券代码的记录数目, API执行后,保存了实际返回的记录数目</param>
        /// <param name="Result">此API执行返回后，Result内保存了返回的查询数据, 形式为表格数据，行数据之间通过\n字符分割，列数据之间通过\t分隔。一般要分配1024*1024字节的空间。出错时为空字符串。</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串。</param>
        /// <returns>成功返货true, 失败返回false</returns>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool TdxL2Hq_GetSecurityQuotes(byte[] Market, string[] Zqdm, ref short Count, StringBuilder Result, StringBuilder ErrInfo);

        /// <summary>
        /// 获取F10资料的分类
        /// </summary>
        /// <param name="Market">市场代码,   0->深圳     1->上海</param>
        /// <param name="Zqdm">证券代码</param>
        /// <param name="Result">此API执行返回后，Result内保存了返回的查询数据, 形式为表格数据，行数据之间通过\n字符分割，列数据之间通过\t分隔。一般要分配1024*1024字节的空间。出错时为空字符串。</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串。</param>
        /// <returns>成功返货true, 失败返回false</returns>
        /// 
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool TdxL2Hq_GetCompanyInfoCategory(byte Market, string Zqdm, StringBuilder Result, StringBuilder ErrInfo);

        /// <summary>
        /// 获取F10资料的某一分类的内容
        /// </summary>
        /// <param name="Market">市场代码,   0->深圳     1->上海</param>
        /// <param name="Zqdm">证券代码</param>
        /// <param name="FileName">类目的文件名, 由TdxL2Hq_GetCompanyInfoCategory返回信息中获取</param>
        /// <param name="Start">类目的开始位置, 由TdxL2Hq_GetCompanyInfoCategory返回信息中获取</param>
        /// <param name="Length">类目的长度, 由TdxL2Hq_GetCompanyInfoCategory返回信息中获取</param>
        /// <param name="Result">此API执行返回后，Result内保存了返回的查询数据,出错时为空字符串。</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串。</param>
        /// <returns>成功返货true, 失败返回false</returns>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool TdxL2Hq_GetCompanyInfoContent(byte Market, string Zqdm, string FileName, int Start, int Length, StringBuilder Result, StringBuilder ErrInfo);

        /// <summary>
        /// 获取除权除息信息
        /// </summary>
        /// <param name="Market">市场代码,   0->深圳     1->上海</param>
        /// <param name="Zqdm">证券代码</param>
        /// <param name="Result">此API执行返回后，Result内保存了返回的查询数据,出错时为空字符串。</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串。</param>
        /// <returns>成功返货true, 失败返回false</returns>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool TdxL2Hq_GetXDXRInfo(byte Market, string Zqdm, StringBuilder Result, StringBuilder ErrInfo);

        /// <summary>
        /// 获取财务信息
        /// </summary>
        /// <param name="Market">市场代码,   0->深圳     1->上海</param>
        /// <param name="Zqdm">证券代码</param>
        /// <param name="Result">此API执行返回后，Result内保存了返回的查询数据,出错时为空字符串。</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串。</param>
        /// <returns>成功返货true, 失败返回false</returns>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool TdxL2Hq_GetFinanceInfo(byte Market, string Zqdm, StringBuilder Result, StringBuilder ErrInfo);

        /// <summary>
        /// 批量获取多个证券的十档报价数据
        /// </summary>
        /// <param name="Market">市场代码,   0->深圳     1->上海, 第i个元素表示第i个证券的市场代码</param>
        /// <param name="Zqdm">证券代码, Count个证券代码组成的数组</param>
        /// <param name="Count">API执行前,表示用户要请求的证券数目,最大50, API执行后,保存了实际返回的数目</param>
        /// <param name="Result">此API执行返回后，Result内保存了返回的查询数据, 形式为表格数据，行数据之间通过\n字符分割，列数据之间通过\t分隔。一般要分配1024*1024字节的空间。出错时为空字符串。</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串。</param>
        /// <returns>成功返货true, 失败返回false</returns>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool TdxL2Hq_GetSecurityQuotes10(byte[] Market, string[] Zqdm, ref short Count, StringBuilder Result, StringBuilder ErrInfo);

        /// <summary>
        /// 获取逐笔成交数据
        /// </summary>
        /// <param name="Market">市场代码,   0->深圳     1->上海</param>
        /// <param name="Zqdm">证券代码</param>
        /// <param name="Start">K线开始位置,最后一条K线位置是0, 前一条是1, 依此类推</param>
        /// <param name="Count">API执行前,表示用户要请求的K线数目, API执行后,保存了实际返回的K线数目</param>
        /// <param name="Result">此API执行返回后，Result内保存了返回的查询数据, 形式为表格数据，行数据之间通过\n字符分割，列数据之间通过\t分隔。一般要分配1024*1024字节的空间。出错时为空字符串。</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串。</param>
        /// <returns>成功返货true, 失败返回false</returns>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool TdxL2Hq_GetDetailTransactionData(byte Market, string Zqdm, int Start, ref short Count, StringBuilder Result, StringBuilder ErrInfo);

        /// <summary>
        /// 获取深圳逐笔委托数据
        /// </summary>
        /// <param name="Market">市场代码,   0->深圳     1->上海</param>
        /// <param name="Zqdm">证券代码</param>
        /// <param name="Start">K线开始位置,最后一条K线位置是0, 前一条是1, 依此类推</param>
        /// <param name="Count">API执行前,表示用户要请求的K线数目, API执行后,保存了实际返回的K线数目</param>
        /// <param name="Result">此API执行返回后，Result内保存了返回的查询数据, 形式为表格数据，行数据之间通过\n字符分割，列数据之间通过\t分隔。一般要分配1024*1024字节的空间。出错时为空字符串。</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串。</param>
        /// <returns>成功返货true, 失败返回false</returns>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool TdxL2Hq_GetDetailOrderData(byte Market, string Zqdm, int Start, ref short Count, StringBuilder Result, StringBuilder ErrInfo);

        /// <summary>
        /// 获取买卖队列数据
        /// </summary>
        /// <param name="Market">市场代码,   0->深圳     1->上海</param>
        /// <param name="Zqdm">证券代码</param>
        /// <param name="Result">此API执行返回后，Result内保存了返回的查询数据, 形式为表格数据，行数据之间通过\n字符分割，列数据之间通过\t分隔。一般要分配1024*1024字节的空间。出错时为空字符串。</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串。</param>
        /// <returns>成功返货true, 失败返回false</returns>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool TdxL2Hq_GetBuySellQueue(byte Market, string Zqdm, StringBuilder Result, StringBuilder ErrInfo);

        ///
        ///
        ///   通达信扩展行情
        ///   
        ///

        /// <summary>
        ///  连接通达信扩展行情服务器,服务器地址可在券商软件登录界面中的通讯设置中查得
        /// </summary>
        /// <param name="IP">服务器IP</param>
        /// <param name="Port">服务器端口</param>
        /// <param name="Result">此API执行返回后，Result内保存了返回的查询数据, 形式为表格数据，行数据之间通过\n字符分割，列数据之间通过\t分隔。一般要分配1024*1024字节的空间。出错时为空字符串。</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串。</param>
        /// <returns>成功返货true, 失败返回false</returns>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool TdxExHq_Connect(string IP, int Port, StringBuilder Result, StringBuilder ErrInfo);


        /// <summary>
        /// 断开同服务器的连接
        /// </summary>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern void TdxExHq_Disconnect(int nConnID);


        /// <summary>
        /// 设置超时的函数
        /// 缺省connect - 3000ms, send - 1000ms, recv - 1000ms, 这些在Tdx*_Connect函数中缺省设置
        /// </summary>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        public static extern void TdxExHq_SetTimeout(int nReadTimeout, int nWriteTimeout);

        /// <summary>
        ///  获取扩展行情中支持的各个市场的市场代码
        /// </summary>
        /// <param name="Result">此API执行返回后，Result内保存了返回的查询数据, 形式为表格数据，行数据之间通过\n字符分割，列数据之间通过\t分隔。一般要分配1024*1024字节的空间。出错时为空字符串。</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串。</param>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool TdxExHq_GetMarkets(StringBuilder Result, StringBuilder ErrInfo);



        /// <summary>
        ///  获取所有期货合约的总数
        /// </summary>
        /// <param name="Result">此API执行返回后，Result内保存了返回的合约总数。</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串。</param>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool TdxExHq_GetInstrumentCount(ref int Result, StringBuilder ErrInfo);



        /// <summary>
        ///  获取指定范围的期货合约的代码
        /// </summary>
        // <param name="Start">合约范围的开始位置, 由TdxExHq_GetInstrumentCount返回信息中确定</param>
        /// <param name="Count">合约的数目, 由TdxExHq_GetInstrumentCount返回信息中获取</param>
        /// <param name="Result">此API执行返回后，Result内保存了返回的查询数据,出错时为空字符串。</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串。</param>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool TdxExHq_GetInstrumentInfo(int Start, short Count, StringBuilder Result, StringBuilder ErrInfo);



        /// <summary>
        /// 获取合约的K线数据
        /// </summary>
        /// <param name="Category">K线种类, 
        /// 0->5分钟K线    
        /// 1->15分钟K线    
        /// 2->30分钟K线  
        /// 3->1小时K线    
        /// 4->日K线  
        /// 5->周K线  
        /// 6->月K线  
        /// 7->1分钟  
        /// 8->1分钟K线  
        /// 9->日K线  
        /// 10->季K线  
        /// 11->年K线
        /// < / param>
        /// <param name="Market">市场代码</param>
        /// <param name="Zqdm">证券代码</param>
        /// <param name="Start">K线开始位置,最后一条K线位置是0, 前一条是1, 依此类推</param>
        /// <param name="Count">API执行前,表示用户要请求的K线数目, API执行后,保存了实际返回的K线数目, 最大值800</param>
        /// <param name="Result">此API执行返回后，Result内保存了返回的查询数据, 形式为表格数据，行数据之间通过\n字符分割，列数据之间通过\t分隔。一般要分配1024*1024字节的空间。出错时为空字符串。</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串。</param>
        /// <returns>成功返货true, 失败返回false</returns>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool TdxExHq_GetInstrumentBars(byte Category, byte Market, string Zqdm, int Start, ref short Count, StringBuilder Result, StringBuilder ErrInfo);




        /// <summary>
        /// 获取分时数据
        /// </summary>
        /// <param name="Market">市场代码,</param>
        /// <param name="Zqdm">证券代码</param>
        /// <param name="Result">此API执行返回后，Result内保存了返回的查询数据, 形式为表格数据，行数据之间通过\n字符分割，列数据之间通过\t分隔。一般要分配1024*1024字节的空间。出错时为空字符串。</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串。</param>
        /// <returns>成功返货true, 失败返回false</returns>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool TdxExHq_GetMinuteTimeData(byte Market, string Zqdm, StringBuilder Result, StringBuilder ErrInfo);



        /// <summary>
        /// 获取分时成交数据
        /// </summary>
        /// <param name="Market">市场代码</param>
        /// <param name="Zqdm">证券代码</param>
        /// <param name="Start">K线开始位置,最后一条K线位置是0, 前一条是1, 依此类推</param>
        /// <param name="Count">API执行前,表示用户要请求的K线数目, API执行后,保存了实际返回的K线数目</param>
        /// <param name="Result">此API执行返回后，Result内保存了返回的查询数据, 形式为表格数据，行数据之间通过\n字符分割，列数据之间通过\t分隔。一般要分配1024*1024字节的空间。出错时为空字符串。</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串。</param>
        /// <returns>成功返货true, 失败返回false</returns>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool TdxExHq_GetTransactionData(byte Market, string Zqdm, int Start, ref short Count, StringBuilder Result, StringBuilder ErrInfo);



        /// <summary>
        /// 获取合约的五档报价数据
        /// </summary>
        /// <param name="Market">市场代码</param>
        /// <param name="Zqdm">证券代码</param>
        /// <param name="Result">此API执行返回后，Result内保存了返回的查询数据, 形式为表格数据，行数据之间通过\n字符分割，列数据之间通过\t分隔。一般要分配1024*1024字节的空间。出错时为空字符串。</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串。</param>
        /// <returns>成功返货true, 失败返回false</returns>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool TdxExHq_GetInstrumentQuote(byte Market, string Zqdm, StringBuilder Result, StringBuilder ErrInfo);



        /// <summary>
        /// 获取历史分时成交数据
        /// </summary>
        /// <param name="Market">市场代码,</param>
        /// <param name="Zqdm">证券代码</param>
        /// <param name="Start">K线开始位置,最后一条K线位置是0, 前一条是1, 依此类推</param>
        /// <param name="Count">API执行前,表示用户要请求的K线数目, API执行后,保存了实际返回的K线数目</param>
        /// <param name="Date">日期, 比如2014年1月1日为整数20140101</param>
        /// <param name="Result">此API执行返回后，Result内保存了返回的查询数据, 形式为表格数据，行数据之间通过\n字符分割，列数据之间通过\t分隔。一般要分配1024*1024字节的空间。出错时为空字符串。</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串。</param>
        /// <returns>成功返货true, 失败返回false</returns>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool TdxExHq_GetHistoryTransactionData(byte Market, string Zqdm, int date, int Start, ref short Count, StringBuilder Result, StringBuilder ErrInfo);





        /// <summary>
        /// 获取历史分时数据
        /// </summary>
        /// <param name="Market">市场代码</param>
        /// <param name="Zqdm">证券代码</param>
        /// <param name="Date">日期, 比如2014年1月1日为整数20140101</param>
        /// <param name="Result">此API执行返回后，Result内保存了返回的查询数据, 形式为表格数据，行数据之间通过\n字符分割，列数据之间通过\t分隔。一般要分配1024*1024字节的空间。出错时为空字符串。</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串。</param>
        /// <returns>成功返货true, 失败返回false</returns>
        [DllImport("TradeX2.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool TdxExHq_GetHistoryMinuteTimeData(byte Market, string Zqdm, int date, StringBuilder Result, StringBuilder ErrInfo);

    }
}
