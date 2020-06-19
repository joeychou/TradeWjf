using System; 
using System.Text;
using System.Collections.Generic; 
using System.Data;
namespace Model
{
	 	//c_stock_t_line
		public class stock_t_lineInfo
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
		/// fprice
        /// </summary>		
		private decimal _fprice;
        public decimal fprice
        {
            get{ return _fprice; }
            set{ _fprice = value; }
        }        
		/// <summary>
		/// fvol
        /// </summary>		
		private int _fvol;
        public int fvol
        {
            get{ return _fvol; }
            set{ _fvol = value; }
        }        
		/// <summary>
		/// famount
        /// </summary>		
		private decimal _famount;
        public decimal famount
        {
            get{ return _famount; }
            set{ _famount = value; }
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
		   
	}
}

