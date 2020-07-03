using Common;
using Common.wjf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
namespace wjf_api
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }
        BLL.stock_codeBLL dal = new BLL.stock_codeBLL();
        private void Form1_Load(object sender, EventArgs e)
        {
            StockDrv.Stock_Init(this.Handle, StockDrv.My_Msg_StkData, StockDrv.RCV_WORK_SENDMSG);
        }

        //数据接收处理主程序,从窗口消息中判断是股票行情消息后,把m传过来给这个函数处理就
        public string m_strMessage; //用于传出函数功能调用后的说明,调试字符串,调用者可取出显示

        /// <summary>
        /// 重载接收消息的窗口的消息处理过程
        /// </summary>
        /// <param name="m">消息</param>
        protected override void DefWndProc(ref Message m)
        {
            if ((int)m.Msg == StockDrv.My_Msg_StkData)
            {
                txtMsg.Text = string.Format("wparam:{0},lparam{1}", (UInt32)m.WParam, m.LParam);
                OnStkDataOK(ref m);
                txtMsg.Text = m_strMessage;
                return;
            }
            base.DefWndProc(ref m);
        }

        public void OnStkDataOK(ref Message m)
        {
            try
            {
                //LPARAM由指针转换为tagRCV_DATA结构
                StockTools.tagRCV_DATA recvData = (StockTools.tagRCV_DATA)Marshal.PtrToStructure(m.LParam, typeof(StockTools.tagRCV_DATA));
                int param = (int)m.WParam;
                List<string> stringLot = new List<string>();
                switch (param)
                {
                    case StockDrv.RCV_REPORT://行情数据
                        m_strMessage = string.Format("行情包数量:{0}\r\n", recvData.m_nPacketNum);
                        DateTime dt = DateTime.Now;
                        StockTools.tagRCV_REPORT_STRUCTEx stockData;
                        for (int i = 0; i < recvData.m_nPacketNum; i++)
                        {
                            stockData = (StockTools.tagRCV_REPORT_STRUCTEx)Marshal.PtrToStructure(new IntPtr((int)recvData.m_pData + i * 158 - 2), typeof(StockTools.tagRCV_REPORT_STRUCTEx));
                            string market = new string(stockData.m_wMarket).ToLower();
                            if (market == "sh" || market == "sz")
                            {
                                m_strMessage += string.Format("{0}、", stockData.m_szName);
                                string line_info = stockData.m_szName + "," + stockData.m_fOpen + "," + stockData.m_fLastClose + "," + stockData.m_fNewPrice + "," + stockData.m_fHigh + "," + stockData.m_fLow + ","
                                    + stockData.m_fBuyPrice[0] + "," + stockData.m_fSellPrice[0] + "," + stockData.m_fVolume + "," + Utils.ChangeDataToD(stockData.m_fAmount.ToString()) + "," + stockData.m_fBuyVolume[0] * 100 + "," + stockData.m_fBuyPrice[0] + ","
                                    + stockData.m_fBuyVolume[1] * 100 + "," + stockData.m_fBuyPrice[1] + "," + stockData.m_fBuyVolume[2] * 100 + "," + stockData.m_fBuyPrice[2] + "," + stockData.m_fBuyVolume4 * 100 + "," + stockData.m_fBuyPrice4 + ","
                                    + stockData.m_fBuyVolume5 * 100 + "," + stockData.m_fBuyPrice5 + "," + stockData.m_fSellVolume[0] * 100 + "," + stockData.m_fSellPrice[0] + "," + stockData.m_fSellVolume[1] * 100 + "," + stockData.m_fSellPrice[1] + ","
                                    + stockData.m_fSellVolume[2] * 100 + "," + stockData.m_fSellPrice[2] + "," + stockData.m_fSellVolume4 * 100 + "," + stockData.m_fSellPrice4 + "," + stockData.m_fSellVolume5 * 100 + "," + stockData.m_fSellPrice5 + ","
                                    + dt.ToString("yyyy-MM-dd") + "," + dt.ToString("HH:mm:ss");

                                RedisHelper.Set<string>(market + stockData.m_szLabel, line_info);
                            }
                        }
                        //stringLot.Add(" insert into c_stock_t_line(stock_code,stock_name,update_time,fprice,fvol,famount,remark) values ('"
                        //    + stockData.m_szLabel + "','" + stockData.m_szName + "',now()," + Utils.StrToDecimal(stockData.m_fNewPrice.ToString(), 0)
                        //    + "," + Utils.StrToInt(stockData.m_fVolume.ToString(), 0) + "," + Utils.ChangeDataToD(stockData.m_fAmount.ToString()) + ",'" + line_info + "');");
                        //// 多条更新
                        //int num = dal.TranLot(stringLot);
                        m_strMessage += string.Format("\r\n系统更新数据:{0}\r\n", recvData.m_nPacketNum);
                        break;
                    case StockDrv.RCV_FILEDATA://文件数据
                        m_strMessage = string.Format("收到文件数据");
                        if (recvData.m_wDataType == StockDrv.FILE_HISTORY_EX)//日线
                        {
                            StockTools.tagRCV_EKE_HEADEx HisHead = (StockTools.tagRCV_EKE_HEADEx)Marshal.PtrToStructure(new IntPtr((int)recvData.m_pData), typeof(StockTools.tagRCV_EKE_HEADEx));
                            StockTools.tagRCV_HISTORY_STRUCTEx HisData = new StockTools.tagRCV_HISTORY_STRUCTEx();
                            string wMarket = new string(HisHead.m_wMarket);
                            string stock_code = HisHead.m_szLabel;
                            if ((wMarket == "SH" && stock_code[0] == '6') || (wMarket == "SZ" && stock_code[0] == '0'))
                            {
                                for (int i = 0; i < recvData.m_nPacketNum; i++)
                                {
                                    HisData = (StockTools.tagRCV_HISTORY_STRUCTEx)Marshal.PtrToStructure(new IntPtr((int)recvData.m_pData + i * 32), typeof(StockTools.tagRCV_HISTORY_STRUCTEx));
                                    if (i > 0)
                                    {
                                        stringLot.Add("insert into c_stock_k_line(stock_code,stock_name,update_time,open,preClose,highest,lowest,deal_vol,deal_price,fprice) values ('"
                                             + stock_code + "','" + wMarket + "','" + Utils.ConvertIntDatetime(HisData.m_time).ToString("yyyy-MM-dd HH:mm:ss") + "'," + HisData.m_fOpen
                                             + "," + HisData.m_fClose + "," + HisData.m_fHigh.ToString() + "," + HisData.m_fLow.ToString() + "," + HisData.m_fVolume + ","
                                             + Utils.ChangeDataToD(HisData.m_fAmount.ToString()) + ",0);");
                                    }
                                }
                                int num = dal.TranLot(stringLot);
                                m_strMessage = string.Format("更新日线数据:{0}条\r\n", recvData.m_nPacketNum);
                            }
                        }
                        else if (recvData.m_wDataType == StockDrv.FILE_MINUTE_EX)//分钟
                        {
                            m_strMessage += string.Format(":分钟,数据包数量:{0}", recvData.m_nPacketNum);
                            StockTools.tagRCV_EKE_HEADEx MinuteHead = new StockTools.tagRCV_EKE_HEADEx();
                            StockTools.tagRCV_MINUTE_STRUCTEx MinuteData = new StockTools.tagRCV_MINUTE_STRUCTEx();

                            for (int i = 0; i < recvData.m_nPacketNum; i++)
                            {
                                MinuteHead = (StockTools.tagRCV_EKE_HEADEx)Marshal.PtrToStructure(new IntPtr((int)recvData.m_pData + i * 16), typeof(StockTools.tagRCV_EKE_HEADEx));
                                MinuteData = (StockTools.tagRCV_MINUTE_STRUCTEx)Marshal.PtrToStructure(new IntPtr((int)recvData.m_pData + i * 32), typeof(StockTools.tagRCV_MINUTE_STRUCTEx));
                                string wMarket = new string(MinuteHead.m_wMarket);
                                string stock_code = MinuteHead.m_szLabel;
                                if ((wMarket == "SH" && stock_code[0] == '6') || (wMarket == "SZ" && stock_code[0] == '0'))
                                {
                                    MinuteData = (StockTools.tagRCV_MINUTE_STRUCTEx)Marshal.PtrToStructure(new IntPtr((int)recvData.m_pData + i * 32), typeof(StockTools.tagRCV_MINUTE_STRUCTEx));

                                    string ss = MinuteData.m_fAmount + "" + MinuteData.m_fPrice + "" + MinuteData.m_fVolume + "" + Utils.ConvertIntDatetime(MinuteData.m_time);
                                    m_strMessage = string.Format("{0}", ss);
                                    MessageBox.Show(ss);

                                }
                                MinuteData.ToString();
                            }

                        }
                        else if (recvData.m_wDataType == StockDrv.FILE_5MINUTE_EX)// 补5分钟线数据
                        {
                            m_strMessage += ":5分钟";
                            StockTools.tagRCV_EKE_HEADEx MinuteHead = new StockTools.tagRCV_EKE_HEADEx();
                            StockTools.tagRCV_HISTORY_STRUCTEx MinuteData = new StockTools.tagRCV_HISTORY_STRUCTEx();
                            for (int i = 0; i < recvData.m_nPacketNum; i++)
                            {
                                MinuteHead = (StockTools.tagRCV_EKE_HEADEx)Marshal.PtrToStructure(new IntPtr((int)recvData.m_pData + i * 32), typeof(StockTools.tagRCV_EKE_HEADEx));
                                MinuteData = (StockTools.tagRCV_HISTORY_STRUCTEx)Marshal.PtrToStructure(new IntPtr((int)recvData.m_pData + i * 32), typeof(StockTools.tagRCV_HISTORY_STRUCTEx));
                                if (MinuteHead.m_dwHeadTag == StockDrv.EKE_HEAD_TAG)
                                {
                                    //string strMsg = new string(MinuteHead.m_szLabel);
                                    //MessageBox.Show(strMsg);
                                }
                                MinuteData.ToString();
                            }
                        }
                        else if (recvData.m_wDataType == StockDrv.FILE_POWER_EX)// 补充除权数据
                        {
                            m_strMessage += ":补充除权数据";
                        }
                        else if (recvData.m_wDataType == StockDrv.FILE_BASE_EX)// 基本资料文件,m_szFileName仅包含文件名
                        {
                            m_strMessage += ":资料";
                            StockTools.tagRCV_DATA PowHead = (StockTools.tagRCV_DATA)Marshal.PtrToStructure(new IntPtr((int)recvData.m_pData), typeof(StockTools.tagRCV_DATA));
                            StockTools.tagRCV_FILE_HEADEx m_File = PowHead.m_File;
                            IntPtr pFile = PowHead.m_pData;
                            string cmt = new string(m_File.m_szFileName);
                            FileStream fs = new FileStream("E:\\ak.txt", FileMode.Create);
                            //获得字节数组
                            byte[] data = Encoding.Default.GetBytes(m_File.m_szFileName);
                            //开始写入
                            fs.Write(data, 0, data.Length);
                            //清空缓冲区、关闭流
                            fs.Flush();
                            fs.Close();
                        }
                        else if (recvData.m_wDataType == StockDrv.FILE_NEWS_EX)// 新闻类,其类型由m_szFileName中子目录名来定
                        {
                            m_strMessage += ":新闻";
                        }
                        break;
                    case StockDrv.RCV_FENBIDATA:
                        m_strMessage = string.Format("收到RCV_FENBIDATA分笔数据");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                m_strMessage += string.Format("\r\nerror:{0}", ex.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Log.WriteLog("除权降息，手动操作", "除权信息");
                List<string> stringLot = new List<string>();
                BinaryReader sReader = new BinaryReader(File.Open(@"C:\除权数据.pwr", FileMode.Open));
                sReader.BaseStream.Position = 12;
                int k;
                while (sReader.BaseStream.Length > sReader.BaseStream.Position)
                {
                    string stock_code = Encoding.UTF8.GetString(sReader.ReadBytes(8)).ToLower().Replace("sh", "").Replace("sz", "");
                    sReader.BaseStream.Position += 8;
                    k = sReader.ReadInt32();
                    while (k > 0)
                    {
                        DateTime dt = DateTime.Now;
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
                Log.WriteLog("除权降息，手动操作", $"{string.Join("*******", stringLot.ToArray())}");
                txtMsg.Text = string.Format("除权降息共:{0}条\r\n", num);
            }
            catch (Exception ex)
            {
                Log.WriteLog("除权信息", ex.ToString());
                txtMsg.Text = string.Format("除权降息报错:", ex.ToString());
            }
        }
        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, uint wParam, uint lParam);
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                OdbcConnection ocConn = new OdbcConnection();
                //string table = @"C:\基本面资料.DBF";//接口数据位置
                //string conn =
                //              @"Driver={Microsoft dBASE Driver (*.dbf)};SourceType=DBF;" +
                //              @"Data Source=" + table + ";Exclusive=No;NULL=NO;" +
                //              @"Collate=Machine;BACKGROUNDFETCH=NO;DELETE=NO";
                //ocConn.ConnectionString = conn;
                //ocConn.Open();
                //string strSql = "SELECT * FROM " + table;
                //OdbcDataAdapter oda = new OdbcDataAdapter(strSql, ocConn);
                //DataTable dt = new DataTable();
                //oda.Fill(dt);
                //List<string> stringLot = new List<string>();
                //foreach (DataRow dr in dt.Rows)
                //{
                //    //===========上市日期、股票代码
                //    stringLot.Add("update sys_stock_code set remark='" + dr["SSDATE"].ToString() + "' where stock_code='" + dr["GPDM"] + "';");
                //}
                //stringLot.Add("update sys_stock_code set rd_decl=remark;");
                //stringLot.Add("update sys_stock_code set input_time=now() where date(rd_decl)=date(now());");
                //int num = dal.TranLot(stringLot);
                //txtMsg.Text = string.Format("更新基本面资料共:{0}条\r\n", num);
                //Log.WriteLog("更新基本面资料", "共" + num + "条");

                string directoryPath = @"C:\";//存放的dbf文件夹目录。
                string fileName = @"基本面资料.dbf";//dbf的文件名,这里比如是test.dbf 因为这里做为表名，所以后缀.dbf可以省略，直接是test也可以的。
                string strConn = @"Driver={Microsoft dBASE Driver (*.dbf)};DriverID=277;Dbq=" + directoryPath;

                System.Data.Odbc.OdbcConnection odbcConn = new System.Data.Odbc.OdbcConnection();
                odbcConn.ConnectionString = strConn;
                odbcConn.Open();

                string strSql = @"SELECT * FROM " + fileName;

                OdbcDataAdapter oda = new OdbcDataAdapter(strSql, odbcConn);
                DataTable dt = new DataTable();
                oda.Fill(dt);
                var dtNow = DateTime.Now.ToString("yyyyMMdd");
                List<string> stringLot = new List<string>();
                foreach (DataRow dr in dt.Rows)
                {
                    //上市时间、更新日期、股票代码,程序需要先操作基本面资料，然后在操作除权信息
                    var SSDATE = dr["SSDATE"].ToString();
                    var GXRQ = dr["GXRQ"].ToString();
                    var GPDM = dr["GPDM"].ToString();
                    if (SSDATE == dtNow || GXRQ == dtNow)
                    {
                        var stockList = Utils.GetStockData(GPDM, 1, 0).Split(',');
                        if (stockList.Length >= 32)
                        {
                            string stockName = stockList[0];
                            string stockNameCapital = Utils.GetSpellCode(stockName);
                            stringLot.Add($"delete from sys_stock_code where stock_code='{GPDM}' and stock_code not in (select a.stock_code from (select stock_code from sys_stock_code where stock_code='{GPDM}') a);");
                            stringLot.Add($"update sys_stock_code set remark='{SSDATE}',stock_name='{stockName}',stock_name_capital='{stockNameCapital}' where stock_code='{GPDM}';");
                            stringLot.Add($"update sys_stock_code set rd_decl=remark where stock_code='{GPDM}';");
                            stringLot.Add($"update sys_stock_code set input_time=now() where date(rd_decl)=date(now()) and  stock_code='{GPDM}';");
                        }
                    }
                }

                int num = dal.TranLot(stringLot);
                int num2 = dal.TranLotTo(stringLot);
                txtMsg.Text = string.Format("更新基本面资料共:{0}条\r\n", num);
                Log.WriteLog("更新基本面资料", "共" + num + "条");
            }
            catch (Exception ex)
            {
                txtMsg.Text = string.Format("更新基本面资料报错:", ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                //Log.WriteLog("除权降息，手动操作", "除权信息");
                List<string> stringLot = new List<string>();
                BinaryReader sReader = new BinaryReader(File.Open(@"C:\股本变迁.TDX", FileMode.Open));
                long lgt = sReader.BaseStream.Length;
                long j = 0, z = lgt / 29;
                sReader.BaseStream.Position = 29;
                int dt = Utils.StrToInt(DateTime.Now.AddDays(-2).ToString("yyyyMMdd"), 0);
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
                        if (stock_code.Length == 6 && rd_deal > dt && floatCount < 2000)
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
                            stringLot.Add("update sys_stock_code set remark='" +rd_deal + "',rd_num="
                                + (m_fGive/10) + ",rd_money=" + (m_fProfit/10) + ",rd_txt='" + rd_txt + "',input_time=now() where stock_code='" + stock_code + "';");
                            stringLot.Add($"update sys_stock_code set rd_deal=remark where stock_code='{stock_code}';");
                            //Log.WriteLog("除权降息，手动操作", $"sc={sc},stock_code={stock_code},RQ={rd_deal},xxlx={xxlx},m_fProfit={m_fProfit},m_fGive={m_fGive},F3={F3},F4={F4},i={i},z={z},lgt={lgt}");
                        }
                    }
                }
                sReader.Close();
                int num = dal.TranLot(stringLot);
                int num2 = dal.TranLotTo(stringLot);
                Log.WriteLog("通达信除权降息，手动操作", $"{string.Join("*******", stringLot.ToArray())}");
                txtMsg.Text = string.Format("除权降息共:{0}条\r\n", 1);
            }
            catch (Exception ex)
            {
                Log.WriteLog("除权信息", ex.ToString());
                txtMsg.Text = string.Format("除权降息报错:", ex.ToString());
            }
        }
    }
}
