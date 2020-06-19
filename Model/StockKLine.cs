using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// K线图
    /// </summary>
    public class StockKLine
    {
        public StockKLine()
        {
            this.kmins = new List<kmins>();
        }
        public List<kmins> kmins { get; set; }
    }
    /// <summary>
    /// K线图详细数据
    /// </summary>
    public class kmins
    {
        /// <summary>
        /// 日期
        /// </summary>
        private int _time = 0;
        /// <summary>
        /// 开盘价格
        /// </summary>
        private decimal _open = 0;
        /// <summary>
        /// 昨收价格
        /// </summary>
        private decimal _preClose = 0;
        /// <summary>
        /// 最高价格
        /// </summary>
        private decimal _highest = 0;
        /// <summary>
        /// 最低价格
        /// </summary>
        private decimal _lowest = 0;
        /// <summary>
        /// 当前价格
        /// </summary>
        private decimal _price = 0;
        /// <summary>
        /// 成交量
        /// </summary>
        private decimal _volume = 0;
        /// <summary>
        /// 成交额
        /// </summary>
        private decimal _amount = 0;

        #region ===============封装字段
        public int time
        {
            get { return _time; }
            set { _time = value; }
        }
        public decimal open
        {
            get { return _open; }
            set { _open = value; }
        }
        public decimal preClose
        {
            get { return _preClose; }
            set { _preClose = value; }
        }
        public decimal highest
        {
            get { return _highest; }
            set { _highest = value; }
        }
        public decimal lowest
        {
            get { return _lowest; }
            set { _lowest = value; }
        }
        public decimal price
        {
            get { return _price; }
            set { _price = value; }
        }
        public decimal volume
        {
            get { return _volume; }
            set { _volume = value; }
        }
        public decimal amount
        {
            get { return _amount; }
            set { _amount = value; }
        }
        #endregion

    }
}
