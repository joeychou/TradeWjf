using System; 
using System.Text;
using System.Collections.Generic; 
using System.Data;
namespace Model
{
	 	//c_stock_k_line
		public class stock_k_lineInfo
	{
   		     
      	/// <summary>
		/// auto_increment
        /// </summary>		
		private int _id;
        public int id
        {
            get{ return _id; }
            set{ _id = value; }
        }        
		/// <summary>
		/// stock_code
        /// </summary>		
		private string _stock_code;
        public string stock_code
        {
            get{ return _stock_code; }
            set{ _stock_code = value; }
        }        
		/// <summary>
		/// stock_name
        /// </summary>		
		private string _stock_name;
        public string stock_name
        {
            get{ return _stock_name; }
            set{ _stock_name = value; }
        }        
		/// <summary>
		/// update_time
        /// </summary>		
		private DateTime _update_time;
        public DateTime update_time
        {
            get{ return _update_time; }
            set{ _update_time = value; }
        }        
		/// <summary>
		/// open
        /// </summary>		
		private decimal _open;
        public decimal open
        {
            get{ return _open; }
            set{ _open = value; }
        }        
		/// <summary>
		/// preClose
        /// </summary>		
		private decimal _preclose;
        public decimal preClose
        {
            get{ return _preclose; }
            set{ _preclose = value; }
        }        
		/// <summary>
		/// highest
        /// </summary>		
		private decimal _highest;
        public decimal highest
        {
            get{ return _highest; }
            set{ _highest = value; }
        }        
		/// <summary>
		/// lowest
        /// </summary>		
		private decimal _lowest;
        public decimal lowest
        {
            get{ return _lowest; }
            set{ _lowest = value; }
        }        
		/// <summary>
		/// deal_vol
        /// </summary>		
		private int _deal_vol;
        public int deal_vol
        {
            get{ return _deal_vol; }
            set{ _deal_vol = value; }
        }        
		/// <summary>
		/// deal_price
        /// </summary>		
		private decimal _deal_price;
        public decimal deal_price
        {
            get{ return _deal_price; }
            set{ _deal_price = value; }
        }        
		/// <summary>
		/// now_price
        /// </summary>		
        private decimal _fprice;
        public decimal fprice
        {
            get { return _fprice; }
            set { _fprice = value; }
        }        
		   
	}
}

