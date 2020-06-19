using System; 
using System.Text;
using System.Collections.Generic; 
using System.Data;
namespace Model
{
	 	//c_stock_ex
		public class stock_exInfo
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
		/// time
        /// </summary>		
		private int _time;
        public int time
        {
            get{ return _time; }
            set{ _time = value; }
        }        
		/// <summary>
		/// fgive
        /// </summary>		
		private decimal _fgive;
        public decimal fgive
        {
            get{ return _fgive; }
            set{ _fgive = value; }
        }        
		/// <summary>
		/// fpei
        /// </summary>		
		private decimal _fpei;
        public decimal fpei
        {
            get{ return _fpei; }
            set{ _fpei = value; }
        }        
		/// <summary>
		/// fpei_price
        /// </summary>		
		private decimal _fpei_price;
        public decimal fpei_price
        {
            get{ return _fpei_price; }
            set{ _fpei_price = value; }
        }        
		/// <summary>
		/// fprofit
        /// </summary>		
		private decimal _fprofit;
        public decimal fprofit
        {
            get{ return _fprofit; }
            set{ _fprofit = value; }
        }        
		   
	}
}

