using System; 
using System.Text;
using System.Collections.Generic; 
using System.Data;
namespace Model
{
	 	//sys_stock_code
		public class s_stock_codeInfo
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
		/// rd_decl
        /// </summary>		
		private DateTime _rd_decl;
        public DateTime rd_decl
        {
            get{ return _rd_decl; }
            set{ _rd_decl = value; }
        }        
		/// <summary>
		/// rd_deal
        /// </summary>		
		private DateTime _rd_deal;
        public DateTime rd_deal
        {
            get{ return _rd_deal; }
            set{ _rd_deal = value; }
        }        
		/// <summary>
		/// rd_num
        /// </summary>		
		private decimal _rd_num;
        public decimal rd_num
        {
            get{ return _rd_num; }
            set{ _rd_num = value; }
        }        
		/// <summary>
		/// rd_money
        /// </summary>		
		private decimal _rd_money;
        public decimal rd_money
        {
            get{ return _rd_money; }
            set{ _rd_money = value; }
        }        
		/// <summary>
		/// rd_txt
        /// </summary>		
		private string _rd_txt;
        public string rd_txt
        {
            get{ return _rd_txt; }
            set{ _rd_txt = value; }
        }        
		/// <summary>
		/// remark
        /// </summary>		
		private string _remark;
        public string remark
        {
            get{ return _remark; }
            set{ _remark = value; }
        }        
		/// <summary>
		/// input_time
        /// </summary>		
		private DateTime _input_time;
        public DateTime input_time
        {
            get{ return _input_time; }
            set{ _input_time = value; }
        }        
		   
	}
}

