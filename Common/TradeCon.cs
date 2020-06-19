using System.Runtime.InteropServices;
using System.Text;

namespace Common
{
    /// <summary>
    /// 50ETF行情：通达信交易接口定义
    /// </summary>
    public class TradeCon
    {
        /// <summary>
        /// 交易账户登录
        /// </summary>
        /// <param name="IP">券商交易服务器IP</param>
        /// <param name="Port">券商交易服务器端口</param>
        /// <param name="Version">设置通达信客户端的版本号</param>
        /// <param name="YybID">营业部代码，一般为8888</param>
        /// <param name="AccountNo">完整的登录账号，券商一般使用资金帐户或客户号</param>
        /// <param name="TradeAccount">交易账号，留空串</param>
        /// <param name="JyPassword">交易密码</param>
        /// <param name="TxPassword">通讯密码, 没有就留空串</param>
        /// <param name="ErrInfo">此API执行返回后，如果出错，保存了错误信息说明。一般要分配256字节的空间。没出错时为空字符串。</param>
        /// <returns>返回ClientID:客户端ID，失败时返回-1</returns>
        [DllImport(@"trade.dll", EntryPoint = "Logon", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        //[return: MarshalAs(UnmanagedType.LPStr)]
        public static extern int Logon(string IP, short Port, string Version, short YybID, string AccountNo, string TradeAccount, string JyPassword, string TxPassword, StringBuilder ErrInfo);

        /// <summary>
        /// 交易账户注销
        /// </summary>
        /// <param name="ClientID">客户端ID</param>
        [DllImport("trade.dll", CharSet = CharSet.Ansi)]
        public static extern void Logoff(int ClientID);


        /// <summary>
        /// 查询各种交易数据
        /// </summary>
        /// <param name="ClientID">客户端ID</param>
        /// <param name="Category">表示查询信息的种类，0资金  1股份   2当日委托  3当日成交     4合约代码</param>
        /// <param name="StartTimeStamp">时间戳字符串, 从此时间戳的记录开始返回查询结果, 空字符串表示从最初一条数据开始返回,  查询资金和持仓时无效,通过查询当日委托获知时间戳</param>
        /// <param name="Count">指定返回查询结果的记录数目,查询资金和持仓时无效</param>
        /// <param name="Result">此API执行返回后，Result内保存了返回的查询数据, 形式为表格数据，行数据之间通过\n字符分割，列数据之间通过\t分隔。一般要分配1024*1024字节的空间。出错时为空字符串。</param>
        /// <param name="ErrInfo">同Logon函数的ErrInfo说明</param>
        [DllImport("trade.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern void QueryOptionData(int ClientID, int Category, string StartTimeStamp, int Count, StringBuilder Result, StringBuilder ErrInfo);


        /// <summary>
        /// 下单
        /// </summary>
        /// <param name="ClientID">返回客户端ID</param>
        /// <param name="exchange_id">交易所代码,   0深圳      1上海</param>
        /// <param name="secu_code">合约代码</param>
        /// <param name="bs_flag">买卖类别     0买      1卖</param>
        /// <param name="offset_flag">开平标志  0开仓       1平仓</param>
        /// <param name="order_type">委托类型,取值见下面表格</param>
        /// <param name="order_prop">委托属性,取值见下面表格</param>
        /// 上海市场:  限价GFD:order_type=0 order_prop=0        市价IOC剩余即撤:order_type=1 order_prop=2        市价剩余转限价GFD:order_type=1 order_prop=0     限价FOK全成或撤:order_type=0 order_prop=1     市价FOK全成或撤:order_type=1 order_prop=1
        /// 深圳市场:  限价GFD:order_type=0 order_prop=0        限价FOK全成或撤:order_type=0 order_prop=1        对方最优价格:order_type=1 order_prop=0            本方最优价格:order_type=2 order_prop=0           即时成交剩撤:order_type=3 order_prop=0                    五档即成剩撤:order_type=4 order_prop=0     全额成交或撤:order_type=5 order_prop=0 
        /// <param name="order_price">委托价格 </param>
        /// <param name="order_volume">委托数量</param>
        /// <param name="Result">同上</param>
        /// <param name="ErrInfo">同上</param>
        [DllImport("trade.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool SendOption(int Start, short Count, StringBuilder Result, StringBuilder ErrInfo);


        /// <summary>
        /// 撤单
        /// </summary>
        /// <param name="ClientID">返回客户端ID</param>
        /// <param name="order_ref">表示要撤的目标委托的引用号,  通过查询当日委托获知引用号</param>
        /// <param name="Result">同上</param>
        /// <param name="ErrInfo">同上</param>
        [DllImport("trade.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool CancelOption(int ClientID, string order_ref, StringBuilder Result, StringBuilder ErrInfo);
    }
}
