using System; 
using System.Text;
using System.Collections.Generic; 
using System.Data;
namespace Model
{
	 	//c_stock_code
		public class stock_codeInfo
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
		/// stock_name_capital
        /// </summary>		
		private string _stock_name_capital;
        public string stock_name_capital
        {
            get{ return _stock_name_capital; }
            set{ _stock_name_capital = value; }
        }        
		/// <summary>
		/// stock_board
        /// </summary>		
		private string _stock_board;
        public string stock_board
        {
            get{ return _stock_board; }
            set{ _stock_board = value; }
        }        
		/// <summary>
		/// circulation_value
        /// </summary>		
		private decimal _circulation_value;
        public decimal circulation_value
        {
            get{ return _circulation_value; }
            set{ _circulation_value = value; }
        }        
		/// <summary>
		/// circulation
        /// </summary>		
		private string _circulation;
        public string circulation
        {
            get{ return _circulation; }
            set{ _circulation = value; }
        }        
		/// <summary>
		/// sort_id
        /// </summary>		
		private int _sort_id;
        public int sort_id
        {
            get{ return _sort_id; }
            set{ _sort_id = value; }
        }        
		/// <summary>
		/// status
        /// </summary>		
		private int _status;
        public int status
        {
            get{ return _status; }
            set{ _status = value; }
        }        
		   
	}
}

